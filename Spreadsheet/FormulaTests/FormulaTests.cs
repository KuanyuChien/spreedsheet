using CS3500.Formula;

namespace FormulaTests
{
    [TestClass]
    public class FormulaSyntaxTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Formula f1 = new("(4E3- 22.0)");
            Formula f = new("a22*a1 + 4E3- 22.0 / Ga22");
            double x = double.Parse("4E3");

        }
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
        [TestMethod]
        public void FormulaConstructor_TestOneTokenRule_Valid()
        {
            _ = new Formula("1");
        }

        // --- Tests for Valid Token Rule ---*
        /// <summary>
        ///   <para>
        ///     Make sure a simple well formed formula is accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     This is an example of a test that is not expected to throw an exception, i.e., it succeeds.
        ///     In other words, the formula "1+1" is a valid formula which should not cause any errors.
        ///   </remarks>
        /// </summary>
        [TestMethod]
        public void FormulaConstructor_TestTokenRule_Valid()
        {
            _ = new Formula("1+1");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaConstructor_TestTokenRule_Invalid()
        {
            _ = new Formula("2 # 3");
        }
        // --- Tests for Closing Parenthesis Rule ---*
        /// <summary>
        ///   <para>
        ///     Make sure a formula that test closing parantheses and is accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     The formula "(1+1)" is a valid formula that follow the closing parentheses rule.
        ///     This test is a valid test and should not throw any exception
        ///   </remarks>
        /// </summary>
        [TestMethod]
        public void FormulaConstructor_TestClosingParentheses_Valid()
        {
            _ = new Formula("(1+1)");
        }

        /// <summary>
        ///   <para>
        ///     Make sure a formula that test wrong closing parantheses and is not accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     The formula "7+8)" is an invalid formula that  does not follow the closing parentheses rule.
        ///     This test is not a valid test and should throw any exception
        ///   </remarks>
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaConstructor_TestClosingParantheses_Invalid()
        {
            _ = new Formula("7+8)");
        }

        // --- Tests for Balanced Parentheses Rule ---*
        /// <summary>
        ///   <para>
        ///     Make sure a formula that test balanced parantheses and is accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     The formula "((2+2)+5)" is a valid formula that follow the balanced parentheses rule.
        ///     This test is a valid test and should not throw any exception
        ///   </remarks>
        /// </summary>
        [TestMethod]
        public void FormulaConstructor_TestBalancedParentheses_Valid()
        {
            _ = new Formula("((2+2)+5)");
        }

        /// <summary>
        ///   <para>
        ///     Make sure a formula that test inbalanced parantheses and is not accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     The formula "(5+8" is an invalid formula that  does not follow the balanced parentheses rule.
        ///     This test is not a valid test and should throw any exception
        ///   </remarks>
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaConstructor_TestBalancedParantheses_Invalid()
        {
            _ = new Formula("(5+8");
        }
        // --- Tests for First Token Rule ---*
        /// <summary>
        ///   <para>
        ///     Make sure a formula that has opening parantheses and is accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     The formula "(1+1)" is a valid formula that follow the first token rule.
        ///     This test is a valid test and should not throw any exception
        ///   </remarks>
        [TestMethod]
        public void FormulaConstructor_TestFirstTokenOpeningParanthesis_Valid()
        {
            _ = new Formula("(1+1)");
        }


        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaConstructor_TestFirstTokenClosingParanthesis_Invalid()
        {
            _ = new Formula(")0+5");
        }

        /// <summary>
        ///   <para>
        ///     Make sure a simple well formed formula is accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     This is an example of a test that is not expected to throw an exception, i.e., it succeeds.
        ///     In other words, the formula "1+1" is a valid formula which should not cause any errors.
        ///   </remarks>
        /// </summary>
        [TestMethod]
        public void FormulaConstructor_TestFirstTokenNumber_Valid()
        {
            _ = new Formula("1+1");
        }

        /// <summary>
        ///   <para>
        ///     Make sure a formula that does not follow valid token rule and is not accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     The formula "@+5" is a valid formula that does not follow the first token rule.
        ///     This test is a valid test and should throw any exception
        ///   </remarks>
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaConstructor_TestValidToken_Invalid()
        {
            _ = new Formula("@+5");
        }

        /// <summary>
        ///   <para>
        ///     Make sure a formula that does not follow first token rule and is not accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     The formula "@+5" is a valid formula that does not follow the first token rule.
        ///     This test is a valid test and should throw any exception
        ///   </remarks>
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaConstructor_TestFirstToken_Invalid()
        {
            _ = new Formula("+5+3");
        }

        // --- Tests for  Last Token Rule ---
        /// <summary>
        ///   <para>
        ///     Make sure a formula that follow the last token rule and is accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     The formula "2+6" is a valid formula that follow the last token rule.
        ///     This test is a valid test and should not throw any exception
        ///   </remarks>
        [TestMethod]
        public void FormulaConstructor_TestLastTokenNumber_Valid()
        {
            _ = new Formula("2+6");
        }

        /// <summary>
        ///   <para>
        ///     Make sure a formula that follow the last token rule and is accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     The formula "(6+4)" is a valid formula that follow the last token rule.
        ///     This test is a valid test and should not throw any exception
        ///   </remarks>
        [TestMethod]
        public void FormulaConstructor_TestLastTokenClosingParantheses_Valid()
        {
            _ = new Formula("(6+4)");
        }

        /// <summary>
        ///   <para>
        ///     Make sure a formula that follow the last token rule and is accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     The formula "4+a1" is a valid formula that follow the last token rule.
        ///     This test is a valid test and should not throw any exception
        ///   </remarks>
        [TestMethod]
        public void FormulaConstructor_TestLastTokenVariable_Valid()
        {
            _ = new Formula("4+a1");
        }

        /// <summary>
        ///   <para>
        ///     Make sure a formula that does not follow the last token rule and is not accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     The formula "2+@" is an invalid formula that does not follow the last token rule.
        ///     This test is a valid test and should throw any exception
        ///   </remarks>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaConstructor_TestLastToken_Invalid()
        {
            _ = new Formula("2+@");
        }

        /// <summary>
        ///   <para>
        ///     Make sure a formula that does not follow the last token rule and is not accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     The formula "2+7*" is an invalid formula that does not follow the last token rule.
        ///     This test is an invalid test and should throw any exception
        ///   </remarks>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaConstructor_TestLastTokenOperator_Invalid()
        {
            _ = new Formula("2+7*");
        }

        // --- Tests for Parentheses/Operator Following Rule ---
        [TestMethod]
        public void FormulaConstructor_TestFollowingRuleOperator_Valid()
        {
            _ = new Formula("2+(5*6)");
        }

        /// <summary>
        ///   <para>
        ///     Make sure a formula that does not follow the following rule and is not accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     The formula "2+3/" is an invalid formula that does not follow the following rule.
        ///     This test is an invalid test and should throw any exception
        ///   </remarks>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaConstructor_TestFollowingRuleOperator_Invalid()
        {
            _ = new Formula("2+3/");
        }

        /// <summary>
        ///   <para>
        ///     Make sure a formula that follows the following rule and is accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     The formula "((2+5)+3)" is avalid formula that follows the following rule.
        ///     This test is a valid test and should not throw any exception
        ///   </remarks>
        [TestMethod]
        public void FormulaConstructor_TestFollowingRuleParantheses_Valid()
        {
            _ = new Formula("((2+5)+3)");
        }

        /// <summary>
        ///   <para>
        ///     Make sure a formula that does not follow the following rule and is not accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     The formula "2+(+3)" is an invalid formula that does not follow the following rule.
        ///     This test is an invalid test and should throw any exception
        ///   </remarks>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaConstructor_TestFollowingRuleParantheses_Invalid()
        {
            _ = new Formula("2+(+3)");
        }

        // --- Tests for Extra Following Rule ---

        /// <summary>
        ///   <para>
        ///     Make sure a formula that follows the extra following rule and is accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     The formula "(6+5)+5" is avalid formula that follows the extra following rule.
        ///     This test is a valid test and should not throw any exception
        ///   </remarks>
        [TestMethod]
        public void FormulaConstructor_TestExtraFollowingRuleParantheses_Valid()
        {
            _ = new Formula("(6+5)+5");
        }

        /// <summary>
        ///   <para>
        ///     Make sure a formula that follows the extra following rule and is accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     The formula "(2+5)" is avalid formula that follows the extra following rule.
        ///     This test is a valid test and should not throw any exception
        ///   </remarks>
        [TestMethod]
        public void FormulaConstructor_TestFollowingRuleNumber_Valid()
        {
            _ = new Formula("(2+5)");
        }

        /// <summary>
        ///   <para>
        ///     Make sure a formula that follows the extra following rule and is accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     The formula "2+a1+5" is avalid formula that follows the extra following rule.
        ///     This test is a valid test and should not throw any exception
        ///   </remarks>
        [TestMethod]
        public void FormulaConstructor_TestFollowingRuleVariable_Valid()
        {
            _ = new Formula("2+a1+5");
        }

        /// <summary>
        ///   <para>
        ///     Make sure a formula that does not follow the extra following rule and is not accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     The formula "2+5(a1+3)" is avalid formula that does not follow the extra following rule.
        ///     This test is an invalid test and should throw any exception
        ///   </remarks>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaConstructor_TestFollowingRule_Invalid()
        {
            _ = new Formula("2+5(a1+3)");
        }

        /// <summary>
        ///   <para>
        ///     Make sure a formula that does not follow the extra following rule and is not accepted by the constructor (the constructor
        ///     should not throw an exception).
        ///   </para>
        ///   <remarks>
        ///     The formula "2+5 a1" is avalid formula that does not follow the extra following rule.
        ///     This test is an invalid test and should throw any exception
        ///   </remarks>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaConstructor_TestFollowingRuleVariable_Invalid()
        {
            _ = new Formula("2+5 a1");
        }

        // --- Tests for  ToString Method ---
        /// <summary>
        ///   <para>
        ///     Make sure ToString method create the valid result string when formula does not have parentheses
        ///   </para>
        ///   <remarks>
        ///     The formula "(x2 + 6.00)" should become "X2+6" after the ToString method is called
        ///   </remarks>
        [TestMethod]
        public void FormulaToString_TestToStringBasic_Valid()
        {
            Formula f = new Formula("x2 + 6.00");
            Assert.AreEqual("X2+6", f.ToString());
        }

        // --- Tests for  ToString Method ---
        /// <summary>
        ///   <para>
        ///     Make sure ToString method create the valid result string when formula does have parentheses
        ///   </para>
        ///   <remarks>
        ///     The formula "(y2+ 4E3)" should become "(Y2+4000)" after the ToString method is called
        ///   </remarks>
        [TestMethod]
        public void FormulaToString_TestToStringWithParentheses_Valid()
        {
            Formula f = new Formula("(y2+ 4E3)");
            Assert.AreEqual("(Y2+4000)", f.ToString());
        }

        // --- Tests for  ToString Method ---
        /// <summary>
        ///   <para>
        ///     Make sure ToString method create the valid result string when formula does have parentheses
        ///   </para>
        ///   <remarks>
        ///     The formula "(y2+ 4E3)" should become "(Y2+4000)" after the ToString method is called
        ///   </remarks>
        [TestMethod]
        public void FormulaToString_TestToStringWithTwoDifferentFormula_Valid()
        {
            Formula f = new Formula("(y2+ 4E3)");
            Assert.AreEqual("(Y2+4000)", f.ToString());
            f = new Formula("x1+ y2");
            Assert.AreEqual("X1+Y2", f.ToString());
        }

        // --- Tests for  ToString Method ---
        /// <summary>
        ///   <para>
        ///     Make sure ToString method create the valid result string even when the expected result is incorrect
        ///   </para>
        ///   <remarks>
        ///     The formula "(z3 + 500.00)" should not become "(z3 + 500.00)" after the ToString method is called
        ///   </remarks>
        [TestMethod]
        public void FormulaToString_TestToStringWithParentheses_NotValid()
        {
            Formula f = new Formula("(z3 + 500.00)");
            Assert.AreNotEqual("(z3 + 500.00)", f.ToString());
        }

        // --- Tests for  GetVariable Method ---
        /// <summary>
        ///   <para>
        ///     Make sure GetVariable method create the valid result set with all the variables and no repeat by checking correct expectation
        ///   </para>
        ///   <remarks>
        ///     The formula "(z3 + 500.00)" should not become "(z3 + 500.00)" after the ToString method is called
        ///   </remarks>
        [TestMethod]
        public void FormulaGetVariable_TestGetVariable_Valid()
        {
            Formula f = new Formula("(z3 + x1 + y2 + 37 + x1 + y2)");
            ISet<string> x = f.GetVariables();
            Assert.AreEqual(3, x.Count);
            Assert.IsTrue(x.Contains("X1"));
            Assert.IsTrue(x.Contains("Y2"));
            Assert.IsTrue(x.Contains("Z3"));
        }

        // --- Tests for  GetVariable Method ---
        /// <summary>
        ///   <para>
        ///     Make sure GetVariable method create the valid result set with all the variables and no repeat by checking incorrect expectation
        ///   </para>
        ///   <remarks>
        ///     The formula "(z3 + 500.00)" should not become "(z3 + 500.00)" after the ToString method is called
        ///   </remarks>
        [TestMethod]
        public void FormulaGetVariable_TestGetVariable_NotValid()
        {
            Formula f = new Formula("(z3 + x1 + y2 + 37 + x1 + y2)");
            ISet<string> x = f.GetVariables();
            Assert.AreNotEqual(6, x.Count);
            Assert.IsFalse(x.Contains("x1")); //x1 should be normalized to X1
            Assert.IsFalse(x.Contains("y2")); //y2 should be normalized to Y2
            Assert.IsFalse(x.Contains("z3")); //z3 should be normalized to Z3
            Assert.IsFalse(x.Contains("37")); //There should be no number in variables
        }

        // --- Tests for  GetVariable Method ---
        /// <summary>
        ///   <para>
        ///     Make sure Equals method correctly compared the given sets by comparing their ToStringResult
        ///   </para>
        ///   <remarks>
        ///     The formula "(z3 + x1 + y2 + 37 + x1 + y2)" should be equals to  "(Z3 + X1 + Y2 + 37 + X1 + Y2)" and should return true
        ///   </remarks>
        [TestMethod]
        public void FormulaEquals_TestEquals_Valid()
        {
            Formula f = new Formula("(z3 + x1 + y2 + 37 + x1 + y2)");
            Formula f2 = new Formula("(Z3 + X1 + Y2 + 37 + X1 + Y2)");
            Assert.IsTrue(f.Equals(f2));
        }

        // --- Tests for  GetVariable Method ---
        /// <summary>
        ///   <para>
        ///     Make sure Equals method correctly compared the given sets by comparing their ToStringResult
        ///   </para>
        ///   <remarks>
        ///     The formula "(z3 + x1 + y2 + 37 + x1 + y2)" should not be equals to  "(Z3 + X1 + Y2 + 37 + X1 + Y2)" and should return false
        ///   </remarks>
        [TestMethod]
        public void FormulaEquals_TestEquals_NotValid()
        {
            Formula f = new Formula("(z3 + x1 + y2 + 37 + x1 + y2)");
            Formula f2 = new Formula("(z3 + x1 + y2  + x1 + y2 + 37)");
            Assert.IsFalse(f.Equals(f2));
        }

        // --- Tests for  == Operators ---
        /// <summary>
        ///   <para>
        ///     Make sure == operator correctly compared the given sets by calling Equals
        ///   </para>
        ///   <remarks>
        ///     The formula "(z3 + x1 + y2 + 37 + x1 + y2)" should not be == to  "(Z3 + X1 + Y2 + 37 + X1 + Y2)" and should return false
        ///   </remarks>
        [TestMethod]
        public void FormulaOperatorEquals_TestOperator_NotValid()
        {
            Formula f = new Formula("(z3 + x1 + y2 + 37 + x1 + y2)");
            Formula f2 = new Formula("(z3 + x1 + y2  + x1 + y2 + 37)");
            Assert.IsFalse(f == f2);
        }

        // --- Tests for  == Operators ---
        /// <summary>
        ///   <para>
        ///     Make sure == operator correctly compared the given sets by calling Equals
        ///   </para>
        ///   <remarks>
        ///     The formula "(z3 + x1 + y2)" should be == to  "(Z3 + X1 + Y2)" and should return true
        ///   </remarks>
        [TestMethod]
        public void FormulaOperatorEquals_TestOperator_Valid()
        {
            Formula f = new Formula("(z3 + x1 + y2)");
            Formula f2 = new Formula("(Z3 + X1 + Y2)");
            Assert.IsTrue(f == f2);
        }

        // --- Tests for  != Operators ---
        /// <summary>
        ///   <para>
        ///     Make sure != operator correctly compared the given sets by calling Equals
        ///   </para>
        ///   <remarks>
        ///     The formula "(z3 + x1 + y2)" should be != to  "(Z3 + X1 + Y2)" and should return true
        ///   </remarks>
        [TestMethod]
        public void FormulaOperatorNotEquals_TestOperator_Valid()
        {
            Formula f = new Formula("(z3 + x1 + y2 + 5)");
            Formula f2 = new Formula("(Z3 + X1 + Y2+ 25)");
            Assert.IsTrue(f != f2);
        }

        // --- Tests for  != Operators ---
        /// <summary>
        ///   <para>
        ///     Make sure != operator correctly compared the given sets by calling Equals
        ///   </para>
        ///   <remarks>
        ///     The formula "(z3 + x1 + y2)" should be != to  "(Z3 + X1 + Y2)" and should return false
        ///   </remarks>
        [TestMethod]
        public void FormulaOperatorNotEquals_TestOperator_NotValid()
        {
            Formula f = new Formula("(z3 + x1 + y2)");
            Formula f2 = new Formula("(Z3 + X1 + Y2)");
            Assert.IsFalse(f != f2);
        }
        /// <summary>
        ///   <para>
        ///     Helper method to pass as delegate for tests
        ///   </para>
        ///   <remarks>
        ///     Input for parameters of Evaluate
        ///   </remarks>
        private double SimpleLookUp(string s)
        {
            return 0;
        }

        /// <summary>
        ///   <para>
        ///     Helper method to pass as delegate for tests that invoke Formula Error
        ///   </para>
        ///   <remarks>
        ///     Input for parameters of Evaluate
        ///   </remarks>
        private double ErrorLookUp(string s)
        {
            if(s.GetType() == s.GetType()) { throw new ArgumentException(); }
            
            return 0;
        }

        // --- Tests for Evaluate Method ---
        /// <summary>
        ///   <para>
        ///     Make sure the formula is correctly evaluated the given sets 
        ///   </para>
        ///   <remarks>
        ///     The formula "(8*2*3)" should return 48 and which is true
        ///   </remarks>
        [TestMethod]
        public void FormulaEvalute_TestEvaluateMultiply_Valid()
        {
            Formula f = new Formula("(8*2*3)");
            Assert.AreEqual((double)48, f.Evaluate(SimpleLookUp));           
        }

        // --- Tests for Evaluate Method ---
        /// <summary>
        ///   <para>
        ///     Make sure the formula is correctly evaluated the given sets 
        ///   </para>
        ///   <remarks>
        ///     The formula "8+4/2" should return 10 and which is true
        ///   </remarks>
        [TestMethod]
        public void FormulaEvalute_TestEvaluateAdditionWithDivision_Valid()
        {
            Formula f = new Formula("8+4/2");
            Assert.AreEqual((double)10, f.Evaluate(SimpleLookUp));
        }

        // --- Tests for Evaluate Method ---
        /// <summary>
        ///   <para>
        ///     Make sure the formula is correctly evaluated the given sets 
        ///   </para>
        ///   <remarks>
        ///     The formula "10+(5/2)" should return 12.5 and which is true
        ///   </remarks>
        [TestMethod]
        public void FormulaEvalute_TestEvaluateAdditionWithDivisionInParentheses_Valid()
        {
            Formula f = new Formula("10+(5/2)");
            Assert.AreEqual(12.5, f.Evaluate(SimpleLookUp));
        }

        // --- Tests for Evaluate Method ---
        /// <summary>
        ///   <para>
        ///     Make sure the formula is correctly evaluated the given sets 
        ///   </para>
        ///   <remarks>
        ///     The formula "(8*2*3)+15" should return 63 and which is true
        ///   </remarks>
        [TestMethod]
        public void FormulaEvalute_TestEvaluateMultiplyThenAddition_Valid()
        {
            Formula f = new Formula("(8*2*3)+15");
            Assert.AreEqual((double)63, f.Evaluate(SimpleLookUp));
        }

        // --- Tests for Evaluate Method ---
        /// <summary>
        ///   <para>
        ///     Make sure the formula is correctly using delegate 
        ///   </para>
        ///   <remarks>
        ///     The formula "A3" should return 0 because delegate is instructed to return 0 for any variable
        ///   </remarks>
        [TestMethod]
        public void FormulaEvalute_TestEvaluateLookUp_Valid()
        {
            Formula f = new Formula("A3");
            Assert.AreEqual((double)0, f.Evaluate(SimpleLookUp));
        }

        // --- Tests for Evaluate Method ---
        /// <summary>
        ///   <para>
        ///     Make sure the formula is correctly using delegate 
        ///   </para>
        ///   <remarks>
        ///     The formula "A3" should not return 2 because delegate is instructed to return 0 for any variable
        ///   </remarks>
        [TestMethod]
        public void FormulaEvalute_TestEvaluateLookUp_NotValid()
        {
            Formula f = new Formula("A3");
            Assert.AreNotEqual((double) 2, f.Evaluate(SimpleLookUp));
        }

        // --- Tests for Evaluate Method ---
        /// <summary>
        ///   <para>
        ///     Make sure Evaluator return Formula Error if delegate class throw an Argument Exception
        ///   </para>
        ///   <remarks>
        ///     The test should return true as we are expecting Evaluate to return an error after delegate class throw an Argument Exception
        ///   </remarks>
        [TestMethod]
        public void FormulaEvalute_TestEvaluateErrorLookUp_Valid()
        {
            Formula f = new Formula("A3+10+20");
            Assert.AreEqual((new FormulaError("")).GetType(), f.Evaluate(ErrorLookUp).GetType());
        }

        // --- Tests for Evaluate Method ---
        /// <summary>
        ///   <para>
        ///     Make sure Evaluator return Formula Error if delegate class throw an Argument Exception
        ///   </para>
        ///   <remarks>
        ///     The test should return true as we are expecting Evaluate to return an error after delegate class throw an Argument Exception
        ///   </remarks>
        [TestMethod]
        public void FormulaEvalute_TestEvaluateAdditionWithLookUpVariable_Valid()
        {
            Formula f = new Formula("A3+10+20");
            Assert.AreEqual((double) 30, f.Evaluate(SimpleLookUp));
        }

        // --- Tests for Evaluate Method ---
        /// <summary>
        ///   <para>
        ///     Make sure the formula is correctly evaluated the given sets 
        ///   </para>
        ///   <remarks>
        ///     The formula "(80+20-50+150)" should return 200 and which is true
        ///   </remarks>
        [TestMethod]
        public void FormulaEvalute_TestEvaluateAdditionAndSubtraction_Valid()
        {
            Formula f = new Formula("(80+20-50+150)");
            Assert.AreEqual((double)200, f.Evaluate(SimpleLookUp));
        }

        // --- Tests for Evaluate Method ---
        /// <summary>
        ///   <para>
        ///     Make sure the formula is correctly evaluated the given sets 
        ///   </para>
        ///   <remarks>
        ///     The formula "10*(8*2*5)" should return 800 which is 
        ///   </remarks>
        [TestMethod]
        public void FormulaEvalute_TestEvaluateAdditionWithMultiply_Valid()
        {
            Formula f = new Formula("10*(8*2*5)");
            Assert.AreEqual((double)800, f.Evaluate(SimpleLookUp));
        }

        // --- Tests for FormulaError Class ---
        /// <summary>
        ///   <para>
        ///     Make sure Evaluate correctly return the formula error when dividing by zero
        ///   </para>
        ///   <remarks>
        ///     The formula "10/0" should make evaluate return FormulaError("Divided by Zero") and type Formula Error
        ///   </remarks>
        [TestMethod]
        public void FormulaError_TestEvaluateDivisionByZero_Valid()
        {
            Formula f = new Formula("10/0");
            FormulaError x = (FormulaError)f.Evaluate(SimpleLookUp);
            Assert.IsTrue(new FormulaError("").GetType() == x.GetType()); // Check for similar type
            Assert.AreEqual(new FormulaError("Divided by zero").Reason, x.Reason); // Check for similar reason
        }

        // --- Tests for GetHashCode Method ---
        /// <summary>
        ///   <para>
        ///     Make sure GetHashCode produce the correct HashCode
        ///   </para>
        ///   <remarks>
        ///     HashCode of (ToString()+ tokens[0] + tokens[tokens.Count - 1]).GetHashCode() * tokens.Count
        ///   </remarks>
        [TestMethod]
        public void FormulaGetHashCode_TestGetHashCode_Valid()
        {
            Formula f = new Formula("10/10");
            Assert.AreEqual(("10/101010").GetHashCode() * 3, f.GetHashCode());
        }

        // --- Tests for GetHashCode Method ---
        /// <summary>
        ///   <para>
        ///     Make sure GetHashCode produce the correct HashCode
        ///   </para>
        ///   <remarks>
        ///     HashCode of (ToString()+ tokens[0] + tokens[tokens.Count - 1]).GetHashCode() * tokens.Count
        ///   </remarks>
        [TestMethod]
        public void FormulaGetHashCode_TestGetHashCode_NotValid()
        {
            Formula f = new Formula("10/10");
            Assert.AreNotEqual(("110/101010").GetHashCode() * 3, f.GetHashCode());
        }

        // --- Tests for GetHashCode Method ---
        /// <summary>
        ///   <para>
        ///     Make sure a normalized and not normalized version of the same formula produce the same hashcode
        ///   </para>
        ///   <remarks>
        ///     The formula "A1+B2+C3+D4" hash code should be equal to  "a1+b2+c3+d4" hash code and should return true
        ///   </remarks>
        [TestMethod]
        public void FormulaGetHashCode_TestCompareHashCode_Equal()
        {
            Formula f = new Formula("A1+B2+C3+D4");
            int x = f.GetHashCode();
            f = new Formula("a1+b2+c3+d4");
            Assert.AreEqual(x, f.GetHashCode());
        }

        // --- Tests for GetHashCode Method ---
        /// <summary>
        ///   <para>
        ///     Make sure the formula produce the different hashcode even when they have the same result since c3 and d4 are in opposite location
        ///   </para>
        ///   <remarks>
        ///     The formula "A1+B2+C3+D4" hash code should not be equal to  "a1+b2+d4+c3" hash code and should return true
        ///   </remarks>
        [TestMethod]
        public void FormulaGetHashCode_TestCompareHashCode_NotEqual()
        {
            Formula f = new Formula("A1+B2+C3+D4");
            int x = f.GetHashCode();
            f = new Formula("a1+b2+d4+c3");
            Assert.AreNotEqual(x, f.GetHashCode());
        }
    }
}