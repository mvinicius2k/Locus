using FluentValidation;

namespace Shared;

public class PostSoftValidator : AbstractValidator<PostRequestDTO>
{


    public PostSoftValidator(IDescribes describes){
        RuleFor(p => p.Title)
            .NotEmpty()
            .MaximumLength(Values.Entity.PostTitleMaxLength)
            .WithMessage(describes.NotEmptyOrMaxLength(Values.Entity.PostTitleMaxLength));
        RuleFor(p => p.Content)
            .NotEmpty()
            .MaximumLength(Values.Entity.PostContentMaxLength)
            .WithMessage(describes.NotEmptyOrMaxLength(Values.Entity.PostContentMaxLength));
        RuleFor(p => p.Summary)
            .NotEmpty()
            .MaximumLength(Values.Entity.PostSummaryMaxLength)
            .WithMessage(describes.NotEmptyOrMaxLength(Values.Entity.PostSummaryMaxLength));
        
    }
}
