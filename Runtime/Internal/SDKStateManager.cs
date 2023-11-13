// Module name: com.inseye.unity.sdk
// File name: SDKStateManager.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Inseye.Internal.Interfaces;

namespace Inseye.Internal
{
    internal class SDKStateManager : ISDKStateManager
    {
        private readonly List<WeakReference<IStateUser>> _listeners = new();
        public InseyeSDKState InseyeSDKState { get; private set; }

        public void Dispose()
        {
            while (_listeners.Count != 0)
            {
                var weakRef = _listeners[0];
                if (weakRef.TryGetTarget(out var target))
                {
                    target.Dispose(); // may remove itself from list, don't use iterator
#if DEBUG_INSEYE_SDK
                    UnityEngine.Debug.Log($"Disposing {target.GetType().Namespace} in {nameof(SDKStateManager)}");
#endif
                }

                if (weakRef == _listeners.FirstOrDefault())
                    _listeners.RemoveAt(0);
            }
        }

        void ISDKStateManager.RemoveListener(IStateUser? user)
        {
            var index = 0;
            var sharedInseyeSDKState = InseyeSDKState.Uninitialized;
            while (index < _listeners.Count)
                if (_listeners[index].TryGetTarget(out var target))
                {
                    if (target == user)
                    {
                        _listeners.RemoveAt(index);
                    }
                    else
                    {
                        sharedInseyeSDKState |= target.RequiredInseyeSDKState;
                        index++;
                    }
                }
                else
                {
                    _listeners.RemoveAt(index);
                }

            if (sharedInseyeSDKState != InseyeSDKState)
                TransitionToState(sharedInseyeSDKState);
        }

        void ISDKStateManager.RequireState(IStateUser stateUser)
        {
            if (InseyeSDKState != stateUser.RequiredInseyeSDKState)
            {
                var sharedInseyeSDKState = stateUser.RequiredInseyeSDKState;
                foreach (var weakRef in _listeners)
                    if (weakRef.TryGetTarget(out var target))
                        sharedInseyeSDKState |= target.RequiredInseyeSDKState;

                InseyeSDKState = TransitionToState(sharedInseyeSDKState |= GetCurrentRequest(_listeners));
            }

            _listeners.Add(new WeakReference<IStateUser>(stateUser));
        }

        void ISDKStateManager.RegisterMultipleUsers(IEnumerable<IStateUser> users)
        {
            _listeners.AddRange(users.Select(s => new WeakReference<IStateUser>(s)));
            InseyeSDKState = TransitionToState(GetCurrentRequest(_listeners));
        }

        void ISDKStateManager.TransferAllStateUsersTo(ISDKStateManager manager)
        {
            List<IStateUser> users = new();
            foreach (var weakReference in _listeners)
                if (weakReference.TryGetTarget(out var user))
                    users.Add(user);
            _listeners.Clear();
            manager.RegisterMultipleUsers(users);
        }

        private static InseyeSDKState GetCurrentRequest(List<WeakReference<IStateUser>> listeners)
        {
            var sharedInseyeSDKState = InseyeSDKState.Uninitialized;
            var index = 0;
            while (index < listeners.Count)
                if (listeners[index].TryGetTarget(out var target))
                {
                    if (target is null)
                    {
                        listeners.RemoveAt(index);
                    }
                    else
                    {
                        sharedInseyeSDKState |= target.RequiredInseyeSDKState;
                        index++;
                    }
                }
                else
                {
                    listeners.RemoveAt(index);
                }

            return sharedInseyeSDKState;
        }

        protected virtual InseyeSDKState TransitionToState(InseyeSDKState inseyeSDKState)
        {
            return inseyeSDKState;
        }
    }
}