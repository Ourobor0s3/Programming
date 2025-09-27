namespace SmartHome.Interface
{
    public interface ISensor
    {
        bool TryRead(out decimal value);
    }
}