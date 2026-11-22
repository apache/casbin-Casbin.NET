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

namespace Casbin.Model;

public struct RequestValues : IRequestValues
{
    public static RequestValues Empty { get; } = new();

    public string this[int index] => index switch
    {
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 0;
    public bool TrySetValue<T>(int index, T value) => false;

    internal static string ToStringValue<T>(T value)
    {
        return value as string ?? value.ToString();
    }
}

public struct RequestValues<T> : IRequestValues
{
    public RequestValues(T value) => Value1 = value;
    public T Value1 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 1;

    public bool TrySetValue<T1>(int index, T1 value)
    {
        switch (index)
        {
            case 0 when value is T v:
                Value1 = v;
                return true;
            default:
                return false;
        }
    }
}

public struct RequestValues<T1, T2> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2)
    {
        Value1 = value1;
        Value2 = value2;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 2;

    public bool TrySetValue<T>(int index, T value)
    {
        switch (index)
        {
            case 0 when value is T1 v:
                Value1 = v;
                return true;
            case 1 when value is T2 v:
                Value2 = v;
                return true;
            default:
                return false;
        }
    }
}

public struct RequestValues<T1, T2, T3> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 3;

    public bool TrySetValue<T>(int index, T value)
    {
        switch (index)
        {
            case 0 when value is T1 v:
                Value1 = v;
                return true;
            case 1 when value is T2 v:
                Value2 = v;
                return true;
            case 2 when value is T3 v:
                Value3 = v;
                return true;
            default:
                return false;
        }
    }
}

public struct RequestValues<T1, T2, T3, T4> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3, T4 value4)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }
    public T4 Value4 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 4;

    public bool TrySetValue<T>(int index, T value)
    {
        switch (index)
        {
            case 0 when value is T1 v:
                Value1 = v;
                return true;
            case 1 when value is T2 v:
                Value2 = v;
                return true;
            case 2 when value is T3 v:
                Value3 = v;
                return true;
            case 3 when value is T4 v:
                Value4 = v;
                return true;
            default:
                return false;
        }
    }
}

public struct RequestValues<T1, T2, T3, T4, T5> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
        Value5 = value5;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }
    public T4 Value4 { get; set; }
    public T5 Value5 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 5;

    public bool TrySetValue<T>(int index, T value)
    {
        switch (index)
        {
            case 0 when value is T1 v:
                Value1 = v;
                return true;
            case 1 when value is T2 v:
                Value2 = v;
                return true;
            case 2 when value is T3 v:
                Value3 = v;
                return true;
            case 3 when value is T4 v:
                Value4 = v;
                return true;
            case 4 when value is T5 v:
                Value5 = v;
                return true;
            default:
                return false;
        }
    }
}

public struct RequestValues<T1, T2, T3, T4, T5, T6> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
        Value5 = value5;
        Value6 = value6;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }
    public T4 Value4 { get; set; }
    public T5 Value5 { get; set; }
    public T6 Value6 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        5 => RequestValues.ToStringValue(Value6),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 6;

    public bool TrySetValue<T>(int index, T value)
    {
        switch (index)
        {
            case 0 when value is T1 v:
                Value1 = v;
                return true;
            case 1 when value is T2 v:
                Value2 = v;
                return true;
            case 2 when value is T3 v:
                Value3 = v;
                return true;
            case 3 when value is T4 v:
                Value4 = v;
                return true;
            case 4 when value is T5 v:
                Value5 = v;
                return true;
            case 5 when value is T6 v:
                Value6 = v;
                return true;
            default:
                return false;
        }
    }
}

public struct RequestValues<T1, T2, T3, T4, T5, T6, T7> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
        Value5 = value5;
        Value6 = value6;
        Value7 = value7;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }
    public T4 Value4 { get; set; }
    public T5 Value5 { get; set; }
    public T6 Value6 { get; set; }
    public T7 Value7 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        5 => RequestValues.ToStringValue(Value6),
        6 => RequestValues.ToStringValue(Value7),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 7;

    public bool TrySetValue<T>(int index, T value)
    {
        switch (index)
        {
            case 0 when value is T1 v:
                Value1 = v;
                return true;
            case 1 when value is T2 v:
                Value2 = v;
                return true;
            case 2 when value is T3 v:
                Value3 = v;
                return true;
            case 3 when value is T4 v:
                Value4 = v;
                return true;
            case 4 when value is T5 v:
                Value5 = v;
                return true;
            case 5 when value is T6 v:
                Value6 = v;
                return true;
            case 6 when value is T7 v:
                Value7 = v;
                return true;
            default:
                return false;
        }
    }
}

