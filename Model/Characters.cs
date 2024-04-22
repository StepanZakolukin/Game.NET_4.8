namespace WindowsForm.Model
{
    public class Characters : GameObjects
    {
        public Characters(Point location, string pathToTheFile, int health, int angleInDegrees = 90) : base(location, pathToTheFile, health, 1, angleInDegrees)
        {
        }

        public override void DeductDamage() => Health--;

        public override void CommandAreExecuted(int x, int y)
        {
            Delta = new Point(0, 0);
            Location = new Point(x, y);
        }

        public void Shoot(GameModel model) 
            => model.Map[Walker.MovingForwad[AngleInDegrees % 360] + Location].Add(new Bullet(AngleInDegrees, Location));
    }
}
