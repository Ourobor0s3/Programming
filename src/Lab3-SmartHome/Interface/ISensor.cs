namespace Lab3.Interface
{
    public interface ISensor
    {
        bool TryRead(out decimal value);
    }
}