public struct RequestValues<T1, T2, T3, T4, T5, T6, T7, T8> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
        Value5 = value5;
        Value6 = value6;
        Value7 = value7;
        Value8 = value8;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }
    public T4 Value4 { get; set; }
    public T5 Value5 { get; set; }
    public T6 Value6 { get; set; }
    public T7 Value7 { get; set; }
    public T8 Value8 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        5 => RequestValues.ToStringValue(Value6),
        6 => RequestValues.ToStringValue(Value7),
        7 => RequestValues.ToStringValue(Value8),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 8;

    public bool TrySetValue<T>(int index, T value)
    {
        switch (index)
        {
            case 0 when value is T1 v:
                Value1 = v;
                return true;
            case 1 when value is T2 v:
                Value2 = v;
                return true;
            case 2 when value is T3 v:
                Value3 = v;
                return true;
            case 3 when value is T4 v:
                Value4 = v;
                return true;
            case 4 when value is T5 v:
                Value5 = v;
                return true;
            case 5 when value is T6 v:
                Value6 = v;
                return true;
            case 6 when value is T7 v:
                Value7 = v;
                return true;
            case 7 when value is T8 v:
                Value8 = v;
                return true;
            default:
                return false;
        }
    }
}

public struct RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
        Value5 = value5;
        Value6 = value6;
        Value7 = value7;
        Value8 = value8;
        Value9 = value9;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }
    public T4 Value4 { get; set; }
    public T5 Value5 { get; set; }
    public T6 Value6 { get; set; }
    public T7 Value7 { get; set; }
    public T8 Value8 { get; set; }
    public T9 Value9 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        5 => RequestValues.ToStringValue(Value6),
        6 => RequestValues.ToStringValue(Value7),
        7 => RequestValues.ToStringValue(Value8),
        8 => RequestValues.ToStringValue(Value9),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 9;

    public bool TrySetValue<T>(int index, T value)
    {
        switch (index)
        {
            case 0 when value is T1 v:
                Value1 = v;
                return true;
            case 1 when value is T2 v:
                Value2 = v;
                return true;
            case 2 when value is T3 v:
                Value3 = v;
                return true;
            case 3 when value is T4 v:
                Value4 = v;
                return true;
            case 4 when value is T5 v:
                Value5 = v;
                return true;
            case 5 when value is T6 v:
                Value6 = v;
                return true;
            case 6 when value is T7 v:
                Value7 = v;
                return true;
            case 7 when value is T8 v:
                Value8 = v;
                return true;
            case 8 when value is T9 v:
                Value9 = v;
                return true;
            default:
                return false;
        }
    }
}

public struct RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
        Value5 = value5;
        Value6 = value6;
        Value7 = value7;
        Value8 = value8;
        Value9 = value9;
        Value10 = value10;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }
    public T4 Value4 { get; set; }
    public T5 Value5 { get; set; }
    public T6 Value6 { get; set; }
    public T7 Value7 { get; set; }
    public T8 Value8 { get; set; }
    public T9 Value9 { get; set; }
    public T10 Value10 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        5 => RequestValues.ToStringValue(Value6),
        6 => RequestValues.ToStringValue(Value7),
        7 => RequestValues.ToStringValue(Value8),
        8 => RequestValues.ToStringValue(Value9),
        9 => RequestValues.ToStringValue(Value10),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 10;

    public bool TrySetValue<T>(int index, T value)
    {
        switch (index)
        {
            case 0 when value is T1 v:
                Value1 = v;
                return true;
            case 1 when value is T2 v:
                Value2 = v;
                return true;
            case 2 when value is T3 v:
                Value3 = v;
                return true;
            case 3 when value is T4 v:
                Value4 = v;
                return true;
            case 4 when value is T5 v:
                Value5 = v;
                return true;
            case 5 when value is T6 v:
                Value6 = v;
                return true;
            case 6 when value is T7 v:
                Value7 = v;
                return true;
            case 7 when value is T8 v:
                Value8 = v;
                return true;
            case 8 when value is T9 v:
                Value9 = v;
                return true;
            case 9 when value is T10 v:
                Value10 = v;
                return true;
            default:
                return false;
        }
    }
}

