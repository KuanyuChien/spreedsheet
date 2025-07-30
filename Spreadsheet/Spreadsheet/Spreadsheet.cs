// <copyright file="Spreadsheet.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// Written by Joe Zachary for CS 3500, September 2013
// Update by Profs Kopta and de St. Germain, Fall 2021, Fall 2024
// Kuanyu Chien- Updated return types, Fall 2024
// Kuanyu Chien- Updated documentation, Fall 2024
namespace CS3500.Spreadsheet;

using CS3500.Formula;
using CS3500.DependencyGraph;
using System.Text.RegularExpressions;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;

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
/// 

public class Spreadsheet
{
    /// <summary>
    /// This private instance bject DependencyGraph will track the dependency of the cells
    /// </summary>
    private DependencyGraph dg;
    /// <summary>
    /// This private instance dictionary will record every cells that has value
    /// if a cell was asigned to an empty string, it will be removed from the Cells dictionary
    /// </summary>
    [JsonInclude]
    private Dictionary<string, Cell> Cells;
    /// <summary>
    /// This private string is used to chekc if the input cell name if valid or not
    /// </summary>
    private const string VariableRegExPattern = @"[a-zA-Z]+\d+";

    /// <summary>
    /// A SpreadSheet constructor that take no input, and reset all instence variable. 
    /// </summary>
    public Spreadsheet()
    {
        dg = new();
        Cells = new();
        IsChanged = false;
    }


    /// <summary>
    /// This private Cell class contain the cell's data.
    /// CellContent could be a string , double, or Formula object, depend on which value it was stored.
    /// </summary>
    private class Cell
    {
        /// <summary>
        /// Store cell's value, could be a string, double,or FormulaError
        /// </summary>
        [JsonIgnore]
        public object CellValue { get; set; }

        /// <summary>
        /// Store cell's content(StringForm), could be a string, double, Formula
        /// </summary>
        [JsonIgnore]
        public object CellContent { get; set; }
        /// <summary>
        /// StringForm will record the display version of the cell content. For example, a formula will look loke "=a1+a2", double: "123.0", string: "hello".
        /// </summary>
        public string StringForm { get; set; }
        /// <summary>
        /// a basic cell constructor that json deserializer need. It set cell content and value to (double)0.0, and set StringForm to an empty string.
        /// </summary>
        public Cell()
        {
            CellContent = 0.0;
            CellValue = 0.0;
            StringForm = "";
        }

        /// <summary>
        /// a cell constructor that take only cell's content, and set value to empty string.
        /// </summary>
        /// <param name="_cellContent"></param>
        public Cell(object _cellContent)
        {
            CellContent = _cellContent;
            CellValue = "";
            if (CellContent is Formula)
            {
                Formula f = (Formula)CellContent;
                StringForm = f.ToString();
            }
            else if (CellContent is double)
            {
                double value = (double)CellContent;
                StringForm = value.ToString();
            }
            else
            {
                StringForm = (string)CellContent;
            }
        }
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
        HashSet<string> cellsWithValue = new HashSet<string>();
        foreach (KeyValuePair<string, Cell> cell in Cells)
        {
            cellsWithValue.Add(cell.Key);
        }
        return cellsWithValue;
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
        if (!IsValidCellName(name))
            throw new InvalidNameException();

        string n = name.ToUpper();
        if (Cells.ContainsKey(n))
            return Cells[n].CellContent;
        return "";
    }

