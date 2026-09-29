using Godot;

namespace BlobEatBlob.HelperScripts;

public static class GameConfig
{
    public static class Blob
    {
        public const float Speed = 300.0f;

        public const float Acceleration = Speed * 10;

        public const float Deceleration = Speed * 15;

        public const int StartingSize = 1;

        public const int SmallGrowAmount = 1;

        public const int LargeGrowAmount = 2;
    }

    public static class NonPlayer
    {
        public const int MinStartingGrowth = 0;

        public const int MaxStartingGrowth = 1;

        private const int averageNonPlayerDirectionChangeSeconds = 5; //The average number of seconds it takes before a nonplayer blob changes directions (varies randomly)

        public static int DirectionChangeWeight => Engine.PhysicsTicksPerSecond * averageNonPlayerDirectionChangeSeconds;
    }

    public static class Shark
    {
        public const int AttackCooldownSeconds = 10;

        public const float LungeSpeedMultiplier = 1.5f;

        public const double MaxChargeSeconds = 3;

        public const double lungeSecondsPerChargeSecond = 1.0 / 3;

        public const double LungeRecoverySeconds = 1;

        public const double DeflectWindowSeconds = 0.5;

        public const int DefendCooldownSeconds = 10;
    }

    public static class NonPlayerSpawner
    {
        public const int MaxNonPlayerCount = 30;

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
    }
}