public struct RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10, T11 value11)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
        Value5 = value5;
        Value6 = value6;
        Value7 = value7;
        Value8 = value8;
        Value9 = value9;
        Value10 = value10;
        Value11 = value11;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }
    public T4 Value4 { get; set; }
    public T5 Value5 { get; set; }
    public T6 Value6 { get; set; }
    public T7 Value7 { get; set; }
    public T8 Value8 { get; set; }
    public T9 Value9 { get; set; }
    public T10 Value10 { get; set; }
    public T11 Value11 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        5 => RequestValues.ToStringValue(Value6),
        6 => RequestValues.ToStringValue(Value7),
        7 => RequestValues.ToStringValue(Value8),
        8 => RequestValues.ToStringValue(Value9),
        9 => RequestValues.ToStringValue(Value10),
        10 => RequestValues.ToStringValue(Value11),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 11;

    public bool TrySetValue<T>(int index, T value)
    {
        switch (index)
        {
            case 0 when value is T1 v:
                Value1 = v;
                return true;
            case 1 when value is T2 v:
                Value2 = v;
                return true;
            case 2 when value is T3 v:
                Value3 = v;
                return true;
            case 3 when value is T4 v:
                Value4 = v;
                return true;
            case 4 when value is T5 v:
                Value5 = v;
                return true;
            case 5 when value is T6 v:
                Value6 = v;
                return true;
            case 6 when value is T7 v:
                Value7 = v;
                return true;
            case 7 when value is T8 v:
                Value8 = v;
                return true;
            case 8 when value is T9 v:
                Value9 = v;
                return true;
            case 9 when value is T10 v:
                Value10 = v;
                return true;
            case 10 when value is T11 v:
                Value11 = v;
                return true;
            default:
                return false;
        }
    }
}

public struct RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10, T11 value11, T12 value12)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
        Value5 = value5;
        Value6 = value6;
        Value7 = value7;
        Value8 = value8;
        Value9 = value9;
        Value10 = value10;
        Value11 = value11;
        Value12 = value12;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }
    public T4 Value4 { get; set; }
    public T5 Value5 { get; set; }
    public T6 Value6 { get; set; }
    public T7 Value7 { get; set; }
    public T8 Value8 { get; set; }
    public T9 Value9 { get; set; }
    public T10 Value10 { get; set; }
    public T11 Value11 { get; set; }
    public T12 Value12 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        5 => RequestValues.ToStringValue(Value6),
        6 => RequestValues.ToStringValue(Value7),
        7 => RequestValues.ToStringValue(Value8),
        8 => RequestValues.ToStringValue(Value9),
        9 => RequestValues.ToStringValue(Value10),
        10 => RequestValues.ToStringValue(Value11),
        11 => RequestValues.ToStringValue(Value12),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 12;

    public bool TrySetValue<T>(int index, T value)
    {
        switch (index)
        {
            case 0 when value is T1 v:
                Value1 = v;
                return true;
            case 1 when value is T2 v:
                Value2 = v;
                return true;
            case 2 when value is T3 v:
                Value3 = v;
                return true;
            case 3 when value is T4 v:
                Value4 = v;
                return true;
            case 4 when value is T5 v:
                Value5 = v;
                return true;
            case 5 when value is T6 v:
                Value6 = v;
                return true;
            case 6 when value is T7 v:
                Value7 = v;
                return true;
            case 7 when value is T8 v:
                Value8 = v;
                return true;
            case 8 when value is T9 v:
                Value9 = v;
                return true;
            case 9 when value is T10 v:
                Value10 = v;
                return true;
            case 10 when value is T11 v:
                Value11 = v;
                return true;
            case 11 when value is T12 v:
                Value12 = v;
                return true;
            default:
                return false;
        }
    }
}

