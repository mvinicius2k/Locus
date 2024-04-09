using System.Net.Http.Json;
using Bogus;
using Shared;
using Shared.Models;

namespace Tests;

public static class QuickPopulate
{
    public static async Task<TagResponseDTO> PostTags(HttpClient http, TagRequestDTO tag)
    {

        var response = await http.PostAsJsonAsync(Values.Api.TagCreate, tag);
        if(!response.IsSuccessStatusCode)
            Assert.Fail(response.StatusCode.ToString());
        return await response.Content.ReadFromJsonAsync<TagResponseDTO>();
    }
}
