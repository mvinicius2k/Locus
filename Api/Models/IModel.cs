namespace Api.Models;

public interface IEntity<T>
{
    public T GetId();
    public void SetId(T value);
}
