namespace Api;

[System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class AuthRequiredAttribute : System.Attribute
{

    public AuthRequiredAttribute()
    {
    }
    
   
}