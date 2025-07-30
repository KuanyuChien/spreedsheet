// <copyright file="Formula_PS2.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <summary>
//   <para>
//     This code is provides to start your assignment.  It was written
//     by Profs Joe, Danny, and Jim.  You should keep this attribution
//     at the top of your code where you have your header comment, along
//     with the other required information.
//   </para>
//   <para>
//     You should remove/add/adjust comments in your file as appropriate
//     to represent your work and any changes you make.
//   </para>
// <authors> [Kuanyu Chien] </authors>
// <date> [2024/09/20] </date>
// </summary>


namespace CS3500.Formula;

using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

static class StackExtension
{
    public static bool IsOnTop(this Stack<string> s, string op)
    {
        return s.Count > 0 && op == s.Peek();
    }
}

/// <summary>
///   <para>
///     This class represents formulas written in standard infix notation using standard precedence
///     rules.  The allowed symbols are non-negative numbers written using double-precision
///     floating-point syntax; variables that consist of one ore more letters followed by
///     one or more numbers; parentheses; and the four operator symbols +, -, *, and /.
///   </para>
///   <para>
///     Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
///     a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
///     and "x 23" consists of a variable "x" and a number "23".  Otherwise, spaces are to be removed.
///   </para>
///   <para>
///     For Assignment Two, you are to implement the following functionality:
///   </para>
///   <list type="bullet">
///     <item>
///        Formula Constructor which checks the syntax of a formula.
///     </item>
///     <item>
///        Get Variables
///     </item>
///     <item>
///        ToString
///     </item>
///   </list>
/// </summary>
public class Formula
{
    public delegate double Lookup(string variableName);
    /// <summary>
    ///   All variables are letters followed by numbers.  This pattern
    ///   represents valid variable name strings.
    /// </summary>
    private const string VariableRegExPattern = @"[a-zA-Z]+\d+";
    /// <summary>
    ///   All scientific notation one number followed by E or e followed by another number. This pattern
    ///   represents valid scientific notation strings.
    /// </summary>
    private const string NumberRegExPattern = @"[+-]?(\d+(\.\d*)?|\.\d+)[eE][+-]?\d+";
    /// <summary>
    ///   This variable record every variables when constructor check if variable is a valid variable.
    /// </summary>
    private HashSet<string> Variables = new HashSet<string>();
    /// <summary>
    ///   This variable record every token when constructor check if token is a valid token.
    /// </summary>
    private ArrayList ValidTokens = new ArrayList();



    /// <summary>
    ///   Initializes a new instance of the <see cref="Formula"/> class.
    ///   <para>
    ///     Creates a Formula from a string that consists of an infix expression written as
    ///     described in the class comment.  If the expression is syntactically incorrect,
    ///     throws a FormulaFormatException with an explanatory Message.  See the assignment
    ///     specifications for the syntax rules you are to implement.
    ///   </para>
    ///   <para>
    ///     Non Exhaustive Example Errors:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///        Invalid variable name, e.g., x, x1x  (Note: x1 is valid, but would be normalized to X1)
    ///     </item>
    ///     <item>
    ///        Empty formula, e.g., string.Empty
    ///     </item>
    ///     <item>
    ///        Mismatched Parentheses, e.g., "(("
    ///     </item>
    ///     <item>
    ///        Invalid Following Rule, e.g., "2x+5"
    ///     </item>
    ///   </list>
    /// </summary>
    /// <param name="formula"> The string representation of the formula to be created.</param>
    public Formula(string formula)
    {
        List<string> Tokens = GetTokens(formula);

        if (Tokens.Count == 0)
        {
            throw new FormulaFormatException("Invalied input. There must be at least one token.");
        }
        string prevToken = "";
        int openParenCount = 0;
        int closeParenCount = 0;

        if (!(IsVar(Tokens[0]) || IsNum(Tokens[0]) || Tokens[0].Equals("(")))
        {
            throw new FormulaFormatException("Invalied input. Violate first token rule.");
        }

        foreach (string token in Tokens)
        {

            if (IsVar(token))
            {
                IsPrevNumVarOrClosingParen(prevToken);

                Variables.Add(token.ToUpper());
            }
            else if (IsNum(token))
            {
                IsPrevNumVarOrClosingParen(prevToken);
            }
            else if (token.Equals("("))
            {
                IsPrevNumVarOrClosingParen(prevToken);
                openParenCount++;
            }
            else if (token.Equals(")"))
            {
                closeParenCount++;
                if (closeParenCount > openParenCount) throw new FormulaFormatException("Invalied input. Violate closing parentheses rule.");

                IsPrevOperatorOrOpeningParen(prevToken);
            }
            else if (token.Equals("+") || token.Equals("-") || token.Equals("*") || token.Equals("/"))
            {
                IsPrevOperatorOrOpeningParen(prevToken);
            }
            else
            {
                throw new FormulaFormatException("Invalied input. Violate valid token rule.");
            }

            prevToken = token;
            ValidTokens.Add(prevToken.ToUpper());
        }

        if (closeParenCount != openParenCount) throw new FormulaFormatException("Invalied input. Violate balanced parentheses rule.");

        if (!(IsVar(prevToken) || IsNum(prevToken) || prevToken.Equals(")")))
        {
            throw new FormulaFormatException("Invalied input. Violate last token rule.");
        }
    }

