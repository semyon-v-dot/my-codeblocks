using System.IO;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using Lab1;

namespace LabsTests;

public class Lab1Tests
{

    [TestCase(-8388607, new byte[] {255, 255, 255})]
    [TestCase(-32767, new byte[] {255, 255})]
    [TestCase(-127, new byte[] {255})]
    [TestCase(8388607, new byte[] {127, 255, 255})]
    [TestCase(32767, new byte[] {127, 255})]
    [TestCase(127, new byte[] {127})]
    [TestCase(23, new byte[] {23})]
    [TestCase(52 * 256 + 231, new byte[] {52, 231})]
    [TestCase(-(107 * 256 * 256 + 24 * 256 + 2), new byte[] {235, 24, 2})]
    [TestCase(0, new byte[] {0})]
    public void ByteArrayToIntTests(int resultIntNumber, byte[] byteNumber)
    {
        Assert.AreEqual(resultIntNumber, IntegerNumber.GetIntFromByteNumber(byteNumber));
    }

    [TestCase(new byte[] {255, 255, 255}, -8388607)]
    [TestCase(new byte[] {255, 255}, -32767)]
    [TestCase(new byte[] {255}, -127)]
    [TestCase(new byte[] {127, 255, 255}, 8388607)]
    [TestCase(new byte[] {127, 255}, 32767)]
    [TestCase(new byte[] {127}, 127)]
    [TestCase(new byte[] {23}, 23)]
    [TestCase(new byte[] {52, 231}, 52 * 256 + 231)]
    [TestCase(new byte[] {235, 24, 2}, -(107 * 256 * 256 + 24 * 256 + 2))]
    [TestCase(new byte[] {0}, 0)]
    public void ByteNumberFromIntConstructorTests(byte[] resultByteNumber, int inputIntNumber)
    {
        //Optimize: not constructor tests but GetReversedByteNumberFromInt tests
        Assert.AreEqual(resultByteNumber, new IntegerNumber(inputIntNumber).GetByteNumber());
    }
    
    //Equals to default
    [TestCase(new byte[] {255, 255, 255}, -8388607, 3)]
    [TestCase(new byte[] {255, 255}, -32767, 2)]
    [TestCase(new byte[] {255}, -127, 1)]
    [TestCase(new byte[] {127, 255, 255}, 8388607, 3)]
    [TestCase(new byte[] {127, 255}, 32767, 2)]
    [TestCase(new byte[] {127}, 127, 1)]
    [TestCase(new byte[] {23}, 23, 1)]
    [TestCase(new byte[] {52, 231}, 52 * 256 + 231, 2)]
    [TestCase(new byte[] {235, 24, 2}, -(107 * 256 * 256 + 24 * 256 + 2), 3)]
    [TestCase(new byte[] {0}, 0, 1)]
    //More than default
    [TestCase(new byte[] {128, 127, 255, 255}, -8388607, 4)]
    [TestCase(new byte[] {128, 127, 255}, -32767, 3)]
    [TestCase(new byte[] {0, 127, 255, 255}, 8388607, 4)]
    [TestCase(new byte[] {0, 127, 255}, 32767, 3)]
    [TestCase(new byte[] {0, 23}, 23, 2)]
    [TestCase(new byte[] {0, 52, 231}, 52 * 256 + 231, 3)]
    [TestCase(new byte[] {0, 0, 0}, 0, 3)]
    //Less than default
    [TestCase(new byte[] {255, 255}, -8388607, 2)]
    [TestCase(new byte[] {255}, -32767, 1)]
    [TestCase(new byte[] {255, 255}, 8388607, 2)]
    [TestCase(new byte[] {255}, 32767, 1)]
    [TestCase(new byte[] {}, 23, 0)]
    [TestCase(new byte[] {231}, 52 * 256 + 231, 1)]
    [TestCase(new byte[] {}, 0, 0)]
    public void ByteNumberFromIntAndSizeConstructorTests(
        byte[] resultByteNumber, int inputIntNumber, int sizeOfByteNumber)
    {
        if (sizeOfByteNumber == 0)
            Assert.That(
                () => new IntegerNumber(inputIntNumber, sizeOfByteNumber).GetByteNumber(),
                Throws.ArgumentException);
        else
            Assert.AreEqual(
                resultByteNumber, 
                new IntegerNumber(inputIntNumber, sizeOfByteNumber).GetByteNumber());
    }

