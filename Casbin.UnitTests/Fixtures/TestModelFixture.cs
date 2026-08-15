using System.IO;
using System.Text;
using Casbin.Model;
using Casbin.Persist;
using Casbin.Persist.Adapter.File;

namespace Casbin.UnitTests.Fixtures;

public class TestModelFixture
{
    public static readonly string BasicModelText = ReadTestFile("basic_model.conf");
    public static readonly string BasicPolicyText = ReadTestFile("basic_policy.csv");
    public static readonly string BasicWithoutResourceModelText = ReadTestFile("basic_without_resources_model.conf");
    public static readonly string BasicWithoutResourcePolicyText = ReadTestFile("basic_without_resources_policy.csv");
    public static readonly string BasicWithoutUserModelText = ReadTestFile("basic_without_users_model.conf");
    public static readonly string BasicWithoutUserPolicyText = ReadTestFile("basic_without_users_policy.csv");
    public static readonly string BasicWithRootModelText = ReadTestFile("basic_with_root_model.conf");

    public static readonly string SaaRbacModelText = ReadTestFile("saa_model.conf");
    public static readonly string SaaRbacPolicyText = ReadTestFile("saa_rbac_policy.csv");

    public static readonly string RbacPolicyText = ReadTestFile("rbac_policy.csv");
    public static readonly string RbacCommentText = ReadTestFile("rbac_comment.conf");
    public static readonly string RbacInOperatorModelText = ReadTestFile("rbac_in_operator_model.conf");
    public static readonly string RbacInOperatorPolicyText = ReadTestFile("rbac_in_operator_policy.csv");
    public static readonly string RbacModelText = ReadTestFile("rbac_model.conf");
    public static readonly string RbacWithDenyModelText = ReadTestFile("rbac_with_deny_model.conf");
    public static readonly string RbacWithDenyPolicyText = ReadTestFile("rbac_with_deny_policy.csv");
    public static readonly string RbacWithDomainsModelText = ReadTestFile("rbac_with_domains_model.conf");
    public static readonly string RbacWithDomainsPolicy2Text = ReadTestFile("rbac_with_domains_policy2.csv");
    public static readonly string RbacWithDomainsPolicyText = ReadTestFile("rbac_with_domains_policy.csv");
    public static readonly string RbacWithHierarchyPolicyText = ReadTestFile("rbac_with_hierarchy_policy.csv");
    public static readonly string RbacWithHierarchyWithDomainsPolicyText = ReadTestFile("rbac_with_hierarchy_with_domains_policy.csv");
    public static readonly string RbacWithNotDenyModelText = ReadTestFile("rbac_with_not_deny_model.conf");
    public static readonly string RbacWithResourceRoleModelText = ReadTestFile("rbac_with_resource_roles_model.conf");
    public static readonly string RbacWithResourceRolePolicyText = ReadTestFile("rbac_with_resource_roles_policy.csv");

    public static readonly string AbacModelText = ReadTestFile("abac_model.conf");
    public static readonly string AbacCommentText = ReadTestFile("abac_comment.conf");
    public static readonly string AbacWithEvalModelText = ReadTestFile("abac_rule_model.conf");
    public static readonly string AbacWithEvalPolicyText = ReadTestFile("abac_rule_policy.csv");

    public static readonly string PriorityExplicitModelText = ReadTestFile("priority_explicit_model.conf");
    public static readonly string PriorityExplicitPolicyText = ReadTestFile("priority_explicit_policy.csv");
    public static readonly string PriorityIndeterminatePolicyText = ReadTestFile("priority_indeterminate_policy.csv");
    public static readonly string PriorityModelText = ReadTestFile("priority_model.conf");
    public static readonly string PriorityPolicyText = ReadTestFile("priority_policy.csv");

    public static readonly string SubjectPriorityModelText = ReadTestFile("subject_priority_model.conf");
    public static readonly string SubjectPriorityPolicyText = ReadTestFile("subject_priority_policy.csv");
    public static readonly string SubjectPriorityWithDomainModelText = ReadTestFile("subject_priority_model_with_domain.conf");
    public static readonly string SubjectPriorityWithDomainPolicyText = ReadTestFile("subject_priority_policy_with_domain.csv");

