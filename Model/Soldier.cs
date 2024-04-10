namespace WindowsForm.Model
{
    public class Soldier : GameObjects
    {
        public Soldier(int angleInDegrees, Point location, string pathToTheFile = @"..\..\Images\солдат.png") : base(location, pathToTheFile)
        {
            AngleInDegrees = angleInDegrees;
            DrawingPriority = 1;
            Priority = 80;
        }

        public void TurnLeft() => AngleInDegrees += 270;

        public void TurnRight() => AngleInDegrees += 90;

        public void MoveIsCompleted() => Command.Delta = new Point(0, 0);

        public void GoBack()
        {
            GameObjects neighbour;
            switch (AngleInDegrees % 360)
            {
                case 90:
                    neighbour = GameModel.Map[GameModel.OfSets[(int)CSRotatedBy90.Back] + Location];
                    if (!(neighbour is Wall || neighbour is Soldier || neighbour is Bullet))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy90.Back];
                    break;
                case 180:
                    neighbour = GameModel.Map[GameModel.OfSets[(int)CSRotatedBy180.Back] + Location];
                    if (!(neighbour is Wall || neighbour is Soldier || neighbour is Bullet))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy180.Back];
                    break;
                case 0:
                    neighbour = GameModel.Map[GameModel.OfSets[(int)CSRotatedBy0.Back] + Location];
                    if (!(neighbour is Wall || neighbour is Soldier || neighbour is Bullet))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy0.Back];
                    break;
                case 270:
                    neighbour = GameModel.Map[GameModel.OfSets[(int)CSRotatedBy270.Back] + Location];
                    if (!(neighbour is Wall || neighbour is Soldier || neighbour is Bullet))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy270.Back];
                    break;
            }
        }

        public void GoForwad()
        {
            GameObjects neighbour;
            switch (AngleInDegrees % 360)
            {
                case 90:
                    neighbour = GameModel.Map[GameModel.OfSets[(int)CSRotatedBy90.Forward] + Location];
                    if (!(neighbour is Wall || neighbour is Soldier || neighbour is Bullet))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy90.Forward];
                    break;
                case 180:
                    neighbour = GameModel.Map[GameModel.OfSets[(int)CSRotatedBy180.Forward] + Location];
                    if (!(neighbour is Wall || neighbour is Soldier || neighbour is Bullet))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy180.Forward];
                    break;
                case 0:
                    neighbour = GameModel.Map[GameModel.OfSets[(int)CSRotatedBy0.Forward] + Location];
                    if (!(neighbour is Wall || neighbour is Soldier || neighbour is Bullet))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy0.Forward];
                    break;
                case 270:
                    neighbour = GameModel.Map[GameModel.OfSets[(int)CSRotatedBy270.Forward] + Location];
                    if (!(neighbour is Wall || neighbour is Soldier || neighbour is Bullet))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy270.Forward];
                    break;
            }
        }

        public void GoLeft()
        {
            GameObjects neighbour;
            switch (AngleInDegrees % 360)
            {
                case 90:
                    neighbour = GameModel.Map[GameModel.OfSets[(int)CSRotatedBy90.Left] + Location];
                    if (!(neighbour is Wall || neighbour is Soldier || neighbour is Bullet))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy90.Left];
                    break;
                case 180:
                    neighbour = GameModel.Map[GameModel.OfSets[(int)CSRotatedBy180.Left] + Location];
                    if (!(neighbour is Wall || neighbour is Soldier || neighbour is Bullet))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy180.Left];
                    break;
                case 0:
                    neighbour = GameModel.Map[GameModel.OfSets[(int)CSRotatedBy0.Left] + Location];
                    if (!(neighbour is Wall || neighbour is Soldier || neighbour is Bullet))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy0.Left];
                    break;
                case 270:
                    neighbour = GameModel.Map[GameModel.OfSets[(int)CSRotatedBy270.Left] + Location];
                    if (!(neighbour is Wall || neighbour is Soldier || neighbour is Bullet))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy270.Left];
                    break;
            }
        }

        public void GoRight()
        {
            GameObjects neighbour;
            switch (AngleInDegrees % 360)
            {
                case 90:
                    neighbour = GameModel.Map[GameModel.OfSets[(int)CSRotatedBy90.Right] + Location];
                    if (!(neighbour is Wall || neighbour is Soldier || neighbour is Bullet))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy90.Right];
                    break;
                case 180:
                    neighbour = GameModel.Map[GameModel.OfSets[(int)CSRotatedBy180.Right] + Location];
                    if (!(neighbour is Wall || neighbour is Soldier || neighbour is Bullet))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy180.Right];
                    break;
                case 0:
                    neighbour = GameModel.Map[GameModel.OfSets[(int)CSRotatedBy0.Right] + Location];
                    if (!(neighbour is Wall || neighbour is Soldier || neighbour is Bullet))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy0.Right];
                    break;
                case 270:
                    neighbour = GameModel.Map[GameModel.OfSets[(int)CSRotatedBy270.Right] + Location];
                    if (!(neighbour is Wall || neighbour is Soldier || neighbour is Bullet))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy270.Right];
                    break;
            }
        }

        public void Shoot()
        {
            switch (AngleInDegrees % 360)
            {
                case 90:
                    if (!(GameModel.Map[GameModel.OfSets[(int)CSRotatedBy90.Forward] + Location] is Wall))
                        GameModel.Map[GameModel.OfSets[(int)CSRotatedBy90.Forward] + Location] = new Bullet(AngleInDegrees, Location);
                    break;
                case 180:
                    if (!(GameModel.Map[GameModel.OfSets[(int)CSRotatedBy180.Forward] + Location] is Wall))
                        GameModel.Map[GameModel.OfSets[(int)CSRotatedBy180.Forward] + Location] = new Bullet( AngleInDegrees, Location);
                    break;
                case 0:
                    if (!(GameModel.Map[GameModel.OfSets[(int)CSRotatedBy0.Forward] + Location] is Wall))
                        GameModel.Map[GameModel.OfSets[(int)CSRotatedBy0.Forward] + Location] = new Bullet(AngleInDegrees, Location);
                    break;
                case 270:
                    if (!(GameModel.Map[GameModel.OfSets[(int)CSRotatedBy270.Forward] + Location] is Wall))
                        GameModel.Map[GameModel.OfSets[(int)CSRotatedBy270.Forward] + Location] = new Bullet(AngleInDegrees, Location);
                    break;
            }
        }
    }
}
