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

#if !NET452
using System.Collections.Generic;
using System.Linq;
using Casbin.Model;
using Microsoft.Extensions.Logging;

namespace Casbin
{
    public static class LoggerExtension
    {
        public static void LogEnforceCachedResult<TRequest>(this ILogger logger, in TRequest requestValues, bool result)
            where TRequest : IRequestValues
        {
            logger.LogInformation("Request: {@1} ---> {0} (cached)", result, requestValues);
        }

        public static void LogEnforceResult<TRequest>(this ILogger logger, in TRequest requestValues, bool result)
            where TRequest : IRequestValues
        {
            logger.LogInformation("Request: {@1} ---> {0}", result, requestValues);
        }

        public static void LogEnforceResult<TRequest>(this ILogger logger, in TRequest requestValues,
            bool result, IEnumerable<IEnumerable<string>> explains)
            where TRequest : IRequestValues
        {
            logger.LogInformation("Request: {@1} ---> {0}\nHit Store: {2}", result, requestValues,
                string.Join("\n", explains.Select(explain =>
                    string.Join(", ", explain))));
        }
    }
}
#endif
