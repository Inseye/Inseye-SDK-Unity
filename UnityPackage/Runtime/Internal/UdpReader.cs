// Module name: com.inseye.unity.sdk
// File name: UdpReader.cs
// Last edit: 2023-10-13 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Inseye.Internal
{
    public class UdpReader<T> where T : unmanaged
    {
        private static readonly int StructSize = Marshal.SizeOf<T>();
        private readonly UdpClient _udpClient;
        private bool _disposed;

        /// <summary>
        ///     Creates new gaze data reader.
        /// </summary>
        /// <param name="udpPort">Broadcast UDP port to listen to.</param>
        public UdpReader(int udpPort)
        {
            _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, udpPort));
            // buffer for 10 seconds of signal with 1000 Hz sampling rate
            _udpClient.Client.ReceiveBufferSize = StructSize * 1000 * 10;
            // every client is required to set socket option reuse address, otherwise it will fail to bind to port
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpClient.EnableBroadcast = true;
            // important for avoiding blocking thread in GetGazeData
            _udpClient.Client.Blocking = false;
        }

        /// <summary>
        ///     Reads oldest data sample from UDP port.
        /// </summary>
        /// <param name="outData">Gaze data struct where read data will be written.</param>
        /// <returns>True if data was read, otherwise false.</returns>
        public unsafe bool TryReadData(ref T outData)
        {
            fixed (T* dataPointer = &outData)
            {
                var buffer = new Span<byte>((byte*) dataPointer, StructSize);
                var bytesRead = _udpClient.Client.Receive(buffer, SocketFlags.None, out var error);
                if (error != SocketError.Success || bytesRead != StructSize)
                {
                    LogUnexpectedSocketReading(bytesRead, error);
                    return false;
                }
            }

            return true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~UdpReader()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            _disposed = true;
            if (disposing)
                GC.SuppressFinalize(this);
            _udpClient.Dispose();
        }

        private static void LogUnexpectedSocketReading(int bytesRead, SocketError socketError)
        {
            if (bytesRead > 0 && bytesRead != StructSize)
                Debug.LogWarning(
                    $"Unexpected number of bytes read, expected {StructSize} but read {bytesRead}");
            if (socketError != SocketError.WouldBlock)
                Debug.LogWarning($"While reading socket unexpected socket error occured: {socketError:G}");
        }
    }
}