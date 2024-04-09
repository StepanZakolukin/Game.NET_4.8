namespace WindowsForm.Model
{
    public class Wall : GameObjects
    {
        public Wall(Point location, string pathToTheFile = @"..\..\Images\кирпич.jpg") : base(location, pathToTheFile)
        {
            Priority = 200;
        }
    }
}
