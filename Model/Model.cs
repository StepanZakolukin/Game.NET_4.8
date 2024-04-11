using System;
using System.Collections.Generic;
using System.Linq;
//берем максимальный элемент, те кторые в конфликте с ним доют отрицательный результат убираем, сортируем по приоритету отрисовки
namespace WindowsForm.Model
{
    public class Model
    {
        public event Action StateChanged;
        public List<Bot> ArmyOfBots;
        public static event Action<int, int> CommandsAreExecuted;
        public static Playground Map { get; private set; }
        public static Soldier Player { get; private set; }
        public Model(Playground map)
        {
            Map = map;
            Map[30, 1] = new Soldier(90, new Point(30, 1));
            Player = (Soldier)Map[30, 1];
            ArmyOfBots = new List<Bot>();
        }

        public List<GameObjects>[,] GetCandidatesPerLocation()
        {
            var creatures = new List<GameObjects>[Map.Width, Map.Height];

            for (var x = 0; x < Map.Width; x++)
                for (var y = 0; y < Map.Height; y++)
                    creatures[x, y] = new List<GameObjects>();

            for (var x = 0; x < Map.Width; x++)
                for (var y = 0; y < Map.Height; y++)
                {
                    var targetLogicalLocation = Map[x, y].Location + Map[x, y].Command.Delta;
                    var nextCreature = Map[x, y].Command.TransformTo ?? Map[x, y];
                    creatures[targetLogicalLocation.X, targetLogicalLocation.Y].Add(nextCreature);
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
                    Map[x, y].CommandAreExecuted(x, y);
                }

            StateChanged();
        }

        private static GameObjects SelectWinnerCandidatePerLocation(List<GameObjects>[,] creatures, int x, int y)
        {
            var sortedСreatures = creatures[x, y].OrderBy(creature => creature.Priority);

            return sortedСreatures.Any() ? sortedСreatures.Last() : new Stone(new Point(x, y));
        }

        public void CreateABot()
        {
            var location = FindAPositionToCreateABot();
            Map[location] = new Bot(location);
            ArmyOfBots.Add((Bot)Map[location]);
        }

        public void SetTheBotsInMotion()
        {
            foreach (var bot in ArmyOfBots)
                bot.MakeAMove();
        }

        private Point FindAPositionToCreateABot()
        {
            var random = new Random();
            (var x, var y) = (int.MaxValue, int.MaxValue);

            while (!Map.InBounds(new Point(x, y)) || !(Map[x, y] is Stone))
                (x, y) = (random.Next(1, Map.Width - 1), random.Next(1, Map.Height - 1));

            return new Point(x, y);
        }
    }
}
