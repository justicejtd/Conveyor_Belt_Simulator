using DocumentFormat.OpenXml.Office2016.Drawing.Charts;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProcP.Models
{
    public class Belt
    {
        protected Point centerPoint;
        public bool IsBeltActive { get; set; }
        protected RectangleGeometry rectangleGeometry;
        protected Path drawingPath;
        public int Weight { get; set; }
        public string Name { get; set; }
        public Belt Parent { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public Point location { get; set; } //save xml file
        private MonitorLuggages monitor;
        public static int uniqueid = 0;
        public int Id { get; set; }

        public Belt() {
            monitor = new MonitorLuggages();
        }
        public Belt(Point location)
        {
            IsBeltActive = false;
            monitor = new MonitorLuggages();
            this.location = location;
            this.drawingPath = new Path();
            this.rectangleGeometry = new RectangleGeometry();
            drawingPath.Fill = Brushes.Transparent;
            drawingPath.StrokeThickness = 2;
            drawingPath.Stroke = Brushes.Black;
            Id = uniqueid;
            Belt.uniqueid += 1;

            Rect rect = new Rect(location, new Size(50, 50));
            this.rectangleGeometry.Rect = rect;
            this.drawingPath.Data = this.rectangleGeometry;
            this.centerPoint = new Point(rectangleGeometry.Rect.X + 25, rectangleGeometry.Rect.Y + 25);
        }

        public Point GetCenterPoint()
        {
            return this.centerPoint;
        }

        public Path getPath()
        {
            return this.drawingPath;
        }

        public RectangleGeometry GetRectangleGeometry()
        {
            return this.rectangleGeometry;
        }

        public MonitorLuggages GetMonitor()
        {
            return this.monitor;
        }
    }
}
