using Bogus;
using Humanizer;
using Shared;
using Shared.Helpers;
using Shared.Models;

namespace Tests.Fakers;

public class TagFaker : Faker<TagRequestDTO>
{
    public TagFaker(){
        RuleFor(t => t.Name, f => Guid.NewGuid().ToSafeBase64('+', '#'));
    }
}