    public static readonly string TabsModelText = ReadTestFile("tabs_model.conf");
    public static readonly string TabsPolicyText = ReadTestFile("tabs_policy.csv");
    public static readonly string IpMatchModelText = ReadTestFile("ipmatch_model.conf");
    public static readonly string IpMatchPolicyText = ReadTestFile("ipmatch_policy.csv");
    public static readonly string KeyMatch2ModelText = ReadTestFile("keymatch2_model.conf");
    public static readonly string KeyMatch2PolicyText = ReadTestFile("keymatch2_policy.csv");
    public static readonly string KeyMatchCustomModelText = ReadTestFile("keymatch_custom_model.conf");
    public static readonly string KeyMatchModelText = ReadTestFile("keymatch_model.conf");
    public static readonly string KeyMatchPolicyText = ReadTestFile("keymatch_policy.csv");

    //https://github.com/apache/casbin-Casbin.NET/issues/310
    public static readonly string CommaAndQuotationsModelText = ReadTestFile("comma_quotations_model.conf");
    public static readonly string CommaAndQuotationsPolicyText = ReadTestFile("comma_quotations_policy.csv");

    // https://github.com/apache/casbin-Casbin.NET/issues/134
    public static readonly string MultipleTypeModelText = ReadTestFile("multiple_type_model.conf");
    public static readonly string MultipleTypePolicyText = ReadTestFile("multiple_type_policy.csv");

    // https://github.com/apache/casbin-Casbin.NET/issues/106
    public static readonly string RbacMultipleEvalModelText = ReadTestFile("rbac_multiple_eval_model.conf");
    public static readonly string RbacMultipleEvalPolicyText = ReadTestFile("rbac_multiple_eval_policy.csv");

    // https://github.com/apache/casbin-Casbin.NET/issues/154
    public static readonly string RbacMultipleModelText = ReadTestFile("rbac_multiple_rolemanager_model.conf");
    public static readonly string RbacMultiplePolicyText = ReadTestFile("rbac_multiple_rolemanager_policy.csv");

    // https://github.com/apache/casbin-Casbin.NET/issues/188
    public static readonly string PriorityExplicitDenyOverrideModelText = ReadTestFile("priority_explicit_deny_override_model.conf");
    public static readonly string PriorityExplicitDenyOverridePolicyText = ReadTestFile("priority_explicit_deny_override_policy.csv");

    // https://github.com/apache/casbin-Casbin.NET/issues/229
    public static readonly string SupportCountModelText = ReadTestFile("support_count_model.conf");

    // https://github.com/apache/casbin-Casbin.NET/issues/308
    public static readonly string RbacTokensWithSubstringRelationModelText = ReadTestFile("tokens_with_substring_relation_rbac.conf");
    public static readonly string RbacTokensWithSubstringRelationPolicyText = ReadTestFile("tokens_with_substring_relation_rbac.csv");
    public static readonly string AbacTokensWithSubstringRelationModelText = ReadTestFile("tokens_with_substring_relation_abac.conf");
    public static readonly string AbacTokensWithSubstringRelationPolicyText = ReadTestFile("tokens_with_substring_relation_abac.csv");

    // https://github.com/apache/casbin-Casbin.NET/issues/321
    public static readonly string BackslashLineFeedModelText = ReadTestFile("backslash_feed_model.conf");
    public static readonly string BackslashLineFeedPolicyText = ReadTestFile("backslash_feed_policy.csv");

    // https://github.com/apache/casbin-Casbin.NET/issues/353
    public static readonly string RbacWithIndexMatcherModelText = ReadTestFile("rbac_with_index_matcher_model.conf");
    public static readonly string RbacWithIndexMatcherPolicyText = ReadTestFile("rbac_with_index_matcher_policy.csv");

