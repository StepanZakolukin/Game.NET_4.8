using System;
using System.Linq;

namespace WindowsForm.Model
{
    public class Bot : Characters
    {
        public Bot(Point location, string pathToTheFile = @"..\..\Images\Bot.png") : base(location, pathToTheFile)
        {
        }

        public void MakeAMove(GameModel model)
        {
            if (!(model.Map[model.Player.Location] is Player)) return;

            TurnToThePlayerAndShoot(model);

            var followingLocation = FindingAWay.FindAWay(model.Map, model.Player.Location, Walker.OfSets
                .Select(ofset => Location + ofset)
                .ToHashSet())
                .FirstOrDefault();

            if (followingLocation != null) Delta = followingLocation.Value - Location;
        }

        void TurnToThePlayerAndShoot(GameModel model)
        {
            var distance = Location - model.Player.Location;

            if (distance.Y < 0 && distance.X == 0 && Enumerable.Range(Location.Y + 1, Math.Abs(distance.Y) - 1).All(y => model.Map[Location.X, y] is Stone))
            {
                switch (AngleInDegrees % 360)
                {
                    case 0: AngleInDegrees += 90; break;
                    case 180: AngleInDegrees += 270; break;
                    case 270: AngleInDegrees += 180; break;
                }
                Shoot(model); return;
            }
            else if (distance.Y > 0 && distance.X == 0 && Enumerable.Range(model.Player.Location.Y + 1, distance.Y - 1).All(y => model.Map[Location.X, y] is Stone))
            {
                switch (AngleInDegrees % 360)
                {
                    case 0: AngleInDegrees += 270; break;
                    case 90: AngleInDegrees += 180; break;
                    case 180: AngleInDegrees += 90; break;
                }
                Shoot(model); return;
            }
            else if (distance.X < 0 && distance.Y == 0 && Enumerable.Range(Location.X + 1, Math.Abs(distance.X) - 1).All(x => model.Map[x, Location.Y] is Stone))
            {
                switch (AngleInDegrees % 360)
                {
                    case 90: AngleInDegrees += 270; break;
                    case 180: AngleInDegrees += 180; break;
                    case 270: AngleInDegrees += 90; break;
                }
                Shoot(model); return;
            }
            else if (distance.X > 0 && distance.Y == 0 && Enumerable.Range(model.Player.Location.X + 1, distance.X - 1).All(x => model.Map[x, Location.Y] is Stone))
            {
                switch (AngleInDegrees % 360)
                {
                    case 0: AngleInDegrees += 180; break;
                    case 90: AngleInDegrees += 90; break;
                    case 270: AngleInDegrees += 270; break;
                }
                Shoot(model); return;
            }
        }
    }
}
