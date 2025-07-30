// <copyright file="SpreadsheetPage.razor.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
//author: Hung Nguyen, Kuanyu Chien, Date: 10 / 26 / 2024, Course: CS 3500

namespace GUI.Client.Pages;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics;
using CS3500.Spreadsheet;
using CS3500.Formula;
using Microsoft.VisualBasic;
using CS3500.DependencyGraph;
using System.Drawing;

/// <summary>
/// Class represent the SpreadSheet page C# code and dinding through blazor, it contain save and load method to save and load spreadsheet.
/// and metohds to change cell color and highlight cell dependency. 
/// In addition, the spreadsheet allow use to input number, string, or a formula by add a '=' in fromt of the formula just like excel.
/// </summary>
public partial class SpreadsheetPage
{
    /// <summary>
    /// Based on your computer, you could shrink/grow this value based on performance.
    /// </summary>
    private const int ROWS = 50;

    /// <summary>
    /// Number of columns, which will be labeled A-Z.
    /// </summary>
    private const int COLS = 26;

    /// <summary>
    /// Provides an easy way to convert from an index to a letter (0 -> A)
    /// </summary>
    private char[] Alphabet { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();


    /// <summary>
    ///   Gets or sets the name of the file to be saved
    /// </summary>
    private string FileSaveName { get; set; } = "Spreadsheet.sprd";


    /// <summary>
    ///   <para> Gets or sets the data for all of the cells in the spreadsheet GUI. </para>
    ///   <remarks>Backing Store for HTML</remarks>
    /// </summary>
    private string[,] CellsBackingStore { get; set; } = new string[ROWS, COLS];
    
    /// <summary>
    /// create the spreadsheet object for the page 
    /// </summary>
    public Spreadsheet excel = new Spreadsheet();

    /// <summary>
    /// the current selected cell, default is A1
    /// </summary>
    private string SelectedCell = "A1";

    /// <summary>
    /// the current row of the current cell
    /// </summary>
    private int CurrentRow = 0;

    /// <summary>
    /// the current column of the current cell
    /// </summary>
    private int CurrentCol = 0;

    /// <summary>
    /// the text area that user allow to inpur what that are store in the cell
    /// </summary>
    private ElementReference TextArea;

    /// <summary>
    /// usre's input from the text area
    /// </summary>
    private string Input = string.Empty;

    /// <summary>
    /// record the cell that are currently been highlighted.
    /// </summary>
    private List<string> HighlightedCell = new List<string>();

    /// <summary>
    /// record the cell and cell color that has been changed.
    /// </summary>
    private string ExcelColor = "";

    /// <summary>
    /// the color which selected by user that they want to change to, defaul is nothing(white).
    /// </summary>
    private string SelectedColor = "";

    /// <summary>
    /// record every cell's color
    /// </summary>
    private string[,] CellColors { get; set; } = new string[ROWS, COLS];

    /// <summary>
    /// does a exception occured while using the spreadsheet page
    /// </summary>
    private bool hasException = false;

    /// <summary>
    /// the error message to be showed in the pop up message box when an expected is thrown 
    /// </summary>
    private string ErrorMessage = "An Error Occurred!";

    /// <summary>
    /// does the spreadsheet has been highlighted or not
    /// </summary>
    private bool isHighlighting = false;

    /// <summary>
    /// the cell that need to be updated when user set up new data and need to change the cell's dependents or highlight some cells or remove highlight.
    /// </summary>
    private ICollection<string> ToBeUpdatedCollection = new List<string>();

    /// <summary>
    /// when user click which color they want to change to. The method set the SelectedColor variable to the color thay want.
    /// </summary>
    /// <param name="e"></param>
    private void ChangeSelectedColor(ChangeEventArgs e)
    {
        SelectedColor = e.Value?.ToString() ?? "";
        if (SelectedColor.Equals(""))
        {
            return;
        }
        if (SelectedColor.Equals("red"))
        {
            SelectedColor = "rgba(255,0,0,0.25)";
        }
        else if (SelectedColor.Equals("green"))
        {
            SelectedColor = "rgba(0,255,0,0.25)";
        }
        else if (SelectedColor.Equals("blue"))
        {
            SelectedColor = "rgba(0,0,255,0.25)";
        }
    }

    /// <summary>
    /// return the cell color from CellColors array, if the cell is not in the array it will set tthe cell to white.
    /// </summary>
    /// <param name="row">row of the cell</param>
    /// <param name="col">column of the cell</param>
    /// <returns></returns>
    private string GetCellColor(int row, int col)
    {
        return CellColors[row, col] ?? "white"; // Default to white if no color is set
    }

    /// <summary>
    /// Handler for when a cell is clicked
    /// </summary>
    /// <param name="row">The row component of the cell's coordinates</param>
    /// <param name="col">The column component of the cell's coordinates</param>
    private void CellClicked(int row, int col)
    {

        CurrentCol = col;
        CurrentRow = row;
        SelectedCell = CellNameConversion();
        HighlightDependcies();
        if (SelectedColor != "")
            CellColors[row, col] = SelectedColor;
        // Add colors save details into a string for saving later
        if (SelectedColor != "" && SelectedColor != "white")
            ExcelColor += SelectedCell + " " + SelectedColor + " ";
        else
            ExcelColor += SelectedCell + " " + "white" + " ";       
        TextArea.FocusAsync();
    }

    /// <summary>
    /// set the cell's content from user's input. 
    /// </summary>
    /// <param name="e"></param>
    private void CellContentChanged(ChangeEventArgs e)
    {
        string data = e.Value!.ToString() ?? "";
        string cellName = CellNameConversion();
        Input = data;
        try
        {
            ToBeUpdatedCollection = excel.SetContentsOfCell(cellName, Input);
            showOnSpreadsheetGUI();
        }
        catch (Exception ex)
        {
            HandelError(ex);
        }
    }

    /// <summary>
    /// a helper method that will show the pop up box by setting the hasException to true, and show the error type
    /// </summary>
    /// <param name="ex"></param>
    private void HandelError(Exception ex)
    {
        ErrorMessage = $"Exception occurred: {ex.GetType().Name}";
        hasException = true;
    }

    /// <summary>
    /// a helper method when use close the pop up, it change the hasException to false, and colse the pop up.
    /// </summary>
    private void DismissError()
    {
        hasException = false;
    }

    /// <summary>
    /// return the valied current cell name like A1 or F10 by convert CurretnRow and CurrentCol.
    /// </summary>
    /// <returns></returns>
    private string CellNameConversion()
    {
        char letter = (char)(CurrentCol + 65);
        string cellName = letter.ToString() + (CurrentRow + 1);
        return cellName;
    }

    /// <summary>
    /// Saves the current spreadsheet, by providing a download of a file
    /// containing the json representation of the spreadsheet. It also
    /// saves colored cells and their colors into Z100 for reloading colors
    /// </summary>
    private async void SaveFile()
    {
        excel.SetContentsOfCell("Z100", ExcelColor);             
        string JsonString = excel.GetJsonString();
        await JSRuntime.InvokeVoidAsync("downloadFile", FileSaveName,
            JsonString);
    }

    /// <summary>
    /// return the current cell's StringForm.
    /// </summary>
    /// <returns></returns>
    private string GenerateCellStringForm()
    {
        object contents = excel.GetCellContents(CellNameConversion());
        string StringForm = "";
        if (contents is Formula) { StringForm = "=" + contents.ToString(); }
        else { StringForm = "" + contents; }
        return StringForm;
    }

    /// <summary>
    /// This method will run when the file chooser is used, for loading a file.
    /// Uploads a file containing a json representation of a spreadsheet, and 
    /// replaces the current sheet with the loaded one.
    /// </summary>
    /// <param name="args">The event arguments, which contains the selected file name</param>
    private async void HandleFileChooser(EventArgs args)
    {
        try
        {
            string fileContent = string.Empty;

            InputFileChangeEventArgs eventArgs = args as InputFileChangeEventArgs ?? throw new Exception("unable to get file name");
            if (eventArgs.FileCount == 1)
            {
                var file = eventArgs.File;
                if (file is null)
                {
                    return;
                }

                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);

                // fileContent will contain the contents of the loaded file
                fileContent = await reader.ReadToEndAsync();
                // Load spreadsheet from JSON
                excel.loadFromJson(fileContent);
                // Find the list of cells to show on the spreadsheet
                ToBeUpdatedCollection = excel.GetNamesOfAllNonemptyCells();
                // Reset the spreadsheet GUI back into it's original form
                CellsBackingStore = new string[ROWS, COLS];
                CellColors = new string[ROWS, COLS];
                // Load the new cells onto the spreadsheet
                showOnSpreadsheetGUI();
                // Load the colors into the cells
                string ColorDataString = excel.GetCellValue("Z100").ToString() ?? "";           // Get the color string from cell Z100
                string[] colorList = ColorDataString.Split(" ");                                // Split the string stored at Cell Z100 with white space
                string cell = "";
                for (int i = 0; i < colorList.Length - 1; i++)
                {
                    // If index is even, then the string a cell name, if it is odd then the string is a cell color
                    if (i % 2 == 0)
                    {
                        cell = colorList[i];
                    }
                    else
                    {
                        ConvertCellNameToRowAndCol(cell, out int row, out int col);             // Get the cell indexes from given cell name
                        CellColors[row, col] = colorList[i];                                    // Change cell colors to the string at the odd index
                    }
                    StateHasChanged();                                                          // Update visual

                }
            }
        }
        catch (Exception ex)
        {
            HandelError(ex);
        }
    }

