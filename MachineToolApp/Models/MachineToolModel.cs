using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MachineToolApp.Models 
{
    public class MachineToolModel : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private int _materialLevel; 
        private string _status = string.Empty;
        private int _partsProducedCount; 
        private bool _isEffectivelyWorking = true; 

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public int MaterialLevel 
        {
            get => _materialLevel;
            set { _materialLevel = value; OnPropertyChanged(); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public int PartsProducedCount 
        {
            get => _partsProducedCount;
            set { _partsProducedCount = value; OnPropertyChanged(); }
        }

        public bool IsEffectivelyWorking 
        {
            get => _isEffectivelyWorking;
            set { _isEffectivelyWorking = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}