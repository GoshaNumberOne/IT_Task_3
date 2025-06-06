using System.Windows; 

namespace MachineToolApp
{
    public class Technician : ITechnician
    {
        public void HandleBreakdownEvent(object? sender, string message)
        {
            if (sender is MachineTool machine)
            {
                Task.Delay(5000).ContinueWith(t =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        machine.FinishRepair();
                    });
                });
            }
        }
    }
}