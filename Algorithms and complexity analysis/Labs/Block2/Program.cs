namespace Block2;

class Program
{
    public static void Main(string[] args)
    {
        var graph = new GraphToColor(5);
        
        // graph.AddEdge(0, 1);
        // graph.AddEdge(0, 2);
        // graph.AddEdge(1, 2);
        // graph.AddEdge(1, 4);
        // graph.AddEdge(2, 4);
        // graph.AddEdge(4, 3);
        
        graph.AddEdge(0, 1);
        graph.AddEdge(0, 2);
        graph.AddEdge(1, 2);
        graph.AddEdge(1, 3);
        graph.AddEdge(2, 3);
        graph.AddEdge(3, 4);
        
        Console.Write(string.Join(" ", graph.GreedyColoring()));
    }
}