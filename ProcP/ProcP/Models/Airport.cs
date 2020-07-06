using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;


namespace ProcP.Models
{
    public class Airport
    {
        private int nrOfCarts;
        private int nrOfEmployees;
        private List<Belt> departureGates;
        private List<Belt> checkInGates;

        public int NrOfFlights { get; set; }
        public List<int> endingIndexes { get; set; }
        public List<int> startingIndexes { get; set; }

        public Airport()
        {

        }

        public Airport(int nrOfFlights, int nrOfCarts, int nrOfEmployees)
        {
            this.NrOfFlights = nrOfFlights;
            this.nrOfCarts = nrOfCarts;
            this.nrOfEmployees = nrOfEmployees;
            checkInGates = new List<Belt>();
            departureGates = new List<Belt>();
            endingIndexes = new List<int>();
            startingIndexes = new List<int>();
            SetGates();
        }

        private void SetGates()
        {
            //Add checkIn gates
            Belt belt = new Belt(new Point(285, 105));
            belt.getPath().Fill = Brushes.Yellow;
            checkInGates.Add(belt);

            belt = new Belt(new Point(285, 255));
            belt.getPath().Fill = Brushes.DarkBlue;
            checkInGates.Add(belt);

            belt = new Belt(new Point(285, 405));
            belt.getPath().Fill = Brushes.Red;
            checkInGates.Add(belt);
            //checkInGates.Add(new Gate(new Point(285, 555)));
            //checkInGates.Add(new Gate(new Point(285, 705)));

            //Add departure gates
            belt = new Belt(new Point(985, 105));
            belt.getPath().Fill = Brushes.Yellow;
            departureGates.Add(belt);

            belt = new Belt(new Point(985, 255));
            belt.getPath().Fill = Brushes.DarkBlue;
            departureGates.Add(belt);

            belt = new Belt(new Point(985, 405));
            belt.getPath().Fill = Brushes.Red;
            departureGates.Add(belt);
            //departureGates.Add(new Gate(new Point(985, 555)));
            //departureGates.Add(new Gate(new Point(985, 705)));
        }

        public List<Belt> GetAllCheckInGates()
        {
            return checkInGates;
        }

        public List<Belt> GetAlldepartureGates()
        {
            return departureGates;
        }

        public int GetNrOfLuggagePerCheckIn()
        {
            return (NrOfFlights / checkInGates.Count) * 25;
        }
    }
}
