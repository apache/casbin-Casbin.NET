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
using System.Collections.Generic;

namespace Casbin.Functions
{
    internal class FunctionMap
    {
        internal IDictionary<string, Delegate> FunctionDict { get; private set; }

        private void AddFunction(string name, Delegate function)
        {
            FunctionDict.Add(name, function);
        }

        internal static FunctionMap LoadFunctionMap()
        {
            var map = new FunctionMap { FunctionDict = new Dictionary<string, Delegate>() };

            map.AddFunction("keyGet", BuiltInFunctions.KeyGet);
            map.AddFunction("keyGet2", BuiltInFunctions.KeyGet2);
            map.AddFunction("keyGet3", BuiltInFunctions.KeyGet3);
            map.AddFunction("keyMatch", BuiltInFunctions.KeyMatch);
            map.AddFunction("keyMatch2", BuiltInFunctions.KeyMatch2);
            map.AddFunction("keyMatch3", BuiltInFunctions.KeyMatch3);
            map.AddFunction("keyMatch4", BuiltInFunctions.KeyMatch4);
            map.AddFunction("keyMatch5", BuiltInFunctions.KeyMatch5);
            map.AddFunction("regexMatch", BuiltInFunctions.RegexMatch);
            map.AddFunction("ipMatch", BuiltInFunctions.IPMatch);
            map.AddFunction("globMatch", BuiltInFunctions.GlobMatch);
            return map;
        }
    }
}
