namespace MachineToolApp
{
    public interface ITechnician
    {
        void HandleBreakdownEvent(object? sender, string message);
    }
}