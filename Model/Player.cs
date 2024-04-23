using System.Linq;

namespace WindowsForm.Model
{
    public class Player : Characters
    {
        public Player(int angleInDegrees, Point location, string pathToTheFile = @"..\..\Images\Player.png", int health = 3) : base(location, pathToTheFile, health, angleInDegrees)
        {
        }

        public void TurnLeft()
        {
            AngleInDegrees += 270;
            AngleInDegrees %= 360;
        }

        public void TurnRight()
        {
            AngleInDegrees += 90;
            AngleInDegrees %= 360;
        }

        public void GoBack(GameModel model)
        {
            var neighbour = model.Map[Walker.OfSets[(int)Go.Back] + Location];

            if (!neighbour.Any(creature => creature.DeadInConflict(this)))
                Delta = Walker.OfSets[(int)Go.Back];
        }

        public void GoForwad(GameModel model)
        {
            var neighbour = model.Map[Walker.OfSets[(int)Go.Forwad] + Location];

            if (!neighbour.Any(creature => creature.DeadInConflict(this)))
                Delta = Walker.OfSets[(int)Go.Forwad];
        }

        public void GoLeft(GameModel model)
        {
            var neighbour = model.Map[Walker.OfSets[(int)Go.Left] + Location];

            if (!neighbour.Any(creature => creature.DeadInConflict(this)))
                Delta = Walker.OfSets[(int)Go.Left];
        }

        public void GoRight(GameModel model)
        {
            var neighbour = model.Map[Walker.OfSets[(int)Go.Right] + Location];

            if (!neighbour.Any(creature => creature.DeadInConflict(this)))
                Delta = Walker.OfSets[(int)Go.Right];
        }
    }
}