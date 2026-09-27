using System;

namespace Blobeatblob.Enums;

public enum BlobGrowAmount
{
    Small,
    Large
}

public static class DirectionExtensions
{

public static int Size(this BlobGrowAmount growAmount)
    {
        return growAmount switch
        {
            BlobGrowAmount.Small => 1,
            BlobGrowAmount.Large => 2,
            _ => throw new ArgumentOutOfRangeException(nameof(growAmount))
        };
    }
}

