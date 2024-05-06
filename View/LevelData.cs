namespace WindowsForm.View
{
    public class LevelData
    {
        public readonly int Level;
        public int Record { get; set; }
        public bool Available { get; set; }
        public int PossibleNumberOfPoints { get; set; }
        public LevelData(params string[] data)
        {
            Level = int.Parse(data[0]);
            Available = bool.Parse(data[1]);
            Record = int.Parse(data[2]);
            PossibleNumberOfPoints = int.Parse(data[3]);
        }

        public override string ToString() => $"{Level};{Available};{Record};{PossibleNumberOfPoints}";
    }
}
