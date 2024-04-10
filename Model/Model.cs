using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsForm.Model
{
    public class Model
    {
        public event Action StateChanged;
        public static Playground Map { get; private set; }
        public static Bot Bot { get; private set; }
        public static Soldier Player { get; private set; }
        public Model(Playground map)
        {
            Map = map;
            Map[30, 1] = new Soldier(90, new Point(30, 1));
            Player = (Soldier)Map[30, 1];
            Map[1, 1] = new Bot(new Point(1, 1));
            Bot = (Bot)Map[1, 1];
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
                    var creature = SelectWinnerCandidatePerLocation(creaturesPerLocation, x, y);
                    if (creature is Soldier soldier) soldier.MoveIsCompleted();
                    else if (creature is Bot bot) bot.MoveIsCompleted();
                    creature.Location = new Point(x, y);
                    Map[x, y] = creature;
                }

            StateChanged();
        }

        private static GameObjects SelectWinnerCandidatePerLocation(List<GameObjects>[,] creatures, int x, int y)
        {
            var sortedСreatures = creatures[x, y].OrderBy(creature => creature.Priority);

            return sortedСreatures.Any() ? sortedСreatures.Last() : new Stone(new Point(x, y));
        }
    }
}
