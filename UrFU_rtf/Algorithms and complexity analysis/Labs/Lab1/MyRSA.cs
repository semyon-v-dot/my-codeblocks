using System.Numerics;
using System.Text;

namespace Lab1;

public class MyRSA
{
    public const string DirPath =
        @"D:\Repos\Github\sandbox\UrFU_rtf\Algorithms and complexity analysis\Labs\Lab1";
    
    // private readonly BigInteger _firstPrimeNumber = 
    //     BigInteger.Parse("5952375489097797526619376154050271099954201063542419729565738822130169832558603" +
    //                      "643");
    // private readonly BigInteger _secondPrimeNumber = 
    //     BigInteger.Parse("1311584681678192016405886702778348906691760859143036835210494658595077597");
    private readonly BigInteger _modulus = 
        BigInteger.Parse("7807044511097407282109874868135407004876031649759474728764966784299916587117598" +
                         "097140587086644129723966592099572477954900854596303362328951705311551885871");
    private readonly BigInteger _publicExponent = 65537;
    private readonly BigInteger _secretExponent =
        BigInteger.Parse("6282254396474418539704723605304743262830878452070821349327525722929409352593349" +
                         "476039238165759229391755493439010746328446013505935818198891488163103897305");

    private const int QuantityOfBytesInEncodedNumber = 50;

    public void DoRSA()
    {

        var data = File.ReadAllBytes(DirPath + @"\TextForRSA.txt");
        var dataBigInts = GetBigIntsFromBytes(data);
        
        var quantityOfBytesInLastEncodedNumber = data.Length % QuantityOfBytesInEncodedNumber;
        quantityOfBytesInLastEncodedNumber =
            quantityOfBytesInLastEncodedNumber == 0
                ? QuantityOfBytesInEncodedNumber
                : quantityOfBytesInLastEncodedNumber;
        
        var encryptedBigInts = dataBigInts.Select(Encrypt); 
        var encryptedText = encryptedBigInts.Select(bigInt => bigInt.ToString()).ToArray();
        
        File.WriteAllLines(DirPath + @"\EncryptedText", encryptedText);
        
        var decryptedBigInts = encryptedText.Select(BigInteger.Parse).Select(Decrypt);
        var decryptedData = 
            GetBytesFromBigInts(decryptedBigInts.ToArray(), quantityOfBytesInLastEncodedNumber);

        File.WriteAllText(
            DirPath + @"\DecryptedText.txt", Encoding.UTF8.GetString(decryptedData));
    }
    
    public BigInteger[] GetBigIntsFromBytes(IEnumerable<byte> bytes)
    {
        var bigInts = new List<BigInteger>();
        var strBytes = new StringBuilder();
        foreach (var singleByte in bytes)
        {
            strBytes.Append(GetThreeDigitsByte(singleByte));

            if (strBytes.Length != QuantityOfBytesInEncodedNumber * 3) 
                continue;
            
            bigInts.Add(BigInteger.Parse(strBytes.ToString()));
            strBytes.Clear();
        }
        if (strBytes.Length != 0)
            bigInts.Add(BigInteger.Parse(strBytes.ToString()));
        
        return bigInts.ToArray();
    }

    public string GetThreeDigitsByte(byte singleByte)
        => singleByte switch
        {
            < 10 => "00" + singleByte,
            < 100 => "0" + singleByte,
            _ => singleByte.ToString()
        };
    
    public byte[] GetBytesFromBigInts(BigInteger[] bigInts, int quantityOfBytesInLastEncodedNumber)
    {
        if (bigInts.Length == 0)
            return Array.Empty<byte>();
        
        var bytes = new List<byte>();
        var bigIntStr = new StringBuilder();
        for (var i = 0; i < bigInts.Length - 1; i++)
        {
            bigIntStr.Append(bigInts[i].ToString());
            while (bigIntStr.Length < QuantityOfBytesInEncodedNumber * 3)
                bigIntStr.Insert(0, "0");
            bytes.AddRange(GetThreeCharsStringsFromString(bigIntStr.ToString()).Select(byte.Parse));
            
            bigIntStr.Clear();
        }
        bigIntStr.Append(bigInts[^1].ToString());
        while (bigIntStr.Length < quantityOfBytesInLastEncodedNumber * 3)
            bigIntStr.Insert(0, "0");
        bytes.AddRange(GetThreeCharsStringsFromString(bigIntStr.ToString()).Select(byte.Parse));

        return bytes.ToArray();
    }

    public string[] GetThreeCharsStringsFromString(string inputString)
    {
        if (inputString.Length % 3 != 0)
            throw new ArgumentException("Input string must be with length as multiple of three");

        var outputString = new string[inputString.Length / 3];
        for (var i = 0; i < outputString.Length; i++)
            outputString[i] = inputString.Substring(i * 3, 3);

        return outputString;
    }

    private BigInteger Encrypt(BigInteger data) 
        => BigInteger.ModPow(data, _publicExponent, _modulus);

    private BigInteger Decrypt(BigInteger encryptedData)
        => BigInteger.ModPow(encryptedData, _secretExponent, _modulus);
}