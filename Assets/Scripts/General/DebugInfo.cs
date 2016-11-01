using UnityEngine;
using System.Collections;

public static class DebugInfo
{
    public static string currentTile;

    public static int currentOxygen, maxOxygen;

    public static string GetString()
    {
        return string.Format("Debug Info:\n\tCurrent Tile: {0}\n\tOxygen: {1}/{2}", currentTile, currentOxygen, maxOxygen);
    }
}
