using System;
using System.Linq;

namespace WindowsForm.Model
{
    public class Bot : Characters
    {
        public Bot(Point location, string pathToTheFile = @"..\..\Images\солдат.png") : base(location, pathToTheFile)
        {
        }

        public void MakeAMove()
        {
            if (!(Model.Map[Model.Player.Location] is Player)) return;

            TurnToThePlayerAndShoot();

            var followingLocation = FindingAWay.FindAWay(Model.Player.Location, Walker.OfSets
                .Select(ofset => Location + ofset)
                .ToHashSet())
                .FirstOrDefault();

            if (followingLocation != null) Delta = followingLocation.Value - Location;
        }

        void TurnToThePlayerAndShoot()
        {
            var distance = Location - Model.Player.Location;

            if (distance.Y < 0 && distance.X == 0 && ChecTheExpediencyOfTheShot(Location.Y + 1, Math.Abs(distance.Y) - 1))
            {
                switch (AngleInDegrees % 360)
                {
                    case 0: AngleInDegrees += 90; break;
                    case 180: AngleInDegrees += 270; break;
                    case 270: AngleInDegrees += 180; break;
                }
                Shoot(); return;
            }
            else if (distance.Y > 0 && distance.X == 0 && ChecTheExpediencyOfTheShot(Model.Player.Location.Y + 1, distance.Y - 1))
            {
                switch (AngleInDegrees % 360)
                {
                    case 0: AngleInDegrees += 270; break;
                    case 90: AngleInDegrees += 180; break;
                    case 180: AngleInDegrees += 90; break;
                }
                Shoot(); return;
            }
            else if (distance.X < 0 && distance.Y == 0 && ChecTheExpediencyOfTheShot(Location.X + 1, Math.Abs(distance.X) - 1))
            {
                switch (AngleInDegrees % 360)
                {
                    case 90: AngleInDegrees += 270; break;
                    case 180: AngleInDegrees += 180; break;
                    case 270: AngleInDegrees += 90; break;
                }
                Shoot(); return;
            }
            else if (distance.X > 0 && distance.Y == 0 && ChecTheExpediencyOfTheShot(Model.Player.Location.X + 1, distance.X - 1))
            {
                switch (AngleInDegrees % 360)
                {
                    case 0: AngleInDegrees += 180; break;
                    case 90: AngleInDegrees += 90; break;
                    case 270: AngleInDegrees += 270; break;
                }
                Shoot(); return;
            }
        }

        bool ChecTheExpediencyOfTheShot(int start, int numberOfIterations)
            => Enumerable.Range(start, numberOfIterations).All(y => Model.Map[Location.X, y] is Stone);
    }
}
