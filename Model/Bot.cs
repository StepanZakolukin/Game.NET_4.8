using System;
using System.Linq;

namespace WindowsForm.Model
{
    public class Bot : GameObjects
    {
        public Bot(Point location, string pathToTheFile = @"..\..\Images\солдат.png") : base(location, pathToTheFile)
        {
            Priority = 80;
            DrawingPriority = 1;
        }

        public void MoveIsCompleted() => Command.Delta = new Point(0, 0);

        public void MakeAMove()
        {
            if (!(Model.Map[Model.Player.Location] is Soldier)) return;
            
            var distance = Location - Model.Player.Location;

            if (distance.Y < 0 && distance.X == 0 && Enumerable.Range(Location.Y + 1, Math.Abs(distance.Y) - 1).All(y => Model.Map[Location.X, y] is Stone))
            {
                switch (AngleInDegrees % 360)
                {
                    case 0: AngleInDegrees += 90; break;
                    case 180: AngleInDegrees += 270; break;
                    case 270: AngleInDegrees += 180; break;
                }
                Shoot(); return;
            }
            else if (distance.Y > 0 && distance.X == 0 && Enumerable.Range(Model.Player.Location.Y + 1, distance.Y - 1).All(y => Model.Map[Location.X, y] is Stone))
            {
                switch (AngleInDegrees % 360)
                {
                    case 0: AngleInDegrees += 270; break;
                    case 90: AngleInDegrees += 180; break;
                    case 180: AngleInDegrees += 90; break;
                }
                Shoot(); return;
            }
            else if (distance.X < 0 && distance.Y == 0 && Enumerable.Range(Location.X + 1, Math.Abs(distance.X) - 1).All(x => Model.Map[x, Location.Y] is Stone))
            {
                switch (AngleInDegrees % 360)
                {
                    case 90: AngleInDegrees += 270; break;
                    case 180: AngleInDegrees += 180; break;
                    case 270: AngleInDegrees += 90; break;
                }
                Shoot(); return;
            }
            else if (distance.X > 0 && distance.Y == 0 && Enumerable.Range(Model.Player.Location.X + 1, distance.X - 1).All(x => Model.Map[x, Location.Y] is Stone))
            {
                switch (AngleInDegrees % 360)
                {
                    case 0: AngleInDegrees += 180; break;
                    case 90: AngleInDegrees += 90; break;
                    case 270: AngleInDegrees += 270; break;
                }
                Shoot(); return;
            }

            var followingLocation = FindingAWay.FindAWay(Model.Player.Location, Playground.OfSets
                .Select(ofset => Location + ofset)
                .ToHashSet())
                .FirstOrDefault();

            if (followingLocation != null) Command.Delta = followingLocation.Value - Location;
        }

        public void Shoot()
        {
            switch (AngleInDegrees % 360)
            {
                case 90:
                    if (!(Model.Map[Playground.OfSets[(int)CSRotatedBy90.Forward] + Location] is Wall))
                        Model.Map[Playground.OfSets[(int)CSRotatedBy90.Forward] + Location] = new Bullet(AngleInDegrees, Location);
                    break;
                case 180:
                    if (!(Model.Map[Playground.OfSets[(int)CSRotatedBy180.Forward] + Location] is Wall))
                        Model.Map[Playground.OfSets[(int)CSRotatedBy180.Forward] + Location] = new Bullet(AngleInDegrees, Location);
                    break;
                case 0:
                    if (!(Model.Map[Playground.OfSets[(int)CSRotatedBy0.Forward] + Location] is Wall))
                        Model.Map[Playground.OfSets[(int)CSRotatedBy0.Forward] + Location] = new Bullet(AngleInDegrees, Location);
                    break;
                case 270:
                    if (!(Model.Map[Playground.OfSets[(int)CSRotatedBy270.Forward] + Location] is Wall))
                        Model.Map[Playground.OfSets[(int)CSRotatedBy270.Forward] + Location] = new Bullet(AngleInDegrees, Location);
                    break;
            }
        }
    }
}
