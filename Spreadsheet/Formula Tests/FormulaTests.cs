// <copyright file="FormulaSyntaxTests.cs" company="UofU-CS3500">
//   Copyright ?2024 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> [Insert Your Name] </authors>
// <date> [Insert the Date] </date>

namespace CS3500.FormulaTests;

using CS3500.Formula;

/// <summary>
///   <para>
///     The following class shows the basics of how to use the MSTest framework,
///     including:
///   </para>
///   <list type="number">
///     <item> How to catch exceptions. </item>
///     <item> How a test of valid code should look. </item>
///   </list>
/// </summary>
[TestClass]
public class FormulaSyntaxTests
{
    // --- Tests for One Token Rule ---

    /// <summary>
    ///   <para>
    ///     This test makes sure the right kind of exception is thrown
    ///     when trying to create a formula with no tokens.
    ///   </para>
    ///   <remarks>
    ///     <list type="bullet">
    ///       <item>
    ///         We use the _ (discard) notation because the formula object
    ///         is not used after that point in the method.  Note: you can also
    ///         use _ when a method must match an interface but does not use
    ///         some of the required arguments to that method.
    ///       </item>
    ///       <item>
    ///         string.Empty is often considered best practice (rather than using "") because it
    ///         is explicit in intent (e.g., perhaps the coder forgot to but something in "").
    ///       </item>
    ///       <item>
    ///         The name of a test method should follow the MS standard:
    ///         https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices
    ///       </item>
    ///       <item>
    ///         All methods should be documented, but perhaps not to the same extent
    ///         as this one.  The remarks here are for your educational
    ///         purposes (i.e., a developer would assume another developer would know these
    ///         items) and would be superfluous in your code.
    ///       </item>
    ///       <item>
    ///         Notice the use of the attribute tag [ExpectedException] which tells the test
    ///         that the code should throw an exception, and if it doesn't an error has occurred;
    ///         i.e., the correct implementation of the constructor should result
    ///         in this exception being thrown based on the given poorly formed formula.
    ///       </item>
    ///     </list>
    ///   </remarks>
    ///   <example>
    ///     <code>
    ///        // here is how we call the formula constructor with a string representing the formula
    ///        _ = new Formula( "5+5" );
    ///     </code>
    ///   </example>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNoTokens_Invalid()
    {
        _ = new Formula("");  // note: it is arguable that you should replace "" with string.Empty for readability and clarity of intent (e.g., not a cut and paste error or a "I forgot to put something there" error).
    }

    // --- Tests for Valid Token Rule ---
    [TestMethod]
    public void FormulaConstructor_TestValidTokensWithShortVariable_Valid()
    {
        _ = new Formula("z1");
    }

