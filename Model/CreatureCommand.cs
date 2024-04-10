namespace WindowsForm.Model
{
	public class CreatureCommand
	{
		public CreatureCommand()
		{
			Delta = new Point(0, 0);
			TransformTo = null;
		}

        public Point Delta;
		public GameObjects TransformTo;
	}
}