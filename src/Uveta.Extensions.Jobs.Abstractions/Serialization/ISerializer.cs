namespace Uveta.Extensions.Jobs.Abstractions.Serialization
{
    public interface ISerializer<T>
    {
        string Serialize(T instance);
        T Deserialize(string value);
    }
}
