namespace Api.Models;

public interface IModel<T>
{
    public T GetId();
    public void SetId(T value);
}
