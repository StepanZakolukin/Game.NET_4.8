namespace WindowsForm.Model
{
    public class Bullet : GameObjects
    {
        public Bullet(int angleInDegrees, Point location, string pathToTheFile = @"..\..\Images\пуля.png", int drawingPriority = 1)
            : base(location, pathToTheFile, drawingPriority)
        {
            AngleInDegrees = angleInDegrees;
            Priority = 2;
            Forward();
        }

        public void Forward()
        {
            var neighbour = Model.Map[Walker.MovingForwad[AngleInDegrees % 360] + Location];

            if (!(neighbour is Wall || neighbour is Player || neighbour is Bullet))
                Delta = Walker.MovingForwad[AngleInDegrees % 360];
        }
    }
}
