using UnityEngine;
using System.Collections;

public static class DebugInfo
{
    public static bool displayDebugInfo = false;

    public static float framesPerSecond;

    public static string currentTile;

    public static int currentOxygen, maxOxygen;

    public static string GetString()
    {
        return string.Format("Debug Info:\n\tFPS:{0}\n\tCurrent Tile: {1}\n\tOxygen: {2}/{3}", framesPerSecond, currentTile, currentOxygen, maxOxygen);
    }
}
