namespace WindowsForm.Model
{
    public class Characters : GameObjects
    {
        public Characters(Point location, string pathToTheFile) : base(location, pathToTheFile)
        {
        }

        public void CommandAreExecuted() => Command.Delta = new Point(0, 0);

        public void Shoot()
        {
            if (!(Model.Map[Walker.MovingForwad[AngleInDegrees % 360] + Location] is Wall))
                Model.Map[Walker.MovingForwad[AngleInDegrees % 360] + Location] = new Bullet(AngleInDegrees, Location);
        }
    }
}
