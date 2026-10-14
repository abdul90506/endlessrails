namespace EndlessRails
{
    /// <summary>Top-level game state machine.</summary>
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Tutorial
    }

    /// <summary>Cell types used by procedural row patterns.</summary>
    public enum TrackCell
    {
        Empty = 0,
        Coin = 1,
        BarrierJump = 2,      // low barrier - jump over
        BarrierSlide = 3,     // overhead beam - slide under
        Cone = 4,             // stumble-only hazard
        Block = 5,            // construction block - full lane block
        LowWall = 6,          // must jump or change lane
        Train = 7,            // long stationary train
        TrainMoving = 8,      // train passing through
        Vehicle = 9,          // vehicle crossing at rail crossing
        PickupMagnet = 10,
        PickupJetpack = 11,
        PickupShield = 12,
        PickupMultiplier = 13
    }

    /// <summary>How lethal an obstacle is on contact.</summary>
    public enum ObstacleImpact
    {
        Lethal,          // game over
        Stumble          // stumble + slow down, chaser closes in
    }

    /// <summary>Power-up identifiers.</summary>
    public enum PickupType
    {
        Magnet,
        Jetpack,
        Shield,
        Multiplier
    }

    /// <summary>Footstep surface classification.</summary>
    public enum SurfaceType
    {
        Concrete,
        Gravel,
        Metal,
        Wet
    }

    /// <summary>Character body type for animation/voice variants.</summary>
    public enum RunnerGender
    {
        Male,
        Female
    }

    /// <summary>Weather states for the environment system.</summary>
    public enum WeatherState
    {
        Clear,
        Overcast,
        LightRain
    }

    /// <summary>Mission objective kinds.</summary>
    public enum MissionType
    {
        CollectCoins,
        RunDistance,
        JumpCount,
        SlideCount,
        CollectPickups,
        ReachScore,
        PlayRuns
    }
}
