namespace WindowsForm.Model
{
    public class Characters : GameObjects
    {
        public Characters(Point location, string pathToTheFile, int drawingPriority = 1) : base(location, pathToTheFile, drawingPriority)
        {
            Priority = 1;
        }

        public override void CommandAreExecuted(int x, int y)
        {
            Delta = new Point(0, 0);
            Location = new Point(x, y);
        }

        public void Shoot()
        {
            if (!(Model.Map[Walker.MovingForwad[AngleInDegrees % 360] + Location] is Wall))
                Model.Map[Walker.MovingForwad[AngleInDegrees % 360] + Location] = new Bullet(AngleInDegrees, Location);
        }
    }
}
