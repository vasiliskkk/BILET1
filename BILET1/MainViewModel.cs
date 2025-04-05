using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace BILET1
{

    public class MainViewModel : INotifyPropertyChanged
    {
        private TransportProblem _problem;
        private string _resultText;

        public ObservableCollection<double> Supply { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> Demand { get; } = new ObservableCollection<double>();
        public ObservableCollection<ObservableCollection<double>> Costs { get; } = new ObservableCollection<ObservableCollection<double>>();

        public string ResultText
        {
            get => _resultText;
            set { _resultText = value; OnPropertyChanged(); }
        }

        public ICommand SolveCommand { get; }
        public ICommand AddSupplyCommand { get; }
        public ICommand AddDemandCommand { get; }

        public MainViewModel()
        {
            SolveCommand = new RelayCommand(SolveProblem);
            AddSupplyCommand = new RelayCommand(AddSupply);
            AddDemandCommand = new RelayCommand(AddDemand);

            // Инициализация с тестовыми данными
            Supply.Add(300);
            Supply.Add(400);
            Supply.Add(500);

            Demand.Add(250);
            Demand.Add(350);
            Demand.Add(400);
            Demand.Add(200);

            for (int i = 0; i < Supply.Count; i++)
            {
                var row = new ObservableCollection<double>();
                for (int j = 0; j < Demand.Count; j++)
                {
                    row.Add(10 + i + j); // Произвольные стоимости
                }
                Costs.Add(row);
            }
        }

        private void SolveProblem()
        {
            try
            {
                _problem = new TransportProblem
                {
                    Supply = Supply.ToArray(),
                    Demand = Demand.ToArray(),
                    Costs = new double[Supply.Count, Demand.Count]
                };

                for (int i = 0; i < Supply.Count; i++)
                    for (int j = 0; j < Demand.Count; j++)
                        _problem.Costs[i, j] = Costs[i][j];

                _problem.CheckBalanced();
                _problem.SolveByNorthWestCorner();

                DisplayResults();
            }
            catch (Exception ex)
            {
                ResultText = $"Ошибка: {ex.Message}";
            }
        }

        private void DisplayResults()
        {
            var sb = new StringBuilder();

            sb.AppendLine(_problem.IsBalanced ? "Сбалансированная задача" : "Несбалансированная задача");
            sb.AppendLine();

            sb.AppendLine("Опорный план:");
            for (int i = 0; i < _problem.Solution.GetLength(0); i++)
            {
                for (int j = 0; j < _problem.Solution.GetLength(1); j++)
                {
                    sb.Append($"{_problem.Solution[i, j],8:0.##}");
                }
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine($"Общая стоимость: {_problem.TotalCost:0.##}");

            ResultText = sb.ToString();
        }

        private void AddSupply()
        {
            Supply.Add(0);
            var row = new ObservableCollection<double>();
            for (int j = 0; j < Demand.Count; j++) row.Add(0);
            Costs.Add(row);
        }

        private void AddDemand()
        {
            Demand.Add(0);
            foreach (var row in Costs) row.Add(0);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}