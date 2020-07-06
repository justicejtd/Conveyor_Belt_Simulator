using System.Windows;

namespace ProcP.Models
{
    public class Gate : Belt
    {
        private int luggageCount;

        public Gate(Point location) : base(location)
        {

        }

        public void AddLuggage()
        {
            luggageCount++;
        }

        public void SendCartToPlane()
        {

        }
    }
}
