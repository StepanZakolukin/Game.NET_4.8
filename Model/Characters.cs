namespace WindowsForm.Model
{
    public class Characters : GameObjects
    {
        public Characters(Point location, string pathToTheFile) : base(location, pathToTheFile)
        {
        }

        public override void CommandAreExecuted(int x, int y)
        {
            Command.Delta = new Point(0, 0);
            Location = new Point(x, y);
        }

        public void Shoot()
        {
            if (!(Model.Map[Walker.MovingForwad[AngleInDegrees % 360] + Location] is Wall))
                Model.Map[Walker.MovingForwad[AngleInDegrees % 360] + Location] = new Bullet(AngleInDegrees, Location);
        }
    }
}