    /// <summary>
    /// the helper method will update the spreadsheet page's CellsBackingStore array, so spread sheet could refresh and show the updated value.
    /// </summary>
    /// <returns></returns>
    private ICollection<string> showOnSpreadsheetGUI()
    {

        foreach (string cell in ToBeUpdatedCollection)
        {
            ///when hit the Z100 cell(non exist in spreadsheet page cell which contain the cell color from the saving data) will ignore the cell
            if (cell == "Z100") { continue; }
            ConvertCellNameToRowAndCol(cell, out int row, out int col);
            CellsBackingStore[row, col] = excel.GetCellValue(cell).ToString() ?? "";
            TextArea.FocusAsync();
        }
        return ToBeUpdatedCollection;

    }


    /// <summary>
    /// a driver method that will be triggered when the highlight button is pressed.
    /// if is already highlighted it remove the highlight, if not it highlight the dpendencies.
    /// </summary>
    private void HighlightButtonPressed()
    {
        isHighlighting = !isHighlighting;
        if (!isHighlighting)
        {
            RemoveHighlight();
        }
        else
        {
            HighlightDependcies();
        }

    }

    /// <summary>
    /// the private instance will record the previous cell color before highlight the cell.
    /// and use it to return to the original color when remove highlight. 
    /// </summary>
    private string[,] previousCellColors { get; set; } = new string[ROWS, COLS];

