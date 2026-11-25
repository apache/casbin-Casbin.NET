// Licensed to the Apache Software Foundation (ASF) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The ASF licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
//
//   https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using System;
using System.Net;
using System.Net.Sockets;

namespace Casbin
{
    // ReSharper disable once InconsistentNaming
    public static class IPAddressExtension
    {
        public static bool Match(this IPAddress matchIpAddress, IPAddress ipAddress, byte matchCidr)
        {
            if (matchIpAddress.AddressFamily != ipAddress.AddressFamily)
            {
                return false;
            }

            byte bytesLength = matchIpAddress.AddressFamily switch
            {
                AddressFamily.InterNetwork => 4, //IPv4 Length
                AddressFamily.InterNetworkV6 => 16, //IPv6 Length
                _ => throw new NotSupportedException("Unable support other address family."),
            };

            var ipBytes1 = matchIpAddress.GetAddressBytes().AsSpan();
            var ipBytes2 = ipAddress.GetAddressBytes().AsSpan();
            int remainingMaskBits = matchCidr;

            for (int i = 0; i < bytesLength; i++)
            {
                byte cidrByte = remainingMaskBits switch
                {
                    >= 8 => 0xFF,
                    <= 0 => 0x00,
                    _ => (byte)(0xFF << (8 - remainingMaskBits))
                };

                if ((ipBytes1[i] & cidrByte) != (ipBytes2[i] & cidrByte))
                {
                    return false;
                }

                remainingMaskBits -= 8;
            }

            return true;
        }
    }
}
