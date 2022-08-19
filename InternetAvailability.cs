﻿using System;
using System.Runtime;
using System.Runtime.InteropServices;

public class InternetAvailability
{
    [DllImport("wininet.dll")]
    private extern static bool InternetGetConnectedState(out int description, int reservedValue);
    public static bool InternetAvailability_IsAvailable()
    {
        int description;
        return InternetGetConnectedState(out description, 0);
    }
}
