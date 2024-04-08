using System.Diagnostics;
using Ductus.FluentDocker.Model.Common;
using Ductus.FluentDocker.Model.Compose;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Impl;
using Newtonsoft.Json.Linq;
using Shared;

namespace Tests;

public class TestcontainerFixture : IDisposable
{
    private ICompositeService compositeService;
    private IHostService? dockerHost;

    public TestcontainerFixture()
    {

        EnsureDockerHost();

        var file = Path.Combine(Directory.GetCurrentDirectory(),
            (TemplateString)"docker-compose.debug.yml");
        var fileNoAwait = Path.Combine(Directory.GetCurrentDirectory(),
            (TemplateString)"docker-compose.debug.noattach.yml");
        compositeService = new DockerComposeCompositeService(
            dockerHost,
            new DockerComposeConfig
            {
                ComposeFilePath = new List<string> { file, fileNoAwait },
                ForceRecreate = true,
                RemoveOrphans = true,
                StopOnDispose = true,
                ComposeVersion = ComposeVersion.V2,

            });
        try
        {
            compositeService.Start();
        }
        catch (Exception e)
        {
            compositeService.Dispose();
            throw e;
        }
    }

    /// <summary>
    /// Obtém um http quando as funções do functions estão prontas.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception">Quando o número de tentativas foi atingido</exception>
    public async Task<HttpClient> GetClientWhenDone(int attemps = 80, int delayMs = 500, string statusRoute = "admin/host/status")
    {
        var count = 1;
        var http = new HttpClient
        {
            BaseAddress = new Uri(TestValues.RootAdress)
        };

        while (count++ <= attemps)
        {
            try
            {
                var response = await http.GetAsync(statusRoute);
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && AzureFunctionsIsRunning(content))
                    return new HttpClient
                    {
                        BaseAddress = new Uri(TestValues.ApiAdress)
                    };


            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine("Conexão não estabelecida, nova tentativa em " + delayMs);

            }
            await Task.Delay(delayMs);

        }

        Assert.Fail("Número de tentativas excedido");
        throw new Exception();


    }

    /// <summary>
    /// Processa o json provido pelo admin do azure functions
    /// </summary>
    private bool AzureFunctionsIsRunning(string statusResponseJson)
    {
        var json = JObject.Parse(statusResponseJson);
        var state = json.SelectToken("state");
        if (state == null)
            Assert.Fail("Formato de json irreconhecível: " + statusResponseJson);
        return state.Value<string>() == "Running";
    }
    
    private void EnsureDockerHost()
    {
        if (dockerHost?.State == ServiceRunningState.Running)
            return;

        var hosts = new Hosts().Discover();
        dockerHost = hosts.FirstOrDefault(x => x.IsNative) ?? hosts.FirstOrDefault(x => x.Name == "default");

        if (null != dockerHost)
        {
            if (dockerHost.State != ServiceRunningState.Running)
                dockerHost.Start();

            return;
        }

        if (hosts.Count > 0)
            dockerHost = hosts.First();

        if (null != dockerHost)
            return;

        EnsureDockerHost();
    }

    public void Dispose()
    {
        compositeService = null!;
        try
        {
            compositeService?.Dispose();
        }
        catch
        {
            // ignored
        }
    }


}
