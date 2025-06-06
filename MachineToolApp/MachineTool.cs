
namespace MachineToolApp
{
    public class MachineTool : IMachineTool, IDisposable
    {
        public event EventHandler<string>? NeedMaterial;
        public event EventHandler<string>? BrokeDown;
        public event EventHandler<string>? Repaired; 
        public event EventHandler<int>? PartProduced; 

        private Random _rnd = new Random();
        private int _materialLevel = 100; 
        private bool _isWorking = true;
        private bool _isBroken = false; 
        private readonly string _name;

        public string Name => _name;

        public int MaterialLevel 
        {
            get => _materialLevel;
            private set => _materialLevel = value;
        }

        public bool IsWorking => _isWorking;
        public bool IsBroken => _isBroken;

        public MachineTool(string name)
        {
            _name = name;
            _materialLevel = 70; 
            StartProductionCycle();
        }

        private async void StartProductionCycle()
        {
            await Task.Run(async () =>
            {
                while (_isWorking || _isBroken) 
                {
                    await Task.Delay(1500); 

                    if (_isBroken)
                    {
                        continue;
                    }

                    if (!_isWorking)
                    {
                        continue;
                    }
                    
                    if (_materialLevel > 0)
                    {
                        _materialLevel -= 10; 
                        PartProduced?.Invoke(this, 1); 
                    }

                    if (_materialLevel <= 0)
                    {
                        NeedMaterial?.Invoke(this, $"{_name}: Закончился материал!");
                        _materialLevel = 0;
                        _isWorking = false; 
                    }

                    if (_isWorking && _materialLevel >= 0 && _rnd.Next(100) < 10) 
                    {
                        _isWorking = false; 
                        _isBroken = true;
                        BrokeDown?.Invoke(this, $"{_name}: Произошла авария!");
                    }
                    
                    if (!_isBroken && _materialLevel <= 0) 
                    {
                        _isWorking = false; 
                    }
                }
            });
        }

        public void LoadMaterial(int amount)
        {
            _materialLevel += amount;
            if (_materialLevel > 100) _materialLevel = 100;

            if (_materialLevel > 0 && !_isBroken) 
            {
                _isWorking = true; 
            }
        }

        public void FinishRepair() 
        {
            _isBroken = false;
            if (_materialLevel > 0) 
            {
                _isWorking = true;
            }
            else
            {
                _isWorking = false; 
                NeedMaterial?.Invoke(this, $"{_name}: Ремонт завершен, но нет материала.");
            }
            Repaired?.Invoke(this, $"{_name}: Ремонт завершён");
        }

        public void Dispose()
        {
            _isWorking = false; 
            _isBroken = false; 
            NeedMaterial = null;
            BrokeDown = null;
            Repaired = null;
            PartProduced = null;
        }
    }
}