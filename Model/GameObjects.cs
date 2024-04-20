using System.Drawing;

namespace WindowsForm.Model
{
    public class GameObjects
    {
        public readonly int Priority;
        public readonly Image Picture;
        public int AngleInDegrees { get; protected set; }
        public Point Location { get; protected set; }
        public Point Delta { get; protected set; }
        public int Health { get; protected set; }
        public GameObjects(Point location, string pathToTheFile, int health, int priority = 0)
        {
            Picture = Image.FromFile(pathToTheFile);
            Location = location;
            Delta = new Point(0, 0);
            AngleInDegrees = 90;
            Health = health;
            Priority = priority;
        }

        public virtual void DeductDamage() => Health = Health;
        public virtual void CommandAreExecuted(int x, int y) => Location = new Point(x, y);

        public virtual bool DeadInConflict(GameObjects gameObjects) => false;
    }
}