    /// <summary>
    /// Check if the closing parenthesis or operators' previous token is operator or opening parenthesis. 
    /// If it's that means the formula vioalte parenthesis/operator following rule. 
    /// </summary>
    /// <param name="previousToken"> previous token in formula</param>
    /// <exception cref="FormulaFormatException">throw formula format exception when formula break parenthesis/operator following rule</exception>
    private void IsPrevOperatorOrOpeningParen(string previousToken)
    {
        if (previousToken.Equals("(") || previousToken.Equals("+") || previousToken.Equals("-") || previousToken.Equals("*") || previousToken.Equals("/"))
        {
            throw new FormulaFormatException("Invalied input. Violate Parenthesis/Operator following rule.");
        }

    }

    /// <summary>
    /// Check if the number, variable or opening parenthesis' previous token is number, variable or closing parenthesis. 
    /// If it's that means the formula vioalte extra following rule.
    /// </summary>
    /// <param name="previousToken"> previous token in formula </param>
    /// <exception cref="FormulaFormatException">throw formula format exception when formula break parenthesis/operator following rule</exception>
    private void IsPrevNumVarOrClosingParen(string previousToken)
    {

        if (IsNum(previousToken) || IsVar(previousToken) || previousToken.Equals(")"))
        {
            throw new FormulaFormatException("Invalied input. Violate Extra following rule.");
        }

    }

