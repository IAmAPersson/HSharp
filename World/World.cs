using System;
using System.Diagnostics;

public static class World
{
    private static Tuple<TimeSpan, DateTime> Value;
    public static Stopwatch Stop = new Stopwatch();
    public static void RefreshWorld()
    {
        Value = new Tuple<TimeSpan, DateTime>(Stop.Elapsed, DateTime.Now);
    }
}