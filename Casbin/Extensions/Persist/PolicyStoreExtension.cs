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

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Casbin.Model;
using CsvHelper;
using CsvHelper.Configuration;

namespace Casbin.Persist;

public static class PolicyStoreExtension
{
    public static bool TryLoadPolicyLine(this IPolicyStore store, string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            return false;
        }

        if (line.StartsWith("#"))
        {
            return false;
        }

        CsvParser parser = new(new StringReader(line),
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                AllowComments = true,
                HasHeaderRecord = false,
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                BadDataFound = null,
                WhiteSpaceChars = new []{' ', '\t'}
            });
        if (parser.Read() is false)
        {
            return false;
        }

        string[] tokens = parser.Record;
        return store.TryLoadPolicyLine(tokens);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static bool TryLoadPolicyLine(this IPolicyStore store, IReadOnlyList<string> lineTokens)
    {
        string type = lineTokens[0];
        string section = type.Substring(0, 1);
        IPolicyValues values = Policy.ValuesFrom(lineTokens.Skip(1).ToList());
        return store.AddPolicy(section, type, values);
    }
}
