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
// </summary>
// Co-Author: Hung Nguyen, Date: 9/22/2024, Course: CS 3500
namespace CS3500.Formula;

using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

/// <summary>
///   <para>
///     This class represents formulas written in standard infix notation using standard precedence
///     rules.  The allowed symbols are non-negative numbers written using double-precision
///     floating-point syntax; variables that consist of one ore more letters followed by
///     one or more numbers; parentheses; and the four operator symbols +, -, *, and /.
///   </para>
///   <para>
///     Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
///     a single variable, "tokens y" consists of two variables "tokens" and y; "x23" is a single variable;
///     and "tokens 23" consists of a variable "tokens" and a number "23".  Otherwise, spaces are to be removed.
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
    /// <summary>
    ///   All variables are letters followed by numbers.  This pattern
    ///   represents valid variable name strings.
    /// </summary>
    private const string VariableRegExPattern = @"[a-zA-Z]+\d+";
    private List<string> tokens; // List of tokens to keep track of the formula
    private string ToStringResult; // String that store the result of ToString

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
    ///        Invalid variable name, e.g., tokens, x1x  (Note: x1 is valid, but would be normalized to X1)
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
        tokens = GetTokens(formula);
        bool PreviousIsNumber = false;  // Bool to check if previous token is a number
        bool IsNumber = false;          // Bool to check if token is number
        int OpenParenthesesCount = 0;   // Total count of "("
        int CloseParenthesesCount = 0;  // Total count of ")"
        ToStringResult = "";


        bool isOp = false;

        // Check if formula follows one token rule (Rule 1)
        if (tokens.Count <= 0) { throw new FormulaFormatException("Formula have to have at least 1 token"); }
        // Check if formula follows first token rule (Rule 5)
        if (!IsVar(tokens[0]) && !Double.TryParse(tokens[0],out _) && tokens[0] != "(")
        {
            throw new FormulaFormatException("Formula can start with a number, variable or open parentheses");
        }
        // Check if formula follows last token rule(Rule 6)
        if (IsOperator(tokens[tokens.Count-1]) || tokens[tokens.Count - 1] == "(")
        {
            throw new FormulaFormatException("Formula can only end with a number, variable or close parentheses");
        }

        for (int i =0; i < tokens.Count; i++)
        {
            //Setup formula variables and counters
            isOp = IsOperator(tokens[i]);
            //Check if the token is a number, parse it and replace it in the current list of tokens
            if (Double.TryParse(tokens[i], out double n)) { IsNumber = true; tokens[i] = n.ToString(); } else { IsNumber = false; }
            if (tokens[i] == "(") { OpenParenthesesCount++; } else if (tokens[i] == ")") { CloseParenthesesCount++; }

            // Check if formula follows valid token rule (Rule 2)
            if (tokens[i] != "(" && tokens[i] != ")" && !isOp && !IsNumber && !IsVar(tokens[i])) 
            { 
                throw new FormulaFormatException("All tokens in the formula must be valid"); 
            }
            // Check if formula follows closing parentheses rule (Rule 3)
            if (CloseParenthesesCount > OpenParenthesesCount) 
            { 
                throw new FormulaFormatException("There cannot be more close parentheses than open parentheses at any point in the formula"); 
            }
            
            // Check if formula follows following rule equation (Rule 7)
            if ( i > 0 && (tokens[i - 1] == "(" || IsOperator(tokens[i-1])) && (!IsVar(tokens[i]) && !IsNumber && tokens[i] != "(" )) 
            { 
                throw new FormulaFormatException("After an open parentheses, only a number, variable or another variable is allowed"); 
            }
            //Check if formula follows extra following rule (Rule 8)
            if ((PreviousIsNumber && tokens[i] != ")" && !isOp)||(i+1 < tokens.Count && tokens[i] == ")" && !IsOperator(tokens[i+1]) && tokens[i+1]!=")")) 
            { 
                throw new FormulaFormatException("After a number must be an operator or close parentheses"); 
            } 
            if(IsNumber) { PreviousIsNumber=true; } else { PreviousIsNumber = false; }
            tokens[i] = tokens[i].ToUpper(); //Normalize every token that pass through the method, as directed by 
            ToStringResult += tokens[i];
        }
        // Check if formula follow balanced parentheses rule (Rule 4)
        if (OpenParenthesesCount != CloseParenthesesCount) 
        { 
            throw new FormulaFormatException("Number of parentheses is uneven"); 
        }   
    }

    /// <summary>
    /// Helper method Return a boolean that is the result of a check if character is an operator or not
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    private bool IsOperator(string token)
    {
        //Check if string token is any of these operators, return true if correct
        if (token == "-" || token == "+" || token == "/" || token == "*") { return true; } else { return false; } 
    }
    /// <summary>
    ///   <para>
    ///     Returns a set of all the variables in the formula.
    ///   </para>
    ///   <remarks>
    ///     Important: no variable may appear more than once in the returned set, even
    ///     if it is used more than once in the Formula.
    ///   </remarks>
    ///   <para>
    ///     For example, if N is a method that converts all the letters in a string to upper case:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>new("x1+y1*z1").GetVariables() should enumerate "X1", "Y1", and "Z1".</item>
    ///     <item>new("x1+X1"   ).GetVariables() should enumerate "X1".</item>
    ///   </list>
    /// </summary>
    /// <returns> the set of variables (string names) representing the variables referenced by the formula. </returns>
    public ISet<string> GetVariables()
    {
        ISet<string> variables = new HashSet<string>();
        foreach (string token in tokens) 
        {
            if (IsVar(token) && !variables.Contains(token)) { variables.Add(token); }
        }
        return variables;
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
        return ToStringResult;
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
        if (obj == null) { return false; }
        else
        {
            if (!(obj is Formula)) { return false; }
            else
            {
                return obj.ToString() == this.ToString();
            }
        }
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
        Stack<double> ValueStack = new Stack<double> ();
        Stack<string> OperatorStack = new Stack<string> ();
        double n = 0;
        bool DividedByZero = false;
        foreach (string token in tokens) 
        {
            if (DividedByZero) { return new FormulaError("Divided by zero"); }
            if (token == "(")                   // Check if token is (
            {
                OperatorStack.Push(token);      // Push ( into OperatorStack
                continue;                       // Move on to next token
            }
            else if (token == ")")
            {
                if (OperatorStack.Peek() == "+" || OperatorStack.Peek() == "-")
                {
                    Calculate(ValueStack, OperatorStack.Pop());
                }
                if (OperatorStack.Peek() == "(") { OperatorStack.Pop(); }
                if (OperatorStack.Count > 0 && (OperatorStack.Peek() == "*" || OperatorStack.Peek() == "/"))
                {
                    DividedByZero = Calculate(ValueStack, OperatorStack.Pop());
                }
                continue;
            }

            else if (Double.TryParse(token, out n)) // Try to convert token into double
            {
                if (ValueStack.Count == 0)
                {
                    ValueStack.Push(n);
                    continue;
                }
                if (OperatorStack.Peek() == "*" || OperatorStack.Peek() == "/")
                {
                    ValueStack.Push(n);         // Since I am using a helper method, push n into stack to pop it twice
                    DividedByZero = Calculate(ValueStack, OperatorStack.Pop());
                    continue;
                }
                else
                {
                    ValueStack.Push(n);         // Push token into the stack
                }
            }
            else if (IsOperator(token))                  // Check if token is an operator
            {
                if (token == "*" || token == "/")
                {
                    OperatorStack.Push(token);      // Push * or / into OperatorStack
                    continue;                       //Move on to next token to skip the rest of the code that won't be used
                }
                if (token == "+" || token == "-")
                {

                    if (OperatorStack.Count > 0 && (OperatorStack.Peek() == "+" || OperatorStack.Peek() == "-"))
                    {
                        Calculate(ValueStack, OperatorStack.Pop()); // Call helper method to calculate and push result into stack
                        OperatorStack.Push(token);
                        continue;
                    }
                    else
                    {
                        OperatorStack.Push(token);      // I would put continue here as well, but for code coverage I removed it
                    }
                }
            }
            else 
            {
                try
                {
                    ValueStack.Push(lookup(token));     //Error should be thrown here if token is not valid}
                }
                catch (ArgumentException)
                {
                    return new FormulaError("Invalid Token");
                }
            }
        }

        while(OperatorStack.Count > 0) { DividedByZero = Calculate(ValueStack, OperatorStack.Pop()); }
        if (DividedByZero) { return new FormulaError("Divided by zero"); }
        if (OperatorStack.Count == 0) { return ValueStack.Pop(); }
        return ValueStack.Pop();
       

    }

    /// <summary>
    ///   <para>
    ///     Push the result of the caculation into value stack. A helper method that calculate using the given values and operator
    ///   </para>
    /// </summary>
    /// <returns> Nothing, modify valuestack by pushing the result of the calculation </returns>
    private bool Calculate( Stack<double> ValueStack, string op)
    {
        double value1 = ValueStack.Pop();
        double value2 = ValueStack.Pop();
        if(op =="+") { ValueStack.Push(value1 + value2); }
        else if (op == "-") { ValueStack.Push(value2 - value1); }
        else if (op == "*") { ValueStack.Push(value1 * value2); }
        else if (op == "/") 
        {
            if (value1 != 0) { ValueStack.Push(value2 / value1); }
            else { return true; }
        }
        return false;
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
        return (ToString()+ tokens[0] + tokens[tokens.Count - 1]).GetHashCode() * tokens.Count; //Multiply the Hash Code of (Normalized string + first and last token) by the number of tokens to get the hashCode
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

/// <summary>
///   Any method meeting this type signature can be used for
///   looking up the value of a variable.
/// </summary>
/// <exception cref="ArgumentException">
///   If a variable name is provided that is not recognized by the implementing method,
///   then the method should throw an ArgumentException.
/// </exception>
/// <param name="variableName">
///   The name of the variable (e.g., "A1") to lookup.
/// </param>
/// <returns> The value of the given variable (if one exists). </returns>
public delegate double Lookup(string variableName);

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



