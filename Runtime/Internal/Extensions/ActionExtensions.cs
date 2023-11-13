// Module name: com.inseye.unity.sdk
// File name: ActionExtensions.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using UnityEngine;

namespace Inseye.Internal.Extensions
{
    internal static class ActionExtensions
    {
        public static void SafeInvoke(this Action delegates)
        {
            if (delegates is null)
                return;
            foreach (var dele in delegates.GetInvocationList())
                if (dele is Action action)
                    try
                    {
                        action.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
        }

        public static void SafeInvoke<T>(this Action<T> delegates, T argument)
        {
            if (delegates is null)
                return;
            foreach (var dele in delegates.GetInvocationList())
                if (dele is Action<T> action)
                    try
                    {
                        action.Invoke(argument);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
        }
    }
}