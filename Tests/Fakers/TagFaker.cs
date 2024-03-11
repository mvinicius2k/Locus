using Bogus;
using Shared;
using Shared.Models;

namespace Tests.Fakers;

public class TagFaker : Faker<TagRequestDTO>
{
    public TagFaker(){
        RuleFor(t => t.Name, f => f.Lorem.Word().Substring(0, Values.Entity.TagNameMaxLength-1));
    }
}
