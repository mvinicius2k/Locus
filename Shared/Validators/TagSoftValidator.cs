﻿using FluentValidation;
using Shared.Models;

namespace Shared;

public class TagSoftValidator : AbstractValidator<TagRequestDTO>
{
    public TagSoftValidator(IDescribes describes){
        RuleFor(t => t.Name)
            .NotEmpty()
            .MaximumLength(Values.Entity.TagNameMaxLength)
            .WithMessage(describes.NotEmptyOrMaxLength(Values.Entity.TagNameMaxLength))
            .Matches(@"^[a-zA-Z0-9\p{L}][a-zA-Z0-9\p{L}+#-]*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            .WithMessage(describes.InvalidText("Somente letras, números e caracteres #,+,- não no início"));
    }
}
