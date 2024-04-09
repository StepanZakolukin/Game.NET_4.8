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

        public void TurnLeft() => Command.AngularDistance = 270;

        public void TurnRight() => Command.AngularDistance = 90;

        public void GoBack()
        {
            switch (AngleInDegrees % 360)
            {
                case 90:
                    if (!(GameModel.Map[GameModel.OfSets[(int)CSRotatedBy90.Back] + Location] is Wall))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy90.Back];
                    break;
                case 180:
                    if (!(GameModel.Map[GameModel.OfSets[(int)CSRotatedBy180.Back] + Location] is Wall))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy180.Back];
                    break;
                case 0:
                    if (!(GameModel.Map[GameModel.OfSets[(int)CSRotatedBy0.Back] + Location] is Wall))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy0.Back];
                    break;
                case 270:
                    if (!(GameModel.Map[GameModel.OfSets[(int)CSRotatedBy270.Back] + Location] is Wall))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy270.Back];
                    break;
            }
        }

        public void GoForwad()
        {
            switch (AngleInDegrees % 360)
            {
                case 90:
                    if (!(GameModel.Map[GameModel.OfSets[(int)CSRotatedBy90.Forward] + Location] is Wall))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy90.Forward];
                    break;
                case 180:
                    if (!(GameModel.Map[GameModel.OfSets[(int)CSRotatedBy180.Forward] + Location] is Wall))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy180.Forward];
                    break;
                case 0:
                    if (!(GameModel.Map[GameModel.OfSets[(int)CSRotatedBy0.Forward] + Location] is Wall))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy0.Forward];
                    break;
                case 270:
                    if (!(GameModel.Map[GameModel.OfSets[(int)CSRotatedBy270.Forward] + Location] is Wall))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy270.Forward];
                    break;
            }
        }

        public void GoLeft()
        {
            switch (AngleInDegrees % 360)
            {
                case 90:
                    if (!(GameModel.Map[GameModel.OfSets[(int)CSRotatedBy90.Left] + Location] is Wall))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy90.Left];
                    break;
                case 180:
                    if (!(GameModel.Map[GameModel.OfSets[(int)CSRotatedBy180.Left] + Location] is Wall))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy180.Left];
                    break;
                case 0:
                    if (!(GameModel.Map[GameModel.OfSets[(int)CSRotatedBy0.Left] + Location] is Wall))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy0.Left];
                    break;
                case 270:
                    if (!(GameModel.Map[GameModel.OfSets[(int)CSRotatedBy270.Left] + Location] is Wall))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy270.Left];
                    break;
            }
        }

        public void GoRight()
        {
            switch (AngleInDegrees % 360)
            {
                case 90:
                    if (!(GameModel.Map[GameModel.OfSets[(int)CSRotatedBy90.Right] + Location] is Wall))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy90.Right];
                    break;
                case 180:
                    if (!(GameModel.Map[GameModel.OfSets[(int)CSRotatedBy180.Right] + Location] is Wall))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy180.Right];
                    break;
                case 0:
                    if (!(GameModel.Map[GameModel.OfSets[(int)CSRotatedBy0.Right] + Location] is Wall))
                        Command.Delta = GameModel.OfSets[(int)CSRotatedBy0.Right];
                    break;
                case 270:
                    if (!(GameModel.Map[GameModel.OfSets[(int)CSRotatedBy270.Right] + Location] is Wall))
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