    /// <summary>
    /// the method is a toogle that will highlight the selected cell's dependents and dpendees, dependents are orange, dependees 
    /// are yellow whenever highlight button is pressed.
    /// </summary>
    private void HighlightDependcies()
    {
        if (isHighlighting)
        {
            RemoveHighlight();
            DependencyGraph Dgraph = excel.GetDependencyGraph();
            foreach (string cell in Dgraph.GetDependees(SelectedCell))
            {
                HighlightedCell.Add(cell);
                ConvertCellNameToRowAndCol(cell, out int row, out int col);
                previousCellColors[row, col] = CellColors[row, col];
                CellColors[row, col] = "rgba(255,255,0,0.5)";
            }
            foreach (string cell in Dgraph.GetDependents(SelectedCell))
            {
                HighlightedCell.Add(cell);
                ConvertCellNameToRowAndCol(cell, out int row, out int col);
                previousCellColors[row, col] = CellColors[row, col];
                CellColors[row, col] = "rgba(255,165,0,0.5)";
            }
        }
    }

    /// <summary>
    /// a helper method that will get the HighlightedCell list, then remove all the yellow highlighted cell,
    /// and restore the cell color. 
    /// </summary>
    private void RemoveHighlight()
    {
        foreach (string cell in HighlightedCell)
        {

            ConvertCellNameToRowAndCol(cell, out int row, out int col);
            if (previousCellColors[row, col] == "" || previousCellColors[row, col] == "white")
            {
                CellColors[row, col] = "";
            }
            else
                CellColors[row, col] = previousCellColors[row, col];
        }
        HighlightedCell = new List<string>();
    }

    /// <summary>
    /// a helper method that convert cell name to row and col and return it using out reference. 
    /// </summary>
    /// <param name="cellName">input cell name to be converted</param>
    /// <param name="row">row integer to return</param>
    /// <param name="col">column integer to return</param>
    private void ConvertCellNameToRowAndCol(string cellName, out int row, out int col)
    {
        row = Convert.ToInt32(cellName.Substring(1)) - 1;
        col = Convert.ToInt32(cellName[0]) - 65;
    }
}