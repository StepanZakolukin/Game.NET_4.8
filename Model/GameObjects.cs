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
        public CreatureCommand Command { get; private set; }
        public GameObjects(Point location, string pathToTheFile)
        {
            Picture = Image.FromFile(pathToTheFile);
            Location = location;
            Command = new CreatureCommand();
            AngleInDegrees = 90;
            DrawingPriority = 0;
        }

        public virtual void CommandAreExecuted(int x, int y) => Location = new Point(x, y);

        public virtual bool DeadInConflict(GameObjects conflictedObject)
        {
            return false;
        }
    }
}
