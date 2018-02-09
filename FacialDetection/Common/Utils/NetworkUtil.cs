//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace FacialDetectionCommon.Utils
{
    public sealed class NetworkUtil
    {
        public static IPAddress GetPrimaryAddress(AddressFamily family = AddressFamily.InterNetwork)
        {
            return Dns.GetHostEntry(Environment.MachineName)
                      .AddressList
                      .Where(addr => addr.AddressFamily == family)
                      .FirstOrDefault();
        }

        public static string GetSystemMacAddress()
        {
            return (from nic in NetworkInterface.GetAllNetworkInterfaces()
                    where nic.OperationalStatus == OperationalStatus.Up
                    select nic.GetPhysicalAddress().ToString()).FirstOrDefault();
        }
    }
}
