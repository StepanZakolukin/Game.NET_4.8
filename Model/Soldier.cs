namespace WindowsForm.Model
{
    public class Soldier : Characters
    {
        public Soldier(int angleInDegrees, Point location, string pathToTheFile = @"..\..\Images\солдат.png") : base(location, pathToTheFile)
        {
            AngleInDegrees = angleInDegrees;
            DrawingPriority = 1;
            Priority = 80;
        }

        public void TurnLeft() => AngleInDegrees += 270;

        public void TurnRight() => AngleInDegrees += 90;

        public void GoBack()
        {
            var neighbour = Model.Map[Walker.MovingBack[AngleInDegrees % 360] + Location];

            if (!(neighbour is Wall || neighbour is Bot || neighbour is Bullet))
                Command.Delta = Walker.MovingBack[AngleInDegrees % 360];
        }

        public void GoForwad()
        {
            var neighbour = Model.Map[Walker.MovingForwad[AngleInDegrees % 360] + Location];

            if (!(neighbour is Wall || neighbour is Bot || neighbour is Bullet))
                Command.Delta = Walker.MovingForwad[AngleInDegrees % 360];
        }

        public void GoLeft()
        {
            var neighbour = Model.Map[Walker.MovingLeft[AngleInDegrees % 360] + Location];

            if (!(neighbour is Wall || neighbour is Bot || neighbour is Bullet))
                Command.Delta = Walker.MovingLeft[AngleInDegrees % 360];
        }

        public void GoRight()
        {
            var neighbour = Model.Map[Walker.MovingRight[AngleInDegrees % 360] + Location];

            if (!(neighbour is Wall || neighbour is Bot || neighbour is Bullet))
                Command.Delta = Walker.MovingRight[AngleInDegrees % 360];
        }
    }
}