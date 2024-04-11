using System.Text.Json;
using FluentValidation.Results;

namespace Shared;


public class ModelState
{
    public readonly HashSet<IValidationDescription> Errors = [];
    public bool IsValid => Errors.Count == 0;

    public void AppendErrors(ValidationResult validationResult){
        foreach (var error in validationResult.Errors)
        {
            Errors.Add(new ValidationDescription{
                PropertyName = error.PropertyName,
                Message = error.ErrorMessage
            });
        }
        
    }

    public IEnumerable<object> GroupByProperty()
    {
        var grouped = Errors.GroupBy(e => e.PropertyName, e => e.Message , (key, value) => new {
            PropertyName = key,
            Message = value
        });
        return grouped;
    }

    public Dictionary<string, List<string>> AsPropertyAndMessages(){
        var dict = new Dictionary<string, List<string>>();
        foreach (var error in Errors)
            if(dict.ContainsKey(error.PropertyName))
                dict[error.PropertyName].Add(error.Message);
            else
                dict[error.PropertyName] = new List<string>(){ error.Message };
        return dict;
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
            Message = err.ErrorMessage,
            
        });
        return new ModelState(errors);

    }


}
