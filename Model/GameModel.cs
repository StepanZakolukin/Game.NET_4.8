using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsForm.Model
{
    public class GameModel
    {
        public event Action StateChanged;
        public static GameMap Map { get; private set; }
        public static Bot Bot { get; private set; }
        public static Soldier Player { get; private set; }
        private readonly List<CreatureAnimation> Animations;
        public static Point[] OfSets = new Point[] { new Point(0, 1), new Point(0, -1), new Point(-1, 0), new Point(1, 0) };
        public GameModel(GameMap map)
        {
            Map = map;
            Animations = new List<CreatureAnimation>();
            Map[30, 1] = new Soldier(90, new Point(30, 1));
            Player = (Soldier)Map[30, 1];
            Map[1, 1] = new Bot(new Point(1, 1));
            Bot = (Bot)Map[1, 1];
        }

        public void BeginAct()
        {
            Animations.Clear();

            for (var x = 0; x < Map.Width; x++)
                for (var y = 0; y < Map.Height; y++)
                {
                    Animations.Add(new CreatureAnimation {
                            Command = Map[x, y].Command,
                            Creature = Map[x, y],
                            Location = Map[x, y].Location,
                            TargetLogicalLocation = Map[x, y].Location + Map[x, y].Command.Delta });
                }
        }

        public void EndAct()
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

        private List<GameObjects>[,] GetCandidatesPerLocation()
        {
            var creatures = new List<GameObjects>[Map.Width, Map.Height];

            for (var x = 0; x < Map.Width; x++)
                for (var y = 0; y < Map.Height; y++)
                    creatures[x, y] = new List<GameObjects>();

            foreach (var e in Animations)
            {
                var x = e.TargetLogicalLocation.X;
                var y = e.TargetLogicalLocation.Y;
                var nextCreature = e.Command.TransformTo ?? e.Creature;
                creatures[x, y].Add(nextCreature);
            }

            return creatures;
        }

        private static GameObjects SelectWinnerCandidatePerLocation(List<GameObjects>[,] creatures, int x, int y)
        {
            var sortedСreatures = creatures[x, y].OrderBy(creature => creature.Priority);

            return sortedСreatures.Any() ? sortedСreatures.Last() : new Stone(new Point(x, y));
        }
    }
}