    [TestMethod]
    public void FormulaConstructor_TestValidTokensWithLongVariable_Valid()
    {
        _ = new Formula("CvvF430");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestValidTokensWithInvalidVariable_Invalid()
    {
        _ = new Formula("c");
        _ = new Formula("23c");
        _ = new Formula("2c6zx");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestValidTokensWithInvalidOperator_Invalid()
    {
        _ = new Formula("4 % 2");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestValidTokensWithInvalidSymbol_Invalid()
    {
        _ = new Formula("#");
    }


    [TestMethod]
    public void FormulaConstructor_TestValidAddOperatorTokens_Valid()
    {
        _ = new Formula("1+2");
    }

    [TestMethod]
    public void FormulaConstructor_TestValidMinusOperatorTokens_Valid()
    {
        _ = new Formula("3-2");
    }

    [TestMethod]
    public void FormulaConstructor_TestValidMultiplyOperatorTokens_Valid()
    {
        _ = new Formula("2*5");
    }

    [TestMethod]
    public void FormulaConstructor_TestValidDivisionOperatorTokens_Valid()
    {
        _ = new Formula("9/3");
    }

    [TestMethod]
    public void FormulaConstructor_TestValidIntegerNumbersTokens_Valid()
    {
        _ = new Formula("23");
    }

    [TestMethod]
    public void FormulaConstructor_TestValidDecimalPointNumbersTokens_Valid()
    {
        _ = new Formula("10.33");
    }

    [TestMethod]
    public void FormulaConstructor_TestValidScientificNotationNumbersTokens_Valid()
    {
        _ = new Formula("2e5");
    }

    [TestMethod]
    public void FormulaConstructor_TestValidENotationNumbersTokens_Valid()
    {
        _ = new Formula("2E-3");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestValidTokensWithNegativeNumber_Invalid()
    {
        _ = new Formula("-1 + 1");
    }

    // --- Tests for Closing Parenthesis Rule
    [TestMethod]
    public void FormulaConstructor_TestClosingParenthesesRule_Valid()
    {
        _ = new Formula("((1 + 1) + 1)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParenthesesRule_Invalid()
    {
        _ = new Formula("(1) + 1)(1");
    }
    // --- Tests for Balanced Parentheses Rule
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestBalancedParenthesesWithMoreOpeningParentheses_Invalid()
    {
        _ = new Formula("((1+1)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestBalancedParenthesesWithMoreClosingParentheses_Invalid()
    {
        _ = new Formula("((1+1)))");
    }
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestBalancedParenthesesWithNoOpeningParentheses_Invalid()
    {
        _ = new Formula("1+1)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestBalancedParenthesesWithNoClosingParentheses_Invalid()
    {
        _ = new Formula("(1+1");
    }
    [TestMethod]
    public void FormulaConstructor_TestBalancedgParenthesesRule_Valid()
    {
        _ = new Formula("(1+1) * ((2 + 2) - 1)");
    }
    // --- Tests for First Token Rule

    [TestMethod]
    public void FormulaConstructor_TestFirstTokenIsOpeningParenthesis_Valid()
    {
        _ = new Formula("(1+1)");
    }

    [TestMethod]
    public void FormulaConstructor_TestFirstTokenIsNumber_Valid()
    {
        _ = new Formula("1+1");
    }

    [TestMethod]
    public void FormulaConstructor_TestFirstTokenIsVariable_Valid()
    {
        _ = new Formula("q3*1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestFirstTokenIsOperator_Invalid()
    {
        _ = new Formula("+ 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestFirstTokenIsClosingParentheses_Invalid()
    {
        _ = new Formula("))");
    }

    // --- Tests for  Last Token Rule ---
    [TestMethod]
    public void FormulaConstructor_TestLastTokenIsNumber_Valid()
    {
        _ = new Formula("q3/1");
    }

    [TestMethod]
    public void FormulaConstructor_TestLastTokenIsVariable_Valid()
    {
        _ = new Formula("1+a1");
    }

    [TestMethod]
    public void FormulaConstructor_TestLastTokenIsClosingParenthesis_Valid()
    {
        _ = new Formula("(7*3)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastTokenIsOperator_Invalid()
    {
        _ = new Formula("5 /");
        _ = new Formula("t5 *");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastTokenIsOpeningParentheses_Invalid()
    {
        _ = new Formula("((");
    }
    // --- Tests for Parentheses/Operator Following Rule ---
    [TestMethod]
    public void FormulaConstructor_TestParenthesisFollowing_Valid()
    {
        _ = new Formula("(7*3)");
        _ = new Formula("(Ss1)");
        _ = new Formula("((2/1))");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestParenthesisFollowingClosingParentheses_Invalid()
    {
        _ = new Formula("()");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestParenthesisFollowingOperator_Invalid()
    {
        _ = new Formula("(/6)");
    }

    [TestMethod]
    public void FormulaConstructor_TestOperatorFollowing_Valid()
    {
        _ = new Formula("1 * 1");
        _ = new Formula("1 + a1");
        _ = new Formula("33 / (1) ");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperatorFollowingClosingParentheses_Invalid()
    {
        _ = new Formula("(1-)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperatorFollowingOperator_Invalid()
    {
        _ = new Formula("1+-1");
    }
    // --- Tests for Extra Following Rule ---
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingNumberFollowedbyNumber_Invalid()
    {
        _ = new Formula("1 2E-4");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingNumberFollowedbyVariable_Invalid()
    {
        _ = new Formula("1 pq123");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingNumberFollowedbyOpeningParentheses_Invalid()
    {
        _ = new Formula("1 (5)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingVariableFollowedbyNumber_Invalid()
    {
        _ = new Formula("w12 50");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingVariableFollowedbyVariable_Invalid()
    {
        _ = new Formula("w12 tt5");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingVariableFollowedbyOpeningParentheses_Invalid()
    {
        _ = new Formula("w12 (5)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingClosingParenthesesFollowedbyNumber_Invalid()
    {
        _ = new Formula("(1) 12");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingClosingParenthesesFollowedbyVariable_Invalid()
    {
        _ = new Formula("(1) e3");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingClosingParenthesesFollowedbyOpeningParentheses_Invalid()
    {
        _ = new Formula("(1) (2)");
    }

    [TestMethod]
    public void GetVariables_FormulaWithNoVariable_EmptySet()
    {
        Formula f = new Formula("5+1*3");
        Assert.AreEqual(f.GetVariables().Count, 0);
    }

    [TestMethod]
    public void GetVariables_FormulaWithTwoVariable_SizeTwoSet()
    {
        Formula f = new Formula("(5+1*3) / Ac01 + (zXX3 - 9)");
        Assert.AreEqual(f.GetVariables().Count, 2);
        Assert.IsTrue(f.GetVariables().Contains("AC01"));
        Assert.IsTrue(f.GetVariables().Contains("ZXX3"));
    }

    [TestMethod]
    public void GetVariables_FormulaWithSameVariableName_SizeOneSet()
    {
        Formula f = new Formula("Zxx3 + (zXX3 - 9)");
        Assert.AreEqual(f.GetVariables().Count, 1);
        foreach(string variable in f.GetVariables())
        {
            Assert.AreEqual(variable, "ZXX3");
        }
    }

    [TestMethod]
    public void ToString_FormulaWithDoubleNumAndVariable()
    {
        Formula f = new Formula("X1 + 5.0000");
        string expect = "X1+5";
        Assert.AreEqual(f.ToString(), expect);
    }

    [TestMethod]
    public void ToString_FormulaWithSimilarVariables()
    {
        Formula f = new Formula("X1 + 13 - x1 * (X01 / 10)");
        string expect = "X1+13-X1*(X01/10)";
        Assert.AreEqual(f.ToString(), expect);
    }


    [TestMethod]
    public void ToString_FormulaWithSimilarVariablesAndScientificNotation()
    {
        Formula f = new Formula("ga22*a1 + 4E3- 22.0 + 3e-1 / Ga22");
        string expect = "GA22*A1+4000-22+0.3/GA22";
        Assert.AreEqual(f.ToString(), expect);
    }

}