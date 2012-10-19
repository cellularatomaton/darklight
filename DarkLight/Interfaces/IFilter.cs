namespace DarkLight.Interfaces
{
    public interface IFilter<T>
    {
        bool IsPassedBy(T message);
    }
}