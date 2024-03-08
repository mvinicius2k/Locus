using System.Collections.Specialized;
using System.Net.Http.Json;
using System.Web;
using Api.Models;
using FluentAssertions;
using Newtonsoft.Json;
using Shared;
using Shared.Api;
using Shared.Helpers;
using Shared.Models;

namespace Tests.Integration;

public class TagTest
{
    [Fact]
    public async Task Get_QueryTags_ShouldSucess(){
        var query = new EntityQuery{
            Page = 1,
            PageSize = 10,
            OrderBy = nameof(Tag.Name),
            Filter = $"{nameof(Tag.Name)} != 'Protegido'",
            Select = $"new {{ {nameof(Tag.Name)} }}"
        };

        var serializedQuery = JsonConvert.SerializeObject(query);

        var routeQuery = new NameValueCollection{
            {Values.Api.TagGetQuery, HttpUtility.UrlEncode(serializedQuery) }
        };

        var client = new HttpClient{
            BaseAddress = new Uri($"{TestValues.ApiAdress}/{Values.Api.TagGet}"),
        };

        var response = await client.GetAsync(routeQuery.ToQueryWithPrefix());
        var responseText = response.Content.ReadFromJsonAsync<TagResponseDTO[]>();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        responseText.Should().NotBeNull();
        

    }
}
