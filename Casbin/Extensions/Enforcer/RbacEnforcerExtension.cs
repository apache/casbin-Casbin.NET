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
using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin
{
    public static class RbacEnforcerExtension
    {
        #region Has roles or users

        /// <summary>
        ///     Determines whether a user has a role.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="name"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static bool HasRoleForUser(this IEnforcer enforcer, string name, string role, string domain = null)
        {
            IEnumerable<string> roles = enforcer.GetRolesForUser(name, domain);
            return roles.Any(roleEnum => roleEnum.Equals(role));
        }

        #endregion

        #region Get permissions

        /// <summary>
        ///     Gets permissions for a user or role.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user">User or role</param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<string>> GetPermissionsForUser(this IEnforcer enforcer, string user,
            string domain = null) =>
            domain is null
                ? enforcer.GetFilteredPolicy(0, user)
                : enforcer.GetFilteredPolicy(0, user, domain);

        #endregion

        #region Get roles or users

        /// <summary>
        /// Gets the roles that a user has.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="name"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetRolesForUser(this IEnforcer enforcer, string name, string domain = null)
        {
            return domain is null
                ? enforcer.Model.GetRoleManager().GetRoles(name)
                : enforcer.Model.GetRoleManager().GetRoles(name, domain);
        }

        /// <summary>
        /// Gets the users that has a role.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="name"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetUsersForRole(this IEnforcer enforcer, string name, string domain = null)
        {
            return domain is null
                ? enforcer.Model.GetRoleManager().GetUsers(name)
                : enforcer.Model.GetRoleManager().GetUsers(name, domain);
        }

        /// <summary>
        /// Gets the users that has roles.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetUsersForRoles(this IEnforcer enforcer, params string[] names)
        {
            return enforcer.GetUsersForRoles(names as IEnumerable<string>);
        }

        /// <summary>
        /// Gets the users that has roles.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetUsersForRoles(this IEnforcer enforcer, IEnumerable<string> names)
        {
            var userIds = new List<string>();
            foreach (string name in names)
            {
                userIds.AddRange(enforcer.Model.GetRoleManager().GetUsers(name));
            }

            return userIds;
        }

        #endregion

        #region Add roles or users

        /// <summary>
        /// Adds a role for a user.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the user already has the role (aka not affected).</returns>
        public static bool AddRoleForUser(this IEnforcer enforcer, string user, string role, string domain = null)
        {
            return domain is null
                ? enforcer.AddGroupingPolicy(user, role)
                : enforcer.AddGroupingPolicy(user, role, domain);
        }

        /// <summary>
        /// Adds a role for a user.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the user already has the role (aka not affected).</returns>
        public static Task<bool> AddRoleForUserAsync(this IEnforcer enforcer, string user, string role,
            string domain = null)
        {
            return domain is null
                ? enforcer.AddGroupingPolicyAsync(user, role)
                : enforcer.AddGroupingPolicyAsync(user, role, domain);
        }

        /// <summary>
        /// AddRolesForUser adds roles for a user.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the user already has the role (aka not affected).</returns>
        public static bool AddRolesForUser(this IEnforcer enforcer, string user, IEnumerable<string> role,
            string domain = null)
        {
            return domain is null
                ? enforcer.AddGroupingPolicies(role.Select(r => new List<string> { user, r }))
                : enforcer.AddGroupingPolicies(role.Select(r => new List<string> { user, r, domain }));
        }

        /// <summary>
        /// AddRolesForUser adds roles for a user.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the user already has the role (aka not affected).</returns>
        public static Task<bool> AddRolesForUserAsync(this IEnforcer enforcer, string user, IEnumerable<string> role,
            string domain = null)
        {
            return domain is null
                ? enforcer.AddGroupingPoliciesAsync(role.Select(r => new List<string> { user, r }))
                : enforcer.AddGroupingPoliciesAsync(role.Select(r => new List<string> { user, r, domain }));
        }

        #endregion

        #region Delete roles or users

        /// <summary>
        /// Deletes a role for a user.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the user does not have the role (aka not affected).</returns>
        public static bool DeleteRoleForUser(this IEnforcer enforcer, string user, string role, string domain = null)
        {
            return domain is null
                ? enforcer.RemoveGroupingPolicy(user, role)
                : enforcer.RemoveGroupingPolicy(user, role, domain);
        }

        /// <summary>
        /// Deletes a role for a user.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the us/*does */not have the role (aka not affected).</returns>
        public static Task<bool> DeleteRoleForUserAsync(this IEnforcer enforcer, string user, string role,
            string domain = null)
        {
            return domain is null
                ? enforcer.RemoveGroupingPolicyAsync(user, role)
                : enforcer.RemoveGroupingPolicyAsync(user, role, domain);
        }

        /// <summary>
        /// Deletes all roles for a user.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the user does not have any roles (aka not affected).</returns>
        public static bool DeleteRolesForUser(this IEnforcer enforcer, string user, string domain = null)
        {
            return domain is null
                ? enforcer.RemoveFilteredGroupingPolicy(0, user)
                : enforcer.RemoveFilteredGroupingPolicy(0, user, string.Empty, domain);
        }

        /// <summary>
        /// Deletes all roles for a user.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user"></param>
        /// <param name="domain"></param>
        /// <returns>Returns false if the user does not have any roles (aka not affected).</returns>
        public static Task<bool> DeleteRolesForUserAsync(this IEnforcer enforcer, string user, string domain = null)
        {
            return domain is null
                ? enforcer.RemoveFilteredGroupingPolicyAsync(0, user)
                : enforcer.RemoveFilteredGroupingPolicyAsync(0, user, string.Empty, domain);
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user"></param>
        /// <returns>Returns false if the user does not exist (aka not affected).</returns>
        public static bool DeleteUser(this IEnforcer enforcer, string user)
        {
            bool groupResult = enforcer.RemoveFilteredGroupingPolicy(0, user);
            bool result = enforcer.RemoveFilteredPolicy(0, user);
            return groupResult || result;
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user"></param>
        /// <returns>Returns false if the user does not exist (aka not affected).</returns>
        public static async Task<bool> DeleteUserAsync(this IEnforcer enforcer, string user)
        {
            bool groupResult = await enforcer.RemoveFilteredGroupingPolicyAsync(0, user);
            bool result = await enforcer.RemoveFilteredPolicyAsync(0, user);
            return groupResult || result;
        }

        /// <summary>
        /// Deletes a role.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="role"></param>
        /// <returns>Returns false if the role does not exist (aka not affected).</returns>
        public static bool DeleteRole(this IEnforcer enforcer, string role)
        {
            bool groupResult = enforcer.RemoveFilteredGroupingPolicy(1, role);
            bool result = enforcer.RemoveFilteredPolicy(0, role);
            return groupResult || result;
        }

        /// <summary>
        /// Deletes a role.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="role"></param>
        public static async Task<bool> DeleteRoleAsync(this IEnforcer enforcer, string role)
        {
            bool groupResult = await enforcer.RemoveFilteredGroupingPolicyAsync(1, role);
            bool result = await enforcer.RemoveFilteredPolicyAsync(0, role);
            return groupResult || result;
        }

        #endregion

        #region Has permissions

        /// <summary>
        /// Determines whether a user has a permission.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public static bool HasPermissionForUser(this IEnforcer enforcer, string user, params string[] permission)
        {
            return HasPermissionForUser(enforcer, user, permission as IEnumerable<string>);
        }

        /// <summary>
        /// Determines whether a user has a permission.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public static bool HasPermissionForUser(this IEnforcer enforcer, string user, IEnumerable<string> permission)
        {
            var parameters = new List<string> { user };
            parameters.AddRange(permission);
            return enforcer.HasPolicy(parameters);
        }

        #endregion

        #region Add permissions

        /// <summary>
        /// Adds a permission for a user or role.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns> Returns false if the user or role already has the permission (aka not affected).</returns>
        public static bool AddPermissionForUser(this IEnforcer enforcer, string user, params string[] permission)
        {
            return AddPermissionForUser(enforcer, user, permission as IEnumerable<string>);
        }

        /// <summary>
        /// Adds a permission for a user or role.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns> Returns false if the user or role already has the permission (aka not affected).</returns>
        public static Task<bool> AddPermissionForUserAsync(this IEnforcer enforcer, string user,
            params string[] permission)
        {
            return AddPermissionForUserAsync(enforcer, user, permission as IEnumerable<string>);
        }

        /// <summary>
        /// Adds a permission for a user or role.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the user or role already has the permission (aka not affected).</returns>
        public static bool AddPermissionForUser(this IEnforcer enforcer, string user, IEnumerable<string> permission)
        {
            var parameters = new[] { user }.Concat(permission);
            return enforcer.AddPolicy(parameters);
        }

        /// <summary>
        /// Adds multiple permissions for a user or role.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the user or role already has the permission (aka not affected).</returns>
        public static Task<bool> AddPermissionForUserAsync(this IEnforcer enforcer, string user,
            IEnumerable<string> permission)
        {
            var parameters = new[] { user }.Concat(permission);
            return enforcer.AddPolicyAsync(parameters);
        }

        #endregion

        #region Delete permissions

        /// <summary>
        /// DeletePermission deletes a permission.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the permission does not exist (aka not affected).</returns>
        public static bool DeletePermission(this IEnforcer enforcer, params string[] permission)
        {
            return enforcer.RemoveFilteredPolicy(1, permission);
        }

        /// <summary>
        /// DeletePermission deletes a permission.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the permission does not exist (aka not affected).</returns>
        public static Task<bool> DeletePermissionAsync(this IEnforcer enforcer, params string[] permission)
        {
            return enforcer.RemoveFilteredPolicyAsync(1, permission);
        }


        /// <summary>
        /// DeletePermission deletes a permission.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the permission does not exist (aka not affected).</returns>
        public static bool DeletePermission(this IEnforcer enforcer, IEnumerable<string> permission)
        {
            return DeletePermission(enforcer, permission.ToArray());
        }

        /// <summary>
        /// DeletePermission deletes a permission.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the permission does not exist (aka not affected).</returns>
        public static Task<bool> DeletePermissionAsync(this IEnforcer enforcer, IEnumerable<string> permission)
        {
            return DeletePermissionAsync(enforcer, permission.ToArray());
        }

        /// <summary>
        /// DeletePermissionForUser deletes a permission for a user or role.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public static bool DeletePermissionForUser(this IEnforcer enforcer, string user, params string[] permission)
        {
            return DeletePermissionForUser(enforcer, user, permission as IEnumerable<string>);
        }

        /// <summary>
        /// DeletePermissionForUser deletes a permission for a user or role.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public static Task<bool> DeletePermissionForUserAsync(this IEnforcer enforcer, string user,
            params string[] permission)
        {
            return DeletePermissionForUserAsync(enforcer, user, permission as IEnumerable<string>);
        }


        /// <summary>
        /// DeletePermissionForUser deletes a permission for a user or role.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the user or role does not have any permissions (aka not affected).</returns>
        public static bool DeletePermissionForUser(this IEnforcer enforcer, string user, IEnumerable<string> permission)
        {
            var parameters = new[] { user }.Concat(permission);
            return enforcer.RemovePolicy(parameters);
        }

        /// <summary>
        /// DeletePermissionForUser deletes a permission for a user or role.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user">User or role</param>
        /// <param name="permission"></param>
        /// <returns>Returns false if the user or role does not have any permissions (aka not affected).</returns>
        public static Task<bool> DeletePermissionForUserAsync(this IEnforcer enforcer, string user,
            IEnumerable<string> permission)
        {
            var parameters = new[] { user }.Concat(permission);
            return enforcer.RemovePolicyAsync(parameters);
        }

        /// <summary>
        /// DeletePermissionsForUser deletes permissions for a user or role.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user">User or role</param>
        /// <returns>Returns false if the user or role does not have any permissions (aka not affected).</returns>
        public static bool DeletePermissionsForUser(this IEnforcer enforcer, string user)
        {
            return enforcer.RemoveFilteredPolicy(0, user);
        }

        /// <summary>
        /// DeletePermissionsForUser deletes permissions for a user or role.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user">User or role</param>
        /// <returns>Returns false if the user or role does not have any permissions (aka not affected).</returns>
        public static Task<bool> DeletePermissionsForUserAsync(this IEnforcer enforcer, string user)
        {
            return enforcer.RemoveFilteredPolicyAsync(0, user);
        }

        #endregion

        #region Get implicit roles or users

        /// <summary>
        /// Gets implicit roles that a user has.
        /// Compared to GetRolesForUser(), this function retrieves indirect roles besides direct roles.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="name"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetImplicitRolesForUser(this IEnforcer enforcer, string name,
            string domain = null)
        {
            HashSet<string> roleSet = new() { name };
            Queue<string> queue = new();
            queue.Enqueue(name);
            var sections = enforcer.Model.Sections;

            while (queue.Count is not 0)
            {
                string nowRole = queue.Dequeue();
                foreach (KeyValuePair<string, RoleAssertion> assertion in sections.GetRoleAssertions())
                {
                    var roles = domain is null
                        ? assertion.Value.RoleManager.GetRoles(nowRole)
                        : assertion.Value.RoleManager.GetRoles(nowRole, domain);

                    foreach (string role in roles)
                    {
                        if (roleSet.Add(role))
                        {
                            queue.Enqueue(role);
                        }
                    }
                }
            }
            roleSet.Remove(name);
            var roleList = roleSet.ToList();
            roleList.Sort();
            return roleList;
        }

        /// <summary>
        /// <para>Gets implicit permissions for a user or role.</para>
        /// <para>Compared to GetPermissionsForUser(), this function retrieves permissions for inherited roles.</para>
        /// <para>For example:</para>
        /// <para>p, admin, data1, read</para>
        /// <para>p, alice, data2, read</para>
        /// <para>g, alice, admin </para>
        /// <para>GetPermissionsForUser("alice") can only get: [["alice", "data2", "read"]].</para>
        /// <para>But GetImplicitPermissionsForUser("alice") will get: [["admin", "data1", "read"], ["alice", "data2", "read"]].</para>
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<string>> GetImplicitPermissionsForUser(this IEnforcer enforcer,
            string user, string domain = null)
        {
            var roles = new List<string> { user };
            roles.AddRange(GetImplicitRolesForUser(enforcer, user, domain));
            var result = new List<IEnumerable<string>>();
            foreach (string role in roles)
            {
                result.AddRange(GetPermissionsForUser(enforcer, role, domain));
            }
            return result;
        }

        public static IEnumerable<string> GetImplicitUsersForPermission(this IEnforcer enforcer,
            params string[] permission)
        {
            return GetImplicitUsersForPermission(enforcer, permission as IEnumerable<string>);
        }

        public static IEnumerable<string> GetImplicitUsersForPermission(this IEnforcer enforcer,
            IEnumerable<string> permissions)
        {
            var policySubjects = enforcer.GetAllSubjects();
            var groupInherit = enforcer.InternalGetValuesForFieldInPolicyAllTypes("g", 1);
            var groupSubjects = enforcer.InternalGetValuesForFieldInPolicyAllTypes("g", 0);

            string[] permissionList = permissions as string[] ?? permissions.ToArray();
            if (Request.SupportGeneric(permissionList.Length + 1) is false)
            {
                return policySubjects.Concat(groupSubjects).Distinct()
                    .Where(subject =>
                        enforcer.Enforce(Request.CreateValues(new[] { subject }.Concat(permissionList).Cast<object>()
                            .ToArray())))
                    .Except(groupInherit);
            }

            var users = (permissionList.Length + 1) switch
            {
                2 => policySubjects.Concat(groupSubjects).Distinct()
                    .Where(subject => enforcer.Enforce(subject, permissionList[0]))
                    .Except(groupInherit),
                3 => policySubjects.Concat(groupSubjects).Distinct()
                    .Where(subject => enforcer.Enforce(subject, permissionList[0], permissionList[1]))
                    .Except(groupInherit),
                4 => policySubjects.Concat(groupSubjects).Distinct()
                    .Where(subject =>
                        enforcer.Enforce(subject, permissionList[0], permissionList[1], permissionList[2]))
                    .Except(groupInherit),
                5 => policySubjects.Concat(groupSubjects).Distinct()
                    .Where(subject => enforcer.Enforce(subject, permissionList[0], permissionList[1], permissionList[2],
                        permissionList[3]))
                    .Except(groupInherit),
                6 => policySubjects.Concat(groupSubjects).Distinct()
                    .Where(subject => enforcer.Enforce(subject, permissionList[0], permissionList[1], permissionList[2],
                        permissionList[3], permissionList[4]))
                    .Except(groupInherit),
                7 => policySubjects.Concat(groupSubjects).Distinct()
                    .Where(subject => enforcer.Enforce(subject, permissionList[0], permissionList[1], permissionList[2],
                        permissionList[3], permissionList[4], permissionList[5]))
                    .Except(groupInherit),
                8 => policySubjects.Concat(groupSubjects).Distinct()
                    .Where(subject => enforcer.Enforce(subject, permissionList[0], permissionList[1], permissionList[2],
                        permissionList[3], permissionList[4], permissionList[5], permissionList[6]))
                    .Except(groupInherit),
                9 => policySubjects.Concat(groupSubjects).Distinct()
                    .Where(subject => enforcer.Enforce(subject, permissionList[0], permissionList[1], permissionList[2],
                        permissionList[3], permissionList[4], permissionList[5], permissionList[6], permissionList[7]))
                    .Except(groupInherit),
                10 => policySubjects.Concat(groupSubjects).Distinct()
                    .Where(subject => enforcer.Enforce(subject, permissionList[0], permissionList[1], permissionList[2],
                        permissionList[3], permissionList[4], permissionList[5], permissionList[6], permissionList[7],
                        permissionList[8]))
                    .Except(groupInherit),
                11 => policySubjects.Concat(groupSubjects).Distinct()
                    .Where(subject => enforcer.Enforce(subject, permissionList[0], permissionList[1], permissionList[2],
                        permissionList[3], permissionList[4], permissionList[5], permissionList[6], permissionList[7],
                        permissionList[8], permissionList[9]))
                    .Except(groupInherit),
                12 => policySubjects.Concat(groupSubjects).Distinct()
                    .Where(subject => enforcer.Enforce(subject, permissionList[0], permissionList[1], permissionList[2],
                        permissionList[3], permissionList[4], permissionList[5], permissionList[6], permissionList[7],
                        permissionList[8], permissionList[9], permissionList[10]))
                    .Except(groupInherit),
                _ => throw new InvalidOperationException("The length of permissions is invalid")
            };
            return users;
        }

        #endregion

        #region RBAC APIs with domains

        public static IEnumerable<string> GetDomainsForUser(this IEnforcer enforcer, string name,
            string roleType = null)
        {
            roleType ??= PermConstants.DefaultRoleType;
            return enforcer.Model.GetRoleManager(roleType).GetDomains(name);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="name"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<string>
            GetRolesForUserInDomain(this IEnforcer enforcer, string name, string domain) =>
            enforcer.Model.GetRoleManager().GetRoles(name, domain);

        /// <summary>
        ///
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user">User or role</param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<string>> GetPermissionsForUserInDomain(this IEnforcer enforcer,
            string user, string domain)
        {
            return enforcer.GetFilteredPolicy(0, user, domain);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static bool AddRoleForUserInDomain(this IEnforcer enforcer, string user, string role, string domain)
        {
            return enforcer.AddGroupingPolicy(user, role, domain);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static Task<bool> AddRoleForUserInDomainAsync(this IEnforcer enforcer, string user, string role,
            string domain)
        {
            return enforcer.AddGroupingPolicyAsync(user, role, domain);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static bool DeleteRoleForUserInDomain(this IEnforcer enforcer, string user, string role, string domain)
        {
            return enforcer.RemoveGroupingPolicy(user, role, domain);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static Task<bool> DeleteRoleForUserInDomainAsync(this IEnforcer enforcer, string user, string role,
            string domain)
        {
            return enforcer.RemoveGroupingPolicyAsync(user, role, domain);
        }

        #endregion
    }
}
