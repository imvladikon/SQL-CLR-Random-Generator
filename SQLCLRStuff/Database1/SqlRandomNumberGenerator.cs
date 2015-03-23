using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Security.Cryptography;

public partial class SqlRandomNumberGenerator
{
    private static readonly SecureRandom _rnd = new SecureRandom();
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlInt32 GetRandomInt(SqlInt32 from, SqlInt32 to)
    {
        if (from.IsNull || to.IsNull)
            return 0;
        return _rnd.Next(from.Value, to.Value);
    }

    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlDouble GetRandomDouble()
    {
        return _rnd.NextDouble();
    }
}

public class SecureRandom : RandomNumberGenerator
{
    private readonly RandomNumberGenerator rng = new RNGCryptoServiceProvider();


    public int Next()
    {
        var data = new byte[sizeof(int)];
        rng.GetBytes(data);
        return BitConverter.ToInt32(data, 0) & (int.MaxValue - 1);
    }

    public int Next(int maxValue)
    {
        return Next(0, maxValue);
    }

    public int Next(int minValue, int maxValue)
    {
        if (minValue > maxValue)
        {
            throw new ArgumentOutOfRangeException();
        }
        return (int)Math.Floor((minValue + ((double)maxValue - minValue) * NextDouble()));
    }

    public double NextDouble()
    {
        var data = new byte[sizeof(uint)];
        rng.GetBytes(data);
        var randUint = BitConverter.ToUInt32(data, 0);
        return randUint / (uint.MaxValue + 1.0);
    }

    public override void GetBytes(byte[] data)
    {
        rng.GetBytes(data);
    }

    public override void GetNonZeroBytes(byte[] data)
    {
        rng.GetNonZeroBytes(data);
    }
}
