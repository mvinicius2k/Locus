using FluentValidation;
using Microsoft.AspNetCore.Http;
using Shared;

namespace Api;

public class TagValidator : Shared.TagSoftValidator
{
    public const string RuleSetAdd = "Add";
    public const string RuleSetUpdate = "Update";
    private readonly ITagRepository _repository;
    public TagValidator(IDescribes describes, ITagRepository repository) : base(describes)
    {
        _repository = repository;   
        //RuleFor(t => t).NotNull().WithErrorCode(StatusCodes.Status404NotFound.ToString());
        RuleSet(RuleSetAdd, () =>
        {
            
            RuleFor(t => t.Name)
                .MustAsync(async (name, token) => (await repository.GetByName(name.ToLower())) == null)
                .WithMessage(t => describes.UnavaliableUserName(t.Name.ToLower()));
        });
        RuleSet(RuleSetUpdate, () =>
        {
            RuleFor(t => t.Name)
                .MustAsync(async (name, token) => await CanUpdate(name.ToLower()))
                .WithMessage(t => describes.UnavaliableUserName(t.Name.ToLower()));
        });

    }

    private async ValueTask<bool> CanUpdate(string newName){
        newName = newName.ToLower();
        var existing = await _repository.GetByName(newName);
        return existing == null || existing.Name == newName;
    }
}