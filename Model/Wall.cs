namespace WindowsForm.Model
{
    public class Wall : GameObjects
    {
        public Wall(Point location, string pathToTheFile = @"..\..\Images\Wall.jpg") : base(location, pathToTheFile)
        {
            Priority = 3;
            AngleInDegrees = 0;
        }

        public override bool DeadInConflict(GameObjects gameObjects) => !(gameObjects is Wall);
    }
}
