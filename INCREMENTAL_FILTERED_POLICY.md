# Incremental Filtered Policy Loading

## Overview

Casbin.NET now supports incremental filtered policy loading, which allows you to load multiple filtered policy sets without overwriting previously loaded policies. This is useful when you need to load both filtered `p` policies and `g` policies, or different sets of `p` policies from your custom adapter.

## Methods

### LoadIncrementalFilteredPolicy

Appends a filtered policy from file/database without clearing the existing policies.

```csharp
public static bool LoadIncrementalFilteredPolicy(this IEnforcer enforcer, IPolicyFilter filter)
public static Task<bool> LoadIncrementalFilteredPolicyAsync(this IEnforcer enforcer, IPolicyFilter filter)
```

## Usage Examples

### Example 1: Loading Multiple Policy Types

```csharp
using Casbin;
using Casbin.Model;
using Casbin.Persist;
using Casbin.Persist.Adapter.File;

// Setup
var enforcer = new Enforcer("path/to/model.conf");
var adapter = new FileAdapter("path/to/policy.csv");
enforcer.SetAdapter(adapter);

// Load only alice's p policies
enforcer.LoadFilteredPolicy(
    new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(new[] { "alice" }))
);

// Incrementally load the data2_admin role's p policies
enforcer.LoadIncrementalFilteredPolicy(
    new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(new[] { "data2_admin" }))
);

// Incrementally load alice's g policies (role assignments)
enforcer.LoadIncrementalFilteredPolicy(
    new PolicyFilter(PermConstants.DefaultRoleType, 0, Policy.ValuesFrom(new[] { "alice" }))
);

// Now alice can access resources through both direct policies and role inheritance
```

### Example 2: Loading Policies for Multiple Users

```csharp
// Load alice's policies
enforcer.LoadFilteredPolicy(
    new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(new[] { "alice" }))
);

// Add bob's policies incrementally (alice's policies are preserved)
enforcer.LoadIncrementalFilteredPolicy(
    new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(new[] { "bob" }))
);

// Both alice and bob's policies are now active
```

## Comparison with LoadFilteredPolicy

### LoadFilteredPolicy
- **Behavior**: Clears all existing policies and loads only the filtered policies
- **Use case**: When you want to replace the entire policy set with a filtered subset

```csharp
// Load alice's policies
enforcer.LoadFilteredPolicy(filter1);

// Load bob's policies (alice's policies are cleared)
enforcer.LoadFilteredPolicy(filter2);
// Result: Only bob's policies are active
```

### LoadIncrementalFilteredPolicy
- **Behavior**: Keeps existing policies and adds filtered policies
- **Use case**: When you want to build up a policy set from multiple filtered subsets

```csharp
// Load alice's policies
enforcer.LoadFilteredPolicy(filter1);

// Add bob's policies (alice's policies are kept)
enforcer.LoadIncrementalFilteredPolicy(filter2);
// Result: Both alice and bob's policies are active
```

## PolicyFilter Usage

The `PolicyFilter` class accepts three parameters:
1. **policyType**: The type of policy (e.g., `PermConstants.DefaultPolicyType` for "p", `PermConstants.DefaultRoleType` for "g")
2. **fieldIndex**: The starting field index to filter on (0-based)
3. **values**: The values to filter by

```csharp
// Filter p policies where the first field (subject) is "alice"
new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(new[] { "alice" }))

// Filter g policies where the first field (user) is "alice"
new PolicyFilter(PermConstants.DefaultRoleType, 0, Policy.ValuesFrom(new[] { "alice" }))

// Filter p policies where the second field (object) is "data1"
new PolicyFilter(PermConstants.DefaultPolicyType, 1, Policy.ValuesFrom(new[] { "data1" }))
```

## Additional Notes

- Both `LoadFilteredPolicy` and `LoadIncrementalFilteredPolicy` will clear the enforcer cache and rebuild role links if `AutoBuildRoleLinks` is enabled
- The `IsFiltered` flag on the adapter will be set to `true` when using either method
- Async versions of both methods are available for asynchronous operations

## Related Documentation

- [Casbin Policy Subset Loading](https://casbin.apache.org/docs/policy-subset-loading/)
- [Casbin Adapters](https://casbin.apache.org/docs/adapters)
