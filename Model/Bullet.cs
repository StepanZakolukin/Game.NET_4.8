namespace WindowsForm.Model
{
    public class Bullet : GameObjects
    {
        public Bullet(int angleInDegrees, Point location, string pathToTheFile = @"..\..\Images\пуля.png") : base(location, pathToTheFile)
        {
            AngleInDegrees = angleInDegrees;
            DrawingPriority = 1;
            Priority = 100;
            GoForwad();
        }

        public void GoForwad()
        {
            switch (AngleInDegrees % 360)
            {
                case 90:
                    Command.Delta = Playground.OfSets[(int)CSRotatedBy90.Forward];
                    break;
                case 180:
                    Command.Delta = Playground.OfSets[(int)CSRotatedBy180.Forward];
                    break;
                case 0:
                    Command.Delta = Playground.OfSets[(int)CSRotatedBy0.Forward];
                    break;
                case 270:
                    Command.Delta = Playground.OfSets[(int)CSRotatedBy270.Forward];
                    break;
            }
        }
    }
}