public struct RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10, T11 value11, T12 value12, T13 value13)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
        Value5 = value5;
        Value6 = value6;
        Value7 = value7;
        Value8 = value8;
        Value9 = value9;
        Value10 = value10;
        Value11 = value11;
        Value12 = value12;
        Value13 = value13;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }
    public T4 Value4 { get; set; }
    public T5 Value5 { get; set; }
    public T6 Value6 { get; set; }
    public T7 Value7 { get; set; }
    public T8 Value8 { get; set; }
    public T9 Value9 { get; set; }
    public T10 Value10 { get; set; }
    public T11 Value11 { get; set; }
    public T12 Value12 { get; set; }
    public T13 Value13 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        5 => RequestValues.ToStringValue(Value6),
        6 => RequestValues.ToStringValue(Value7),
        7 => RequestValues.ToStringValue(Value8),
        8 => RequestValues.ToStringValue(Value9),
        9 => RequestValues.ToStringValue(Value10),
        10 => RequestValues.ToStringValue(Value11),
        11 => RequestValues.ToStringValue(Value12),
        12 => RequestValues.ToStringValue(Value13),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 13;

    public bool TrySetValue<T>(int index, T value)
    {
        switch (index)
        {
            case 0 when value is T1 v:
                Value1 = v;
                return true;
            case 1 when value is T2 v:
                Value2 = v;
                return true;
            case 2 when value is T3 v:
                Value3 = v;
                return true;
            case 3 when value is T4 v:
                Value4 = v;
                return true;
            case 4 when value is T5 v:
                Value5 = v;
                return true;
            case 5 when value is T6 v:
                Value6 = v;
                return true;
            case 6 when value is T7 v:
                Value7 = v;
                return true;
            case 7 when value is T8 v:
                Value8 = v;
                return true;
            case 8 when value is T9 v:
                Value9 = v;
                return true;
            case 9 when value is T10 v:
                Value10 = v;
                return true;
            case 10 when value is T11 v:
                Value11 = v;
                return true;
            case 11 when value is T12 v:
                Value12 = v;
                return true;
            case 12 when value is T13 v:
                Value13 = v;
                return true;
            default:
                return false;
        }
    }
}

public struct RequestValues<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : IRequestValues
{
    public RequestValues(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10, T11 value11, T12 value12, T13 value13, T14 value14)
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
        Value5 = value5;
        Value6 = value6;
        Value7 = value7;
        Value8 = value8;
        Value9 = value9;
        Value10 = value10;
        Value11 = value11;
        Value12 = value12;
        Value13 = value13;
        Value14 = value14;
    }

    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
    public T3 Value3 { get; set; }
    public T4 Value4 { get; set; }
    public T5 Value5 { get; set; }
    public T6 Value6 { get; set; }
    public T7 Value7 { get; set; }
    public T8 Value8 { get; set; }
    public T9 Value9 { get; set; }
    public T10 Value10 { get; set; }
    public T11 Value11 { get; set; }
    public T12 Value12 { get; set; }
    public T13 Value13 { get; set; }
    public T14 Value14 { get; set; }

    public string this[int index] => index switch
    {
        0 => RequestValues.ToStringValue(Value1),
        1 => RequestValues.ToStringValue(Value2),
        2 => RequestValues.ToStringValue(Value3),
        3 => RequestValues.ToStringValue(Value4),
        4 => RequestValues.ToStringValue(Value5),
        5 => RequestValues.ToStringValue(Value6),
        6 => RequestValues.ToStringValue(Value7),
        7 => RequestValues.ToStringValue(Value8),
        8 => RequestValues.ToStringValue(Value9),
        9 => RequestValues.ToStringValue(Value10),
        10 => RequestValues.ToStringValue(Value11),
        11 => RequestValues.ToStringValue(Value12),
        12 => RequestValues.ToStringValue(Value13),
        13 => RequestValues.ToStringValue(Value14),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 14;

    public bool TrySetValue<T>(int index, T value)
    {
        switch (index)
        {
            case 0 when value is T1 v:
                Value1 = v;
                return true;
            case 1 when value is T2 v:
                Value2 = v;
                return true;
            case 2 when value is T3 v:
                Value3 = v;
                return true;
            case 3 when value is T4 v:
                Value4 = v;
                return true;
            case 4 when value is T5 v:
                Value5 = v;
                return true;
            case 5 when value is T6 v:
                Value6 = v;
                return true;
            case 6 when value is T7 v:
                Value7 = v;
                return true;
            case 7 when value is T8 v:
                Value8 = v;
                return true;
            case 8 when value is T9 v:
                Value9 = v;
                return true;
            case 9 when value is T10 v:
                Value10 = v;
                return true;
            case 10 when value is T11 v:
                Value11 = v;
                return true;
            case 11 when value is T12 v:
                Value12 = v;
                return true;
            case 12 when value is T13 v:
                Value13 = v;
                return true;
            case 13 when value is T14 v:
                Value14 = v;
                return true;
            default:
                return false;
        }
    }
}

