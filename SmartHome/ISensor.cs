namespace SmartHome
{
    public interface ISensor
    {
        bool TryRead(out decimal value);
    }
}