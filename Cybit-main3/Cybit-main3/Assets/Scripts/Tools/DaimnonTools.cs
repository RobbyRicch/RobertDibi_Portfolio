using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


// sort by string Property list.Sort((x, y) => string.Compare(x.Property, y.Property));
// sort by int Property list.Sort((x, y) => x.Property.CompareTo(y.Property));

public static class DaimnonTools
{
    #region Float Time Handler
    private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static float GetDateTimeAsFloat(DateTime dateTime)
    {
        return (float)(dateTime.ToUniversalTime() - UnixEpoch).TotalSeconds;
    }
    public static DateTime GetFloatAsDateTime(float timestamp)
    {
        return UnixEpoch.AddSeconds(timestamp).ToLocalTime();
    }
    #endregion
}
