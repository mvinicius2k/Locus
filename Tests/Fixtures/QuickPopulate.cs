using Bogus;
using Shared;
using Shared.Models;

namespace Tests;

public static class QuickPopulate
{
    public static async Task PostTags(HttpClient http, TagRequestDTO tag)
    {

        var response = await http.PostAsJsonAsync(Values.Api.TagAdd, tag);
        if(!response.IsSuccessStatusCode)
            Assert.Fail(response.StatusCode.ToString());
    }
}
