using System.Numerics;

namespace Lab1;

public static class MyRSA
{
    private static readonly BigInteger PublicExponent = BigInteger.Parse("506344276674308774731265968867");
    private static readonly BigInteger SecretExponent = BigInteger.Parse("158895884583869479056912095063");
    private static readonly BigInteger Modulus = BigInteger.Multiply(PublicExponent, SecretExponent);
    
    public static BigInteger Encrypt(BigInteger data) 
        => BigInteger.ModPow(data, PublicExponent, Modulus);
    
    public static BigInteger Decrypt(BigInteger encryptedData)
        => BigInteger.ModPow(encryptedData, SecretExponent, Modulus);
}