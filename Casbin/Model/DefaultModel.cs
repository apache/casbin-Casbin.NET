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
using System.IO;
using Casbin.Caching;
using Casbin.Config;
using Casbin.Effect;
using Casbin.Evaluation;
using Casbin.Functions;
using Casbin.Model.Holder;
using Casbin.Rbac;

namespace Casbin.Model
{
    public class DefaultModel : IModel
    {
        public string Path { get; private set; }
        public ISections Sections { get; } = new DefaultSections();
        public PolicyStoreHolder PolicyStoreHolder { get; } = new() { PolicyStore = new DefaultPolicyStore() };
        public EffectorHolder EffectorHolder { get; } = new() { Effector = new DefaultEffector() };
        public AdapterHolder AdapterHolder { get; } = new();
        public WatcherHolder WatcherHolder { get; } = new();
        public IEnforceViewCache EnforceViewCache { get; set; } = new EnforceViewCache();
        public IEnforceCache EnforceCache { get; set; } = new EnforceCache(new EnforceCacheOptions());
        public IExpressionHandler ExpressionHandler { get; set; } = new ExpressionHandler();
        public IGFunctionCachePool GFunctionCachePool { get; set; } = new GFunctionCachePool();

        public void LoadModelFromFile(string path)
        {
            Path = path;
            LoadModel(DefaultConfig.CreateFromFile(path));
        }

        public void LoadModelFromText(string text)
        {
            LoadModel(DefaultConfig.CreateFromText(text));
        }

        public bool AddDef(string section, string key, string value)
        {
            bool added = Sections.AddSection(section, key, value);
            if (added is false)
            {
                return false;
            }

            if (section.Equals(PermConstants.Section.PolicySection))
            {
                PolicyAssertion assertion = Sections.GetPolicyAssertion(key);
                LoadPolicyAssertion(key, assertion);
            }

            if (section.Equals(PermConstants.Section.RoleSection))
            {
                RoleAssertion assertion = Sections.GetRoleAssertion(key);
                LoadRoleAssertion(key, assertion);
            }

            return true;
        }

        /// <summary>
        ///     Creates a default model.
        /// </summary>
        /// <returns></returns>
        public static IModel Create() => new DefaultModel();

        /// <summary>
        ///     Creates a default model from file.
        /// </summary>
        /// <param name="path">The path of the model file.</param>
        /// <returns></returns>
        public static IModel CreateFromFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The model file is not found.", path);
            }

            IModel model = Create();
            model.LoadModelFromFile(path);
            return model;
        }

        /// <summary>
        /// Creates a default model from text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IModel CreateFromText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            IModel model = Create();
            model.LoadModelFromText(text);
            return model;
        }

        /// <summary>
        ///     Creates a default model from file. (go like API)
        /// </summary>
        /// <param name="path">The path of the model file.</param>
        /// <returns></returns>
        public static IModel NewModelFromFile(string path) => CreateFromFile(path);

        /// <summary>
        ///     Creates a default model from text. (go like API)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IModel NewModelFromText(string text) => CreateFromText(text);

        private void LoadModel(IConfig config)
        {
            Sections.LoadSection(config, PermConstants.Section.RequestSection);
            Sections.LoadSection(config, PermConstants.Section.PolicySection);
            Sections.LoadSection(config, PermConstants.Section.RoleSection);
            Sections.LoadSection(config, PermConstants.Section.PolicyEffectSection);
            Sections.LoadSection(config, PermConstants.Section.MatcherSection);
            foreach (KeyValuePair<string, PolicyAssertion> pair in Sections.GetPolicyAssertions(PermConstants.Section
                         .PolicySection))
            {
                LoadPolicyAssertion(pair.Key, pair.Value);
            }

            if (Sections.ContainsSection(PermConstants.Section.RoleSection))
            {
                foreach (KeyValuePair<string, RoleAssertion> pair in Sections.GetRoleAssertions(PermConstants.Section
                             .RoleSection))
                {
                    LoadRoleAssertion(pair.Key, pair.Value);
                }
            }
        }

        private void LoadPolicyAssertion(string type, PolicyAssertion assertion)
        {
            PolicyStoreHolder.PolicyStore.AddNode(PermConstants.Section.PolicySection, type, assertion);
            assertion.PolicyManager = new DefaultPolicyManager(PermConstants.Section.PolicySection, type,
                PolicyStoreHolder, AdapterHolder);
        }

        private void LoadRoleAssertion(string type, RoleAssertion assertion)
        {
            PolicyStoreHolder.PolicyStore.AddNode(PermConstants.Section.RoleSection, type, assertion);
            assertion.PolicyManager = new DefaultPolicyManager(PermConstants.Section.RoleSection, type,
                PolicyStoreHolder, AdapterHolder);
            assertion.RoleManager = new DefaultRoleManager(10);
            ExpressionHandler.SetFunction(type, BuiltInFunctions.GenerateGFunction(
                assertion.RoleManager, GFunctionCachePool.GetCache(type)));
        }

        public bool Validate()
        {
            // Required sections check.
            if (Sections.ContainsSection(PermConstants.Section.RequestSection) is false ||
                Sections.ContainsSection(PermConstants.Section.PolicySection) is false ||
                Sections.ContainsSection(PermConstants.Section.PolicyEffectSection) is false ||
                Sections.ContainsSection(PermConstants.Section.MatcherSection) is false)
            {
                return false;
            }
            return true;
        }
    }
}
