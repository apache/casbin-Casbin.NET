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

using Casbin.Caching;
using Casbin.Evaluation;
using Casbin.Model.Holder;

namespace Casbin.Model
{
    public interface IModel
    {
        public string Path { get; }
        public ISections Sections { get; }

        public PolicyStoreHolder PolicyStoreHolder { get; }
        public AdapterHolder AdapterHolder { get; }
        public EffectorHolder EffectorHolder { get; }
        public WatcherHolder WatcherHolder { get; }

        public IExpressionHandler ExpressionHandler { get; set; }
        public IEnforceViewCache EnforceViewCache { get; set; }
        public IEnforceCache EnforceCache { get; set; }
        public IGFunctionCachePool GFunctionCachePool { get; set; }

        public void LoadModelFromFile(string path);
        public void LoadModelFromText(string text);

        public bool AddDef(string section, string key, string value);
    }
}
