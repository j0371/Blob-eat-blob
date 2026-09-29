using System;
using BlobEatBlob.GameScripts;

namespace Blobeatblob.Enums.GameStates;

public enum BlobStates
{
    Normal,
    Suppressed,
    Protected,
    Cloaked,
    Shrouded
}

public enum LungeStates
{
    AttackReady,
    LungeCharging,
    Lunging,
    LungeRecovery,
    OnCooldown
}

public enum DeflectStates
{
    DeflectReady,
    Deflecting,
    OnCooldown
}