    [TestCase(1, 1)]
    [TestCase(0, 0)]
    [TestCase(-1, -1)]
    [TestCase(100000000, 100000000)]
    [TestCase(-100000000, -100000000)]
    public void UnaryPlusTests(int expectedIntNumber, int inputIntNumber)
    {
        var intNumber = new IntegerNumber(inputIntNumber);
        Assert.AreEqual(expectedIntNumber, (+intNumber).ToInt());
    }
    
    [TestCase(1, -1)]
    [TestCase(0, 0)]
    [TestCase(-1, 1)]
    [TestCase(100000000, -100000000)]
    [TestCase(-100000000, 100000000)]
    public void UnaryMinusTests(int expectedIntNumber, int inputIntNumber)
    {
        var intNumber = new IntegerNumber(inputIntNumber);
        Assert.AreEqual(expectedIntNumber, (-intNumber).ToInt());
    }
    
    [TestCase(2, 1, 1)]
    [TestCase(0, 0, 0)]
    [TestCase(-2, -1, -1)]
    [TestCase(99, 100, -1)]
    [TestCase(0, -100000000, 100000000)]
    public void BinaryPlusTests(int expectedResult, int a, int b)
    {
        var aNumber = new IntegerNumber(a);
        var bNumber = new IntegerNumber(b);
        
        Assert.AreEqual(expectedResult, (aNumber + bNumber).ToInt());
    }
    
    [TestCase(0, 1, 1)]
    [TestCase(0, 0, 0)]
    [TestCase(0, -1, -1)]
    [TestCase(101, 100, -1)]
    [TestCase(-200000000, -100000000, 100000000)]
    public void BinaryMinusTests(int expectedResult, int a, int b)
    {
        var aNumber = new IntegerNumber(a);
        var bNumber = new IntegerNumber(b);
        
        Assert.AreEqual(expectedResult, (aNumber - bNumber).ToInt());
    }
    [TestCase(1, 1, 1)]
    [TestCase(0, 0, 0)]
    [TestCase(0, 0, -1)]
    [TestCase(-100, 100, -1)]
    [TestCase(123 * -123, -123, 123)]
    public void MultiplicationTests(int expectedResult, int a, int b)
    {
        var aNumber = new IntegerNumber(a);
        var bNumber = new IntegerNumber(b);
        
        Assert.AreEqual(expectedResult, (aNumber * bNumber).ToInt());        
    }
    [TestCase(1, 1, 1)]
    [TestCase(0, 0, 1)]
    [TestCase(0, 0, -1)]
    [TestCase(100, 100, 1)]
    [TestCase(1, 123, 123)]
    [TestCase(127/5, 127, 5)]
    [TestCase(33/6, 33, 6)]
    public void EuclideanDivisionTests(int expectedResult, int a, int b)
    {
        var aNumber = new IntegerNumber(a);
        var bNumber = new IntegerNumber(b);
        
        Assert.AreEqual(expectedResult, (aNumber / bNumber).ToInt());        
    }
    [TestCase(0, 0, 1)]
    [TestCase(0, 100, 2)]
    [TestCase(0, 123, 123)]
    [TestCase(2, 127, 5)]
    [TestCase(3, 33, 6)]
    public void GetRemainderOfDivisionTests(int expectedResult, int a, int b)
    {
        var aNumber = new IntegerNumber(a);
        var bNumber = new IntegerNumber(b);
        
        Assert.AreEqual(expectedResult, (aNumber % bNumber).ToInt());        
    }
    [TestCase(false, 0, 1)]
    [TestCase(true, 0, 0)]
    [TestCase(false, 1, 0)]
    [TestCase(false, 100, 2)]
    [TestCase(true, 123, 123)]
    [TestCase(false, -127, 5)]
    [TestCase(false, 33, -6)]
    [TestCase(true, -6, -6)]
    public void AreEqualOperatorTests(bool expectedResult, int a, int b)
    {
        var aNumber = new IntegerNumber(a);
        var bNumber = new IntegerNumber(b);
        
        Assert.AreEqual(expectedResult, aNumber == bNumber);        
    }
    [TestCase(true, 0, 1)]
    [TestCase(false, 0, 0)]
    [TestCase(true, 1, 0)]
    [TestCase(true, 100, 2)]
    [TestCase(false, 123, 123)]
    [TestCase(true, -127, 5)]
    [TestCase(true, 33, -6)]
    [TestCase(false, -6, -6)]
    public void AreNotEqualOperatorTests(bool expectedResult, int a, int b)
    {
        var aNumber = new IntegerNumber(a);
        var bNumber = new IntegerNumber(b);
        
        Assert.AreEqual(expectedResult, aNumber != bNumber);        
    }
    [TestCase(true, 0, 1)]
    [TestCase(false, 0, 0)]
    [TestCase(false, 1, 0)]
    [TestCase(false, 100, 2)]
    [TestCase(false, 123, 123)]
    [TestCase(true, -127, 5)]
    [TestCase(false, 33, -6)]
    [TestCase(false, -6, -6)]
    public void LessThatOperatorTests(bool expectedResult, int a, int b)
    {
        var aNumber = new IntegerNumber(a);
        var bNumber = new IntegerNumber(b);
        
        Assert.AreEqual(expectedResult, aNumber < bNumber);        
    }
    [TestCase(false, 0, 1)]
    [TestCase(false, 0, 0)]
    [TestCase(true, 1, 0)]
    [TestCase(true, 100, 2)]
    [TestCase(false, 123, 123)]
    [TestCase(false, -127, 5)]
    [TestCase(true, 33, -6)]
    [TestCase(false, -6, -6)]
    public void MoreThanOperatorTests(bool expectedResult, int a, int b)
    {
        var aNumber = new IntegerNumber(a);
        var bNumber = new IntegerNumber(b);
        
        Assert.AreEqual(expectedResult, aNumber > bNumber);        
    }
    
