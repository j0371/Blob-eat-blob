using Godot;

namespace BlobEatBlob.HelperScripts;

public static class GameConfig
{
    public static class Blob
    {
        public const float Speed = 400.0f;

        public const float Acceleration = Speed * 10;

        public const float Deceleration = Speed * 15;

        public const double SuppressionSeconds = 2;

        public const float SuppressedSpeedFactor = 1f / 6;

        public const int StartingSize = 10;

        public const float BlobScaleFactor = 1f / 3;

        public const int SmallGrowAmount = 1;

        public const int LargeGrowAmount = 3;
    }

    public static class NonPlayer
    {
        public const int blobSpawnSizeRangeFromPlayer = 5;

        private const int averageNonPlayerDirectionChangeSeconds = 5; //The average number of seconds it takes before a nonplayer blob changes directions (varies randomly)

        public static int DirectionChangeWeight => Engine.PhysicsTicksPerSecond * averageNonPlayerDirectionChangeSeconds;

        public static float LevelOneSpeed => Blob.Speed * .6f;

        public const float DeflectChance = 0.8f;

        public const double DeflectPressSeconds = 0.1;

        public const int LungeUnlockLevel = 3;

        public const int DeflectUnlockLevel = 3;
    }

    public static class Player
    {
        public const int IncreasedSizeToLevelUp = 10;

        public const int LungeUnlockLevel = 2;

        public const int DeflectUnlockLevel = 2;

        public const int StartingLevel = 1;
    }

    public static class Shark
    {
        public const int AttackCooldownSeconds = 5;

        public const int DefendCooldownSeconds = 5;

        public const float LungeSpeedMultiplier = 3f;

        public const double MaxChargeSeconds = 1;

        public const double lungeSecondsPerChargeSecond = 1.0 / 3;

        public const double LungeRecoverySeconds = 1;

        public const double DeflectWindowSeconds = 0.25;

        public const float DeflectPushbackSpeed = Blob.Speed * 3;

        public const double DeflectPushbackSeconds = 0.2;
    }

    public static class NonPlayerSpawner
    {
        public const int MaxNonPlayerCount = 200;

        public const float MarginMultiplier = 2f;
    }

    public static class InputActions
    {
        public const string Up = "up";

        public const string Down = "down";

        public const string Left = "left";

        public const string Right = "right";

        public const string Attack = "attack";

        public const string Defend = "defend";

        public const string Fullscreen = "fullscreen";

        public const float fullscreenPressDelay = .2f;
    }
}
