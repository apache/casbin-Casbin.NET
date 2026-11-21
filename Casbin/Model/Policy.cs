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
using System.Linq;
using Casbin.Persist;

namespace Casbin.Model;

public static class Policy
{
    public static bool SupportGeneric(int count)
    {
        return count is >= 1 and <= 14;
    }

    public static PolicyValues<T1> CreateValues<T1>(T1 value1) => new(value1);

    public static PolicyValues<T1, T2> CreateValues<T1, T2>(T1 value1, T2 value2) => new(value1, value2);

    public static PolicyValues<T1, T2, T3> CreateValues<T1, T2, T3>(T1 value1, T2 value2, T3 value3) =>
        new(value1, value2, value3);

    public static PolicyValues<T1, T2, T3, T4>
        CreateValues<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4) => new(value1, value2, value3, value4);

    public static PolicyValues<T1, T2, T3, T4, T5> CreateValues<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5) => new(value1, value2, value3, value4, value5);

    public static PolicyValues<T1, T2, T3, T4, T5, T6> CreateValues<T1, T2, T3, T4, T5, T6>(T1 value1, T2 value2,
        T3 value3, T4 value4, T5 value5, T6 value6) => new(value1, value2, value3, value4, value5, value6);

    public static PolicyValues<T1, T2, T3, T4, T5, T6, T7> CreateValues<T1, T2, T3, T4, T5, T6, T7>(T1 value1,
        T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) =>
        new(value1, value2, value3, value4, value5, value6, value7);

    public static PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8> CreateValues<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value1,
        T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8) =>
        new(value1, value2, value3, value4, value5, value6, value7, value8);

    public static PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8, T9> CreateValues<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9) =>
        new(value1, value2, value3, value4, value5, value6, value7, value8, value9);

    public static PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
        CreateValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5,
            T6 value6, T7 value7, T8 value8, T9 value9, T10 value10) =>
        new(value1, value2, value3, value4, value5, value6, value7, value8, value9, value10);

    public static PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
        CreateValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 value1, T2 value2, T3 value3, T4 value4,
            T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11) =>
        new(value1, value2, value3, value4, value5, value6, value7, value8, value9, value10, value11);

    public static PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
        CreateValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 value1, T2 value2, T3 value3, T4 value4,
            T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11, T12 value12) =>
        new(value1, value2, value3, value4, value5, value6, value7, value8, value9, value10, value11, value12);

    public static PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
        CreateValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T1 value1, T2 value2, T3 value3,
            T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11, T12 value12,
            T13 value13) =>
        new(value1, value2, value3, value4, value5, value6, value7, value8, value9, value10, value11, value12, value13);

    public static PolicyValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
        CreateValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T1 value1, T2 value2, T3 value3,
            T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11, T12 value12,
            T13 value13, T14 value14) =>
        new(value1, value2, value3, value4, value5, value6, value7, value8, value9, value10, value11, value12, value13, value14);

    public static IPolicyValues ValuesFrom(IReadOnlyList<string> values) =>
        values.Count switch
        {
            1 => CreateValues(values[0] ?? string.Empty),
            2 => CreateValues(values[0] ?? string.Empty, values[1] ?? string.Empty),
            3 => CreateValues(values[0] ?? string.Empty, values[1] ?? string.Empty, values[2] ?? string.Empty),
            4 => CreateValues(values[0] ?? string.Empty, values[1] ?? string.Empty, values[2] ?? string.Empty,
                values[3] ?? string.Empty),
            5 => CreateValues(values[0] ?? string.Empty, values[1] ?? string.Empty, values[2] ?? string.Empty,
                values[3] ?? string.Empty, values[4] ?? string.Empty),
            6 => CreateValues(values[0] ?? string.Empty, values[1] ?? string.Empty, values[2] ?? string.Empty,
                values[3] ?? string.Empty, values[4] ?? string.Empty, values[5] ?? string.Empty),
            7 => CreateValues(values[0] ?? string.Empty, values[1] ?? string.Empty, values[2] ?? string.Empty,
                values[3] ?? string.Empty, values[4] ?? string.Empty, values[5] ?? string.Empty,
                values[6] ?? string.Empty),
            8 => CreateValues(values[0] ?? string.Empty, values[1] ?? string.Empty, values[2] ?? string.Empty,
                values[3] ?? string.Empty, values[4] ?? string.Empty, values[5] ?? string.Empty,
                values[6] ?? string.Empty, values[7] ?? string.Empty),
            9 => CreateValues(values[0] ?? string.Empty, values[1] ?? string.Empty, values[2] ?? string.Empty,
                values[3] ?? string.Empty, values[4] ?? string.Empty, values[5] ?? string.Empty,
                values[6] ?? string.Empty, values[7] ?? string.Empty, values[8] ?? string.Empty),
            10 => CreateValues(values[0] ?? string.Empty, values[1] ?? string.Empty, values[2] ?? string.Empty,
                values[3] ?? string.Empty, values[4] ?? string.Empty, values[5] ?? string.Empty,
                values[6] ?? string.Empty, values[7] ?? string.Empty, values[8] ?? string.Empty,
                values[9] ?? string.Empty),
            11 => CreateValues(values[0] ?? string.Empty, values[1] ?? string.Empty, values[2] ?? string.Empty,
                values[3] ?? string.Empty, values[4] ?? string.Empty, values[5] ?? string.Empty,
                values[6] ?? string.Empty, values[7] ?? string.Empty, values[8] ?? string.Empty,
                values[9] ?? string.Empty, values[10] ?? string.Empty),
            12 => CreateValues(values[0] ?? string.Empty, values[1] ?? string.Empty, values[2] ?? string.Empty,
                values[3] ?? string.Empty, values[4] ?? string.Empty, values[5] ?? string.Empty,
                values[6] ?? string.Empty, values[7] ?? string.Empty, values[8] ?? string.Empty,
                values[9] ?? string.Empty, values[10] ?? string.Empty, values[11] ?? string.Empty),
            13 => CreateValues(values[0] ?? string.Empty, values[1] ?? string.Empty, values[2] ?? string.Empty,
                values[3] ?? string.Empty, values[4] ?? string.Empty, values[5] ?? string.Empty,
                values[6] ?? string.Empty, values[7] ?? string.Empty, values[8] ?? string.Empty,
                values[9] ?? string.Empty, values[10] ?? string.Empty, values[11] ?? string.Empty,
                values[12] ?? string.Empty),
            14 => CreateValues(values[0] ?? string.Empty, values[1] ?? string.Empty, values[2] ?? string.Empty,
                values[3] ?? string.Empty, values[4] ?? string.Empty, values[5] ?? string.Empty,
                values[6] ?? string.Empty, values[7] ?? string.Empty, values[8] ?? string.Empty,
                values[9] ?? string.Empty, values[10] ?? string.Empty, values[11] ?? string.Empty,
                values[12] ?? string.Empty, values[13] ?? string.Empty),
            _ => new StringListPolicyValues(values)
        };

    public static IPolicyValues ValuesFrom(IReadOnlyList<string> values, int requiredCount) =>
        requiredCount switch
        {
            1 => CreateValues(values.GetValueOrDefault(0) ?? string.Empty),
            2 => CreateValues(values.GetValueOrDefault(0) ?? string.Empty, values.GetValueOrDefault(1) ?? string.Empty),
            3 => CreateValues(values.GetValueOrDefault(0) ?? string.Empty, values.GetValueOrDefault(1) ?? string.Empty,
                values.GetValueOrDefault(2) ?? string.Empty),
            4 => CreateValues(values.GetValueOrDefault(0) ?? string.Empty, values.GetValueOrDefault(1) ?? string.Empty,
                values.GetValueOrDefault(2) ?? string.Empty, values.GetValueOrDefault(3) ?? string.Empty),
            5 => CreateValues(values.GetValueOrDefault(0) ?? string.Empty, values.GetValueOrDefault(1) ?? string.Empty,
                values.GetValueOrDefault(2) ?? string.Empty, values.GetValueOrDefault(3) ?? string.Empty,
                values.GetValueOrDefault(4) ?? string.Empty),
            6 => CreateValues(values.GetValueOrDefault(0) ?? string.Empty, values.GetValueOrDefault(1) ?? string.Empty,
                values.GetValueOrDefault(2) ?? string.Empty, values.GetValueOrDefault(3) ?? string.Empty,
                values.GetValueOrDefault(4) ?? string.Empty, values.GetValueOrDefault(5) ?? string.Empty),
            7 => CreateValues(values.GetValueOrDefault(0) ?? string.Empty, values.GetValueOrDefault(1) ?? string.Empty,
                values.GetValueOrDefault(2) ?? string.Empty, values.GetValueOrDefault(3) ?? string.Empty,
                values.GetValueOrDefault(4) ?? string.Empty, values.GetValueOrDefault(5) ?? string.Empty,
                values.GetValueOrDefault(6) ?? string.Empty),
            8 => CreateValues(values.GetValueOrDefault(0) ?? string.Empty, values.GetValueOrDefault(1) ?? string.Empty,
                values.GetValueOrDefault(2) ?? string.Empty, values.GetValueOrDefault(3) ?? string.Empty,
                values.GetValueOrDefault(4) ?? string.Empty, values.GetValueOrDefault(5) ?? string.Empty,
                values.GetValueOrDefault(6) ?? string.Empty, values.GetValueOrDefault(7) ?? string.Empty),
            9 => CreateValues(values.GetValueOrDefault(0) ?? string.Empty, values.GetValueOrDefault(1) ?? string.Empty,
                values.GetValueOrDefault(2) ?? string.Empty, values.GetValueOrDefault(3) ?? string.Empty,
                values.GetValueOrDefault(4) ?? string.Empty, values.GetValueOrDefault(5) ?? string.Empty,
                values.GetValueOrDefault(6) ?? string.Empty, values.GetValueOrDefault(7) ?? string.Empty,
                values.GetValueOrDefault(8) ?? string.Empty),
            10 => CreateValues(values.GetValueOrDefault(0) ?? string.Empty, values.GetValueOrDefault(1) ?? string.Empty,
                values.GetValueOrDefault(2) ?? string.Empty, values.GetValueOrDefault(3) ?? string.Empty,
                values.GetValueOrDefault(4) ?? string.Empty, values.GetValueOrDefault(5) ?? string.Empty,
                values.GetValueOrDefault(6) ?? string.Empty, values.GetValueOrDefault(7) ?? string.Empty,
                values.GetValueOrDefault(8) ?? string.Empty, values.GetValueOrDefault(9) ?? string.Empty),
            11 => CreateValues(values.GetValueOrDefault(0) ?? string.Empty, values.GetValueOrDefault(1) ?? string.Empty,
                values.GetValueOrDefault(2) ?? string.Empty, values.GetValueOrDefault(3) ?? string.Empty,
                values.GetValueOrDefault(4) ?? string.Empty, values.GetValueOrDefault(5) ?? string.Empty,
                values.GetValueOrDefault(6) ?? string.Empty, values.GetValueOrDefault(7) ?? string.Empty,
                values.GetValueOrDefault(8) ?? string.Empty, values.GetValueOrDefault(9) ?? string.Empty,
                values.GetValueOrDefault(10) ?? string.Empty),
            12 => CreateValues(values.GetValueOrDefault(0) ?? string.Empty, values.GetValueOrDefault(1) ?? string.Empty,
                values.GetValueOrDefault(2) ?? string.Empty, values.GetValueOrDefault(3) ?? string.Empty,
                values.GetValueOrDefault(4) ?? string.Empty, values.GetValueOrDefault(5) ?? string.Empty,
                values.GetValueOrDefault(6) ?? string.Empty, values.GetValueOrDefault(7) ?? string.Empty,
                values.GetValueOrDefault(8) ?? string.Empty, values.GetValueOrDefault(9) ?? string.Empty,
                values.GetValueOrDefault(10) ?? string.Empty, values.GetValueOrDefault(11) ?? string.Empty),
            13 => CreateValues(values.GetValueOrDefault(0) ?? string.Empty, values.GetValueOrDefault(1) ?? string.Empty,
                values.GetValueOrDefault(2) ?? string.Empty, values.GetValueOrDefault(3) ?? string.Empty,
                values.GetValueOrDefault(4) ?? string.Empty, values.GetValueOrDefault(5) ?? string.Empty,
                values.GetValueOrDefault(6) ?? string.Empty, values.GetValueOrDefault(7) ?? string.Empty,
                values.GetValueOrDefault(8) ?? string.Empty, values.GetValueOrDefault(9) ?? string.Empty,
                values.GetValueOrDefault(10) ?? string.Empty, values.GetValueOrDefault(11) ?? string.Empty,
                values.GetValueOrDefault(12) ?? string.Empty),
            14 => CreateValues(values.GetValueOrDefault(0) ?? string.Empty, values.GetValueOrDefault(1) ?? string.Empty,
                values.GetValueOrDefault(2) ?? string.Empty, values.GetValueOrDefault(3) ?? string.Empty,
                values.GetValueOrDefault(4) ?? string.Empty, values.GetValueOrDefault(5) ?? string.Empty,
                values.GetValueOrDefault(6) ?? string.Empty, values.GetValueOrDefault(7) ?? string.Empty,
                values.GetValueOrDefault(8) ?? string.Empty, values.GetValueOrDefault(9) ?? string.Empty,
                values.GetValueOrDefault(10) ?? string.Empty, values.GetValueOrDefault(11) ?? string.Empty,
                values.GetValueOrDefault(12) ?? string.Empty, values.GetValueOrDefault(13) ?? string.Empty),
            _ => new StringListPolicyValues(values, requiredCount)
        };

    public static IPolicyValues ValuesFrom(IEnumerable<string> values) =>
        ValuesFrom(values as IReadOnlyList<string> ?? values.ToArray());

    public static IPolicyValues ValuesFrom(IEnumerable<string> values, int requiredCount) =>
        ValuesFrom(values as IReadOnlyList<string> ?? values.ToArray(), requiredCount);

    public static IPolicyValues ValuesFrom(IPersistPolicy values)
    {
        int count;
        if (string.IsNullOrWhiteSpace(values.Value14) is false)
        {
            count = 14;
        }
        else if (string.IsNullOrWhiteSpace(values.Value13) is false)
        {
            count = 13;
        }
        else if (string.IsNullOrWhiteSpace(values.Value12) is false)
        {
            count = 12;
        }
        else if (string.IsNullOrWhiteSpace(values.Value11) is false)
        {
            count = 11;
        }
        else if (string.IsNullOrWhiteSpace(values.Value10) is false)
        {
            count = 10;
        }
        else if (string.IsNullOrWhiteSpace(values.Value9) is false)
        {
            count = 9;
        }
        else if (string.IsNullOrWhiteSpace(values.Value8) is false)
        {
            count = 8;
        }
        else if (string.IsNullOrWhiteSpace(values.Value7) is false)
        {
            count = 7;
        }
        else if (string.IsNullOrWhiteSpace(values.Value6) is false)
        {
            count = 6;
        }
        else if (string.IsNullOrWhiteSpace(values.Value5) is false)
        {
            count = 5;
        }
        else if (string.IsNullOrWhiteSpace(values.Value4) is false)
        {
            count = 4;
        }
        else if (string.IsNullOrWhiteSpace(values.Value3) is false)
        {
            count = 3;
        }
        else if (string.IsNullOrWhiteSpace(values.Value2) is false)
        {
            count = 2;
        }
        else if (string.IsNullOrWhiteSpace(values.Value1) is false)
        {
            count = 1;
        }
        else
        {
            count = 0;
        }

        return count switch
        {
            1 => CreateValues(values.Value1),
            2 => CreateValues(values.Value1, values.Value2),
            3 => CreateValues(values.Value1, values.Value2, values.Value3),
            4 => CreateValues(values.Value1, values.Value2, values.Value3, values.Value4),
            5 => CreateValues(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5),
            6 => CreateValues(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6),
            7 => CreateValues(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6, values.Value7),
            8 => CreateValues(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6, values.Value7, values.Value8),
            9 => CreateValues(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6, values.Value7, values.Value8, values.Value9),
            10 => CreateValues(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6, values.Value7, values.Value8, values.Value9, values.Value10),
            11 => CreateValues(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6, values.Value7, values.Value8, values.Value9, values.Value10, values.Value11),
            12 => CreateValues(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6, values.Value7, values.Value8, values.Value9, values.Value10, values.Value11,
                values.Value12),
            13 => CreateValues(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6, values.Value7, values.Value8, values.Value9, values.Value10, values.Value11,
                values.Value12, values.Value13),
            14 => CreateValues(values.Value1, values.Value2, values.Value3, values.Value4, values.Value5,
                values.Value6, values.Value7, values.Value8, values.Value9, values.Value10, values.Value11,
                values.Value12, values.Value13, values.Value14),
            _ => throw new ArgumentOutOfRangeException(nameof(count), count, "Count must be between 1 and 14.")
        };
    }

    public static IPolicyValues ValuesFrom(IPersistPolicy values, int requiredCount) =>
        requiredCount switch
        {
            1 => CreateValues(values.Value1 ?? string.Empty),
            2 => CreateValues(values.Value1 ?? string.Empty, values.Value2 ?? string.Empty),
            3 => CreateValues(values.Value1 ?? string.Empty, values.Value2 ?? string.Empty,
                values.Value3 ?? string.Empty),
            4 => CreateValues(values.Value1 ?? string.Empty, values.Value2 ?? string.Empty,
                values.Value3 ?? string.Empty, values.Value4 ?? string.Empty),
            5 => CreateValues(values.Value1 ?? string.Empty, values.Value2 ?? string.Empty,
                values.Value3 ?? string.Empty, values.Value4 ?? string.Empty, values.Value5 ?? string.Empty),
            6 => CreateValues(values.Value1 ?? string.Empty, values.Value2 ?? string.Empty,
                values.Value3 ?? string.Empty, values.Value4 ?? string.Empty, values.Value5 ?? string.Empty,
                values.Value6 ?? string.Empty),
            7 => CreateValues(values.Value1 ?? string.Empty, values.Value2 ?? string.Empty,
                values.Value3 ?? string.Empty, values.Value4 ?? string.Empty, values.Value5 ?? string.Empty,
                values.Value6 ?? string.Empty, values.Value7 ?? string.Empty),
            8 => CreateValues(values.Value1 ?? string.Empty, values.Value2 ?? string.Empty,
                values.Value3 ?? string.Empty, values.Value4 ?? string.Empty, values.Value5 ?? string.Empty,
                values.Value6 ?? string.Empty, values.Value7 ?? string.Empty, values.Value8 ?? string.Empty),
            9 => CreateValues(values.Value1 ?? string.Empty, values.Value2 ?? string.Empty,
                values.Value3 ?? string.Empty, values.Value4 ?? string.Empty, values.Value5 ?? string.Empty,
                values.Value6 ?? string.Empty, values.Value7 ?? string.Empty, values.Value8 ?? string.Empty,
                values.Value9 ?? string.Empty),
            10 => CreateValues(values.Value1 ?? string.Empty, values.Value2 ?? string.Empty,
                values.Value3 ?? string.Empty, values.Value4 ?? string.Empty, values.Value5 ?? string.Empty,
                values.Value6 ?? string.Empty, values.Value7 ?? string.Empty, values.Value8 ?? string.Empty,
                values.Value9 ?? string.Empty, values.Value10 ?? string.Empty),
            11 => CreateValues(values.Value1 ?? string.Empty, values.Value2 ?? string.Empty,
                values.Value3 ?? string.Empty, values.Value4 ?? string.Empty, values.Value5 ?? string.Empty,
                values.Value6 ?? string.Empty, values.Value7 ?? string.Empty, values.Value8 ?? string.Empty,
                values.Value9 ?? string.Empty, values.Value10 ?? string.Empty,
                values.Value11 ?? string.Empty),
            12 => CreateValues(values.Value1 ?? string.Empty, values.Value2 ?? string.Empty,
                values.Value3 ?? string.Empty, values.Value4 ?? string.Empty, values.Value5 ?? string.Empty,
                values.Value6 ?? string.Empty, values.Value7 ?? string.Empty, values.Value8 ?? string.Empty,
                values.Value9 ?? string.Empty, values.Value10 ?? string.Empty,
                values.Value11 ?? string.Empty, values.Value12 ?? string.Empty),
            13 => CreateValues(values.Value1 ?? string.Empty, values.Value2 ?? string.Empty,
                values.Value3 ?? string.Empty, values.Value4 ?? string.Empty, values.Value5 ?? string.Empty,
                values.Value6 ?? string.Empty, values.Value7 ?? string.Empty, values.Value8 ?? string.Empty,
                values.Value9 ?? string.Empty, values.Value10 ?? string.Empty,
                values.Value11 ?? string.Empty, values.Value12 ?? string.Empty,
                values.Value13 ?? string.Empty),
            14 => CreateValues(values.Value1 ?? string.Empty, values.Value2 ?? string.Empty,
                values.Value3 ?? string.Empty, values.Value4 ?? string.Empty, values.Value5 ?? string.Empty,
                values.Value6 ?? string.Empty, values.Value7 ?? string.Empty, values.Value8 ?? string.Empty,
                values.Value9 ?? string.Empty, values.Value10 ?? string.Empty,
                values.Value11 ?? string.Empty, values.Value12 ?? string.Empty,
                values.Value13 ?? string.Empty, values.Value14 ?? string.Empty),
            _ => throw new ArgumentOutOfRangeException(nameof(requiredCount), requiredCount,
                "Required count must be between 1 and 14.")
        };

    public static IReadOnlyList<IPolicyValues> ValuesListFrom(IEnumerable<IEnumerable<string>> valuesList)
    {
        IEnumerable<IPolicyValues> policies = valuesList.Select(values => ValuesFrom(values));
        return policies as IReadOnlyList<IPolicyValues> ?? policies.ToArray();
    }

    public static IReadOnlyList<IPolicyValues> ValuesListFrom(IEnumerable<IEnumerable<string>> valuesList,
        int requiredCount)
    {
        IEnumerable<IPolicyValues> policies = valuesList.Select(values => ValuesFrom(values, requiredCount));
        return policies as IReadOnlyList<IPolicyValues> ?? policies.ToArray();
    }
}
