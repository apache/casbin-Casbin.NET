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

public partial class DefaultPolicyStore
{
    internal ref struct Iterator
    {
        private int _index;
        private readonly int _size;
        private readonly Node _node;

        internal Iterator(Node node)
        {
            _node = node;
            _index = 0;
            _size = node.Policy.Count;
        }

        public bool HasNext() => _index < _size;

        public bool GetNext(out IPolicyValues values)
        {
            if (_node is null)
            {
                values = default;
                return false;
            }

            if (_index is 0 && _node.Lock.IsReadLockHeld is false)
            {
                _node.Lock.EnterReadLock();
            }

            if (_index < _size)
            {
                values = _node.Policy[_index++];
                return true;
            }

            if (_node.Lock.IsReadLockHeld)
            {
                _node.Lock.ExitReadLock();
            }

            values = default;
            return false;
        }

        public void Interrupt()
        {
            if (_node.Lock.IsReadLockHeld)
            {
                _node.Lock.ExitReadLock();
            }
        }
    }
}
