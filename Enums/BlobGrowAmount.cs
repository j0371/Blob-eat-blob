using System;
using BlobEatBlob.HelperScripts;

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
            BlobGrowAmount.Small => GameConfig.Blob.SmallGrowAmount,
            BlobGrowAmount.Large => GameConfig.Blob.LargeGrowAmount,
            _ => throw new ArgumentOutOfRangeException(nameof(growAmount))
        };
    }
}

