// <copyright file="Spreadsheet.c//s" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// Written by Joe Zachary for CS 3500, September 2013
// Update by Profs Kopta and de St. Germain, Fall 2021, Fall 2024
//     - Updated return types
//     - Updated documentation
// Co-author: Hung Nguyen, Date: 10/20/2024, Course: CS 3500
namespace CS3500.Spreadsheet;

using CS3500.Formula;
using CS3500.DependencyGraph;
using static CS3500.Spreadsheet.Spreadsheet;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Security.AccessControl;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;

/// <summary>
///   <para>
///     Thrown to indicate that a change to a cell will cause a circular dependency.
///   </para>
/// </summary>
public class CircularException : Exception
{
}

/// <summary>
///   <para>
///     Thrown to indicate that a name parameter was invalid.
///   </para>
/// </summary>
public class InvalidNameException : Exception
{
}

/// <summary>
/// <para>
///   Thrown to indicate that a read or write attempt has failed with
///   an expected error message informing the user of what went wrong.
/// </para>
/// </summary>
public class SpreadsheetReadWriteException : Exception
{
    /// <summary>
    ///   <para>
    ///     Creates the exception with a message defining what went wrong.
    ///   </para>
    /// </summary>
    /// <param name="msg"> An informative message to the user. </param>
    public SpreadsheetReadWriteException(string msg)
    : base(msg)
    {
    }
}

/// <summary>
///   <para>
///     An Spreadsheet object represents the state of a simple spreadsheet.  A
///     spreadsheet represents an infinite number of named cells.
///   </para>
/// <para>
///     Valid Cell Names: A string is a valid cell name if and only if it is one or
///     more letters followed by one or more numbers, e.g., A5, BC27.
/// </para>
/// <para>
///    Cell names are case insensitive, so "x1" and "X1" are the same cell name.
///    Your code should normalize (uppercased) any stored name but accept either.
/// </para>
/// <para>
///     A spreadsheet represents a cell corresponding to every possible cell name.  (This
///     means that a spreadsheet contains an infinite number of cells.)  In addition to
///     a name, each cell has a contents and a value.  The distinction is important.
/// </para>
/// <para>
///     The <b>contents</b> of a cell can be (1) a string, (2) a double, or (3) a Formula.
///     If the contents of a cell is set to the empty string, the cell is considered empty.
/// </para>
/// <para>
///     By analogy, the contents of a cell in Excel is what is displayed on
///     the editing line when the cell is selected.
/// </para>
/// <para>
///     In a new spreadsheet, the contents of every cell is the empty string. Note:
///     this is by definition (it is IMPLIED, not stored).
/// </para>
/// <para>
///     The <b>value</b> of a cell can be (1) a string, (2) a double, or (3) a FormulaError.
///     (By analogy, the value of an Excel cell is what is displayed in that cell's position
///     in the grid.) We are not concerned with cell values yet, only with their contents,
///     but for context:
/// </para>
/// <list type="number">
///   <item>If a cell's contents is a string, its value is that string.</item>
///   <item>If a cell's contents is a double, its value is that double.</item>
///   <item>
///     <para>
///       If a cell's contents is a Formula, its value is either a double or a FormulaError,
///       as reported by the Evaluate method of the Formula class.  For this assignment,
///       you are not dealing with values yet.
///     </para>
///   </item>
/// </list>
/// <para>
///     Spreadsheets are never allowed to contain a combination of Formulas that establish
///     a circular dependency.  A circular dependency exists when a cell depends on itself,
///     either directly or indirectly.
///     For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
///     A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
///     dependency.
/// </para>
/// </summary>
public class Spreadsheet
{
    /// <summary>
    ///   All variables are letters followed by numbers.  This pattern
    ///   represents valid variable name strings. (From Formula.cs)
    /// </summary>
    private const string VariableRegExPattern = @"[a-zA-Z]+\d+";
    /// <summary>
    /// Dictionary to connect a name string to a corresponding cell
    /// </summary>
    [JsonInclude]
    private Dictionary<string, Cell> Cells;
    /// <summary>
    /// A dependency graph to keep track of dependency between each cells 
    /// </summary>
    private DependencyGraph CellDgraph;
    /// <summary>
    /// True if this spreadsheet has been changed since it was 
    /// created or saved (whichever happened most recently),
    /// False otherwise.
    /// </summary>
    [JsonIgnore]
    public bool Changed { get; private set; }