    // https://github.com/apache/casbin-Casbin.NET/issues/354
    public static readonly string AbacWithDynamicValueTypeModelText = ReadTestFile("abac_with_dynamic_value_type_model.conf");
    public static readonly string AbacWithDynamicValueTypePolicyText = ReadTestFile("abac_with_dynamic_value_type_policy.csv");

    // https://github.com/apache/casbin-Casbin.NET/issues/364
    public static readonly string AbacNotUsingPolicyModelText = ReadTestFile("abac_not_using_policy_model.conf");
    public static readonly string AbacRuleEffectPolicyText = ReadTestFile("abac_rule_effect_policy.csv");
    public static readonly string AbacRuleModelText = ReadTestFile("abac_rule_model.conf");
    public static readonly string AbacRulePolicyText = ReadTestFile("abac_rule_policy.csv");

    public static IModel GetNewAbacModel() => GetNewTestModel(AbacModelText);

    public static IModel GetNewAbacWithEvalModel() => GetNewTestModel(AbacWithEvalModelText, AbacWithEvalPolicyText);

    public static IModel GetBasicTestModel() => GetNewTestModel(BasicModelText, BasicPolicyText);

    public static IModel GetBasicWithoutResourceTestModel() =>
        GetNewTestModel(BasicWithoutResourceModelText, BasicWithoutResourcePolicyText);

    public static IModel GetBasicWithoutUserTestModel() =>
        GetNewTestModel(BasicWithoutUserModelText, BasicWithoutUserPolicyText);

    public static IModel GetNewKeyMatchTestModel() => GetNewTestModel(KeyMatchModelText, KeyMatchPolicyText);

    public static IModel GetNewKeyMatch2TestModel() => GetNewTestModel(KeyMatch2ModelText, KeyMatch2PolicyText);

    public static IModel GetNewPriorityTestModel() => GetNewTestModel(PriorityModelText, PriorityPolicyText);

    public static IModel GetNewPriorityExplicitTestModel() =>
        GetNewTestModel(PriorityExplicitModelText, PriorityExplicitPolicyText);

    public static IModel GetNewPriorityExplicitDenyOverrideModel() => GetNewTestModel(PriorityExplicitDenyOverrideModelText,
        PriorityExplicitDenyOverridePolicyText);

    public static IModel GetNewSaaRbacTestModel() => GetNewTestModel(SaaRbacModelText, SaaRbacPolicyText);

    public static IModel GetNewRbacTestModel() => GetNewTestModel(RbacModelText, RbacPolicyText);

    public static IModel GetNewRbacWithDenyTestModel() => GetNewTestModel(RbacWithDenyModelText, RbacWithDenyPolicyText);

    public static IModel GetNewRbacWithDomainsTestModel() =>
        GetNewTestModel(RbacWithDomainsModelText, RbacWithDomainsPolicyText);

    public static IModel GetNewRbacWithResourceRoleTestModel() =>
        GetNewTestModel(RbacWithResourceRoleModelText, RbacWithResourceRolePolicyText);

    public static IModel GetNewMultipleTypeTestModel() => GetNewTestModel(MultipleTypeModelText, MultipleTypePolicyText);

    public static IModel GetNewCommaAndQuotationsModel() =>
        GetNewTestModel(CommaAndQuotationsModelText, CommaAndQuotationsPolicyText);

    public static IModel GetNewTabsModel() =>
        GetNewTestModel(TabsModelText, TabsPolicyText);

    public static IModel GetNewTestModel(string modelText) => DefaultModel.CreateFromText(modelText);

    public static IModel GetNewTestModel(string modelText, string policyText) =>
        LoadModelFromMemory(GetNewTestModel(modelText), policyText);

    public static string GetTestFile(string fileName) => Path.Combine("Examples", fileName);

    private static IModel LoadModelFromMemory(IModel model, string policy)
    {
        using MemoryStream ms = new(Encoding.UTF8.GetBytes(policy));
        FileAdapter fileAdapter = new(ms);
        fileAdapter.LoadPolicy(model);
        model.SortPolicy();
        return model;
    }

    private static string ReadTestFile(string fileName) =>
        File.ReadAllText(GetTestFile(fileName));
}
