using System.Collections.Immutable;

namespace Shared;

public class ModelState
{
    public readonly HashSet<IValidationDescription> Errors = [];
    public bool IsValid => Errors.Count != 0;

    public Dictionary<string, List<string>> GetAsPair()
    {
        var dict = new Dictionary<string, List<string>>();

        foreach (var error in Errors)
        {
            if(dict.TryGetValue(error.PropertyName, out var list))
                list.Add(error.Message);
            else
                dict[error.PropertyName] = new List<string>();
        }

        return dict;
    }
}
