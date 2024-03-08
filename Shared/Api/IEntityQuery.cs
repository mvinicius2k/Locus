using System.Runtime.Serialization;

namespace Shared.Api
{
    public interface IEntityQuery 
    {
        public int Page {get;}
        public int PageSize {get;}
        public string? Filter {get;}
        public string? OrderBy {get;}
        public string[] Expand {get;}
        public string? Select {get;}
        
    }
}
