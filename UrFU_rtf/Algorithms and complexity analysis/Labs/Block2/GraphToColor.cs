
namespace Block2;

public class GraphToColor
{
    private readonly Dictionary<int, List<int>> _adjacencyDict;

    public GraphToColor(int numberOfVertexes)
    {
        _adjacencyDict = new Dictionary<int, List<int>>();
        for (var i = 0; i < numberOfVertexes; i++)
            _adjacencyDict.Add(i, new List<int>());
    }
    public List<(int, List<int>)> GetAdjacencyDictKeysValues() 
        => _adjacencyDict.Keys
            .Zip(_adjacencyDict.Values.ToList())
            .ToList();

    public void AddEdge(int a, int b)
    {
        _adjacencyDict[a].Add(b);
        _adjacencyDict[b].Add(a);
    }

    public int[] GreedyColoring()
    {
        var resultColors = new int[_adjacencyDict.Count];
        Array.Fill(resultColors, -1);
        resultColors[0] = 0;
        var available = new bool[_adjacencyDict.Count];
        Array.Fill(available, true);

        for (var i = 1; i < _adjacencyDict.Count; i++)
        {
            foreach (var vertex in _adjacencyDict[i])
                if (resultColors[vertex] != -1)
                    available[resultColors[vertex]] = false;
            
            int j;
            for (j = 0; j < _adjacencyDict.Count; j++)
                if (available[j])
                    break;

            resultColors[i] = j;
            
            Array.Fill(available, true);
        }

        return resultColors;
    }
}