using System.Collections.Generic;
using Casbin.Util;
using Xunit;

namespace Casbin.UnitTests.UtilTests;

public class StringUtilTest
{
    public static IEnumerable<object[]> RemoveCommentsTestData =
    [
        ["r.act == p.act", "r.act == p.act # comments"],
        ["r.act == p.act", "r.act == p.act#comments"],
        ["r.act == p.act", "r.act == p.act###"], ["", "### comments"],
        ["r.act == p.act", "r.act == p.act"]
    ];

    public static IEnumerable<object[]> ReplaceEvalTestData =
    [
        [
            "eval(rule1)", "(a == b)",
            new Dictionary<string, string> { ["rule1"] = "a == b" }],
        [
            "eval(rule1) && c && d", "(a == b) && c && d",
            new Dictionary<string, string> { ["rule1"] = "a == b" }
        ],
        [
            "eval(rule1)", "eval(rule1)", null
        ],
        [
            "eval(rule1) && c && d", "eval(rule1) && c && d", null
        ],
        [
            "eval(rule1) || eval(rule2)", "(a == b) || (a == c)",
            new Dictionary<string, string> { ["rule1"] = "a == b", ["rule2"] = "a == c" }
        ],
        [
            "eval(rule1) || eval(rule2) && c && d", "(a == b) || (a == c) && c && d",
            new Dictionary<string, string> { ["rule1"] = "a == b", ["rule2"] = "a == c" }
        ],
        [
            "eval(rule1) || eval(rule2)", "(a == b) || eval(rule2)",
            new Dictionary<string, string> { ["rule1"] = "a == b" }
        ],
        [
            "eval(rule1) || eval(rule2) && c && d", "(a == b) || eval(rule2) && c && d",
            new Dictionary<string, string> { ["rule1"] = "a == b" }
        ],
        [
            "eval(rule1) || eval(rule2)", "eval(rule1) || (a == c)",
            new Dictionary<string, string> { ["rule2"] = "a == c" }
        ],
        [
            "eval(rule1) || eval(rule2) && c && d", "eval(rule1) || (a == c) && c && d",
            new Dictionary<string, string> { ["rule2"] = "a == c" }
        ],
        [
            "eval(rule1) || eval(rule2)", "eval(rule1) || eval(rule2)", null
        ],
        [
            "eval(rule1) || eval(rule2) && c && d", "eval(rule1) || eval(rule2) && c && d", null
        ]
    ];

    public static IEnumerable<object[]> ReplaceInListOperatorTestData =
    [
        ["r.obj in (\"a\", \"b\")", "(r.obj == \"a\" || r.obj == \"b\")"],
        ["r.obj in [\"a\", \"b\"]", "(r.obj == \"a\" || r.obj == \"b\")"],
        ["r.obj in (\"a\")", "(r.obj == \"a\")"],
        ["r.obj in (\"a,b\", \"c\")", "(r.obj == \"a,b\" || r.obj == \"c\")"],
        ["r.obj in (\"a, \\\"b,c\\\"\", \"d\")", "(r.obj == \"a, \\\"b,c\\\"\" || r.obj == \"d\")"],
        ["r.obj in (\"a\\\\\", \"b\")", "(r.obj == \"a\\\\\" || r.obj == \"b\")"],
        ["r.obj in (\"f(x)\", \"[y]\")", "(r.obj == \"f(x)\" || r.obj == \"[y]\")"],
        ["r.obj in [\"f(x)\", \"[y]\"]", "(r.obj == \"f(x)\" || r.obj == \"[y]\")"],
        ["r.obj in (p.obj)", "r.obj in (p.obj)"]
    ];

    [Theory]
    [MemberData(nameof(ReplaceInListOperatorTestData))]
    public void TestReplaceInListOperator(string expressionString, string expected) =>
        Assert.Equal(expected, StringUtil.ReplaceInListOperator(expressionString));

    [Theory]
    [MemberData(nameof(RemoveCommentsTestData))]
    public void TestRemoveComments(string except, string actual) => Assert.Equal(except, actual.RemoveComments());

    [Theory]
    [MemberData(nameof(ReplaceEvalTestData))]
    public void TestReplaceEval(
        string oldExpressionString,
        string newExpressionString,
        IDictionary<string, string> rules) =>
        Assert.Equal(newExpressionString, StringUtil.ReplaceEval(oldExpressionString, rules));
}
