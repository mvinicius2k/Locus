using FluentValidation;
using Shared.Models;

namespace Shared;

public class TagValidator : AbstractValidator<TagRequestDTO>
{
    public TagValidator(IDescribes describes){
        RuleFor(t => t.Name)
            .NotEmpty()
            .MaximumLength(Values.Entity.TagNameMaxLength)
            .WithMessage(describes.NotEmptyOrMaxLength(Values.Entity.TagNameMaxLength));
    }
}