    /// <summary>
    /// This helper method check if the input cell name is valid or not
    /// </summary>
    /// <param name="name">cell name to be checked</param>
    /// <returns>If the cell name is valid return true, if not return false</returns>
    private static bool IsValidCellName(string name)
    {
        string standaloneVarPattern = $"^{VariableRegExPattern}$";
        return Regex.IsMatch(name, standaloneVarPattern);
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
        string n = name.ToUpper();
        SetOrUpdateCell(n, number);
        Cells[n].CellValue = number;
        foreach (string dependee in dg.GetDependees(n))
        {
            dg.RemoveDependency(dependee, n);
        }
        List<string> cellDependents = new(GetCellsToRecalculate(n));

        return cellDependents;
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
        string n = name.ToUpper();
        SetOrUpdateCell(n, text);

        if (Cells.ContainsKey(n))
            Cells[n].CellValue = text;

        List<string> cellDependents = new(GetCellsToRecalculate(n));
        return cellDependents;

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
        string n = name.ToUpper();
        List<string> cellDependents = new();
        List<string> listOfDependees = new();
        if (Cells.ContainsKey(n))
        {
            //if it is overwrite a Formula cell, remove all the dependency with dependees.
            if (Cells[n].CellContent is Formula)
            {
                foreach (string dependee in dg.GetDependees(n))
                {
                    listOfDependees.Add(dependee);
                    dg.RemoveDependency(dependee, n);
                }
            }
        }
        //add new dependency with the cell and variable in new formula. 
        foreach (string var in formula.GetVariables())
        {
            dg.AddDependency(var, n);
        }
        //try if the new dependency graph has cycle, if true undo what have done, then throw CircularException
        try
        {
            cellDependents = new(GetCellsToRecalculate(n));
        }
        catch (CircularException)
        {
            foreach (string var in formula.GetVariables())
            {
                dg.RemoveDependency(var, n);

            }
            foreach(string dependee in listOfDependees)
            {
                dg.AddDependency(dependee, n);
            }
            throw new CircularException();
        }
        SetOrUpdateCell(n, formula);
        return cellDependents;
    }
    /// <summary>
    /// a helper method to update cell's content, it will check if the name is in the Cells(check if the cell to be overwrited is empty or not)
    /// if it's not empty check if it is set to empty string(become empty cell) if not overwrite the conetent.
    /// if it's overwrite a empty cell, check if input content is an empty string, if not all the new cell and its content into Cells.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="content"></param>
    private void SetOrUpdateCell(string name, object content)
    {
        if (Cells.ContainsKey(name))
        {
            if (content.Equals(""))
            {
                Cells.Remove(name);
            }
            else

                Cells[name].CellContent = content;
        }
        else
        {
            if (!content.Equals(""))
                Cells.Add(name, new Cell(content));
        }
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
        string n = name.ToUpper();
        if (dg.HasDependents(n))
        {
            return dg.GetDependents(n);
        }
        return new HashSet<string>();
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
    /// Is a recursive method it will get all the direct and indirect dependents, and return all the cells it has visited.
    /// </summary>
    /// <param name="start">the starting cell(dependee)</param>
    /// <param name="name">the dependent</param>
    /// <param name="visited"> the cells method has been through, if visit a visited point means cycle exist. </param>
    /// <param name="changed"> the cells need to change.</param>
    /// <exception cref="CircularException">if there is a cycle in the dependency graph it will throw exception</exception>
    private void Visit(string start, string name, ISet<string> visited, LinkedList<string> changed)
    {
        visited.Add(name);
        foreach (string n in GetDirectDependents(name))
        {
            if (n.Equals(start))
            {
                throw new CircularException();
            }
            else if (!visited.Contains(n))
            {
                Visit(start, n, visited, changed);
            }
        }

        changed.AddFirst(name);
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
    /// record if the spreadsheet is changed.
    /// </summary>
    private bool IsChanged = false;
    /// <summary>
    /// True if this spreadsheet has been changed since it was 
    /// created or saved (whichever happened most recently),
    /// False otherwise.
    /// </summary>
    [JsonIgnore]
    public bool Changed { get { return IsChanged; } private set { } }

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
            
            dg = new DependencyGraph();
            Cells = new Dictionary<string, Cell>();
            IsChanged = false;
            if (!File.Exists(filename))
            {
                throw new SpreadsheetReadWriteException("Invalid file path, can't find file.");
            }
            string jsonContent = File.ReadAllText(filename);
            Spreadsheet? rebuilt_sp = JsonSerializer.Deserialize<Spreadsheet>(jsonContent);
            if (rebuilt_sp is null)
            {
                throw new SpreadsheetReadWriteException($"Failed to load the spreadsheet from {filename}. The file can't be convert to a spread sheet");
            }

            foreach (KeyValuePair<string, Cell> cell in rebuilt_sp.Cells)
            {
                this.SetContentsOfCell(cell.Key, cell.Value.StringForm);
            }
        }
        catch (Exception ex)
        {
            throw new SpreadsheetReadWriteException($"Failed to load the spreadsheet from {filename}. Throw {ex}");
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
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            string json = JsonSerializer.Serialize(this, jsonOptions);
            Console.WriteLine(json);
            File.WriteAllText(filename, json);
        }
        catch (Exception ex)
        {
            throw new SpreadsheetReadWriteException($"Failed to save the spreadsheet to {filename}. Throw {ex}");

        }
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
        string n = name.ToUpper();
        if (!IsValidCellName(n))
            throw new InvalidNameException();
        //return empty string if it is an empty cell
        if (!Cells.ContainsKey(n))
        {
            return "";
        }

        //if the formula cell has never been evaluate before, this method will evalute an return the result for it.
        if (Cells[n].CellContent is Formula && Cells[n].CellValue.Equals(""))
        {
            Formula f = (Formula)Cells[n].CellContent;
            return f.Evaluate(findFormulaCellValue);
        }
        else
        {
            //get the cell value
            return Cells[n].CellValue;
        }

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
        //check InValied cell name
        if (!IsValidCellName(name))
            throw new InvalidNameException();
        string n = name.ToUpper();
        // first try parse the input-cell's content is double or not. 
        double parsed_double;
        if (double.TryParse(content, out parsed_double))
        {
            // if content is double, set the cell value to double, and reevaluate its dpendents
            IsChanged = true;
            List<string> cellsToChange = (List<string>)this.SetCellContents(n, parsed_double);
            Cells[n].StringForm = content;
            foreach (string cellToChange in cellsToChange)
            {
                if (cellToChange != n)
                {
                    if (Cells[cellToChange].CellContent is Formula)
                    {
                        Formula cellContentFormula = (Formula)Cells[cellToChange].CellContent;
                        Cells[cellToChange].CellValue = cellContentFormula.Evaluate(findFormulaCellValue);
                    }
                }
            }
            return cellsToChange;
        }
        //check if it's an empty string
        else if (content != "")
        {
            // if it's not an empty string and it first chat is '=', then it's a formula.
            if (content[0] == '=')
            {
                //remove the beginning '=', stroe the frmula into cell content
                string formulaString = content.Remove(0, 1);
                Formula f = new Formula(formulaString);
                IsChanged = true;
                List<string> dependentsCells = (List<string>)this.SetCellContents(n, f);
                if (Cells.ContainsKey(n))
                    Cells[n].StringForm = content;
                return dependentsCells;
            }
            else
            {
                //if it is not formula it's a string, set the string into cell content
                IsChanged = true;
                List<string> dependentsCells = (List<string>)this.SetCellContents(n, content);
                if (Cells.ContainsKey(n))
                    Cells[n].StringForm = content;
                return dependentsCells;
            }
        }
        // if it is tring to set an empty string into cell
        else
        {
            // cehck if the orinal cell is empty or not, if it is set not empty to empty, IsChange become true
            if (Cells.ContainsKey(n))
            {
                IsChanged = true;
                Cells[n].StringForm = content;
            }
            List<string> dependentsCells = (List<string>)this.SetCellContents(n, content);

            return dependentsCells;
        }
    }
    /// <summary>
    /// A helper recursive method that will find the certen cell value (double) and return it.
    /// </summary>
    /// <param name="cellName">the cell name that evaluate need</param>
    /// <returns>a double number that come from evalute a formula or a double type cell.</returns>
    /// <exception cref="ArgumentException">if this method can't find the value( could be lack of variable or one of cell is string cell), throw the excpetion</exception>
    private double findFormulaCellValue(string cellName)
    {
        //check if the cell is empty or not
        if (Cells.ContainsKey(cellName))
        {
            object content = Cells[cellName].CellContent;
            if (content is double)
                return (double)content;
            else if (content is Formula)
            {
                foreach (string dependee in dg.GetDependees(cellName))
                {
                    if (Cells[dependee].CellContent is Formula)
                    {
                        Formula formula = (Formula)Cells[dependee].CellContent;
                        Cells[dependee].CellValue = (double)formula.Evaluate(findFormulaCellValue);
                    }
                    else if (Cells[dependee].CellContent is string)
                    {
                        throw new ArgumentException();
                    }
                }
                Formula f = (Formula)Cells[cellName].CellContent;
                Cells[cellName].CellValue = (double)f.Evaluate(findFormulaCellValue);
                return (double)Cells[cellName].CellValue;
            }
            else { throw new ArgumentException(); }
        }
        else throw new ArgumentException();
    }
}

