using System.Collections.Generic;
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

        public const float BlobScaleFactor = 0.5f;

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
        public const int IncreasedSizeToLevelUp = 15;

        public const int LungeUnlockLevel = 2;

        public const int DeflectUnlockLevel = 2;

        public const int StartingLevel = 1; //If StartingLevel > 1, this is a debug cheat for playtesting
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

    public static class Tutorials
    {
        //level -> tutorial shown when the player reaches that level (level 1 is shown on first start)
        public static readonly Dictionary<int, string> ByLevel = new()
        {
            [1] = """
                Welcome to Blob Eat Blob!

                In this game, you will play as a blob in an ocean of other
                blobs. The basic objective of this game is to eat smaller
                blobs, and avoid being eaten by bigger blobs.

                ->  You play as the blob in the middle of the screen
                    that has a blue ring around it.

                ->  Bigger blobs have a yellow ring around them, you will be
                    eaten if you collide with them.

                ->  Smaller blobs have a red ring around them, you can eat them
                    by moving towards them and colliding with them which will
                    increase your size and you will gain progress towards your
                    "Shark Level".

                ->  Your Shark Level is determined by how big you are, and will
                    unlock additional gameplay elements for levels 2 and 3.
                    Being eaten will reset your progress on your next Shark Level.

                ->  Basic Controls: WASD will move your blob through the
                ocean environment. Pressing Escape will toggle fullscreen mode.
                
                """,
            [2] = """
                You have gained Shark Abilities! and enemy blobs move faster.

                ->  There are two statuses that shark abilites can apply
                    or inflict to you and other blobs. Protected, and suppressed.

                ->  When protected, blobs cannot be eaten by larger blobs.
                    blobs that are protected will appear yellow.

                ->  When suppressed, blobs can be eaten by smaller blobs.
                    Blobs that are suppressed will appear red.

                ->  You can now lunge. You will lunge towards your mouse
                    pointer when you activate lunge. You press and hold
                    your left mouse button to charge your lunge, and
                    it will activate when released or when fully charged.
                    on a failed lunge, you will be suppressed for a short time.

                ->  You can now deflect. When you press the right mouse button,
                    you will be protected for a short duration, and will
                    pushback any blob that you collide with.

                ->  During a lunge or deflect, you will be protected and,
                    cause any blob you collide with to be suppressed.

                ->  Each ability has a moderate cooldown that is displayed,
                    at the top of the screen. Eating another blob will instantly
                    finish the cooldown on your abilities, allowing them to be used again.
                """,
            [3] = """
                You are now level three, and have grown large enough to encounter enemy sharks.

                ->  Enemy sharks have the deflect ability. A successful deflect will counter your attacks.

                ->  You can continue to grow, and increase your shark level indefinitely, but there won't
                    Be anymore abilities to earn or new blob types to encounter.

                Good luck against the other sharks!
                """,
        };
    }
}
