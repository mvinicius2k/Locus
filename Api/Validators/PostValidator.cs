using System.Linq.Expressions;
using Api.Helpers.ValitationExtensions;
using Api.Models;
using FluentValidation;
using Shared;

namespace Api;

public class PostValidator : PostSoftValidator
{

    public const string RuleSetAdd = "Add";
    public const string RuleSetUpdate = "Update";
    private readonly IUserRepository _userRepository;
    
    public PostValidator(IDescribes describes, IUserRepository userRepository, IImageRepository imageRepository) : base(describes)
    {
        _userRepository = userRepository;

        RuleFor(post => post.OwnerId)
            .IndexExists(userRepository.GetById)
            .WithMessage(p => describes.EntityNotFound(nameof(User), p.OwnerId));
        
        When(post => post.FeaturedImageId != null, () => {

            RuleFor(post => (int) post.FeaturedImageId!)
                .IndexExists(imageRepository.GetById)
                .WithMessage(p => describes.EntityNotFound(nameof(Image), p.FeaturedImageId!));
        });
        
        
            

    }

    

    

}