    [Test]
    public void RSATest()
    {
        new MyRSA().DoRSA();
        
        var textForRSA = File.ReadAllText(MyRSA.DirPath + @"\TextForRSA.txt");
        var decryptedText = File.ReadAllText(MyRSA.DirPath + @"\DecryptedText.txt");
        if (decryptedText != "")
            Assert.AreEqual(textForRSA, decryptedText);
    }
    
    [TestCase("123", 123)]
    [TestCase("023", 23)]
    [TestCase("003", 3)]
    [TestCase("000", 0)]
    public void GetThreeDigitsByteTests(string expected, byte singleByte)
    {
        Assert.AreEqual(expected, new MyRSA().GetThreeDigitsByte(singleByte));
    }

    [TestCase(new [] { "123" }, new byte[] {123})]
    public void GetBigIntsFromBytesTests(string[] expectedStr, byte[] bytes)
    {
        Assert.AreEqual(expectedStr.Select(BigInteger.Parse), new MyRSA().GetBigIntsFromBytes(bytes));
    }

    [TestCase(new[] {"123"}, "123")]
    [TestCase(new[] {"123", "123", "123", "123"}, "123123123123")]
    [TestCase(new string[] {}, "")]
    public void GetThreeCharsStringsFromStringTests(string[] expected, string input)
    {
        Assert.AreEqual(expected, new MyRSA().GetThreeCharsStringsFromString(input));
    }

    [TestCase(new byte[] {123}, new[] {"123"}, 1)]
    [TestCase(new byte[] {}, new string[] {}, 0)]
    public void GetBytesFromBigIntsTests(
        byte[] expected, string[] bigIntsStrings, int quantityOfBytesInLastNumber)
    {
        Assert.AreEqual(
            expected, 
            new MyRSA().GetBytesFromBigInts(
                bigIntsStrings.Select(BigInteger.Parse).ToArray(), quantityOfBytesInLastNumber));
    }
}