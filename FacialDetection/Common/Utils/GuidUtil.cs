//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using System;
using System.Security.Cryptography;
using System.Text;

namespace FacialDetectionCommon.Utils
{
    public sealed class GuidUtil
    {
        public static Guid GetDeterministicGuid(string input)
        {
            //use MD5 hash to get a 16-byte hash of the string.

            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();

            var inputBytes = Encoding.Default.GetBytes(input);
            var hashBytes = provider.ComputeHash(inputBytes);

            return new Guid(hashBytes);
        }
    }
}
