using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Json;
using System.Web;
using Api.Models;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Shared;
using Shared.Api;
using Shared.Helpers;
using Shared.Models;
using Tests.Fakers;
using Xunit.Abstractions;

namespace Tests.Integration;

public class TagTest
{
    private readonly ITestOutputHelper _output;
    private readonly HttpClient _client;
    public TagTest(ITestOutputHelper output)
    {
        _output = output;
        _client = new HttpClient{
            BaseAddress = new Uri(TestValues.ApiAdress),
        };
    }

    [Fact]
    public async Task Get_QueryTags_ShouldSucess(){

        var tags = new TagFaker().Generate(10);
        foreach (var tag in tags)
            await QuickPopulate.PostTags(_client, tag);
        
        var exclude = tags.Last().Name;
        var query = new EntityQuery{
            Page = 1,
            PageSize = 10,
            OrderBy = nameof(Tag.Name),
            Filter = $"{nameof(Tag.Name)} != \"{exclude}\"",
            Select = $"new {{ {nameof(Tag.Name)} }}"
        };

        var serializedQuery = JsonConvert.SerializeObject(query);

        var routeQuery = new NameValueCollection{
            {Values.Api.TagGetQuery, HttpUtility.UrlEncode(serializedQuery) }
        };

        
        var requestUri = Values.Api.TagGet + "?" + routeQuery.ToQuery();
        var response = await _client.GetAsync(requestUri);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var responseText = await response.Content.ReadFromJsonAsync<TagResponseDTO[]>();
        responseText.Should().NotContain(x => x.Name == exclude);
    }

    [Theory, MemberData(nameof(ValidTags))]
    public async Task Post_ValidTags_ShoundReturn201(TagRequestDTO data){
        

        var response = await _client.PostAsJsonAsync(Values.Api.TagAdd, data);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var responseData = await response.Content.ReadFromJsonAsync<TagResponseDTO>();
        responseData.Should().NotBeNull();
        responseData.Name.Normalize().Should().Be(data.Name.Normalize());


    }

    [Theory,
    InlineData("tag com espaço", HttpStatusCode.UnprocessableEntity),
    InlineData("", HttpStatusCode.UnprocessableEntity),
    InlineData("¬¬", HttpStatusCode.UnprocessableEntity),
    InlineData("☺", HttpStatusCode.UnprocessableEntity),
    InlineData("00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", HttpStatusCode.UnprocessableEntity),
    ]
    public async Task Post_InvalidTags_ShouldReturnCorrectStatusCode(string? tag, HttpStatusCode statusCode){
     

        var response = await _client.PostAsJsonAsync(Values.Api.TagAdd, new TagRequestDTO{ Name = tag });

        response.StatusCode.Should().Be(statusCode);
        
    }


    #region MemberData
    public static TheoryData<TagRequestDTO> ValidTags(){
        var count = 5;
        var faker = new TagFaker();

        var tags = faker.Generate(count);
        return new TheoryData<TagRequestDTO>(tags);
    }
    #endregion    

    
}
