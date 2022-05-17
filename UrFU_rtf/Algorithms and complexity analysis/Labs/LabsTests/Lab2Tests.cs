using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Lab2;

namespace LabsTests;

public class Lab2Tests
{
    private const string DirPath =
        @"D:\Repos\Github\sandbox\UrFU_rtf\Algorithms and complexity analysis\Labs\Lab2";    
    private const string SortDataTxt = DirPath + @"\SortData.txt";
    private const string SortedDataTxt = DirPath + @"\SortedData.txt";
    private const string SortFuncsInfoTxt = 
        @"D:\Repos\Github\sandbox\UrFU_rtf\Algorithms and complexity analysis\Labs\LabsTests\"
        + @"SortFuncsInfo.txt";

    private readonly SortFuncs _sortFuncs = new();
    
    // [OneTimeSetUp]
    // public void StartTimerAndMemoryCheck()
    // {
    //     throw new NotImplementedException();
    // }

    // [TearDown]
    // public void TimerAndMemoryCheck()
    // {
    //     // Print into "SortFuncsInfo.txt"
    //     throw new NotImplementedException();
    // }
    //
    [Test]
    public void TestBubbleSort()
    {
        _sortFuncs.SortFuncForTxtOutput = _sortFuncs.BubbleSort;

        TestSort(_sortFuncs);
    }
    
    [Test]
    public void TestQuicksort()
    {
        _sortFuncs.SortFuncForTxtOutput = _sortFuncs.Quicksort;

        TestSort(_sortFuncs);
    }
        
    [Test]
    public void TestTreeSort()
    {
        _sortFuncs.SortFuncForTxtOutput = _sortFuncs.TreeSort;

        TestSort(_sortFuncs);
    }
    
    [Test]
    public void TestInsertionSort()
    {
        _sortFuncs.SortFuncForTxtOutput = _sortFuncs.InsertionSort;

        TestSort(_sortFuncs);
    }
    
    [Test]
    public void TestMergeSort()
    {
        _sortFuncs.SortFuncForTxtOutput = _sortFuncs.MergeSort;

        TestSort(_sortFuncs);
    }
    
    [Test]
    public void TestRedBlackTreeSort()
    {
        _sortFuncs.SortFuncForTxtOutput = _sortFuncs.RedBlackTreeSort;

        TestSort(_sortFuncs);
    }
    
    private void TestSort(SortFuncs sortFuncs)
    {
        sortFuncs.FillAndGetSortData();
        sortFuncs.FillAndGetSortedData();
        
        var sortDataStrings = File.ReadAllLines(SortDataTxt).Select(int.Parse).ToArray();
        var sortedDataStrings = File.ReadAllLines(SortedDataTxt).Select(int.Parse).ToArray();
        Array.Sort(sortDataStrings);
        
        Assert.AreEqual(sortDataStrings, sortedDataStrings);
    }
}