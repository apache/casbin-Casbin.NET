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

public class PolicyAssertion : Assertion
{
    public PolicyAssertion() => Section = PermConstants.Section.PolicySection;

    public IPolicyManager PolicyManager { get; internal set; }

    public PolicyScanner<TRequest> Scan<TRequest>(in TRequest request) where TRequest : IRequestValues =>
        new(PolicyManager.Scan(), request);

    public bool TryGetPriorityIndex(out int index)
    {
        if (Tokens is null)
        {
            index = -1;
            return false;
        }

        return Tokens.TryGetValue(PermConstants.Token.Priority, out index);
    }

    public bool TryGetDomainIndex(out int index)
    {
        if (Tokens is null)
        {
            index = -1;
            return false;
        }

        return Tokens.TryGetValue(PermConstants.Token.Domain, out index);
    }

    public bool TryGetSubjectIndex(out int index)
    {
        if (Tokens is null)
        {
            index = -1;
            return false;
        }

        return Tokens.TryGetValue(PermConstants.Token.Subject, out index);
    }
}
