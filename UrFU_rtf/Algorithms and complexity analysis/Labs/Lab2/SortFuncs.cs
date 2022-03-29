using System.Xml;

namespace Lab2;

public class SortFuncs
{
    // 1. Bubble
    // 2. QSort
    // 3. SortTree
    // 4. Сортировка вставками
    // 5. Сортировка слиянием
    // 6. Сортировка с помощью красно-черного дерева

    public Func<int[], int[]> SortFuncForTxtOutput;

    private const string DirPath =
        @"D:\Repos\Github\sandbox\UrFU_rtf\Algorithms and complexity analysis\Labs\Lab2";    
    private const string SortDataTxt = DirPath + @"\SortData.txt";
    private const string SortedDataTxt = DirPath + @"\SortedData.txt";
    private const string SortDataXml = DirPath + @"\SortData.xml";
    
    private const int SizeOfIntsArray = 10_000;
    
    private RbTreeNode? _rbTreeRoot;

    private enum RbTreeColor
    {
        Red,
        Black
    }

    private class BinaryTreeNode
    {
        private BinaryTreeNode? _left;
        private BinaryTreeNode? _right;
        private readonly int _key;

        public BinaryTreeNode(int k)
        {
            _key = k;
        }
        
        public void Insert(BinaryTreeNode node) 
        {
            if (node._key < _key)
                if (_left != null) 
                    _left.Insert(node);
                else 
                    _left = node;
            else if (_right != null) 
                _right.Insert(node);
            else _right = node;
        }
        
        public void Traverse(ICollection<int> outData) 
        {
            _left?.Traverse(outData);

            outData.Add(_key);

            _right?.Traverse(outData);
        }
    }

    private class RbTreeNode
    {
        public RbTreeColor? Colour;
        public RbTreeNode? Left;
        public RbTreeNode? Right;
        public RbTreeNode? Parent;
        public readonly int Data;

        public RbTreeNode(int data)
        {
            Data = data;
        }
    }
    
    public int[] FillAndGetSortedData()
    {
        var intsData = ReadXmlInts();

        WriteTxtInts(SortFuncForTxtOutput(intsData), SortedDataTxt);

        return intsData;
    }

    public int[] FillAndGetSortData()
    {
        var intsData = GetIntsData();
            
        WriteTxtInts(intsData, SortDataTxt);
        WriteXmlInts(intsData);

        return intsData;
    }
    
    public int[] BubbleSort(int[] data)
    {
        var outputData = data.ToArray();
        for (var i = 0; i < data.Length; i++)
        for (var j = 0; j < data.Length - 1; j++)
            if (outputData[j] > outputData[j + 1])
                (outputData[j], outputData[j + 1]) = (outputData[j + 1], outputData[j]);

        return outputData;
    }
    
    public int[] Quicksort(int[] data)
    {
        var outputData = data.ToArray();
        QuickSort(outputData, 0, outputData.Length - 1);

        return outputData;
    }
        
    public int[] TreeSort(int[] data)
    {
        var root = new BinaryTreeNode(data[0]);
        for (var i = 1; i < data.Length; i++)
            root.Insert(new BinaryTreeNode(data[i]));

        var outData = new List<int>();
        root.Traverse(outData);

        return outData.ToArray();
    }
    
    public int[] InsertionSort(int[] data)
    {
        var outData = data.ToArray();
        for (var i = 1; i < outData.Length; i++)
        for (var j = i; j > 0 && outData[j - 1] > outData[j]; j--)
            (outData[j - 1], outData[j]) = (outData[j], outData[j - 1]); 

        return outData;
    }

    public int[] MergeSort(int[] data)
    {
        var outData = data.ToArray();
        
        MergeSort(outData, 0, outData.Length - 1);

        return outData;
    }

    public int[] RedBlackTreeSort(int[] data)
    {
        foreach (var item in data)
            RbTreeInsert(item);

        var outData = new List<int>();
        RbTreeTraverse(_rbTreeRoot, outData);

        return outData.ToArray();
    }

    private void QuickSort(int[] array, int left, int right)
    {
        var l = left;
        var r = right;
        var avg = array[(l + r) / 2];
        do
        {
            while (array[l] < avg)
                ++l;
            while (array[r] > avg)
                --r;
            
            if (l > r) 
                continue;
            
            if (l < r)
                (array[l], array[r]) = (array[r], array[l]);
            
            ++l;
            --r;
        }
        while (l <= r);
        
        if (left < r)
            QuickSort(array, left, r);
        if (l < right)
            QuickSort(array, l, right);
    }

    private void MergeSort(int[] arr, int l, int r)
    {
        if (l < r) 
        {
            var m = l + (r - l) / 2;
            
            MergeSort(arr, l, m);
            MergeSort(arr, m + 1, r);
  
            Merge(arr, l, m, r);
        }
    }

    private void Merge(int[] arr, int l, int m, int r)
    {
        var n1 = m - l + 1;
        var n2 = r - m;
        var arrL = new int[n1];
        var arrR = new int[n2];
        int i, j;
  
        for (i = 0; i < n1; ++i)
            arrL[i] = arr[l + i];
        for (j = 0; j < n2; ++j)
            arrR[j] = arr[m + 1 + j];
  
        i = 0;
        j = 0;
        var k = l;
        
        while (i < n1 && j < n2) 
        {
            if (arrL[i] <= arrR[j]) 
            {
                arr[k] = arrL[i];
                i++;
            }
            else 
            {
                arr[k] = arrR[j];
                j++;
            }
            
            k++;
        }
        while (i < n1) 
        {
            arr[k] = arrL[i];
            i++;
            k++;
        }
        while (j < n2) 
        {
            arr[k] = arrR[j];
            j++;
            k++;
        }
    }
    
