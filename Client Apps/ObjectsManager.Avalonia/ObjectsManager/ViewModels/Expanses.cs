using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.ViewModels
{
    public class Expanses
    {
        private string _name;
        public string Name { get => _name; set => _name = value; }
        
        private double _expectedExp;
        public double ExpectedExp { get => _expectedExp; set => _expectedExp = value; }
        
        private double _actualExp;
        public double ActualExp { get => _actualExp; set => _actualExp = value; }

        private double _exaggeration;
        public double Exaggeration { get => _exaggeration; set => _exaggeration = value; }
        public ObservableCollection<Expanses> Children { get; } = new();

        public Expanses(string name, double expectedExp, double actualExp, double exaggeration)
        {
            Name = name;
            ExpectedExp = expectedExp;
            ActualExp = actualExp;
            Exaggeration = exaggeration;
        }
    }
}
