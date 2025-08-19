namespace Systems.Utilities.Annotations
{
    public interface IUnmanaged<TSelf>
        where TSelf: unmanaged, IUnmanaged<TSelf>
    {
        
    }
}