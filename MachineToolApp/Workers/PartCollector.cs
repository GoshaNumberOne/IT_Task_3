using System.Diagnostics; 

namespace MachineToolApp
{
    public class PartCollector : ILoader
    {
        public int TotalPartsCollected { get; private set; }

        public void CollectFinishedPart(object? sender, int partCount)
        {
            if (sender is MachineTool machine)
            {
                TotalPartsCollected += partCount;
                Debug.WriteLine($"Loader collected {partCount} part(s) from {machine.Name}. Total collected by this loader: {TotalPartsCollected}");
            }
        }
    }
}