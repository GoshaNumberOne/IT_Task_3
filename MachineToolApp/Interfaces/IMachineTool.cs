namespace MachineToolApp
{
    public interface IMachineTool
    {
        string Name { get; }
        void LoadMaterial(int amount);
    }
}