    /// <summary>
    /// Constructor that initialize a Spreadsheet when it is called
    /// </summary>
    ///  <para>
    ///     Creates a new Spreadsheet. Intialize a cell dictionary, dependency graph and a changed variable
    ///  </para>
    ///  
    /// <param name="name"> A string name that the cell is associated to </param>
    /// <param name="content"> An object content that can be string double or Formula since setcell methods of Spreadsheet has 3 different parameter types </param>
    public Spreadsheet()
    {
        Cells = new Dictionary<string, Cell>();
        CellDgraph = new DependencyGraph();
        Changed = false;
    }
    /// <summary>
    ///   Provides a copy of the normalized names of all of the cells in the spreadsheet
    ///   that contain information (i.e., non-empty cells).
    /// </summary>
    /// <returns>
    ///   A set of the names of all the non-empty cells in the spreadsheet.
    /// </returns>
    public ISet<string> GetNamesOfAllNonemptyCells()
    {
        HashSet<string> NonEmptyCellNames = new HashSet<string>(Cells.Keys);             // Result set
        return NonEmptyCellNames;
    }

    /// <summary>
    ///   Returns the contents (as opposed to the value) of the named cell.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   Thrown if the name is invalid.
    /// </exception>
    ///
    /// <param name="name">The name of the spreadsheet cell to query. </param>
    /// <returns>
    ///   The contents as either a string, a double, or a Formula.
    ///   See the class header summary.
    /// </returns>
    public object GetCellContents(string name)
    {
        name = IsValidCellName(name);                                                      // If name is not valid, throw InvalidNameException. Also normalized name
        if (Cells.ContainsKey(name)) { return Cells[name].Content; }     // Check if spreadsheet have name as key, if not return string.Empty (Empty Content)
        return string.Empty;
    }

    /// <summary>
    ///  Set the contents of the named cell to the given number.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    ///
    /// <param name="name"> The name of the cell. </param>
    /// <param name="number"> The new contents of the cell. </param>
    /// <returns>
    ///   <para>
    ///     This method returns an ordered list consisting of the passed in name
    ///     followed by the names of all other cells whose value depends, directly
    ///     or indirectly, on the named cell.
    ///   </para>
    ///   <para>
    ///     The order must correspond to a valid dependency ordering for recomputing
    ///     all of the cells, i.e., if you re-evaluate each cells in the order of the list,
    ///     the overall spreadsheet will be correctly updated.
    ///   </para>
    ///   <para>
    ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///     list [A1, B1, C1] is returned, i.e., A1 was changed, so then A1 must be
    ///     evaluated, followed by B1, followed by C1.
    ///   </para>
    /// </returns>
    private IList<string> SetCellContents(string name, double number)
    {
        if (Cells.ContainsKey(name)) 
        { 
            Cells[name].Content = number;                                          
            Cells[name].Value = number;                                            // If name existed, replace the content and value of cell under name
        }   
        else { Cells.Add(name, new Cell(number)); }
        if (CellDgraph.HasDependees(name))
        {
            foreach(string dependee in CellDgraph.GetDependees(name))                      
            {               
                    CellDgraph.RemoveDependency(dependee, name);                           // Check if cell has dependee, look for them and remove them as a number cannot have variable in it                   
            }
        }
        return new List<string>(GetCellsToRecalculate(name));                              // GetCellsToRecalculate will produce the list
    }

    /// <summary>
    ///   The contents of the named cell becomes the given text.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="text"> The new contents of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, string text)
    {
        if (text != string.Empty)
        {                                                                                       // If name is not valid, throw InvalidNameException. Also normalized name
            if (Cells.ContainsKey(name)) 
            { 
                Cells[name].Content = text;
                Cells[name].Value = text;                                            // If name existed, replace the content and value of cell under name
            }
            else { Cells.Add(name, new Cell(text)); }
        }
        if (CellDgraph.HasDependees(name))
        {
            foreach (string dependee in CellDgraph.GetDependees(name))
            {
                CellDgraph.RemoveDependency(dependee, name);                           // Check if cell has dependee, look for them and remove them as a number cannot have variable in it                   
            }
        }
        return new List<string>(GetCellsToRecalculate(name));                                   // GetCellsToRecalculate will produce the list, excel can combine strings so I assume text cells still need to be reevaluated
    }

