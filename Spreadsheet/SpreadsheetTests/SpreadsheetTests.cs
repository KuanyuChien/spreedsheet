// Author: Hung Nguyen, Date: 9/28/2024, Course: CS 3500
using CS3500.Formula;
using CS3500.Spreadsheet;
using System;
using System.Reflection;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {

        // --- Tests for GetNamesOfAllNonemptyCells ---*
        /// <summary>
        ///   <para>
        ///     Make sure that GetNameOfAllNonemptyCells return the correct set with all the expected cell names
        ///   </para>
        ///   <remarks>
        ///     Expecting set that has all the inserted cell names ["A1", "B1", "C1"]
        ///   </remarks>
        /// </summary>
        [TestMethod]
        public void Spreadsheet_TestGetNamesOfAllNonemptyCells_Valid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5");
            s.SetContentsOfCell("B1", "dog");
            s.SetContentsOfCell("C1", "A1+B1+3");
            List<string> expected = new List<string>(["A1", "B1", "C1"]);
            ISet<string> result = s.GetNamesOfAllNonemptyCells();
            Assert.AreEqual(3, s.GetNamesOfAllNonemptyCells().Count);
            foreach (string x in result)
            {
                Assert.IsTrue(expected.Contains(x));
            }
        }

        // --- Tests for TestSetCellContentTypeSwitchUp ---*
        /// <summary>
        ///   <para>
        ///     Make sure that SetContentsOfCell correctly change the result as different types of input is put into its parameter, string should get Formula Error, formula and double
        ///     should change the output of dependents
        ///   </para>
        ///   <remarks>
        ///     An empty string content should disqualify D1 as a nonempty cell so the result should not include it and have a count of 3 
        ///   </remarks>
        /// </summary>
        [TestMethod]
        public void Spreadsheet_TestSetCellContentTypeSwitchUp_Valid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5");
            s.SetContentsOfCell("B1", "=D1");
            s.SetContentsOfCell("E1", "3");
            s.SetContentsOfCell("D1", "8");
            s.SetContentsOfCell("C1", "=B1+E1+A1");
            Assert.AreEqual((double)16, s.GetCellValue("C1"));
            s.SetContentsOfCell("B1", "5");
            Assert.AreEqual((double)5, s.GetCellValue("B1"));
            Assert.AreEqual((double)13, s.GetCellValue("C1"));
            s.SetContentsOfCell("B1", "=E1");
            Assert.AreEqual((double)11, s.GetCellValue("C1"));
            s.SetContentsOfCell("B1", "dog");
            Assert.AreEqual("dog", s.GetCellContents("B1"));
            Assert.IsTrue((s.GetCellValue("C1")) is FormulaError);
        }


        // --- Tests for TestSaveAndLoadOfASpreadsheet ---*
        /// <summary>
        ///   <para>
        ///   Make sure Saving and Loading correctly, getting all the set values and contents after saving and loading the cells 
        ///   </para>
        ///   <remarks>
        ///     After setting up the original spreadsheet, save it and loading it. Asserts should return all the same value and contents for saved and loaded cells
        ///   </remarks>
        /// </summary>
        [TestMethod]
        public void Spreadsheet_TestSaveAndLoadOfASpreadsheet_Valid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5");
            s.SetContentsOfCell("B1", "=D1");
            s.SetContentsOfCell("E1", "3");
            s.SetContentsOfCell("D1", "8");
            s.SetContentsOfCell("C1", "=B1+E1+A1");
            Assert.IsTrue(s.Changed);                                           // Spreadsheet has been changed
            s.Save("file.txt");
            Assert.IsFalse(s.Changed);                                          // Since we just saved, spreadsheet has not been changed
            s = new Spreadsheet();
            s = new Spreadsheet("file.txt");
            Assert.IsTrue(s.Changed);                                           // Spreadsheet has been changed since we loaded another spreadsheet into it
            Assert.AreEqual((double)16, s.GetCellValue("C1"));                  // Make sure all the value is still the same
            Assert.AreEqual((double)8, s.GetCellValue("B1"));
            Assert.AreEqual((double)8, s.GetCellValue("D1"));
            Assert.AreEqual((double)3, s.GetCellValue("E1"));
            Assert.AreEqual((double)5, s.GetCellValue("A1"));
            Assert.AreEqual((double)3, s.GetCellContents("E1"));                // Make sure all the contents are the same
            Assert.AreEqual((double)5, s.GetCellContents("A1"));
            Assert.AreEqual((double)8, s.GetCellContents("D1"));
            Assert.AreEqual(new Formula("D1"), s.GetCellContents("B1"));
            Assert.AreEqual(new Formula("B1+E1+A1"), s.GetCellContents("C1"));

        }

        // --- Tests for TestSaveThrow ---*
        /// <summary>
        ///   <para>
        ///   Make sure Saving Throws Exception when the address does not exist
        ///   </para>
        ///   <remarks>
        ///     When an address does not exist, a SpreadsheetReadWriteException should be thrown when save is called
        ///   </remarks>
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Spreadsheet_TestSaveThrow_Valid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5");
            s.SetContentsOfCell("B1", "=D1");
            s.SetContentsOfCell("E1", "3");
            s.SetContentsOfCell("D1", "8");
            s.SetContentsOfCell("C1", "=B1+E1+A1");
            s.Save("/missing/save.json");
        }

        // --- Tests for TestSaveThrow ---*
        /// <summary>
        ///   <para>
        ///   Make sure Loading Throws Exception when the address does not exist
        ///   </para>
        ///   <remarks>
        ///     When an address does not exist, a SpreadsheetReadWriteException should be thrown when save is called
        ///   </remarks>
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Spreadsheet_TestLoadThrowValid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5");
            s.SetContentsOfCell("B1", "=D1");
            s.SetContentsOfCell("E1", "3");
            s.SetContentsOfCell("D1", "8");
            s.SetContentsOfCell("C1", "=B1+E1+A1");
            s = new Spreadsheet("/missing/save.json");
        }

        // --- Tests for StessTestSetContentOfCell ---*
        /// <summary>
        ///   <para>
        ///   Make sure FormulaError are returned for every content addition before a true value is added into the final cell. Make sure all the cells update when final cell is added
        ///   </para>
        ///   <remarks>
        ///     All the cells added should have a FormulaError until A200 is added into the cell dictionary with value of 100 
        ///   </remarks>
        /// </summary>
        [TestMethod]
        public void Spreadsheet_StessTestSetContentOfCell()
        {
            Spreadsheet s = new Spreadsheet();
            for(int i =1; i <= 200; i++)
            {
                s.SetContentsOfCell("A" + i, "=A" + (i + 1));
                Assert.IsTrue((s.GetCellValue("A"+i)) is FormulaError);
                if (i == 200) { s.SetContentsOfCell("A200", "100"); }
            }
                for (int j = 1; j < 200; j++)
                {
                    Assert.AreEqual((double)100, s.GetCellValue("A"+j));
                }
        }

        // --- Tests for StessTestSetContentOfCell ---*
        /// <summary>
        ///   <para>
        ///   Make sure loading and saving work as even if they are called multiple time and each time only adding 1 element. The final spreadsheet should still have all the cells added
        ///    even when it's excessively save and load. It should still add the correct cell and keep track of it
        ///   </para>
        ///   <remarks>
        ///     Expected behavior is all cells are save and loaded correctly with Cells goes from A1 to A200 
        ///   </remarks>
        /// </summary>
        [TestMethod]
        public void Spreadsheet_StessTestSaveLoad()
        {
            Spreadsheet s = new Spreadsheet();
            for (int i = 1; i <= 200; i++)
            {
                if (i > 1) { s = new Spreadsheet("file.txt"); }
                s.SetContentsOfCell("A" + i, ""+i);
                s.Save("file.txt");
                s = new Spreadsheet();
            }
            s = new Spreadsheet("file.txt");
            for (int i = 1; i <= 200; i++)
            {
                Assert.AreEqual((double)i, s.GetCellValue("A" + i));
                Assert.AreEqual((double)i, s.GetCellContents("A" + i));
            }

        }

        // --- Tests for Spreadsheet_StessTestSetContentOfCellWithOnePrimaryDependee ---*
        /// <summary>
        ///   <para>
        ///   Make sure that all cells will update correctly when primary dependee A1 is updated with a value. Each cell with depend on A1 and the cell before it
        ///   </para>
        ///   <remarks>
        ///     This means all cell with have the value of A1*(i-2)+ A2 so when we set A1 to 10 and A2 to 2 it should be 10(i-2)+5. A3 will be 15, A50 will be 485, etc.
        ///   </remarks>
        /// </summary>
        [TestMethod]
        public void Spreadsheet_StessTestSetContentOfCellWithOnePrimaryDependee()
        {
            Spreadsheet s = new Spreadsheet();
            for (int i = 3; i <= 200; i++)
            {
                
                s.SetContentsOfCell("A" + i, "=A1" + "+A"+(i-1) );
            }
            s.SetContentsOfCell("A1", "10");
            s.SetContentsOfCell("A2", "5");
            for (int i = 3; i <= 200; i++)
            {

                Assert.AreEqual((double)(10*(i-2)+5),s.GetCellValue("A" + i));
            }
        }

        // --- Tests for StressTestSetContentOfCellWithEvaluation ---*
        /// <summary>
        ///   <para>
        ///   Make sure adding a series of cells together from A2 to A200 result in the correct answer and multiplying from A2 to A50 results in the correct answers
        ///   </para>
        ///   <remarks>
        ///     When an address does not exist, a SpreadsheetReadWriteException should be thrown when save is called
        ///   </remarks>
        ///   Each cells name will have the same digit stored in it as values, i.e. A2 has value 2, A50 has value 50, etc.
        /// </summary>
        [TestMethod]
        public void Spreadsheet_StressTestSetContentOfCellWithEvaluation()
        {
            Spreadsheet s = new Spreadsheet();
            string x = "=A2";
            string y = x;
            double sum = 2;
            double result = 2;
            s.SetContentsOfCell("A2", "2");
            for (int i = 3; i <= 200; i++)
            {
                sum += i;
                s.SetContentsOfCell("A" + i, ""+i);
                if (i < 50)
                {
                    y += "* A" + i;
                    result *= i;
                }
                else if(i== 50)
                {
                    y += "*A50";
                    result *= i;
                }
                if (i < 200) 
                { 
                    x += "+ A" + i;
                }
                else 
                { 
                    x += "+A200";
                }   
            }
            s.SetContentsOfCell("A1", x);
            s.SetContentsOfCell("A0", y);
            Assert.AreEqual(sum, s.GetCellValue("A1"));    // Result should be 20999 which is correct
            Assert.AreEqual(result, s.GetCellValue("A0")); // Result should be 50! which is correct
        }

        // --- Tests for this[string name] ---*
        /// <summary>
        ///   <para>
        ///     Make sure this[string name] will return the correct result as if we are calling GetCellValue
        ///   </para>
        ///   <remarks>
        ///     An empty string content should disqualify D1 as a nonempty cell so the result should not include it and have a count of 3 
        ///   </remarks>
        /// </summary>
        [TestMethod]
        public void Spreadsheet_TestthisMethod_Valid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5");
            Assert.AreEqual((double) 5, s["A1"]);
            Assert.AreEqual(s.GetCellValue("A1"), s["A1"]);

        }

        // --- Tests for Invalid Name Exception ---*
        /// <summary>
        ///   <para>
        ///     Make sure that a InvalidNameException is thrown when an empty string is put as a cell name
        ///   </para>
        ///   <remarks>
        ///     A variable with at least 1 character and 1 digit is expected so a string without digits like dog should throw InvalidNameException
        ///   </remarks>
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Spreadsheet_TestInvalidNameException_Valid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("dog", "5");
        }

        // --- Tests for Invalid Name Exception ---*
        /// <summary>
        ///   <para>
        ///     Make sure that a InvalidNameException is not thrown when a valid name is put as a cell name
        ///   </para>
        ///   <remarks>
        ///     A variable with at least 1 character and 1 digit is expected so dog1 should be a viable input
        ///   </remarks>
        /// </summary>
        [TestMethod]
        public void Spreadsheet_TestInvalidNameException_Invalid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("dog1", "5");
        }

        // --- Tests for Invalid Name Exception ---*
        /// <summary>
        ///   <para>
        ///     Make sure that a InvalidNameException is thrown when an empty string is put as a cell name
        ///   </para>
        ///   <remarks>
        ///     A variable with at least 1 character and 1 digit is expected so an empty string should throw InvalidNameException
        ///   </remarks>
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Spreadsheet_TestInvalidNameExceptionWithEmptyName_Valid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(string.Empty, "5");
        }

        // --- Tests for Circular Exception ---*
        /// <summary>
        ///   <para>
        ///     Make sure that a Circular Exception is thrown when a cell is calling itself in a formula
        ///   </para>
        ///   <remarks>
        ///     A1 cannot be a dependent of itself so it cannot have A1 in its formula and a Circular Exception is expected
        ///   </remarks>
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void Spreadsheet_TestCircularException_Valid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A1+B1+A1");
        }

        // --- Tests for Circular Exception ---*
        /// <summary>
        ///   <para>
        ///     Make sure that a Circular Exception is thrown when a dependent cell called upon a dependee cell
        ///   </para>
        ///   <remarks>
        ///     C1 is a dependent of A1 so it cannot call A1 and a Circular Exception is expected
        ///   </remarks>
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void Spreadsheet_TestCircularExceptionWithMultipleFormula_Valid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A1+B1+C1");
            s.SetContentsOfCell("B1", "5");
            s.SetContentsOfCell("C1", "=A1+B1");  
        }

        // --- Tests for Get Cell Contents ---*
        /// <summary>
        ///   <para>
        ///     Make sure set cell accept unnormalized variable and output the expected content
        ///   </para>
        ///   <remarks>
        ///     a1 and A1 should be treated the same and return the same content
        ///   </remarks>
        /// </summary>
        [TestMethod]
        public void Spreadsheet_TestGetCellContents_Valid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5");
            Assert.AreEqual((double)5, s.GetCellContents("a1"));
            Assert.AreEqual((double)5, s.GetCellContents("A1"));
        }

        // --- Tests for GetCellContents With Unnormalized Names ---*
        /// <summary>
        ///   <para>
        ///     Make sure spreadsheet can accept both unnormalized and normalized character
        ///   </para>
        ///   <remarks>
        ///     A1, B1 and C1 are expected to be treated the same as a1, b1 and c1 so they are expected to return the same corresponding content
        ///   </remarks>
        /// </summary>
        [TestMethod]
        public void Spreadsheet_TestGetCellContentsWithNotNormalizedNames_Valid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "dog");
            s.SetContentsOfCell("B1", "cat");
            s.SetContentsOfCell("C1", "hippo");
            List<string> result = new List<string>(["A1", "B1", "C1"]);
            Assert.AreEqual("dog", s.GetCellContents("a1"));
            Assert.AreEqual("cat", s.GetCellContents("b1"));
            Assert.AreEqual("hippo", s.GetCellContents("c1"));
            
        }

        // --- Tests for GetEmptyCellContents ---*
        /// <summary>
        ///   <para>
        ///     Make sure an Empty Cell returns an empty string as content and does not show up when looking for nonempty cells
        ///   </para>
        ///   <remarks>
        ///     D1 is empty and should not register as a nonempty cell, so the count of GetNamesOfAllNonemptyCells is expected to be 0
        ///   </remarks>
        /// </summary>
        [TestMethod]
        public void Spreadsheet_TestGetEmptyCellContents_Valid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("D1", string.Empty);
            Assert.AreEqual(string.Empty, s.GetCellContents("D1"));
            List<string> result = new List<string>();
            Assert.AreEqual(result.Count, s.GetNamesOfAllNonemptyCells().Count);
        }


        // --- Tests for GetEmptyCellContents ---*
        /// <summary>
        ///   <para>
        ///     Make sure an Empty Cell returns an empty string as content and does not show up when looking for nonempty cells
        ///   </para>
        ///   <remarks>
        ///     D1 is empty and should have "dog" as content and C1 has "" as content which should also be an empty string and not register as a nonempty cell
        ///   </remarks>
        /// </summary>
        [TestMethod]
        public void Spreadsheet_TestGetEmptyCellContents_InValid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("D1", string.Empty);
            s.SetContentsOfCell("C1", "");
            Assert.AreNotEqual("dog", s.GetCellContents("D1"));
            List<string> result = new List<string>();
            Assert.AreNotEqual(1, s.GetNamesOfAllNonemptyCells().Count);
            
        }
    }
}