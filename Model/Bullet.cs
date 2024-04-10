namespace WindowsForm.Model
{
    public class Bullet : GameObjects
    {
        public Bullet(int angleInDegrees, Point location, string pathToTheFile = @"..\..\Images\пуля.png") : base(location, pathToTheFile)
        {
            AngleInDegrees = angleInDegrees;
            DrawingPriority = 1;
            Priority = 100;
            Forward();
        }

        public void Forward()
        {
            var neighbour = Model.Map[Walker.MovingForwad[AngleInDegrees % 360] + Location];

            if (!(neighbour is Wall || neighbour is Soldier || neighbour is Bullet))
                Command.Delta = Walker.MovingForwad[AngleInDegrees % 360];
        }
    }
}
