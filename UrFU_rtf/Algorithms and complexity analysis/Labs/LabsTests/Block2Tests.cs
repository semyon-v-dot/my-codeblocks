using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Block2;

namespace LabsTests;

public class Block2Tests
{
    [TestCase(@"5
0 1
0 2
1 2
1 4
2 4
4 3")]
    [TestCase(@"5
0 1
0 2
1 2
1 3
2 3
3 4")]
    public void GraphColoringTest(string input)
    {
        var lines = input.Split('\n');
        var graph = new GraphToColor(int.Parse(lines.First()));
        AddEdges(graph, lines.Skip(1));

        CheckResultColoring(graph, graph.GreedyColoring());
    }

    private void AddEdges(GraphToColor graph, IEnumerable<string> inputLines)
    {
        var intInput = inputLines
            .Select(line => line.Split(" "))
            .Select(lines => Tuple.Create(int.Parse(lines[0]), int.Parse(lines[1])));
        foreach (var (item1, item2) in intInput)
            graph.AddEdge(item1, item2);
    }

    private void CheckResultColoring(GraphToColor graph, int[] resultColoring)
    {
        var dictKeysValues = graph.GetAdjacencyDictKeysValues();
        foreach (var (currentVertex, adjacentVertexes) in dictKeysValues)
        foreach (var vertex in adjacentVertexes)
            if (vertex != currentVertex && resultColoring[vertex] == resultColoring[currentVertex])
                Assert.Fail($"Adjacent vertexes with the same color: {vertex} {currentVertex}");
        
    }
}