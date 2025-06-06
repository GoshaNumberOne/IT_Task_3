using MachineToolApp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Runtime.CompilerServices;

namespace MachineToolApp
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int _totalPartsProduced;
        public int TotalPartsProduced
        {
            get => _totalPartsProduced;
            set { _totalPartsProduced = value; OnPropertyChanged(); }
        }

        public ObservableCollection<MachineToolModel> MachineTools { get; set; }
        public ICommand AddMachineToolCommand { get; }
        public ICommand LoadMaterialCommand { get; }
        public ICommand RemoveMachineToolCommand { get; }

        private readonly Dictionary<MachineToolModel, MachineTool> _machineToolLogicMap = new();

        private readonly Technician _technician;
        private readonly PartCollector _partCollector; 

        public MainViewModel()
        {
            MachineTools = new ObservableCollection<MachineToolModel>();
            AddMachineToolCommand = new RelayCommand(AddMachineTool);
            LoadMaterialCommand = new RelayCommand(LoadMaterial);
            RemoveMachineToolCommand = new RelayCommand(RemoveMachineTool);

            _technician = new Technician();
            _partCollector = new PartCollector();

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(300); 
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (var model in MachineTools)
                        {
                            if (_machineToolLogicMap.TryGetValue(model, out MachineTool? logic))
                            {
                                model.MaterialLevel = logic.MaterialLevel;
                                model.IsEffectivelyWorking = logic.IsWorking && !logic.IsBroken;

                                if (logic.IsBroken)
                                {
                                    model.Status = "Авария! Ждет ремонта...";
                                }
                                else if (!logic.IsWorking && logic.MaterialLevel <= 0)
                                {
                                    model.Status = "Нет материала";
                                }
                                else if (!logic.IsWorking && logic.MaterialLevel > 0)
                                {
                                    model.Status = "Готов к работе / Остановлен";
                                }
                                else if (logic.IsWorking)
                                {
                                    model.Status = "Работает";
                                }
                            }
                        }
                    });
                }
            });
        }

        private void AddMachineTool(object? param)
        {
            int nextMachineNumber = 1; 

            if (MachineTools.Any())
            {
                nextMachineNumber = MachineTools
                    .Select(m => {
                        string nameWithoutPrefix = m.Name.StartsWith("Станок ") ? m.Name.Substring("Станок ".Length) : m.Name;
                        string numberPart = nameWithoutPrefix.Trim();
                        int.TryParse(numberPart, out int existingNumber);
                        return existingNumber;
                    })
                    .Where(n => n > 0) 
                    .DefaultIfEmpty(0) 
                    .Max() + 1;
            }

            var model = new MachineToolModel
            {
                Name = $"Станок {nextMachineNumber}", 
                MaterialLevel = 70, 
                Status = "Инициализация...",
                IsEffectivelyWorking = true
            };

            var machineLogic = new MachineTool(model.Name);
            _machineToolLogicMap.Add(model, machineLogic);

            machineLogic.NeedMaterial += (sender, message) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    model.Status = message;
                    model.IsEffectivelyWorking = false;
                });
            };
            machineLogic.BrokeDown += (sender, message) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    model.Status = message;
                    model.IsEffectivelyWorking = false;
                });
                _technician.HandleBreakdownEvent(sender, message);
            };
            machineLogic.Repaired += (sender, message) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    model.Status = message;
                });
            };
            machineLogic.PartProduced += (sender, count) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    model.PartsProducedCount += count;
                    TotalPartsProduced += count;
                });
                _partCollector.CollectFinishedPart(sender, count);
            };

            Application.Current.Dispatcher.Invoke(() => {
                if (machineLogic.IsBroken) model.Status = "Авария!";
                else if (!machineLogic.IsWorking && machineLogic.MaterialLevel <= 0) model.Status = "Нет материала";
                else model.Status = "Готов к работе";
            });

            MachineTools.Add(model);
        }

        private void LoadMaterial(object? param)
        {
            if (param is MachineToolModel model && _machineToolLogicMap.TryGetValue(model, out MachineTool? logic))
            {
                logic.LoadMaterial(50); 

                Application.Current.Dispatcher.Invoke(() =>
                {
                    model.MaterialLevel = logic.MaterialLevel;
                    model.Status = "Материал загружен";
                    model.IsEffectivelyWorking = !logic.IsBroken;
                });

                Task.Delay(1000).ContinueWith(t =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (logic.IsWorking && !logic.IsBroken)
                        {
                            model.Status = "Работает";
                        }
                        else if (logic.IsBroken)
                        {
                            model.Status = "Авария! Ждет ремонта...";
                        }
                        else if (logic.MaterialLevel <= 0)
                        {
                            model.Status = "Нет материала";
                        }
                    });
                });
            }
        }

        private void RemoveMachineTool(object? param)
        {
            if (param is MachineToolModel model)
            {
                if (_machineToolLogicMap.TryGetValue(model, out MachineTool? logic))
                {
                    logic.Dispose(); 
                    _machineToolLogicMap.Remove(model);
                }
                MachineTools.Remove(model);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}