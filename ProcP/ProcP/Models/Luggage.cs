using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProcP.Models
{
    public class Luggage
    {
        private Point startPoint;
        private EllipseGeometry animatedEllipseGeometry;
        private Path ellipsePath;
        public bool IsInBelt { get; set; }
        public int Id { get; private set; }

        public Luggage(Point startPoint)
        {
            IsInBelt = false;
            this.startPoint = startPoint;
            this.animatedEllipseGeometry = new EllipseGeometry(new Rect(startPoint, new Size(20, 20)));
            ellipsePath = new Path();
            ellipsePath.Data = animatedEllipseGeometry;
            ellipsePath.Fill = Brushes.Blue;
        }

        public Luggage(Point startPoint, int id)
        {
            this.Id = id;
            IsInBelt = false;
            this.startPoint = startPoint;
            this.animatedEllipseGeometry = new EllipseGeometry(new Rect(startPoint, new Size(20, 20)));
            
            ellipsePath = new Path();
            ellipsePath.Data = animatedEllipseGeometry;
            ellipsePath.Fill = Brushes.Blue;
        }

        public EllipseGeometry GetELlipseGeometry()
        {
            return this.animatedEllipseGeometry;
        }

        public Path GetPath()
        {
            return this.ellipsePath;
        }
    }
}
