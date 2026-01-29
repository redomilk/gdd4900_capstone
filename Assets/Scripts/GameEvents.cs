using UnityEngine;
using System;

public static class GameEvents
{
    public static Action OnPlayerStartFalling;
    public static Action OnPlayerHitWater;
    public static Action OnPlayerLeftWater;
    public static Action OnPlayerSuspended;
    public static Action OnPlayerEnterAirPocket;
    public static Action OnPlayerExitAirPocket;

    public static Action<float, float> OnHealthChanged;  // current, max
    public static Action<float, float> OnOxygenChanged;  // current, max
    public static Action OnPlayerDied;

    public static Action<float> OnDepthChanged;
    public static Action<string> OnAugmentCollected;
}