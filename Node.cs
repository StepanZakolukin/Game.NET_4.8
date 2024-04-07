public class Node
{
    public readonly Point Coordinates;
    public Node(Point coordinates)
    {
        Coordinates = coordinates;
    }
    public Node Forward { get; set; }
    public Node Back { get; set; }
    public Node Left { get; set; }
    public Node Right { get; set; }
}
