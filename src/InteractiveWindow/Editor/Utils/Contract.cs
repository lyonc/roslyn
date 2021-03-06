﻿using System;
using System.Diagnostics;

namespace Roslyn.Editor.InteractiveWindow
{
    internal static class Contract
    {
        [DebuggerDisplay("Unreachable")]
        public static Exception Unreachable
        {
            get
            {
                Debug.Fail("This code path should not be reachable");
                return new InvalidOperationException();
            }
        }
    }
}
