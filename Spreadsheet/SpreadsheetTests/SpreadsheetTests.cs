using CS3500.Formula;
using CS3500.Spreadsheet;
using System.Runtime.Intrinsics.X86;
using System.Text.Encodings.Web;
using System.Text.Json;
// <summary>
//   <para>
//    This unit test is for testing method in SpreadSheets
//   </para>
// <authors> [Kuanyu Chien] </authors>
// <date> [2024/09/20] </date>
// </summary>
namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        [TestMethod]
        public void GetNamesOfAllNonemptyCells_GetNameFromEmptySheet_sizeZero()
        {
            Spreadsheet sp = new Spreadsheet();
            Assert.AreEqual(0, sp.GetNamesOfAllNonemptyCells().Count);
        }

        [TestMethod]
        public void GetNamesOfAllNonemptyCells_SomeCellContentHaveBeenRemove()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.SetContentsOfCell("a1", "apple");
            sp.SetContentsOfCell("b1", "bat");
            sp.SetContentsOfCell("a1", "");
            sp.SetContentsOfCell("c1", "cat");
            sp.SetContentsOfCell("d1", "duck");
            sp.SetContentsOfCell("c1", "");
            Assert.AreEqual(2, sp.GetNamesOfAllNonemptyCells().Count);
        }

        [TestMethod]
        public void SetCellContents_SetDoubleInEmptySheet_sizeOne()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.SetContentsOfCell("a1", "0.1");
            Assert.AreEqual(1, sp.GetNamesOfAllNonemptyCells().Count);
        }

        [TestMethod]
        public void SetCellContents_SetMultipleDoubleInCell_sizeFive()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.SetContentsOfCell("a1", "0.1");
            sp.SetContentsOfCell("b1", " 0.1");
            sp.SetContentsOfCell("c1", "0.1");
            sp.SetContentsOfCell("D1", "0.1");
            sp.SetContentsOfCell("F1", "0.1");
            Assert.AreEqual(5, sp.GetNamesOfAllNonemptyCells().Count);
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("A1"));
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("B1"));
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("C1"));
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("D1"));
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("F1"));
            Assert.IsFalse(sp.GetNamesOfAllNonemptyCells().Contains("f1"));
        }

        [TestMethod]
        public void SetCellContents_RewriteContentDoubleCell()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.SetContentsOfCell("a1", "0.1");
            sp.SetContentsOfCell("b1", "0.2");
            sp.SetContentsOfCell("A1", "0.3");
            Assert.AreEqual(2, sp.GetNamesOfAllNonemptyCells().Count);
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("A1"));
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("B1"));
            Assert.AreEqual(0.3, sp.GetCellContents("A1"));
        }

        [TestMethod]
        public void SetCellContents_CheckRewriteContentDoubleCellReturnList()
        {
            Spreadsheet sp = new Spreadsheet();
            List<string> original_return_list = (List<string>)sp.SetContentsOfCell("a1", "0.1");
            sp.SetContentsOfCell("b1", "0.2");
            List<string> return_list = (List<string>)sp.SetContentsOfCell("A1", "0.3");
            Assert.AreEqual(1, original_return_list.Count);
            Assert.AreEqual(1, return_list.Count);
            Assert.IsTrue(original_return_list.Contains("A1"));
            Assert.IsTrue(return_list.Contains("A1"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContents_InvalidDoubleCellName()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("123", "0.1");
        }

        [TestMethod]
        public void SetCellContents_SetEmptyString()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "0.1");
            List<string> return_list = (List<string>)sp.SetContentsOfCell("A1", "");
            Assert.AreEqual(0, sp.GetNamesOfAllNonemptyCells().Count);
            Assert.AreEqual(1, return_list.Count);
            Assert.IsTrue(return_list.Contains("A1"));
        }

        [TestMethod]
        public void SetCellContents_SetTextInEmptySheet_sizeOne()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.SetContentsOfCell("a1", "hello");
            Assert.AreEqual(1, sp.GetNamesOfAllNonemptyCells().Count);
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("A1"));
        }

        [TestMethod]
        public void SetCellContents_SetMultipleTextInCell_sizeFive()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.SetContentsOfCell("a1", "hi");
            sp.SetContentsOfCell("b1", "hello");
            sp.SetContentsOfCell("c1", "cat");
            sp.SetContentsOfCell("D1", "dog");
            sp.SetContentsOfCell("F1", "food");
            Assert.AreEqual(5, sp.GetNamesOfAllNonemptyCells().Count);
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("A1"));
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("B1"));
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("C1"));
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("D1"));
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("F1"));
            Assert.IsFalse(sp.GetNamesOfAllNonemptyCells().Contains("f1"));
        }

        [TestMethod]
        public void SetCellContents_RewriteContentTextCell()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.SetContentsOfCell("a1", "apple");
            sp.SetContentsOfCell("b1", "bat");
            sp.SetContentsOfCell("A1", "cat");
            Assert.AreEqual(2, sp.GetNamesOfAllNonemptyCells().Count);
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("A1"));
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("B1"));
            Assert.AreEqual("cat", sp.GetCellContents("A1"));
        }

        [TestMethod]
        public void SetCellContents_CheckRewriteContentTextCellReturnList()
        {
            Spreadsheet sp = new Spreadsheet();
            List<string> original_return_list = (List<string>)sp.SetContentsOfCell("a1", "hi");
            sp.SetContentsOfCell("b1", "bat");
            List<string> return_list = (List<string>)sp.SetContentsOfCell("A1", "HI");
            Assert.AreEqual(1, original_return_list.Count);
            Assert.AreEqual(1, return_list.Count);
            Assert.IsTrue(original_return_list.Contains("A1"));
            Assert.IsTrue(return_list.Contains("A1"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContents_InvalidTextCellName()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("123", "hi");
        }

        [TestMethod]
        public void SetCellContents_SetFormulaInEmptySheet_sizeOne()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.SetContentsOfCell("a1", "=1+2");
            Assert.AreEqual(1, sp.GetNamesOfAllNonemptyCells().Count);
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("A1"));
        }

        [TestMethod]
        public void SetCellContents_SetMultipleFormulaInCell_sizeFive()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.SetContentsOfCell("a1", "=1-2");
            sp.SetContentsOfCell("b1", "=1-2");
            sp.SetContentsOfCell("c1", "=3+1");
            sp.SetContentsOfCell("D1", "=4*1");
            sp.SetContentsOfCell("F1", "=5/1");
            Assert.AreEqual(5, sp.GetNamesOfAllNonemptyCells().Count);
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("A1"));
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("B1"));
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("C1"));
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("D1"));
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("F1"));
            Assert.IsFalse(sp.GetNamesOfAllNonemptyCells().Contains("f1"));
        }

        [TestMethod]
        public void SetCellContents_RewriteContentFormulaCell()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.SetContentsOfCell("a1", "=1-1");
            sp.SetContentsOfCell("b1", "=1-1");
            sp.SetContentsOfCell("A1", "=1-2");
            Assert.AreEqual(2, sp.GetNamesOfAllNonemptyCells().Count);
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("A1"));
            Assert.IsTrue(sp.GetNamesOfAllNonemptyCells().Contains("B1"));
            Assert.IsTrue(sp.GetCellContents("A1").Equals(new Formula("1-2")));
        }

        [TestMethod]
        public void SetCellContents_CheckRewriteContentFormulaCellReturnList()
        {
            Spreadsheet sp = new Spreadsheet();
            List<string> original_return_list = (List<string>)sp.SetContentsOfCell("a1", "=1*2");
            sp.SetContentsOfCell("b1", "=2+1");
            List<string> return_list = (List<string>)sp.SetContentsOfCell("A1", "=3*2");
            Assert.AreEqual(1, original_return_list.Count);
            Assert.AreEqual(1, return_list.Count);
            Assert.IsTrue(original_return_list.Contains("A1"));
            Assert.IsTrue(return_list.Contains("A1"));
        }

        [TestMethod]
        public void SetCellContents_AddCellRelation()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.SetContentsOfCell("a1", "1.0");
            sp.SetContentsOfCell("B1", "5.0");
            sp.SetContentsOfCell("c1", "=a1+b1");
            sp.SetContentsOfCell("d1", "=C1*2");
            List<string> return_list = (List<string>)sp.SetContentsOfCell("a1", "2.0");
            Assert.AreEqual(3, return_list.Count);
            Assert.IsTrue(return_list.Contains("A1"));
            Assert.IsTrue(return_list.Contains("C1"));
            Assert.IsTrue(return_list.Contains("D1"));
        }

        [TestMethod]

        [ExpectedException(typeof(CircularException))]
        public void SetCellContents_AddCycledCellRelation()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.SetContentsOfCell("a1", "=c1");
            sp.SetContentsOfCell("b1", "=a1");
            sp.SetContentsOfCell("c1", "=b1");
            List<string> return_list = (List<string>)sp.SetContentsOfCell("a1", "2.0");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContents_InvalidFormulaCellName()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("123", "=1-2");
        }

        [TestMethod]
        public void SetCellContents_checkFormulaReturnList()
        {
            Spreadsheet sp = new Spreadsheet();
            List<string> return_list = (List<string>)sp.SetContentsOfCell("a1", "=1+1");
            Assert.AreEqual(1, return_list.Count);
            Assert.IsTrue(return_list.Contains("A1"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContent_InvalidCellName()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "0.1");
            sp.GetCellContents("abc");
        }

        [TestMethod]
        public void GetCellContent_GetFromDoubleCellContent()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "0.1");
            Assert.AreEqual(0.1, sp.GetCellContents("a1"));
        }

        [TestMethod]
        public void GetCellContent_GetFromTextCellContent()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "hello");
            Assert.AreEqual("hello", sp.GetCellContents("a1"));
        }

        [TestMethod]
        public void GetCellContent_GetFromFormulaCellContent()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "=1+1");
            Assert.IsTrue(sp.GetCellContents("a1").Equals(new Formula("1+1")));
        }

        [TestMethod]
        public void GetCellContent_GetFromEmptyCellContent()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "");
            Assert.AreEqual(sp.GetCellContents("a1"), "");
        }

        [TestMethod]
        public void SetCellContent_OverRideFormula_originalDependencyShouldBeDeleded()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "1");
            sp.SetContentsOfCell("a2", "2");
            sp.SetContentsOfCell("a3", "3");
            sp.SetContentsOfCell("b1", "=a1 + a2");
            sp.SetContentsOfCell("b1", "=a2 + a3");
            Assert.AreEqual(1, sp.SetContentsOfCell("a1", "100").Count);
        }

        [TestMethod]

        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellVaue_invokeInvalidNameException()
        {
            Spreadsheet sp = new();
            sp.GetCellValue("12d");
        }

        [TestMethod]
        public void GetCellVaue_getFromEmptyCell_returnEmptyString()
        {
            Spreadsheet sp = new();
            Assert.IsTrue("".Equals(sp.GetCellValue("a1")));
        }


        [TestMethod]
        public void GetCellVaue_getStringValue()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "hi");
            Assert.IsTrue("hi".Equals(sp.GetCellValue("a1")));
        }


        [TestMethod]
        public void GetCellVaue_getDoubleValue()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "2");
            Assert.AreEqual(2.0, sp.GetCellValue("a1"));
        }

        [TestMethod]
        public void GetCellVaue_getDoubleValueFromFormula()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "2.0");
            sp.SetContentsOfCell("a2", "=a1+5");
            Assert.AreEqual(7.0, sp.GetCellValue("a2"));
        }

        [TestMethod]
        public void GetCellVaue_getFormulaErrorValueFromFormula()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "2.0");
            sp.SetContentsOfCell("a2", "=a1+b1");
            Assert.IsTrue(sp.GetCellValue("a2") is FormulaError);
        }

        [TestMethod]
        public void this_name_getFromEmptyCell_returnEmptyString()
        {
            Spreadsheet sp = new();
            Assert.IsTrue("".Equals(sp["a2"]));
        }

        [TestMethod]
        public void this_name_getStringValue()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "hi");
            Assert.IsTrue("hi".Equals(sp["a1"]));
        }


        [TestMethod]
        public void this_name_getDoubleValue()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "2");
            Assert.AreEqual(2.0, sp["a1"]);
        }

        [TestMethod]
        public void this_name_getDoubleValueFromFormula()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "2.0");
            sp.SetContentsOfCell("a2", "=a1+5");
            Assert.AreEqual(7.0, sp["a2"]);
        }

        [TestMethod]
        public void this_name_getFormulaErrorValueFromFormula()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "2.0");
            sp.SetContentsOfCell("a2", "=a1+b1");
            Assert.IsTrue(sp["a2"] is FormulaError);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SetContentOfCell_SetInvalidFormulaWithTwoOperatorConnect_FormulaFormatException()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "=a1--0");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SetContentOfCell_SetInvalidFormulaWithEmptyFormula_FormulaFormatException()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "=");
        }

        [TestMethod]
        public void SetContentOfCell_returnCorrectOrderOfdependencyChain()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "1");
            sp.SetContentsOfCell("a2", "2");
            sp.SetContentsOfCell("a3", "3");
            sp.SetContentsOfCell("b1", "=a1 + a2 + a3 + 1");
            sp.SetContentsOfCell("c1", "=b1");
            Assert.AreEqual(7.0, sp.GetCellValue("c1"));
            LinkedList<string> expect = new LinkedList<string>();
            expect.AddFirst("C1");
            expect.AddFirst("B1");
            expect.AddFirst("A1");
            Assert.IsTrue(sp.SetContentsOfCell("A1", "100.0").SequenceEqual(expect));
            Assert.AreEqual(106.0, sp.GetCellValue("c1"));
        }

        [TestMethod]
        public void SetContentOfCell_OneVariableIsString_becomeFormulaError()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "hi");
            sp.SetContentsOfCell("a2", "2");
            sp.SetContentsOfCell("b1", "=a1 + a2");
            Assert.IsTrue(sp.GetCellValue("b1") is FormulaError);
        }

        [TestMethod]
        public void SetContentOfCell_ComplexcateVariableChain()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "1");
            sp.SetContentsOfCell("a2", "2");
            sp.SetContentsOfCell("a3", "3");
            sp.SetContentsOfCell("b1", "=a1 + a2 + a3");
            Assert.AreEqual(6.0, sp.GetCellValue("b1"));
            sp.SetContentsOfCell("b2", "=a1 + 1");
            Assert.AreEqual(2.0, sp.GetCellValue("b2"));
            sp.SetContentsOfCell("c1", "=b1*b2-a3");
            Assert.AreEqual(9.0, sp.GetCellValue("c1"));
            sp.SetContentsOfCell("d1", "=c1/3");
            Assert.AreEqual(3.0, sp.GetCellValue("d1"));

            sp.SetContentsOfCell("e1", "=d1");
            Assert.AreEqual(3.0, sp.GetCellValue("e1"));
        }

        [TestMethod]
        public void SetContentOfCell_VariableChain_breakTheDependencyWhenFirstSetNumber()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "=a2");
            sp.SetContentsOfCell("a2", "=a3");
            sp.SetContentsOfCell("a3", "=a4");
            sp.SetContentsOfCell("a4", "=a5");
            LinkedList<string> first_chain = new LinkedList<string>();
            LinkedList<string> last_chain = new LinkedList<string>();
            first_chain.AddFirst("A1");
            first_chain.AddFirst("A2");
            last_chain.AddFirst("A3");
            last_chain.AddFirst("A4");
            last_chain.AddFirst("A5");
            Assert.IsTrue(sp.SetContentsOfCell("A2", "101.0").SequenceEqual(first_chain));
            Assert.IsTrue(sp.SetContentsOfCell("A5", "100.0").SequenceEqual(last_chain));
        }

        [TestMethod]
        public void SetContentOfCell_ComplexcateVariableChainWithString_getFormulaError()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "1");
            sp.SetContentsOfCell("a2", "2");
            sp.SetContentsOfCell("a3", "hi");
            sp.SetContentsOfCell("b1", "=a1 + a2 + a3");
            Assert.IsTrue(sp.GetCellValue("b1") is FormulaError);
            sp.SetContentsOfCell("c1", "=b1*2");
            Assert.IsTrue(sp.GetCellValue("c1") is FormulaError);
        }

        [TestMethod]
        public void IsChanged_NewSpreadSheet_false()
        {
            Spreadsheet sp = new();
            Assert.IsFalse(sp.Changed);
        }

        [TestMethod]
        public void IsChanged_SetStringInCell_true()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a11", "hi");
            Assert.IsTrue(sp.Changed);
        }

        [TestMethod]
        public void IsChanged_SetDoubleInCell_true()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a11", "2.5");
            Assert.IsTrue(sp.Changed);
        }

        [TestMethod]
        public void IsChanged_SetFormulaInCell_true()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a11", "=a1 + 2");
            Assert.IsTrue(sp.Changed);
        }

        [TestMethod]
        public void Save_simpleSpreadSheet()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "hi");
            sp.SetContentsOfCell("b2", "2.0");
            sp.SetContentsOfCell("c3", "=b2+2");
            sp.Save("save.txt");
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            string json = JsonSerializer.Serialize(sp, jsonOptions);
            File.WriteAllText("save1.txt", json);
            string sp_save = File.ReadAllText("save.txt");
            string expect = File.ReadAllText("save1.txt");
            Assert.AreEqual(expect, sp_save);
        }

        [TestMethod]
        public void Save_emptySpreadSheet()
        {
            Spreadsheet sp = new();
            sp.Save("save.txt");
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            string json = JsonSerializer.Serialize(sp, jsonOptions);
            File.WriteAllText("save1.txt", json);
            string sp_save = File.ReadAllText("save.txt");
            string expect = File.ReadAllText("save1.txt");
            Assert.AreEqual(expect, sp_save);
        }


        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Save_saveInvalidPath_throwSpreadsheetReadWriteException()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("a1", "hi");
            sp.SetContentsOfCell("b2", "2.0");
            sp.SetContentsOfCell("c3", "=b2+2");
            sp.Save("/some/nonsense/path.txt");
        }

        [TestMethod]
        public void Spreadsheet_constructor_simpleSpreadSheet()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("A1", "hi");
            sp.SetContentsOfCell("B2", "2.0");
            sp.SetContentsOfCell("C3", "=b2+2");
            sp.Save("save.txt");
            Spreadsheet ss = new Spreadsheet("save.txt");

            Assert.AreEqual("hi", ss.GetCellValue("A1"));

            Assert.AreEqual(2.0, ss.GetCellValue("B2"));

            Assert.AreEqual(4.0, ss.GetCellValue("C3"));
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Spreadsheet_constructor_InvalidFilePath_throwSpreadsheetReadWriteException()
        {
            Spreadsheet sp = new();
            sp.SetContentsOfCell("A1", "hi");
            sp.SetContentsOfCell("B2", "2.0");
            sp.SetContentsOfCell("C3", "=b2+2");
            sp.Save("save.txt");
            Spreadsheet ss = new Spreadsheet("save5.txt");
        }


        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Spreadsheet_constructor_InvalidFileContent_throwSpreadsheetReadWriteException()
        {
            string textContent = "null";
            File.WriteAllText("save.txt", textContent);
            Spreadsheet ss = new Spreadsheet("save.txt");
        }
    }
}