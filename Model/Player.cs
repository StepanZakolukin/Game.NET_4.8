namespace WindowsForm.Model
{
    public class Player : Characters
    {
        public Player(int angleInDegrees, Point location, string pathToTheFile = @"..\..\Images\Player.png") : base(location, pathToTheFile)
        {
            AngleInDegrees = angleInDegrees;
        }

        public void TurnLeft() => AngleInDegrees += 270;

        public void TurnRight() => AngleInDegrees += 90;

        public void GoBack(GameModel model)
        {
            var neighbour = model.Map[Walker.MovingBack[AngleInDegrees % 360] + Location];

            if (!(neighbour is Wall || neighbour is Bot || neighbour is Bullet))
                Delta = Walker.MovingBack[AngleInDegrees % 360];
        }

        public void GoForwad(GameModel model)
        {
            var neighbour = model.Map[Walker.MovingForwad[AngleInDegrees % 360] + Location];

            if (!(neighbour is Wall || neighbour is Bot || neighbour is Bullet))
                Delta = Walker.MovingForwad[AngleInDegrees % 360];
        }

        public void GoLeft(GameModel model)
        {
            var neighbour = model.Map[Walker.MovingLeft[AngleInDegrees % 360] + Location];

            if (!(neighbour is Wall || neighbour is Bot || neighbour is Bullet))
                Delta = Walker.MovingLeft[AngleInDegrees % 360];
        }

        public void GoRight(GameModel model)
        {
            var neighbour = model.Map[Walker.MovingRight[AngleInDegrees % 360] + Location];

            if (!(neighbour is Wall || neighbour is Bot || neighbour is Bullet))
                Delta = Walker.MovingRight[AngleInDegrees % 360];
        }
    }
}