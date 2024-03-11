using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Json;
using System.Web;
using Api.Models;
using FluentAssertions;
using Newtonsoft.Json;
using Shared;
using Shared.Api;
using Shared.Helpers;
using Shared.Models;
using Tests.Fakers;

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

    [Theory, MemberData(nameof(ValidTags))]
    public async Task Post_ValidTags_ShoundReturn201(TagRequestDTO data){
        var client = new HttpClient{
            BaseAddress = new Uri(TestValues.ApiAdress),
        };

        var response = await client.PostAsJsonAsync(Values.Api.TagAdd, data);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var responseData = await response.Content.ReadFromJsonAsync<TagResponseDTO>();
        responseData.Should().NotBeNull();
        responseData.Name.Normalize().Should().Be(data.Name.Normalize());
    }


    #region MemberData
    public static IEnumerable<object[]> ValidTags(){
        var count = 10;
        var faker = new TagFaker();
        var tags = faker.Generate(count);

        foreach (var item in tags)
        {
            yield return new object[] { item };
        }
    }
    #endregion    

    
}
