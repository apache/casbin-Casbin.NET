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

namespace Casbin.Model;

public ref struct PolicyScanner
{
    private DefaultPolicyStore.Iterator _iterator;
    internal PolicyScanner(DefaultPolicyStore.Iterator iterator) => _iterator = iterator;

    public bool HasNext() => _iterator.HasNext();

    public bool GetNext(out IPolicyValues outValues)
    {
        bool hasNext = _iterator.GetNext(out IPolicyValues values);
        outValues = values;
        return hasNext;
    }

    public void Interrupt() => _iterator.Interrupt();
}

public ref struct PolicyScanner<TRequest> where TRequest : IRequestValues
{
    private PolicyScanner _scanner;
    private readonly TRequest _request;

    internal PolicyScanner(PolicyScanner scanner, in TRequest request)
    {
        _scanner = scanner;
        _request = request;
    }

    public bool HasNext() => _scanner.HasNext();

    public bool GetNext(out IPolicyValues outValues)
    {
        bool hasNext = _scanner.GetNext(out IPolicyValues values);
        outValues = values;
        return hasNext;
    }

    public void Interrupt() => _scanner.Interrupt();
}
