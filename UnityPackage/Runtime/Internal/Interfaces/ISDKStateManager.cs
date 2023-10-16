// Module name: com.inseye.unity.sdk
// File name: ISDKStateManager.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using System.Collections.Generic;

namespace Inseye.Internal.Interfaces
{
    /// <summary>
    ///     Object that tracks and manages various sdk users.
    /// </summary>
    internal interface ISDKStateManager : IDisposable
    {
        /// <summary>
        ///     Current internal SDK state.
        /// </summary>
        InseyeSDKState InseyeSDKState { get; }

        /// <summary>
        ///     Requests from state manager state.
        ///     Should add accept user request only if required state can be obtained.
        /// </summary>
        /// <param name="user">State user to register.</param>
        /// <exception cref="System.Exception">Throws exception when state was unable to be acquired.</exception>
        void RequireState(IStateUser user);

        /// <summary>
        ///     Removes state user from list of registered state users.
        ///     Should changed state if requirements changed after user was removed.
        /// </summary>
        /// <param name="user">State user to remove.</param>
        void RemoveListener(IStateUser user);

        /// <summary>
        ///     Registers multiple users at once.
        ///     Should require appropriate state after users transfer.
        ///     Must not throw exceptions.
        /// </summary>
        /// <param name="users">Users to transfer</param>
        void RegisterMultipleUsers(IEnumerable<IStateUser> users);

        /// <summary>
        ///     Moves all state listeners from one ISDKStateManger to other.
        ///     Should clear required state after transfer.
        ///     Must not throw exceptions.
        /// </summary>
        /// <param name="target">ISDKStateManager that will receive new listeners</param>
        void TransferAllStateUsersTo(ISDKStateManager target);
    }
}