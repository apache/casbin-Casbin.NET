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

using System.Linq;

namespace Casbin.Rbac
{
    /// <summary>
    /// GroupRoleManager is used for authorization if the user's group is the role who has permission,
    /// but the group information is in the default format(policy start with "g") and the role information
    /// is in named format(policy start with "g2", "g3", ...).
    /// e.g.
    /// p, admin, domain1, data1, read
    /// g, alice, group1
    /// g2, group1, admin, domain1
    /// As for the previous example, alice should have the permission to read data1, but if we use the
    /// DefaultRoleManager, it will return false.
    /// GroupRoleManager is to handle this situation.
    /// </summary>
    public class GroupRoleManager : DefaultRoleManager
    {
        /// <summary>
        /// GroupRoleManager is the constructor for creating an instance of the 
        /// GroupRoleManager implementation.
        /// </summary>
        /// <param name="maxHierarchyLevel">The maximized allowed RBAC hierarchy level.</param>
        public GroupRoleManager(int maxHierarchyLevel) : base(maxHierarchyLevel)
        {
        }

        /// <summary>
        /// Determines whether role: name1 inherits role: name2. 
        /// domain is a prefix to the roles.
        /// </summary>
        /// <param name="name1"></param>
        /// <param name="name2"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public override bool HasLink(string name1, string name2, string domain = null)
        {
            if (base.HasLink(name1, name2, domain))
            {
                return true;
            }
            var groups = base.GetRoles(name1) ?? Enumerable.Empty<string>();
            return groups.Any(g => HasLink(g, name2, domain));
        }
    }
}
