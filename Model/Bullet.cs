namespace WindowsForm.Model
{
    public class Bullet : GameObjects
    {
        public Bullet(int angleInDegrees, Point location, GameModel model, string pathToTheFile = @"..\..\Images\Bullet.png", int drawingPriority = 1)
            : base(location, pathToTheFile, drawingPriority)
        {
            AngleInDegrees = angleInDegrees;
            Priority = 2;
            Forward(model);
        }

        public void Forward(GameModel model)
        {
            var neighbour = model.Map[Walker.MovingForwad[AngleInDegrees % 360] + Location];

            if (!(neighbour is Wall || neighbour is Player || neighbour is Bullet))
                Delta = Walker.MovingForwad[AngleInDegrees % 360];
        }
    }
}
