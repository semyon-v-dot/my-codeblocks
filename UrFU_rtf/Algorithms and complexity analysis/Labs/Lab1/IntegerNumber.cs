namespace Lab1;

public class IntegerNumber
{
    private readonly byte[] _byteNumber;

    public IntegerNumber(byte[] inputByteNumber)
    {
        _byteNumber = inputByteNumber;
    }

    public IntegerNumber(int inputIntNumber)
    {
        if (inputIntNumber == 0)
            _byteNumber = new byte[] {0};
        else
        {
            var byteNumber = GetReversedByteNumberFromInt(inputIntNumber);
            byteNumber.Reverse();
            
            _byteNumber = byteNumber.ToArray();
        }
    }
    
    public IntegerNumber(int inputIntNumber, int sizeOfByteNumber)
    {
        if (sizeOfByteNumber <= 0)
            throw new ArgumentException(
                "Got non-positive sizeOfByteNumber for IntegerNumber init");
        
        if (inputIntNumber == 0)
            _byteNumber = new byte[sizeOfByteNumber];
        else
        {
            var byteNumber = GetReversedByteNumberFromInt(inputIntNumber);
            var inputIsNegative = inputIntNumber < 0;
            var defaultSizeOfByteNumber = byteNumber.Count;
            if (sizeOfByteNumber > defaultSizeOfByteNumber)
            {
                byteNumber[^1] %= 128;
                for (var i = 0; i < sizeOfByteNumber - defaultSizeOfByteNumber; i++)
                    byteNumber.Add(0);
                if (inputIsNegative)
                    byteNumber[^1] += 128;
            }
            
            byteNumber.Reverse();
            
            if (sizeOfByteNumber < defaultSizeOfByteNumber)
                _byteNumber = byteNumber.Skip(defaultSizeOfByteNumber - sizeOfByteNumber).ToArray();
            else
                _byteNumber = byteNumber.ToArray();
        }
    }

    public static List<byte> GetReversedByteNumberFromInt(int inputIntNumber)
    {
        var byteNumber = new List<byte>();
        var inputIsNegative = inputIntNumber < 0;
        inputIntNumber = Math.Abs(inputIntNumber);
        while (inputIntNumber > 0)
        {
            byteNumber.Add((byte) (inputIntNumber % 256));
            inputIntNumber /= 256;
        }
        if (inputIsNegative)
            byteNumber[^1] += 128; 

        return byteNumber;
    }
        
    public static int GetIntFromByteNumber(byte[] byteNumber)
    {
        var outputInt = byteNumber[0] % 128 * (int)Math.Pow(256, byteNumber.Length - 1);
        for (var i = 1; i < byteNumber.Length; i++)
            outputInt += byteNumber[i] * (int)Math.Pow(256, byteNumber.Length - 1 - i);

        return outputInt * (byteNumber[0] >= 128 ? -1 : 1);
    }

    public static IntegerNumber operator +(IntegerNumber a) => a;
    public static IntegerNumber operator -(IntegerNumber a)
    {
        a.ChangeSign();
        return a;
    }
    public static IntegerNumber operator +(IntegerNumber a, IntegerNumber b)
    {
        var aInt = a.ToInt();
        var bInt = b.ToInt();

        return new IntegerNumber(aInt + bInt);
    }
    public static IntegerNumber operator -(IntegerNumber a, IntegerNumber b)
    {
        var aInt = a.ToInt();
        var bInt = b.ToInt();

        return new IntegerNumber(aInt - bInt);
    } 
    public static IntegerNumber operator *(IntegerNumber a, IntegerNumber b)
    {
        var aIntNumber = a.ToInt();
        var bIntNumber = b.ToInt();

        return new IntegerNumber(aIntNumber * bIntNumber);
    }
    public static IntegerNumber operator /(IntegerNumber a, IntegerNumber b)
    {
        var aIntNumber = a.ToInt();
        var bIntNumber = b.ToInt();

        return new IntegerNumber(aIntNumber / bIntNumber);
    }
    public static IntegerNumber operator %(IntegerNumber a, IntegerNumber b)
    {
        var aIntNumber = a.ToInt();
        var bIntNumber = b.ToInt();

        return new IntegerNumber(aIntNumber % bIntNumber);
    }
    public static bool operator ==(IntegerNumber a, IntegerNumber b)
    {
        var aIntNumber = a.ToInt();
        var bIntNumber = b.ToInt();

        return aIntNumber == bIntNumber;
    }
    public static bool operator !=(IntegerNumber a, IntegerNumber b)
    {
        var aIntNumber = a.ToInt();
        var bIntNumber = b.ToInt();

        return aIntNumber != bIntNumber;  
    }
    public static bool operator <(IntegerNumber a, IntegerNumber b)
    {
        var aIntNumber = a.ToInt();
        var bIntNumber = b.ToInt();

        return aIntNumber < bIntNumber;  
    }
    public static bool operator >(IntegerNumber a, IntegerNumber b)
    {
        var aIntNumber = a.ToInt();
        var bIntNumber = b.ToInt();

        return aIntNumber > bIntNumber;  
    }

    public void ChangeSign()
    {
        _byteNumber[0] += 128;
    }

    public int ToInt() => GetIntFromByteNumber(_byteNumber);

    public byte[] GetByteNumber() => _byteNumber.ToArray();
    
}