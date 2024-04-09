using System.Collections.Generic;
using System.Linq;

namespace WindowsForm.Model
{
    public class GameModel
    {
        public static GameMap Map { get; private set; }
        public readonly Soldier Player;
        private readonly List<CreatureAnimation> Animations;
        public static Point[] OfSets = new Point[] { new Point(0, 1), new Point(0, -1), new Point(-1, 0), new Point(1, 0) };
        public GameModel(GameMap map)
        {
            Map = map;
            Animations = new List<CreatureAnimation>();
            Map[1, 1] = new Soldier(90, new Point(1, 1));
            Player = Map[1, 1] as Soldier;
        }

        public void BeginAct()
        {
            Animations.Clear();

            for (var x = 0; x < Map.MapWidth; x++)
                for (var y = 0; y < Map.MapHeight; y++)
                {
                    Map[x, y].AngleInDegrees += Map[x, y].Command.AngularDistance;

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

            for (var x = 0; x < Map.MapWidth; x++)
                for (var y = 0; y < Map.MapHeight; y++)
                {
                    var creature = SelectWinnerCandidatePerLocation(creaturesPerLocation, x, y);
                    creature.Location = new Point(x, y);
                    if (creature is Soldier) creature.Command = new CreatureCommand();
                    Map[x, y] = creature;
                }
        }

        private List<GameObjects>[,] GetCandidatesPerLocation()
        {
            var creatures = new List<GameObjects>[Map.MapWidth, Map.MapHeight];

            for (var x = 0; x < Map.MapWidth; x++)
                for (var y = 0; y < Map.MapHeight; y++)
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
