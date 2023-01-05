using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using Block2;

namespace LabsTests;

public class Block2Tests
{
    [TestCase(@"<root>
<NumberOfVertexes>5</NumberOfVertexes>
<Vertex>
<First>0</First> <Second>1</Second>
</Vertex>
<Vertex>
<First>0</First> <Second>2</Second>
</Vertex>
<Vertex>
<First>1</First> <Second>2</Second>
</Vertex>
<Vertex>
<First>1</First> <Second>4</Second>
</Vertex>
<Vertex>
<First>2</First> <Second>4</Second>
</Vertex>
<Vertex>
<First>0</First> <Second>1</Second>
</Vertex>
<Vertex>
<First>4</First> <Second>3</Second>
</Vertex>
</root>")]
    [TestCase(@"<root>
<NumberOfVertexes>5</NumberOfVertexes>
<Vertex>
<First>0</First> <Second>1</Second>
</Vertex>
<Vertex>
<First>0</First> <Second>2</Second>
</Vertex>
<Vertex>
<First>1</First> <Second>2</Second>
</Vertex>
<Vertex>
<First>1</First> <Second>3</Second>
</Vertex>
<Vertex>
<First>2</First> <Second>3</Second>
</Vertex>
<Vertex>
<First>3</First> <Second>4</Second>
</Vertex>
</root>")]
    public void GraphColoringTest(string input)
    {
        var docXml = new XmlDocument();
        docXml.LoadXml(input);
        var inputXmlNodes = docXml
            .DocumentElement
            .ChildNodes
            .OfType<XmlNode>()
            .ToArray();
        
        var graph = new GraphToColor(int.Parse(inputXmlNodes.First().InnerText));
        AddEdges(graph, inputXmlNodes.Skip(1));

        CheckResultColoring(graph, graph.GreedyColoring());
    }

    private void AddEdges(GraphToColor graph, IEnumerable<XmlNode> inputNodes)
    {
        var intInput = inputNodes
            .Select(vertex => vertex.ChildNodes)
            .Select(
                firstSecond 
                => Tuple.Create(int.Parse(firstSecond.Item(0).InnerText), 
                                int.Parse(firstSecond.Item(1).InnerText)));
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