public struct StringRequestValues : IRequestValues
{
    public static readonly StringRequestValues Empty = new();

    public StringRequestValues(string value1 = "", string value2 = "", string value3 = "", string value4 = "",
        string value5 = "", string value6 = "", string value7 = "", string value8 = "",
        string value9 = "", string value10 = "", string value11 = "", string value12 = "",
        string value13 = "", string value14 = "")
    {
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
        Value5 = value5;
        Value6 = value6;
        Value7 = value7;
        Value8 = value8;
        Value9 = value9;
        Value10 = value10;
        Value11 = value11;
        Value12 = value12;
        Value13 = value13;
        Value14 = value14;
    }

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

    public string this[int index] => index switch
    {
        0 => Value1,
        1 => Value2,
        2 => Value3,
        3 => Value4,
        4 => Value5,
        5 => Value6,
        6 => Value7,
        7 => Value8,
        8 => Value9,
        9 => Value10,
        10 => Value11,
        11 => Value12,
        12 => Value13,
        13 => Value14,
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public int Count => 14;

    public bool TrySetValue<T>(int index, T value)
    {
        if (value is string v)
        {
            return TrySetValue(index, v);
        }
        return false;
    }

    public bool TrySetValue(int index, string value)
    {
        switch (index)
        {
            case 0:
                Value1 = value;
                return true;
            case 1:
                Value2 = value;
                return true;
            case 2:
                Value3 = value;
                return true;
            case 3:
                Value4 = value;
                return true;
            case 4:
                Value5 = value;
                return true;
            case 5:
                Value6 = value;
                return true;
            case 6:
                Value7 = value;
                return true;
            case 7:
                Value8 = value;
                return true;
            case 8:
                Value9 = value;
                return true;
            case 9:
                Value10 = value;
                return true;
            case 10:
                Value11 = value;
                return true;
            case 11:
                Value12 = value;
                return true;
            case 12:
                Value13 = value;
                return true;
            case 13:
                Value14 = value;
                return true;
            default:
                return false;
        }
    }
}

public readonly struct ListRequestValues<TValue> : IRequestValues
{
    private readonly IList<TValue> _values;

    public ListRequestValues(IEnumerable<TValue> values) =>
        _values = values as IList<TValue> ?? values.ToArray();

    public TValue Value1 => _values[0];
    public TValue Value2 => _values[1];
    public TValue Value3 => _values[2];
    public TValue Value4 => _values[3];
    public TValue Value5 => _values[4];
    public TValue Value6 => _values[5];
    public TValue Value7 => _values[6];
    public TValue Value8 => _values[7];
    public TValue Value9 => _values[8];
    public TValue Value10 => _values[9];
    public TValue Value11 => _values[10];
    public TValue Value12 => _values[11];
    public TValue Value13 => _values[12];
    public TValue Value14 => _values[13];

    public string this[int index] => RequestValues.ToStringValue(_values[index]);
    public int Count => _values.Count;

    public bool TrySetValue<T>(int index, T value)
    {
        if (index < 0 || index >= Count)
        {
            return false;
        }

        if (value is not TValue v)
        {
            return false;
        }

        _values[index] = v;
        return true;
    }
}