    private void RbTreeTraverse(RbTreeNode? current, ICollection<int> outData)
    {
        if (current != null)
        {
            RbTreeTraverse(current.Left, outData);

            outData.Add(current.Data);
            
            RbTreeTraverse(current.Right, outData);
        }
    }
  
    private void RbTreeLeftRotate(RbTreeNode nodeX)
    {
        var nodeY = nodeX.Right;
        nodeX.Right = nodeY.Left;
        
        if (nodeY.Left != null)
            nodeY.Left.Parent = nodeX;
        
        nodeY.Parent = nodeX.Parent;
        
        if (nodeX.Parent == null)
            _rbTreeRoot = nodeY;
        else if (nodeX == nodeX.Parent.Left)
            nodeX.Parent.Left = nodeY;
        else
            nodeX.Parent.Right = nodeY;
        
        nodeY.Left = nodeX;
        nodeX.Parent = nodeY;
    }
    
    private void RbTreeRightRotate(RbTreeNode nodeY)
    {
        var nodeX = nodeY.Left;
        nodeY.Left = nodeX.Right;
        
        if (nodeX.Right != null)
            nodeX.Right.Parent = nodeY;
        
        nodeX.Parent = nodeY.Parent;
        
        if (nodeY.Parent == null)
            _rbTreeRoot = nodeX;
        else if (nodeY == nodeY.Parent.Right)
             nodeY.Parent.Right = nodeX;
        else if (nodeY == nodeY.Parent.Left)
             nodeY.Parent.Left = nodeX;

        nodeX.Right = nodeY;
        nodeY.Parent = nodeX;
    }
    
    private void RbTreeInsert(int item)
    {
        var newItem = new RbTreeNode(item);
        
        if (_rbTreeRoot == null)
        {
            _rbTreeRoot = newItem;
            _rbTreeRoot.Colour = RbTreeColor.Black;
        }
        else
        {
            RbTreeNode? nodeY = null;
            var nodeX = _rbTreeRoot;
            while (nodeX != null)
            {
                nodeY = nodeX;
                nodeX = newItem.Data < nodeX.Data ? nodeX.Left : nodeX.Right;
            }

            newItem.Parent = nodeY;
            if (nodeY == null)
                _rbTreeRoot = newItem;
            else if (newItem.Data < nodeY.Data)
                nodeY.Left = newItem;
            else
                nodeY.Right = newItem;

            newItem.Left = null;
            newItem.Right = null;
            newItem.Colour = RbTreeColor.Red;

            RbTreeInsertFixUp(newItem);
        }
    }

    private void RbTreeInsertFixUp(RbTreeNode item)
    {
        while (item != _rbTreeRoot && item.Parent.Colour == RbTreeColor.Red)
        {
            if (item.Parent == item.Parent.Parent.Left)
            {
                var nodeY = item.Parent.Parent.Right;
                if (nodeY is {Colour: RbTreeColor.Red})
                {
                    item.Parent.Colour = RbTreeColor.Black;
                    nodeY.Colour = RbTreeColor.Black;
                    item.Parent.Parent.Colour = RbTreeColor.Red;
                    item = item.Parent.Parent;
                }
                else
                {
                    if (item == item.Parent.Right)
                    {
                        item = item.Parent;
                        RbTreeLeftRotate(item);
                    }

                    item.Parent.Colour = RbTreeColor.Black;
                    item.Parent.Parent.Colour = RbTreeColor.Red;
                    
                    RbTreeRightRotate(item.Parent.Parent);
                }
            }
            else
            {
                var nodeX = item.Parent.Parent.Left;
                if (nodeX is {Colour: RbTreeColor.Black})
                {
                     item.Parent.Colour = RbTreeColor.Red;
                     nodeX.Colour = RbTreeColor.Red;
                     item.Parent.Parent.Colour = RbTreeColor.Black;
                     item = item.Parent.Parent;
                }
                else
                {
                    if (item == item.Parent.Left)
                    {
                        item = item.Parent;
                        RbTreeRightRotate(item);
                    }
                    
                    item.Parent.Colour = RbTreeColor.Black;
                    item.Parent.Parent.Colour = RbTreeColor.Red;
                    
                    RbTreeLeftRotate(item.Parent.Parent);
                }
            }
            
            _rbTreeRoot.Colour = RbTreeColor.Black;
        }
    }
    
    private int[] ReadXmlInts()
    {
        var docXml = new XmlDocument();
        docXml.Load(SortDataXml);

        return docXml
            .DocumentElement
            .ChildNodes
            .OfType<XmlNode>()
            .Select(node => int.Parse(node.InnerText))
            .ToArray();
    }
        
    private void WriteXmlInts(int[] intsData)
    { 
        File.WriteAllText(SortDataXml, string.Empty);

        using var writer = XmlWriter.Create(SortDataXml);
            
        writer.WriteStartDocument();
        writer.WriteStartElement("intsData");

        for (var i = 0; i < SizeOfIntsArray; i++)
            writer.WriteElementString("element" + i, intsData[i].ToString());
                
        writer.WriteEndElement();
        writer.WriteEndDocument();
                
        writer.Flush();
    }

    private void WriteTxtInts(int[] intsData, string filename)
    {
        File.WriteAllText(filename, string.Empty);

        var intsStrings = new string[SizeOfIntsArray];
        for (var i = 0; i < SizeOfIntsArray; i++)
            intsStrings[i] = intsData[i].ToString();
            
        File.WriteAllLines(filename, intsStrings);
    }

    private int[] GetIntsData()
    {
        var intsData = new int[SizeOfIntsArray];
        var rand = new Random();
        for (var i = 0; i < SizeOfIntsArray; i++)
            intsData[i] = rand.Next(0, SizeOfIntsArray);

        return intsData;
    }
}