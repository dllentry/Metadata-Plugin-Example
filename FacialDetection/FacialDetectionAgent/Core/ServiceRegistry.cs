//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using Microsoft.Practices.Unity;

namespace FacialDetectionAgent.Core
{
    public static class ServiceRegistry
    {
        private static readonly object Lock = new object();

        private static IUnityContainer _container;

        public static void RegisterContainer(IUnityContainer container)
        {
            lock (Lock)
            {
                _container = container;
            }
        }

        public static T Get<T>()
        {
            lock (Lock)
            {
                return _container.Resolve<T>();
            }
        }
    }
}
