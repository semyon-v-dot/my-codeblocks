using System.Xml;


namespace Lab2;

public class Program
{
    public static void Main(string[] args)
    {
        var sortFuncs = new SortFuncs();
        sortFuncs.SortFuncForTxtOutput = sortFuncs.Quicksort;
        
        sortFuncs.FillAndGetSortData();
        sortFuncs.FillAndGetSortedData();
    }
}