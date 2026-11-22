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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Casbin.Rbac
{
    public class DefaultRoleManager : IRoleManager
    {
        private readonly string _defaultDomain = string.Empty;

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, Role>> _allDomains = new();
        private readonly int _maxHierarchyLevel;

        private IEnumerable<string> _cachedAllDomains;
        private readonly ConcurrentDictionary<string, Role> _defaultRoles = new();

        public Func<string, string, bool> MatchingFunc { get; set; }
        public Func<string, string, bool> DomainMatchingFunc { get; set; }

        public bool HasPattern => MatchingFunc is not null;
        public bool HasDomainPattern => DomainMatchingFunc is not null;

        public DefaultRoleManager(int maxHierarchyLevel)
        {
            _allDomains[_defaultDomain] = _defaultRoles;
            _maxHierarchyLevel = maxHierarchyLevel;
        }

        public virtual IEnumerable<string> GetDomains(string name)
        {
            _cachedAllDomains ??= _allDomains.Keys;
            var domains = new HashSet<string>();
            foreach (string domain in _cachedAllDomains)
            {
                if (AnyRolesInDomain(name, domain))
                {
                    domains.Add(domain);
                }
            }
            return domains;
        }

        public virtual IEnumerable<string> GetRoles(string name, string domain = null)
        {
            domain ??= _defaultDomain;
            if (HasDomainPattern is false)
            {
                return GetRolesInDomain(name, domain);
            }

            var roleNames = new List<string>();
            foreach (string matchDomain in GetPatternDomains(domain))
            {
                roleNames.AddRange(GetRolesInDomain(name, matchDomain));
            }

            return roleNames.Distinct();
        }

        private bool AnyRolesInDomain(string name, string domain)
        {
            ConcurrentDictionary<string, Role> roles;

            if (domain == _defaultDomain)
            {
                roles = _defaultRoles;
            }
            else
            {
                if (_allDomains.TryGetValue(domain, out roles) is false)
                {
                    return false;
                }
            }

            if (HasPattern is false)
            {
                return roles.ContainsKey(name);
            }

            foreach (var role in roles)
            {
                if (MatchingFunc(name, role.Key))
                {
                    return true;
                }
            }
            return false;
        }

        private IEnumerable<string> GetRolesInDomain(string name, string domain)
        {
            ConcurrentDictionary<string, Role> roles;

            if (domain == _defaultDomain)
            {
                roles = _defaultRoles;
            }
            else
            {
                if (_allDomains.TryGetValue(domain, out roles) is false)
                {
                    return Enumerable.Empty<string>();
                }
            }

            if (HasPattern is false)
            {
                return roles.TryGetValue(name, out var role)
                    ? role.GetRoles() : Enumerable.Empty<string>();
            }

            var rolesNames = new List<string>();
            foreach (var role in roles)
            {
                if (MatchingFunc(name, role.Key))
                {
                    rolesNames.AddRange(role.Value.GetRoles());
                }
            }
            return rolesNames;
        }

        public virtual IEnumerable<string> GetUsers(string name, string domain = null)
        {
            domain ??= _defaultDomain;

            if (HasDomainPattern is false)
            {
                return GetUsersInDomain(name, domain);
            }

            var userNames = new List<string>();
            foreach (string matchDomain in GetPatternDomains(domain))
            {
                userNames.AddRange(GetUsersInDomain(name, matchDomain));
            }

            return userNames.Distinct();
        }

        private IEnumerable<string> GetUsersInDomain(string name, string domain)
        {
            ConcurrentDictionary<string, Role> roles;

            if (domain == _defaultDomain)
            {
                roles = _defaultRoles;
            }
            else
            {
                if (_allDomains.TryGetValue(domain, out roles) is false)
                {
                    return Enumerable.Empty<string>();
                }
            }

            var userNames = new List<string>();
            foreach (var role in roles)
            {
                if (role.Value.HasDirectRole(name))
                {
                    userNames.Add(role.Key);
                }
            }
            return userNames;
        }

        public virtual bool HasLink(string name1, string name2, string domain = null)
        {
            if (string.Equals(name1, name2))
            {
                return true;
            }

            domain ??= _defaultDomain;
            if (HasDomainPattern is false)
            {
                return HasLinkInDomain(name1, name2, domain);
            }

            foreach (string matchDomain in GetPatternDomains(domain))
            {
                if (HasLinkInDomain(name1, name2, matchDomain))
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasLinkInDomain(string name1, string name2, string domain)
        {
            ConcurrentDictionary<string, Role> roles;

            if (domain == _defaultDomain)
            {
                roles = _defaultRoles;
            }
            else
            {
                if (_allDomains.TryGetValue(domain, out roles) is false)
                {
                    return false;
                }
            }

            if (HasPattern is false)
            {
                return roles.TryGetValue(name1, out var role1) is not false
                       && role1.HasRole(name2, _maxHierarchyLevel);
            }

            foreach (var role in roles)
            {
                if (MatchingFunc(name1, role.Key) is false)
                {
                    continue;
                }

                if (role.Value.HasRole(name2, _maxHierarchyLevel, MatchingFunc))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void AddLink(string name1, string name2, string domain = null)
        {
            domain ??= _defaultDomain;
            _cachedAllDomains = null;
            AddLinkInDomain(name1, name2, domain);
        }

        private void AddLinkInDomain(string name1, string name2, string domain)
        {
            ConcurrentDictionary<string, Role> roles = domain == _defaultDomain
                ? _defaultRoles
                : _allDomains.GetOrAdd(domain, new ConcurrentDictionary<string, Role>());

            bool role1IsNew = roles.ContainsKey(name1) is false;
            bool role2IsNew = roles.ContainsKey(name2) is false;

            Role role1 = roles.GetOrAdd(name1, new Role(name1, domain));
            Role role2 = roles.GetOrAdd(name2, new Role(name2, domain));
            role1.AddRole(role2);

            if (HasDomainPattern)
            {
                if (role1IsNew)
                {
                    AddLinksFromMatchingDomains(role1);
                }
                if (role2IsNew)
                {
                    AddLinksToMatchingDomains(role2);
                }
            }

            if (HasPattern is false)
            {
                return;
            }

            foreach (var role in roles)
            {
                string name = role.Key;
                if (name == name1 || name == name2)
                {
                    return;
                }
                if (MatchingFunc(name, name1) || MatchingFunc(name1, name))
                {
                    role.Value.AddRole(role1);
                }
                if (MatchingFunc(name, name2) || MatchingFunc(name2, name))
                {
                    role2.AddRole(role.Value);
                }
            }
        }

        private void AddLinksFromMatchingDomains(Role role)
        {
            if (HasDomainPattern is false)
            {
                return;
            }

            IEnumerable<string> matchingDomains = GetMatchingDomains(role.Domain);

            foreach (string domain in matchingDomains)
            {
                if (domain == role.Domain || _allDomains.TryGetValue(domain, out var matchingDomain) is false)
                {
                    continue;
                }

                if (HasPattern is false)
                {
                    if (matchingDomain.TryGetValue(role.Name, out var matchingRole) && role != matchingRole)
                    {
                        matchingRole.AddRole(role);
                    };
                    continue;
                }

                var roleNames = matchingDomain.Keys;
                foreach (string roleName in roleNames)
                {
                    if (MatchingFunc(roleName, role.Name) is false)
                    {
                        continue;
                    }
                    if (matchingDomain.TryGetValue(roleName, out var matchingRole) && role != matchingRole)
                    {
                        matchingRole.AddRole(role);
                    }
                }
            }
        }

        private void AddLinksToMatchingDomains(Role role)
        {
            if (HasDomainPattern is false)
            {
                return;
            }

            IEnumerable<string> matchingDomains = GetPatternDomains(role.Domain);

            foreach (string domain in matchingDomains)
            {
                if (domain == role.Domain || _allDomains.TryGetValue(domain, out var matchingDomain) is false)
                {
                    continue;
                }

                if (HasPattern is false)
                {
                    if (matchingDomain.TryGetValue(role.Name, out var matchingRole) && role != matchingRole)
                    {
                        role.AddRole(matchingRole);
                    };
                    continue;
                }

                var roleNames = matchingDomain.Keys;
                foreach (string roleName in roleNames)
                {
                    if (MatchingFunc(role.Name, roleName) is false)
                    {
                        continue;
                    }
                    if (matchingDomain.TryGetValue(roleName, out var matchingRole) && matchingRole != role)
                    {
                        role.AddRole(matchingRole);
                    }
                }

            }
        }

        public virtual void DeleteLink(string name1, string name2, string domain = null)
        {
            domain ??= _defaultDomain;
            ConcurrentDictionary<string, Role> roles;
            if (domain == _defaultDomain)
            {
                roles = _defaultRoles;
            }
            else
            {
                if (_allDomains.TryGetValue(domain, out roles) is false)
                {
                    ThrowHelper.ThrowOneOfNamesNotFoundException();
                    return;
                }
            }

            if (roles.TryGetValue(name1, out var role1) is false
                || roles.TryGetValue(name2, out var role2) is false)
            {
                ThrowHelper.ThrowOneOfNamesNotFoundException();
                return;
            }
            role1.DeleteRole(role2);
        }

        public virtual void Clear()
        {
            _allDomains.Clear();
            _defaultRoles.Clear();
            _allDomains[_defaultDomain] = _defaultRoles;
            _cachedAllDomains = null;
        }

        private IEnumerable<string> GetPatternDomains(string domain)
        {
            List<string> matchDomains = new() { domain };
            _cachedAllDomains ??= _allDomains.Keys;
            if (HasDomainPattern)
            {
                matchDomains.AddRange(_cachedAllDomains.Where(key =>
                    DomainMatchingFunc(domain, key) && key != domain));
            }
            return matchDomains;
        }

        private IEnumerable<string> GetMatchingDomains(string domainPattern)
        {
            List<string> matchDomains = new() { domainPattern };
            _cachedAllDomains ??= _allDomains.Keys;
            if (HasDomainPattern)
            {
                matchDomains.AddRange(_cachedAllDomains.Where(key =>
                    DomainMatchingFunc(key, domainPattern) && key != domainPattern));
            }
            return matchDomains;
        }
    }
}
