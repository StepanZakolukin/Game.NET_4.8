namespace WindowsForm.Model
{
    public class Bullet : GameObjects
    {
        public Bullet(int angleInDegrees, Point location, string pathToTheFile = @"..\..\Images\Bullet.png", int drawingPriority = 1)
            : base(location, pathToTheFile, drawingPriority)
        {
            AngleInDegrees = angleInDegrees;
            Priority = 2;
            Forward();
        }

        public void Forward() => Delta = Walker.MovingForwad[AngleInDegrees % 360];

        public override bool DeadInConflict(GameObjects gameObjects)
            => !(gameObjects is Bullet) && !(gameObjects is Stone);
    }
}
