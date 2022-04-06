using System;
using UnityEngine;

public static class Utils {
    private const int DISPLAY_MEDIUM_INTERVALS = 3;
    private const int DISPLAY_HEAVY_INTERVALS = 5;

    public static bool DisplayMediumFrame => Time.frameCount % DISPLAY_MEDIUM_INTERVALS == 0;
    public static bool DisplayHeavyFrame => Time.frameCount % DISPLAY_HEAVY_INTERVALS == 0;

    public static bool CoinFlip(float chance = .5f)
    {
        return (UnityEngine.Random.value < chance);
    }

    public static bool IsNull(object obj)
    {
        return obj == null || obj.Equals(null) || obj == "null";
    }

    public static Guid IntToGuid(int value)
    {
        byte[] bytes = new byte[16];
        BitConverter.GetBytes(value).CopyTo(bytes, 0);

        return new Guid(bytes);
    }

    public static int GuidToInt(Guid value)
    {
        byte[] b = value.ToByteArray();
        int bint = BitConverter.ToInt32(b, 0);

        return bint;
    }

    public static float GetNearestMultiple(float value,  float factor, int offset = 0)
    {
        return (float)Math.Round(value / factor, MidpointRounding.AwayFromZero) * factor + offset * factor;
    }
            
}
