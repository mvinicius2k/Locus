using System.Text.Json;
using FluentValidation.Results;

namespace Shared;

public class ModelState
{
    public readonly HashSet<IValidationDescription> Errors = [];
    public bool IsValid => Errors.Count != 0;

    public IEnumerable<object> GroupByProperty()
    {
        var grouped = Errors.GroupBy(e => e.PropertyName, e => e.Message , (key, value) => new {
            PropertyName = key,
            Message = value
        });
        return grouped;
    }

    public ModelState(IEnumerable<IValidationDescription> errors)
    {
        Errors = new HashSet<IValidationDescription>(errors);
    }
    public ModelState()
    {
        Errors = new HashSet<IValidationDescription>();
    }

    public static ModelState FromValidationResult(ValidationResult validationResult)
    {
        var errors = validationResult.Errors.Select(err => new ValidationDescription
        {
            PropertyName = err.PropertyName,
            Message = err.ErrorMessage
        });
        return new ModelState(errors);

    }


}
