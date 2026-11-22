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

#if !NET452 && !NET461 && !NET462 && !NETSTANDARD
using System;
using System.Dynamic;
using System.Text.Json;

namespace Casbin.Model;

internal class JsonValue : DynamicObject
{
    private readonly JsonElement _element;

    public JsonValue(string json) :
        this(JsonDocument.Parse(json).RootElement)
    {
    }

    private JsonValue(JsonElement element)
    {
        _element = element;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        if (_element.ValueKind != JsonValueKind.Object)
        {
            result = null;
            return false;
        }

        if (_element.TryGetProperty(binder.Name, out var value))
        {
            result = GetValue(value);
            return true;
        }

        result = null;
        return false;
    }

    public object this[int index] => GetValue(_element[index]);

    public override string ToString()
    {
        return _element.ToString();
    }

    private static object GetValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => new JsonValue(element),
            JsonValueKind.Array => new JsonValue(element),
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.GetInt32(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            JsonValueKind.Undefined => null,
            _ => throw new InvalidOperationException(),
        };
    }
}
#endif
