using System.Drawing;

namespace WindowsForm.Model
{
    public class GameObjects
    {
        public int Priority;
        public int DrawingPriority { get; set; }
        public readonly Image Picture;
        public int AngleInDegrees { get; set; }
        public Point Location { get; set; }
        public Point Delta { get; set; }
        public GameObjects(Point location, string pathToTheFile, int drawingPriority = 0)
        {
            Picture = Image.FromFile(pathToTheFile);
            Location = location;
            Delta = new Point(0, 0);
            AngleInDegrees = 90;
            DrawingPriority = drawingPriority;
        }

        public virtual void CommandAreExecuted(int x, int y) => Location = new Point(x, y);
    }
}
