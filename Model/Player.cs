using System.Linq;

namespace WindowsForm.Model
{
    public class Player : Characters
    {
        public Player(int angleInDegrees, Point location, string pathToTheFile = @"..\..\Images\Player.png") : base(location, pathToTheFile)
        {
            AngleInDegrees = angleInDegrees;
        }

        public override bool DeadInConflict(GameObjects gameObjects) 
            => !(gameObjects is Stone || gameObjects == this);

        public void TurnLeft() => AngleInDegrees += 270;

        public void TurnRight() => AngleInDegrees += 90;

        public void GoBack(GameModel model)
        {
            var neighbour = model.Map[Walker.OfSets[(int)Go.Back] + Location];

            if (!neighbour.Any(creature => DeadInConflict(creature)))
                Delta = Walker.OfSets[(int)Go.Back];
        }

        public void GoForwad(GameModel model)
        {
            var neighbour = model.Map[Walker.OfSets[(int)Go.Forwad] + Location];

            if (!neighbour.Any(creature => DeadInConflict(creature)))
                Delta = Walker.OfSets[(int)Go.Forwad];
        }

        public void GoLeft(GameModel model)
        {
            var neighbour = model.Map[Walker.OfSets[(int)Go.Left] + Location];

            if (!neighbour.Any(creature => DeadInConflict(creature)))
                Delta = Walker.OfSets[(int)Go.Left];
        }

        public void GoRight(GameModel model)
        {
            var neighbour = model.Map[Walker.OfSets[(int)Go.Right] + Location];

            if (!neighbour.Any(creature => DeadInConflict(creature)))
                Delta = Walker.OfSets[(int)Go.Right];
        }
    }
}