    /// <summary>
    /// helper method that check if the token is a number or not.
    /// </summary>
    /// <param name="token">the token to be checked if the token i a number or not</param>
    /// <returns>true if the token is be considered a valied number. false if not </returns>
    private static bool IsNum(string token)
    {
        int parsedInt;
        double parsedDouble;
        if (int.TryParse(token, out parsedInt) || double.TryParse(token, out parsedDouble))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///   <para>
    ///     Returns a set of all the variables in the formula.
    ///   </para>
    ///   <remarks>
    ///     Important: no variable may appear more than once in the returned set, even
    ///     if it is used more than once in the Formula.
	///     Variables should be returned in canonical form, having all letters converted
	///     to uppercase.
    ///   </remarks>
    ///   <list type="bullet">
    ///     <item>new("x1+y1*z1").GetVariables() should return a set containing "X1", "Y1", and "Z1".</item>
    ///     <item>new("x1+X1"   ).GetVariables() should return a set containing "X1".</item>
    ///   </list>
    /// </summary>
    /// <returns> the set of variables (string names) representing the variables referenced by the formula. </returns>
    public ISet<string> GetVariables()
    {
        return Variables;
    }

    /// <summary>
    ///   <para>
    ///     Returns a string representation of a canonical form of the formula.
    ///   </para>
    ///   <para>
    ///     The string will contain no spaces.
    ///   </para>
    ///   <para>
    ///     If the string is passed to the Formula constructor, the new Formula f 
    ///     will be such that this.ToString() == f.ToString().
    ///   </para>
    ///   <para>
    ///     All of the variables in the string will be normalized.  This
    ///     means capital letters.
    ///   </para>
    ///   <para>
    ///       For example:
    ///   </para>
    ///   <code>
    ///       new("x1 + y1").ToString() should return "X1+Y1"
    ///       new("X1 + 5.0000").ToString() should return "X1+5".
    ///   </code>
    ///   <para>
    ///     This code should execute in O(1) time.
    ///   <para>
    /// </summary>
    /// <returns>
    ///   A canonical version (string) of the formula. All "equal" formulas
    ///   should have the same value here.
    /// </returns>
    public override string ToString()
    {
        string result = "";
        foreach (string token in ValidTokens)
        {
            if (IsNum(token))
            {
                int numIntValue;
                Double numDoubleValue;
                if (int.TryParse(token, out numIntValue))
                {
                    result += numIntValue;
                }
                else if (Double.TryParse(token, out numDoubleValue))
                {
                    result += numDoubleValue;
                }

            }
            else if (IsVar(token))
            {
                result += token.ToUpper();
            }
            else
            {
                result += token;
            }
        }
        return result;
    }

    /// <summary>
    ///   Reports whether "token" is a variable.  It must be one or more letters
    ///   followed by one or more numbers.
    /// </summary>
    /// <param name="token"> A token that may be a variable. </param>
    /// <returns> true if the string matches the requirements, e.g., A1 or a1. </returns>
    private static bool IsVar(string token)
    {
        // notice the use of ^ and $ to denote that the entire string being matched is just the variable
        string standaloneVarPattern = $"^{VariableRegExPattern}$";
        return Regex.IsMatch(token, standaloneVarPattern);
    }

    /// <summary>
    ///   <para>
    ///     Given an expression, enumerates the tokens that compose it.
    ///   </para>
    ///   <para>
    ///     Tokens returned are:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>left paren</item>
    ///     <item>right paren</item>
    ///     <item>one of the four operator symbols</item>
    ///     <item>a string consisting of one or more letters followed by one or more numbers</item>
    ///     <item>a double literal</item>
    ///     <item>and anything that doesn't match one of the above patterns</item>
    ///   </list>
    ///   <para>
    ///     There are no empty tokens; white space is ignored (except to separate other tokens).
    ///   </para>
    /// </summary>
    /// <param name="formula"> A string representing an infix formula such as 1*B1/3.0. </param>
    /// <returns> The ordered list of tokens in the formula. </returns>
    private static List<string> GetTokens(string formula)
    {
        List<string> results = [];

        string lpPattern = @"\(";
        string rpPattern = @"\)";
        string opPattern = @"[\+\-*/]";
        string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
        string spacePattern = @"\s+";

        // Overall pattern
        string pattern = string.Format(
                                        "({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                        lpPattern,
                                        rpPattern,
                                        opPattern,
                                        VariableRegExPattern,
                                        doublePattern,
                                        spacePattern);

        // Enumerate matching tokens that don't consist solely of white space.
        foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
        {
            if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
            {
                results.Add(s);
            }
        }

        return results;
    }

    /// <summary>
    ///   <para>
    ///     Evaluates this Formula, using the lookup delegate to determine the values of
    ///     variables.
    ///   </para>
    ///   <remarks>
    ///     When the lookup method is called, it will always be passed a normalized (capitalized)
    ///     variable name.  The lookup method will throw an ArgumentException if there is
    ///     not a definition for that variable token.
    ///   </remarks>
    ///   <para>
    ///     If no undefined variables or divisions by zero are encountered when evaluating
    ///     this Formula, the numeric value of the formula is returned.  Otherwise, a 
    ///     FormulaError is returned (with a meaningful explanation as the Reason property).
    ///   </para>
    ///   <para>
    ///     This method should never throw an exception.
    ///   </para>
    /// </summary>
    /// <param name="lookup">
    ///   <para>
    ///     Given a variable symbol as its parameter, lookup returns the variable's value
    ///     (if it has one) or throws an ArgumentException (otherwise).  This method will expect 
    ///     variable names to be normalized.
    ///   </para>
    /// </param>
    /// <returns> Either a double or a FormulaError, based on evaluating the formula.</returns>


    public object Evaluate(Lookup lookup)
    {
        Stack<string> valueStack = new Stack<string>();
        Stack<string> operatorStack = new Stack<string>();

        foreach (string token in ValidTokens)
        {
            if (IsNum(token) || IsVar(token))
            {
                double tokenValue;
                if (IsNum(token))
                {
                    tokenValue = double.Parse(token);
                }
                else
                {
                    try { tokenValue = lookup(token); }
                    catch (ArgumentException)
                    {
                        return new FormulaError("Can't find the variable value");
                    }
                }

                if (operatorStack.IsOnTop("*") || operatorStack.IsOnTop("/"))
                {
                    double pushValue;

                    if (!TryCalculateDivideByZero(tokenValue, double.Parse(valueStack.Pop()), operatorStack.Pop(), out pushValue))
                    {
                        FormulaError er = new("");
                        return er;
                    }

                    valueStack.Push(pushValue.ToString());
                }
                else
                {
                    valueStack.Push(tokenValue.ToString());
                }
            }

            else if (token.Equals("+") || token.Equals("-"))
            {
                if (operatorStack.IsOnTop("+") || operatorStack.IsOnTop("-"))
                {
                    TryCalculateDivideByZero(double.Parse(valueStack.Pop()), double.Parse(valueStack.Pop()), operatorStack.Pop(), out double pushValue);
                    valueStack.Push(pushValue.ToString());
                }
                operatorStack.Push(token);
            }

            else if (token.Equals("*") || token.Equals("/") || token.Equals("("))
            {
                operatorStack.Push(token);
            }

            else
            {
                if (operatorStack.IsOnTop("+") || operatorStack.IsOnTop("-"))
                {
                    TryCalculateDivideByZero(double.Parse(valueStack.Pop()), double.Parse(valueStack.Pop()), operatorStack.Pop(), out double pushValue);
                    valueStack.Push(pushValue.ToString());

                    operatorStack.Pop();  // pop extra "("
                }
                if (operatorStack.IsOnTop("*") || operatorStack.IsOnTop("/"))
                {
                    if (!TryCalculateDivideByZero(double.Parse(valueStack.Pop()), double.Parse(valueStack.Pop()), operatorStack.Pop(), out double pushValue))
                    {
                        FormulaError er = new("");
                        return er;
                    }
                    valueStack.Push(pushValue.ToString());
                }

            }
        }

        if (operatorStack.Count == 0)
        {
            return double.Parse(valueStack.Pop());
        }
        else
        {
            while (operatorStack.Peek() == "(")
            {
                operatorStack.Pop();
            }

            TryCalculateDivideByZero(double.Parse(valueStack.Pop()), double.Parse(valueStack.Pop()), operatorStack.Pop(), out double pushValue);
            return pushValue;
        }
    }

    /// <summary>
    /// This helper method try to calculate the result of the formula.
    /// When it's divide by zero, it return fasle. Otherwise, true.
    /// It use out key word, so it also return the calculated result.
    /// </summary>
    /// <param name="a">first double number in the foumula</param>
    /// <param name="b">second double number in the foumula</param>
    /// <param name="op">the operator in the formula</param>
    /// <param name="output">output of the evaluation</param>
    /// <returns>false if is divide by zero, true otherwise.</returns>
    private bool TryCalculateDivideByZero(double a, double b, string op, out double output)
    {
        if (op == "+") { output = b + a; return true; }
        else if (op == "-") { output = b - a; return true; }
        else if (op == "*") { output = b * a; return true; }
        else
        {
            if (a == 0.0)
            {
                output = 0.0;
                return false;
            }
            output = b / a;
            return true;
        }
    }


    /// <summary>
    ///   <para>
    ///     Determines if two formula objects represent the same formula.
    ///   </para>
    ///   <para>
    ///     By definition, if the parameter is null or does not reference 
    ///     a Formula Object then return false.
    ///   </para>
    ///   <para>
    ///     Two Formulas are considered equal if their canonical string representations
    ///     (as defined by ToString) are equal.  
    ///   </para>
    /// </summary>
    /// <param name="obj"> The other object.</param>
    /// <returns>
    ///   True if the two objects represent the same formula.
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Formula other) return false;
        other = (Formula)obj;
        return this.ToString().Equals(other.ToString());
    }

    /// <summary>
    ///   <para>
    ///     Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
    ///     case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two
    ///     randomly-generated unequal Formulas have the same hash code should be extremely small.
    ///   </para>
    /// </summary>
    /// <returns> The hashcode for the object. </returns>
    public override int GetHashCode()
    {
        return this.ToString().GetHashCode();
    }


    /// <summary>
    ///   <para>
    ///     Reports whether f1 == f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are the same.</returns>
    public static bool operator ==(Formula f1, Formula f2)
    {
        return f1.Equals(f2);
    }

    /// <summary>
    ///   <para>
    ///     Reports whether f1 != f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are not equal to each other.</returns>
    public static bool operator !=(Formula f1, Formula f2)
    {
        return !f1.Equals(f2);
    }

}


/// <summary>
///   Used to report syntax errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaFormatException"/> class.
    ///   <para>
    ///      Constructs a FormulaFormatException containing the explanatory message.
    ///   </para>
    /// </summary>
    /// <param name="message"> A developer defined message describing why the exception occured.</param>
    public FormulaFormatException(string message)
        : base(message)
    {
        // All this does is call the base constructor. No extra code needed.
    }
}


/// <summary>
/// Used as a possible return value of the Formula.Evaluate method.
/// </summary>
public class FormulaError
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaError"/> class.
    ///   <para>
    ///     Constructs a FormulaError containing the explanatory reason.
    ///   </para>
    /// </summary>
    /// <param name="message"> Contains a message for why the error occurred.</param>
    public FormulaError(string message)
    {
        Reason = message;
    }

    /// <summary>
    ///  Gets the reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
}