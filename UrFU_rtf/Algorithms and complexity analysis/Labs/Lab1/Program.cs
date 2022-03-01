using System.Numerics;
using System.Reflection;
using System.Text;

namespace Lab1;

public class Program
{
    public static void Main(string[] args)
    {
        var dirPath = @"D:\Repos\Github\sandbox\UrFU_rtf\Algorithms and complexity analysis\Labs\Lab1";
        var data = 
            new BigInteger(File.ReadAllBytes(dirPath + @"\TextForRSA.txt"));
        var encryptedData = MyRSA.Encrypt(data).ToByteArray();
        var decryptedData = 
            Encoding.UTF8.GetString(
                MyRSA.Decrypt(new BigInteger(encryptedData)).ToByteArray());
            
        File.WriteAllBytes(dirPath + @"EncryptedText", encryptedData);
        File.WriteAllText(dirPath + @"DecryptedText.txt", decryptedData);
    }
}