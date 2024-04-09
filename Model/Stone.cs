namespace WindowsForm.Model
{
    public class Stone : GameObjects
    {
        public Stone(Point location, string pathToTheFile = @"..\..\Images\камень.jpg") : base(location, pathToTheFile)
        {
            Priority = 0;
        }
    }
}
