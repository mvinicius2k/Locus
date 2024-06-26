﻿using System.Collections.Specialized;
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

public class TagTest : IClassFixture<TestcontainerFixture>, IAsyncLifetime
{
    private readonly ITestOutputHelper _output;
    private readonly TestcontainerFixture _testcontainer;
    private HttpClient _client;
    public TagTest(ITestOutputHelper output, TestcontainerFixture testcontainer)
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

    [Fact]
    public async Task Get_QueryTags_ShouldSucess()
    {

        var tags = new TagFaker().Generate(10);
        foreach (var tag in tags)
            await QuickPopulate.PostTags(_client, tag);

        var exclude = tags.Last().Name;
        var query = new EntityQuery
        {
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

    [Theory,
    InlineData("tagVálida"),
    InlineData("123"),
    InlineData("sfdgdfh"),
    InlineData("C#"),
    InlineData("k")]
    public async Task Post_ValidTags_ShoundReturn201(string data)
    {


        var response = await _client.PostAsJsonAsync(Values.Api.TagCreate, new TagRequestDTO { Name = data });
        var dat = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var responseData = await response.Content.ReadFromJsonAsync<TagResponseDTO>();
        responseData.Should().NotBeNull();
        responseData.Name.Normalize().Should().Be(data.ToLower());


    }

    [Theory,
    InlineData("tag com espaço", HttpStatusCode.UnprocessableEntity),
    InlineData("", HttpStatusCode.UnprocessableEntity),
    InlineData("¬¬", HttpStatusCode.UnprocessableEntity),
    InlineData("☺", HttpStatusCode.UnprocessableEntity),
    InlineData("00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", HttpStatusCode.UnprocessableEntity),
    ]
    public async Task Post_InvalidTags_ShouldReturnCorrectStatusCode(string? tag, HttpStatusCode statusCode)
    {


        var response = await _client.PostAsJsonAsync(Values.Api.TagCreate, new TagRequestDTO { Name = tag });

        response.StatusCode.Should().Be(statusCode);

    }

    [Theory,
    InlineData("tagVálida", "outraVálida"),
    InlineData("123", "1234"),
    InlineData("sfdgdfh", "dgfhfj"),
    InlineData("C#", "C++"),
    InlineData("k", "k")]
    public async Task Rename_Tag_ShouldReturn200(string original, string edit)
    {
        await QuickPopulate.PostTags(_client, new TagRequestDTO { Name = original });

        var response = await _client.PutAsJsonAsync(Values.Api.TagRename.Placeholder(HttpUtility.UrlEncode(original)), new TagRequestDTO { Name = edit });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await response.Content.ReadFromJsonAsync<TagResponseDTO>();
        data.Should().NotBeNull();
        data.Name.Should().Be(edit.ToLower());

    }


    #region MemberData
    public static TheoryData<TagRequestDTO> ValidTags()
    {
        var count = 5;
        var faker = new TagFaker();

        var tags = faker.Generate(count);
        return new TheoryData<TagRequestDTO>(tags);
    }


    #endregion


}
