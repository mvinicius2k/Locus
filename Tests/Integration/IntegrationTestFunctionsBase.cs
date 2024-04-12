using Xunit.Abstractions;

namespace Tests.Integration;

public abstract class IntegrationTestFunctionsBase : IClassFixture<TestcontainerFixture>, IAsyncLifetime{
    protected readonly ITestOutputHelper _output;
    protected readonly TestcontainerFixture _testcontainer;
    protected HttpClient _client;

    protected IntegrationTestFunctionsBase(ITestOutputHelper output, TestcontainerFixture testcontainer)
    {
        _output = output;
        _testcontainer = testcontainer;
    }

    public async Task InitializeAsync()
    {
        _client = await _testcontainer.GetClientWhenDone();
    }

    public Task DisposeAsync()
    {
        
        return Task.CompletedTask;
    }
}
