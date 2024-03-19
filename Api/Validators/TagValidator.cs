using FluentValidation;
using Shared;

namespace Api;

public class TagValidator : Shared.TagSoftValidator
{
    

    public TagValidator(IDescribes describes, ITagRepository repository) : base(describes)
    {
        RuleFor(t => t.Name)
            .MustAsync(async (name,token) => (await repository.GetById(name.ToLower())) == null);

    }
}