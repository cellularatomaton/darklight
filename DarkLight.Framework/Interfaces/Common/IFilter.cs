namespace DarkLight.Framework.Interfaces.Common
{
    public interface IFilter<T>
    {
        bool IsPassedBy(T message);
    }
}