using Bogus;
using Shared;

namespace Tests;

public class PostFaker : Faker<PostRequestDTO>
{
    public PostFaker(Dictionary<string, int[]> ownersAndTheirImages){
        RuleFor(p => p.Title, f => f.Lorem.Sentence());
        RuleFor(p => p.Content, f => f.Lorem.Paragraph());
        RuleFor(p => p.Summary, f => f.Lorem.Lines(1));
        
        var owner = FakerHub.PickRandom(ownersAndTheirImages.Keys.ToArray());

        RuleFor(p => p.OwnerId, f => owner);
        RuleFor(p => p.FeaturedImageId, f => f.PickRandom(ownersAndTheirImages[owner]));

    }
}
