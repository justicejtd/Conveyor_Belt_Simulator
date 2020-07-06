using System.Collections.Generic;

namespace ProcP.Models
{
    public class CurrentState
    {
        public string Flights { get; set; }
        public string Carts { get; set; }
        public string Employees { get; set; }
        public List<Belt> Belts { get; set; }
        public List<double> CartsValues { get; set; }
        public int estimateEmployees { get; set; }
        public int actualEmployees { get; set; }
        public List<double> TotalCartValues { get; set; }

        public CurrentState()
        {
            CartsValues = new List<double>();
            TotalCartValues = new List<double>();
        }
    }
}
