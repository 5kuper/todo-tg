namespace Utilities.Application
{
    public interface IPatch<T>
    {
        public void Apply(T target);
    }
}
