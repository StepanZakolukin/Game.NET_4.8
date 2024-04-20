using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WindowsForm.Model
{
    public class GameModel
    {
        public event Action StateChanged;
        public event Action TheGameIsOver;
        public bool RecordHasBeenUpdated { get; private set; }
        public List<Bot> ArmyOfBots { get; private set; }
        public Playground Map { get; private set; }
        public Player Player { get; private set; }
        private int numberOfBots;
        public int NumberOfPoints { get; private set; }
        public int Record { get; private set; }

        public GameModel(Playground map)
        {
            Map = map;
            var playerLocation = FindAPositionToCreateAnOject();
            Map[playerLocation].Add(new Player(90, playerLocation));
            Player = (Player)Map[playerLocation].Last();
            ArmyOfBots = new List<Bot>();

            var date = File.ReadAllLines(@"..\..\Model\Record.txt").FirstOrDefault();
            Record = date == null ? 0 : int.Parse(date);

            StateChanged += () => File.WriteAllText(@"..\..\Model\Record.txt", Record.ToString());
        }

        public List<GameObjects>[,] GetCandidatesPerLocation()
        {
            var creatures = new List<GameObjects>[Map.Width, Map.Height];

            for (var x = 0; x < Map.Width; x++)
                for (var y = 0; y < Map.Height; y++)
                    creatures[x, y] = new List<GameObjects>();

            for (var x = 0; x < Map.Width; x++)
                for (var y = 0; y < Map.Height; y++)
                    foreach(var creature in Map[x, y])
                    {
                        var targetLogicalLocation = creature.Location + creature.Delta;
                        creatures[targetLogicalLocation.X, targetLogicalLocation.Y].Add(creature);
                    }

            return creatures;
        }

        public void ExecuteTheCommandsOfTheHeroes()
        {
            var creaturesPerLocation = GetCandidatesPerLocation();

            for (var x = 0; x < Map.Width; x++)
                for (var y = 0; y < Map.Height; y++)
                {
                    Map[x, y] = SelectWinnerCandidatePerLocation(creaturesPerLocation, x, y);

                    foreach(var creature in Map[x, y])
                        creature.CommandAreExecuted(x, y);
                }

            if (Record < NumberOfPoints)
            {
                RecordHasBeenUpdated = true;
                Record = NumberOfPoints;
            }

            StateChanged();
            if (!Map[Player.Location].Contains(Player)) TheGameIsOver();
        }

        private static List<GameObjects> SelectWinnerCandidatePerLocation(List<GameObjects>[,] creatures, int x, int y)
        {
            var sortedСreatures = creatures[x, y].OrderBy(creature => creature.Priority);

            if (sortedСreatures.Last() is Bullet && !sortedСreatures.All(creature => creature is Bullet || creature is Stone))
                return new List<GameObjects>() { sortedСreatures.First() };

            return sortedСreatures.Where(creature => !sortedСreatures.Last().DeadInConflict(creature)).ToList();
        }

        public void CreateABot()
        {
            var location = FindAPositionToCreateAnOject();
            Map[location].Add(new Bot(location));
            ArmyOfBots.Add((Bot)Map[location].Last());
            numberOfBots++;
        }

        public void SetTheBotsInMotion(GameModel model)
        {
            ArmyOfBots = ArmyOfBots
                .Where(bot => Map[bot.Location].Contains(bot))
                .ToList();

            NumberOfPoints = numberOfBots - ArmyOfBots.Count;

            foreach (var bot in ArmyOfBots)
                bot.MakeAMove(model);
        }

        private Point FindAPositionToCreateAnOject()
        {
            var random = new Random();
            (var x, var y) = (int.MaxValue, int.MaxValue);

            while (!Map.InBounds(new Point(x, y)) || !Map[x, y].All(creature => creature is Stone) 
                || Player != null && (Math.Abs(Player.Location.X - x) < 4 || Math.Abs(Player.Location.Y - y) < 4))
            {
                (x, y) = (random.Next(1, Map.Width - 1), random.Next(1, Map.Height - 1));
            }

            return new Point(x, y);
        }
    }
}