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
using Casbin.Model;

namespace Casbin.Persist;

public class PersistPolicy : IPersistPolicy, IReadOnlyPersistPolicy
{
    public string Section { get; set; }
    public string Type { get; set; }
    public string Value1 { get; set; }
    public string Value2 { get; set; }
    public string Value3 { get; set; }
    public string Value4 { get; set; }
    public string Value5 { get; set; }
    public string Value6 { get; set; }
    public string Value7 { get; set; }
    public string Value8 { get; set; }
    public string Value9 { get; set; }
    public string Value10 { get; set; }
    public string Value11 { get; set; }
    public string Value12 { get; set; }
    public string Value13 { get; set; }
    public string Value14 { get; set; }

    public static TPersistPolicy Create<TPersistPolicy>(string type, IPolicyValues values)
        where TPersistPolicy : IPersistPolicy, new()
    {
        string section = type.Substring(0, 1);
        return Create<TPersistPolicy>(section, type, values);
    }

    public static TPersistPolicy Create<TPersistPolicy>(string section, string type, IPolicyValues values)
        where TPersistPolicy : IPersistPolicy, new()
    {
        TPersistPolicy persistPolicy = new TPersistPolicy { Section = section, Type = type };
        switch (values.Count)
        {
            case 1:
                persistPolicy.Value1 = values[0];
                break;
            case 2:
                persistPolicy.Value1 = values[0];
                persistPolicy.Value2 = values[1];
                break;
            case 3:
                persistPolicy.Value1 = values[0];
                persistPolicy.Value2 = values[1];
                persistPolicy.Value3 = values[2];
                break;
            case 4:
                persistPolicy.Value1 = values[0];
                persistPolicy.Value2 = values[1];
                persistPolicy.Value3 = values[2];
                persistPolicy.Value4 = values[3];
                break;
            case 5:
                persistPolicy.Value1 = values[0];
                persistPolicy.Value2 = values[1];
                persistPolicy.Value3 = values[2];
                persistPolicy.Value4 = values[3];
                persistPolicy.Value5 = values[4];
                break;
            case 6:
                persistPolicy.Value1 = values[0];
                persistPolicy.Value2 = values[1];
                persistPolicy.Value3 = values[2];
                persistPolicy.Value4 = values[3];
                persistPolicy.Value5 = values[4];
                persistPolicy.Value6 = values[5];
                break;
            case 7:
                persistPolicy.Value1 = values[0];
                persistPolicy.Value2 = values[1];
                persistPolicy.Value3 = values[2];
                persistPolicy.Value4 = values[3];
                persistPolicy.Value5 = values[4];
                persistPolicy.Value6 = values[5];
                persistPolicy.Value7 = values[6];
                break;
            case 8:
                persistPolicy.Value1 = values[0];
                persistPolicy.Value2 = values[1];
                persistPolicy.Value3 = values[2];
                persistPolicy.Value4 = values[3];
                persistPolicy.Value5 = values[4];
                persistPolicy.Value6 = values[5];
                persistPolicy.Value7 = values[6];
                persistPolicy.Value8 = values[7];
                break;
            case 9:
                persistPolicy.Value1 = values[0];
                persistPolicy.Value2 = values[1];
                persistPolicy.Value3 = values[2];
                persistPolicy.Value4 = values[3];
                persistPolicy.Value5 = values[4];
                persistPolicy.Value6 = values[5];
                persistPolicy.Value7 = values[6];
                persistPolicy.Value8 = values[7];
                persistPolicy.Value9 = values[8];
                break;
            case 10:
                persistPolicy.Value1 = values[0];
                persistPolicy.Value2 = values[1];
                persistPolicy.Value3 = values[2];
                persistPolicy.Value4 = values[3];
                persistPolicy.Value5 = values[4];
                persistPolicy.Value6 = values[5];
                persistPolicy.Value7 = values[6];
                persistPolicy.Value8 = values[7];
                persistPolicy.Value9 = values[8];
                persistPolicy.Value10 = values[9];
                break;
            case 11:
                persistPolicy.Value1 = values[0];
                persistPolicy.Value2 = values[1];
                persistPolicy.Value3 = values[2];
                persistPolicy.Value4 = values[3];
                persistPolicy.Value5 = values[4];
                persistPolicy.Value6 = values[5];
                persistPolicy.Value7 = values[6];
                persistPolicy.Value8 = values[7];
                persistPolicy.Value9 = values[8];
                persistPolicy.Value10 = values[9];
                persistPolicy.Value11 = values[10];
                break;
            case 12:
                persistPolicy.Value1 = values[0];
                persistPolicy.Value2 = values[1];
                persistPolicy.Value3 = values[2];
                persistPolicy.Value4 = values[3];
                persistPolicy.Value5 = values[4];
                persistPolicy.Value6 = values[5];
                persistPolicy.Value7 = values[6];
                persistPolicy.Value8 = values[7];
                persistPolicy.Value9 = values[8];
                persistPolicy.Value10 = values[9];
                persistPolicy.Value11 = values[10];
                persistPolicy.Value12 = values[11];
                break;
            case 13:
                persistPolicy.Value1 = values[0];
                persistPolicy.Value2 = values[1];
                persistPolicy.Value3 = values[2];
                persistPolicy.Value4 = values[3];
                persistPolicy.Value5 = values[4];
                persistPolicy.Value6 = values[5];
                persistPolicy.Value7 = values[6];
                persistPolicy.Value8 = values[7];
                persistPolicy.Value9 = values[8];
                persistPolicy.Value10 = values[9];
                persistPolicy.Value11 = values[10];
                persistPolicy.Value12 = values[11];
                persistPolicy.Value13 = values[12];
                break;
            case 14:
                persistPolicy.Value1 = values[0];
                persistPolicy.Value2 = values[1];
                persistPolicy.Value3 = values[2];
                persistPolicy.Value4 = values[3];
                persistPolicy.Value5 = values[4];
                persistPolicy.Value6 = values[5];
                persistPolicy.Value7 = values[6];
                persistPolicy.Value8 = values[7];
                persistPolicy.Value9 = values[8];
                persistPolicy.Value10 = values[9];
                persistPolicy.Value11 = values[10];
                persistPolicy.Value12 = values[11];
                persistPolicy.Value13 = values[12];
                persistPolicy.Value14 = values[13];
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(values), values.Count,
                    "The number of values must be between 1 and 14.");
        }

        return persistPolicy;
    }
}