    /// <summary>
    ///   Set the contents of the named cell to the given formula.
    /// </summary>
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///   <para>
    ///     If changing the contents of the named cell to be the formula would
    ///     cause a circular dependency, throw a CircularException, and no
    ///     change is made to the spreadsheet.
    ///   </para>
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="formula"> The new contents of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, Formula formula)
    {                                                                  
        CellDgraph.ReplaceDependees(name, formula.GetVariables());                                           // Use replace to modify the dependecy graph after new changes 
        object CellValue = 0;
        List<string> cells = new List<string>(GetCellsToRecalculate(name));                                  // throw CircularException before adding to dictionary
        if (!Cells.ContainsKey(name)) 
        {
            CellValue = formula.Evaluate(VariableLookup);
            Cells.Add(name, new Cell(formula));                                                             // Add Cell into the Cell Dictionary
            Cells[name].Value = CellValue;                                                                  // Set Cell Value
        }                                                                                                   // Add new cell into dictionary
        else { Cells[name].Content = formula; }                                                             // Add new cell into dictionary
        return cells;                                                                                       // GetCellsToRecalculate will produce the list and throw CircularException if found 
    }

    /// <summary>
    ///   A helper method that returns a normalized name and checks if given string is a valid cell name (variable) or not. If not it throws InvalidNameException()
    /// </summary>
    private string IsValidCellName(string name)
    {
        if (name == string.Empty || !Regex.IsMatch(name,$"^{VariableRegExPattern}$"))                          // If name is empty or not a valid variable, throw InvalidNameException
        {
            throw new InvalidNameException();
        }
        return name.ToUpper();
    }
    /// <summary>
    ///   Returns an enumeration, without duplicates, of the names of all cells whose
    ///   values depend directly on the value of the named cell.
    /// </summary>
    /// <param name="name"> This <b>MUST</b> be a valid name.  </param>
    /// <returns>
    ///   <para>
    ///     Returns an enumeration, without duplicates, of the names of all cells
    ///     that contain formulas containing name.
    ///   </para>
    ///   <para>For example, suppose that: </para>
    ///   <list type="bullet">
    ///      <item>A1 contains 3</item>
    ///      <item>B1 contains the formula A1 * A1</item>
    ///      <item>C1 contains the formula B1 + A1</item>
    ///      <item>D1 contains the formula B1 - C1</item>
    ///   </list>
    ///   <para> The direct dependents of A1 are B1 and C1. </para>
    /// </returns>
    private IEnumerable<string> GetDirectDependents(string name)
    {
        name = IsValidCellName(name);                                             // If name is not valid, throw InvalidNameException. Also normalized name
        return CellDgraph.GetDependents(name);                                    // Reuse GetDependents method from Dependency Graph to get the list of direct dependents
    }

    /// <summary>
    ///   <para>
    ///     This method is implemented for you, but makes use of your GetDirectDependents.
    ///   </para>
    ///   <para>
    ///     Returns an enumeration of the names of all cells whose values must
    ///     be recalculated, assuming that the contents of the cell referred
    ///     to by name has changed.  The cell names are enumerated in an order
    ///     in which the calculations should be done.
    ///   </para>
    ///   <exception cref="CircularException">
    ///     If the cell referred to by name is involved in a circular dependency,
    ///     throws a CircularException.
    ///   </exception>
    ///   <para>
    ///     For example, suppose that:
    ///   </para>
    ///   <list type="number">
    ///     <item>
    ///       A1 contains 5
    ///     </item>
    ///     <item>
    ///       B1 contains the formula A1 + 2.
    ///     </item>
    ///     <item>
    ///       C1 contains the formula A1 + B1.
    ///     </item>
    ///     <item>
    ///       D1 contains the formula A1 * 7.
    ///     </item>
    ///     <item>
    ///       E1 contains 15
    ///     </item>
    ///   </list>
    ///   <para>
    ///     If A1 has changed, then A1, B1, C1, and D1 must be recalculated,
    ///     and they must be recalculated in an order which has A1 first, and B1 before C1
    ///     (there are multiple such valid orders).
    ///     The method will produce one of those enumerations.
    ///   </para>
    ///   <para>
    ///      PLEASE NOTE THAT THIS METHOD DEPENDS ON THE METHOD GetDirectDependents.
    ///      IT WON'T WORK UNTIL GetDirectDependents IS IMPLEMENTED CORRECTLY.
    ///   </para>
    /// </summary>
    /// <param name="name"> The name of the cell.  Requires that name be a valid cell name.</param>
    /// <returns>
    ///    Returns an enumeration of the names of all cells whose values must
    ///    be recalculated.
    /// </returns>
    private IEnumerable<string> GetCellsToRecalculate(string name)
    {
        LinkedList<string> changed = new();
        HashSet<string> visited = [];
        Visit(name, name, visited, changed);
        return changed;
    }

    /// <summary>
    ///   </para>
    ///   A helper for the GetCellsToRecalculate method.
    ///   </para>
    ///   </para>
    ///   A recursive method that goes into every dependents of the start cell name and all of their dependents to check for Circular Dependency
    ///   and add into a list of dependents that will need to be recalculated. This recursive method use a set to keep track of visited cells and skip them.
    ///   </para>
    ///   <exception cref="CircularException">
    ///   <para>
    ///   If a cell show behavior of circular dependency where it has the original cell as its dependents, throw CircularException
    ///   </para>
    ///   </exception>
    /// </summary>
    private void Visit(string start, string name, ISet<string> visited, LinkedList<string> changed)
    {
        visited.Add(name);                                      // Add the current cell name into the visited list so it won't be revisited
        foreach (string n in GetDirectDependents(name))         // For each loop that runs through every element of the dependent list return by GetDirectDependents()
        {
            if (n.Equals(start))                                // At any point in the recursive that n is equal to the original cell name, CircularException() is thrown
            {
                throw new CircularException();
            }
            else if (!visited.Contains(n))                      // If not visited, go into n to get its dependents
            {
                Visit(start, n, visited, changed);              // Recursive to get dependents of every dependents of the original
            }
        }

        changed.AddFirst(name);                                 // Add current cell to the top of the list, goes from bottoms up so original cell should be first when method is complete
    }
    /// <summary>
    ///   <para>
    ///     Return the value of the named cell, as defined by
    ///     <see cref="GetCellValue(string)"/>.
    ///   </para>
    /// </summary>
    /// <param name="name"> The cell in question. </param>
    /// <returns>
    ///   <see cref="GetCellValue(string)"/>
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///   If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object this[string name]
    {
        
        get { return GetCellValue(name); }
    }


    /// <summary>
    /// Constructs a spreadsheet using the saved data in the file refered to by
    /// the given filename. 
    /// <see cref="Save(string)"/>
    /// </summary>
    /// <exception cref="SpreadsheetReadWriteException">
    ///   Thrown if the file can not be loaded into a spreadsheet for any reason
    /// </exception>
    /// <param name="filename">The path to the file containing the spreadsheet to load</param>
    public Spreadsheet(string filename)
    {
        try
        {
            string JsonString = File.ReadAllText(filename);
            loadFromJson(JsonString);
        }
        catch (Exception e)
        {
            throw new SpreadsheetReadWriteException("Error loading file");
        }
    }

    /// <summary>
    /// A method that does not return anything, it helps create a spreadsheet from a given Json string. This method is used to load
    ///  a Json string to save the spreadsheet using the GUI and help intialize a spreadsheet from the file
    /// </summary>
    public void loadFromJson(string JsonString)
    {
        IList<string> cellToChange = new List<string>();
        Spreadsheet s = JsonSerializer.Deserialize<Spreadsheet>(JsonString);
        Cells = s.Cells;
        CellDgraph = new DependencyGraph();
        foreach (KeyValuePair<string, Cell> cell in Cells)
        {
            SetContentsOfCell(cell.Key, cell.Value.StringForm);
        }
    }


    /// <summary>
    ///   <para>
    ///     Writes the contents of this spreadsheet to the named file using a JSON format.
    ///     If the file already exists, overwrite it.
    ///   </para>
    ///   <para>
    ///     The output JSON should look like the following.
    ///   </para>
    ///   <para>
    ///     For example, consider a spreadsheet that contains a cell "A1" 
    ///     with contents being the double 5.0, and a cell "B3" with contents 
    ///     being the Formula("A1+2"), and a cell "C4" with the contents "hello".
    ///   </para>
    ///   <para>
    ///      This method would produce the following JSON string:
    ///   </para>
    ///   <code>
    ///   {
    ///     "Cells": {
    ///       "A1": {
    ///         "StringForm": "5"
    ///       },
    ///       "B3": {
    ///         "StringForm": "=A1+2"
    ///       },
    ///       "C4": {
    ///         "StringForm": "hello"
    ///       }
    ///     }
    ///   }
    ///   </code>
    ///   <para>
    ///     You can achieve this by making sure your data structure is a dictionary 
    ///     and that the contained objects (Cells) have property named "StringForm"
    ///     (if this name does not match your existing code, use the JsonPropertyName 
    ///     attribute).
    ///   </para>
    ///   <para>
    ///     There can be 0 cells in the dictionary, resulting in { "Cells" : {} } 
    ///   </para>
    ///   <para>
    ///     Further, when writing the value of each cell...
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///       If the contents is a string, the value of StringForm is that string
    ///     </item>
    ///     <item>
    ///       If the contents is a double d, the value of StringForm is d.ToString()
    ///     </item>
    ///     <item>
    ///       If the contents is a Formula f, the value of StringForm is "=" + f.ToString()
    ///     </item>
    ///   </list>
    /// </summary>
    /// <param name="filename"> The name (with path) of the file to save to.</param>
    /// <exception cref="SpreadsheetReadWriteException">
    ///   If there are any problems opening, writing, or closing the file, 
    ///   the method should throw a SpreadsheetReadWriteException with an
    ///   explanatory message.
    /// </exception>
    public void Save(string filename)
    {
        try
        {
            string JSONString = GetJsonString();
            File.WriteAllText(filename, JSONString);
            Changed = false;
        }
        catch (Exception e)
        {
            throw new SpreadsheetReadWriteException("Error saving file");
        }
    }

    /// <summary>
    /// A method that does not return anything, it helps create a Json string from the already existing information in the spreadsheet.
    /// This method is used to generate a Json string to save the spreadsheet using the GUI and help intialize a spreadsheet from the file
    /// </summary>
    public string GetJsonString()
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        options.WriteIndented = true;

        return JsonSerializer.Serialize(this, options);
    }
    /// <summary>
    ///   <para>
    ///     Return the value of the named cell.
    ///   </para>
    /// </summary>
    /// <param name="name"> The cell in question. </param>
    /// <returns>
    ///   Returns the value (as opposed to the contents) of the named cell.  The return
    ///   value should be either a string, a double, or a CS3500.Formula.FormulaError.
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///   If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object GetCellValue(string name)
    {
        name = IsValidCellName(name);
        if (Cells.ContainsKey(name))
        {
            return Cells[name].Value;
        }
        else { return ""; }
        
    }

    /// <summary>
    ///   <para>
    ///     Set the contents of the named cell to be the provided string
    ///     which will either represent (1) a string, (2) a number, or 
    ///     (3) a formula (based on the prepended '=' character).
    ///   </para>
    ///   <para>
    ///     Rules of parsing the input string:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///       <para>
    ///         If 'content' parses as a double, the contents of the named
    ///         cell becomes that double.
    ///       </para>
    ///     </item>
    ///     <item>
    ///         If the string does not begin with an '=', the contents of the 
    ///         named cell becomes 'content'.
    ///     </item>
    ///     <item>
    ///       <para>
    ///         If 'content' begins with the character '=', an attempt is made
    ///         to parse the remainder of content into a Formula f using the Formula
    ///         constructor.  There are then three possibilities:
    ///       </para>
    ///       <list type="number">
    ///         <item>
    ///           If the remainder of content cannot be parsed into a Formula, a 
    ///           CS3500.Formula.FormulaFormatException is thrown.
    ///         </item>
    ///         <item>
    ///           Otherwise, if changing the contents of the named cell to be f
    ///           would cause a circular dependency, a CircularException is thrown,
    ///           and no change is made to the spreadsheet.
    ///         </item>
    ///         <item>
    ///           Otherwise, the contents of the named cell becomes f.
    ///         </item>
    ///       </list>
    ///     </item>
    ///   </list>
    /// </summary>
    /// <returns>
    ///   <para>
    ///     The method returns a list consisting of the name plus the names 
    ///     of all other cells whose value depends, directly or indirectly, 
    ///     on the named cell. The order of the list should be any order 
    ///     such that if cells are re-evaluated in that order, their dependencies 
    ///     are satisfied by the time they are evaluated.
    ///   </para>
    ///   <example>
    ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///     list {A1, B1, C1} is returned.
    ///   </example>
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///     If name is invalid, throws an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///     If a formula would result in a circular dependency, throws CircularException.
    /// </exception>
    public IList<string> SetContentsOfCell(string name, string content)
    {
        IList<string> result = new List<string>();                                                          // A return list so we don't have to change Changed at every return condition
        string CellName = IsValidCellName(name);                                                           // Throw InvalidNameException if invalid cell name is pass through
        if(content == string.Empty) { return result;  }
        else if (Double.TryParse(content,out double number))                                               // If parsing content show a number, set content of cell name as number
        {
            result = SetCellContents(CellName, number); 
        }          
        else if(!(content[0] == '='))                                                                      // If content is not a number or start with a = sign, it is a string, set content of cell as a string
        {
            result = SetCellContents(CellName, content); 
        }
        else if (content[0] == '=')
        {
            Formula f = new Formula(content.Substring(1, content.Length - 1));
            result = SetCellContents(CellName, f);                                                         // Take formula after the = sign at the start of string content, throw Circular Exception if invalid
        }
        foreach (string x in result)                                                                         // Reevaluate all the dependent cells after each changes
        {
            if (Cells[x].Content is Formula)
            {
                Formula f = (Formula)Cells[x].Content;
                Cells[x].Value = f.Evaluate(VariableLookup);
            }
            else if (Cells[x].Content is double)
            {
                Cells[x].StringForm = Cells[x].Content.ToString();
            }
            else
            {
                Cells[x].StringForm = (string)Cells[x].Content;
            }
        }

        Changed = true;                                                                                    // After no exception thrown then changed is true                                                                                     
        return result;
    }

    /// <summary>
    /// A Lookup method that is used to pass through the lookup delegate required by Formula.Evaluate(). This method takes in a variable name and either returns a
    /// double or FormulaError depending on if there is a double value at the targeted cell
    /// </summary>
    /// <param name="variableName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private double VariableLookup(string variableName)
    {
        if (Cells.ContainsKey(variableName))                                                   // If variable exist in dictionary, look for it, else throw ArgumentException
        {
            object result = GetCellValue(variableName);
            if(result is  double) { return (double)result; }
            // Else it cannot be evaluated, throw argument exception
            else { throw new ArgumentException("String cannot be evaluated"); }
        }

        throw new ArgumentException("Variable cannot be found"); ;
    }

    /// <summary>
    /// A method that returns the current dependency list to give Spreadsheet GUI access to the dependencies. We can use it to highlight the dependencies of 
    /// the selected cell when the button is hit.
    /// </summary>
    /// <returns> Return the dependency list linked to this spreadsheet </returns>
    public DependencyGraph GetDependencyGraph()
    {
        return CellDgraph;
    }
    private class Cell
    {
    

        /// <summary>
        /// Object Content because content can be string, double or Formula. 
        /// </summary>
        [JsonIgnore]
        public object Content { get; set; }

        /// <summary>
        /// Object Value that can be string, double or Formula Error
        /// </summary>
        [JsonIgnore]
        public object Value { get; set; } 

        /// <summary>
        /// Object StringForm for JsonString generation
        /// </summary>
        public string StringForm { get; set; }


        /// <summary>
        /// No-argument Constructor for saving and loading JSON file
        /// </summary>
        ///  <para>
        ///  No parameters
        ///  </para>
         public Cell()
        {
        }
        /// <summary>
        /// Constructor that initialize a Cell using given a number
        /// </summary>
        ///  <para>
        ///     Creates a Cell that has a Name and Content
        ///  </para>
        ///  
        /// <param name="name"> A string name that the cell is associated to </param>
        /// <param name="content"> An object content that can be string double or Formula since setcell methods of Spreadsheet has 3 different parameter types </param>
        public Cell(double content)
        {
            Content = content;
            Value = content;
            StringForm = content.ToString();
        }

        /// <summary>
        /// Constructor that initialize a Cell using given a string
        /// </summary>
        ///  <para>
        ///     Creates a Cell that has a Name and Content
        ///  </para>
        ///  
        /// <param name="name"> A string name that the cell is associated to </param>
        /// <param name="content"> An object content that can be string double or Formula since setcell methods of Spreadsheet has 3 different parameter types </param>
        public Cell(string content)
        {
            Content = content;
            Value = content;
            StringForm = content;
        }

        /// <summary>
        /// Constructor that initialize a Cell using given a formula
        /// </summary>
        ///  <para>
        ///     Creates a Cell that has a Name and Content
        ///  </para>
        ///  
        /// <param name="name"> A string name that the cell is associated to </param>
        /// <param name="content"> If a formula is pass through the constructor, an evaluation will be run through it and store the result in value
        public Cell(Formula content)
        {
            Content = content;
            Value = new object();
            StringForm = "="+content.ToString();
        }


        
    }

    


}

