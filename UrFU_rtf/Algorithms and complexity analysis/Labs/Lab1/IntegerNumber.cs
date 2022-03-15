namespace Lab1;

public class IntegerNumber
{
    protected bool Equals(IntegerNumber other)
    {
        return _byteNumber.Equals(other._byteNumber);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((IntegerNumber) obj);
    }

    public override int GetHashCode()
    {
        return _byteNumber.GetHashCode();
    }

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
            
            _byteNumber = 
                sizeOfByteNumber < defaultSizeOfByteNumber 
                    ? byteNumber.Skip(defaultSizeOfByteNumber - sizeOfByteNumber).ToArray() 
                    : byteNumber.ToArray();
        }
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

    public int ToInt() => GetIntFromByteNumber(_byteNumber);

    public byte[] GetByteNumber() => _byteNumber.ToArray();
    
    private static List<byte> GetReversedByteNumberFromInt(int inputIntNumber)
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
    
    private void ChangeSign()
    {
        _byteNumber[0] += 128;
    }
}