using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleGraphicsEditor.Tests
{
    [DoNotParallelize]
    [TestClass]
    public class ProgramTests
    {
        private Shape A = null!;
        private Shape B = null!;
        private Shape C = null!;
        private List<Shape> shapes = null!;

        [TestInitialize]
        public void Setup()
        {
            Shape.Scale = 1.0;
            A = new Shape("A1", "First",
                new List<(short x, short y)> { (0, 0), (1, 1) }, (Color)1, '*', false);
            B = new Shape("B1", "Second",
                new List<(short x, short y)> { (0, 0), (4, 0), (0, 3) }, (Color)2, '#', false);
            C = new Shape("C1", "First",
                new List<(short x, short y)> { (0, 0), (2, 0), (2, 2), (0, 2) }, (Color)1, '#', true);
            shapes = new List<Shape> { A, B, C };
        }

        [TestCleanup]
        public void Cleanup()
        {
            shapes = null!;
            Shape.Scale = 1.0;
        }

        /*     IsStorageFull та TryAddShape     */

        [TestMethod]
        [DataRow(3, true)]
        [DataRow(4, false)]
        [DataRow(2, true)]
        [DataRow(0, true)]
        public void IsStorageFullTest(int maxObjects, bool expected)
        {
            // Act
            bool actual = Program.IsStorageFull(shapes, maxObjects);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsStorageFullTest_empty_list()
        {
            // Act
            bool actual = Program.IsStorageFull(new List<Shape>(), 1);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TryAddShapeTest_has_free_space()
        {
            // Arrange
            Shape extra = new Shape();

            // Act
            bool actual = Program.TryAddShape(shapes, extra, 4);

            // Assert
            Assert.IsTrue(actual);
            Assert.AreEqual(4, shapes.Count);
            Assert.AreSame(extra, shapes[3]);
        }

        [TestMethod]
        [DataRow(3)]
        [DataRow(0)]
        public void TryAddShapeTest_storage_full(int maxObjects)
        {
            // Arrange
            Shape extra = new Shape();

            // Act
            bool actual = Program.TryAddShape(shapes, extra, maxObjects);

            // Assert
            Assert.IsFalse(actual);
            Assert.AreEqual(3, shapes.Count);
            Assert.IsFalse(shapes.Contains(extra));
        }

        /*     FindShapes     */

        [TestMethod]
        public void FindShapesTest_two_different_fields()
        {
            // Act
            List<Shape> actual = Program.FindShapes(shapes, 1, "A1", 2, "First");

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreSame(A, actual[0]);
        }

        [TestMethod]
        public void FindShapesTest_same_field_twice()
        {
            // Act
            List<Shape> actual = Program.FindShapes(shapes, 2, "First", 2, "First");

            // Assert
            Assert.AreEqual(2, actual.Count);
            Assert.AreSame(A, actual[0]);
            Assert.AreSame(C, actual[1]);
        }

        [TestMethod]
        public void FindShapesTest_color_and_symbol()
        {
            // Act
            List<Shape> actual = Program.FindShapes(shapes, 4, "1", 7, "#");

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreSame(C, actual[0]);
        }

        [TestMethod]
        [DataRow(1, "A1", 2, "Second")]
        [DataRow(3, "abc", 3, "abc")]
        [DataRow(0, "x", 1, "A1")]
        [DataRow(1, "a1", 1, "a1")]
        [DataRow(5, "maybe", 5, "maybe")]
        public void FindShapesTest_no_results(int field1, string value1, int field2, string value2)
        {
            // Act
            List<Shape> actual = Program.FindShapes(shapes, field1, value1, field2, value2);

            // Assert
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void FindShapesTest_empty_list()
        {
            // Act
            List<Shape> actual = Program.FindShapes(new List<Shape>(), 1, "A1", 1, "A1");

            // Assert
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void FindShapesTest_does_not_change_source_list()
        {
            // Act
            List<Shape> actual = Program.FindShapes(shapes, 2, "First", 2, "First");

            // Assert
            Assert.AreEqual(3, shapes.Count);
            Assert.AreNotSame(shapes, actual);
        }

        /*     DeleteByNumber     */

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void DeleteByNumberTest_valid_number(int number)
        {
            // Arrange
            Shape target = shapes[number - 1];
            int before = Shape.ShapeCount;

            // Act
            bool actual = Program.DeleteByNumber(shapes, number);

            // Assert
            Assert.IsTrue(actual);
            Assert.AreEqual(2, shapes.Count);
            Assert.IsFalse(shapes.Contains(target));
            Assert.AreEqual(before - 1, (int)Shape.ShapeCount);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(4)]
        [DataRow(100)]
        public void DeleteByNumberTest_invalid_number(int number)
        {
            // Arrange
            int before = Shape.ShapeCount;

            // Act
            bool actual = Program.DeleteByNumber(shapes, number);

            // Assert
            Assert.IsFalse(actual);
            Assert.AreEqual(3, shapes.Count);
            Assert.AreEqual(before, (int)Shape.ShapeCount);
        }

        [TestMethod]
        public void DeleteByNumberTest_empty_list()
        {
            // Act
            bool actual = Program.DeleteByNumber(new List<Shape>(), 1);

            // Assert
            Assert.IsFalse(actual);
        }

        /*     DeleteByField     */

        [TestMethod]
        public void DeleteByFieldTest_several_matches()
        {
            // Arrange
            int before = Shape.ShapeCount;

            // Act
            int actual = Program.DeleteByField(shapes, 2, "First");

            // Assert
            Assert.AreEqual(2, actual);
            Assert.AreEqual(1, shapes.Count);
            Assert.AreSame(B, shapes[0]);
            Assert.AreEqual(before - 2, (int)Shape.ShapeCount);
        }

        [TestMethod]
        public void DeleteByFieldTest_single_match()
        {
            // Arrange
            int before = Shape.ShapeCount;

            // Act
            int actual = Program.DeleteByField(shapes, 1, "A1");

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(2, shapes.Count);
            Assert.IsFalse(shapes.Contains(A));
            Assert.AreEqual(before - 1, (int)Shape.ShapeCount);
        }

        [TestMethod]
        public void DeleteByFieldTest_by_curve()
        {
            // Act
            int actual = Program.DeleteByField(shapes, 5, "true");

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsFalse(shapes.Contains(C));
        }

        [TestMethod]
        [DataRow(1, "Nope")]
        [DataRow(3, "abc")]
        [DataRow(4, "99")]
        [DataRow(5, "maybe")]
        [DataRow(7, "")]
        [DataRow(0, "A1")]
        [DataRow(8, "A1")]
        public void DeleteByFieldTest_no_matches(int field, string value)
        {
            // Arrange
            int before = Shape.ShapeCount;

            // Act
            int actual = Program.DeleteByField(shapes, field, value);

            // Assert
            Assert.AreEqual(0, actual);
            Assert.AreEqual(3, shapes.Count);
            Assert.AreEqual(before, (int)Shape.ShapeCount);
        }

        [TestMethod]
        public void DeleteByFieldTest_empty_list()
        {
            // Act
            int actual = Program.DeleteByField(new List<Shape>(), 1, "A1");

            // Assert
            Assert.AreEqual(0, actual);
        }

        /*     Matches     */

        [TestMethod]
        [DataRow(1, "A1", true)]
        [DataRow(1, "a1", false)]
        [DataRow(2, "First", true)]
        [DataRow(2, "Second", false)]
        [DataRow(3, "2", true)]
        [DataRow(3, "3", false)]
        [DataRow(3, "abc", false)]
        [DataRow(4, "1", true)]
        [DataRow(4, "2", false)]
        [DataRow(4, "99", false)]
        [DataRow(4, "x", false)]
        [DataRow(5, "False", true)]
        [DataRow(5, "false", true)]
        [DataRow(5, "True", false)]
        [DataRow(5, "abc", false)]
        [DataRow(6, "True", true)]
        [DataRow(6, "false", false)]
        [DataRow(7, "*", true)]
        [DataRow(7, "#", false)]
        [DataRow(7, "", false)]
        [DataRow(0, "A", false)]
        [DataRow(8, "A", false)]
        public void MatchesTest(int field, string value, bool expected)
        {
            // Act
            bool actual = Program.Matches(A, field, value);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*     FitsCanvas     */

        [TestMethod]
        [DataRow(0, 0, true)]
        [DataRow(5, -5, true)]
        [DataRow(32766, 0, true)]
        [DataRow(32766, 32766, true)]
        [DataRow(-32768, 0, true)]
        [DataRow(0, -32768, true)]
        [DataRow(32767, 0, false)]
        [DataRow(0, 32767, false)]
        [DataRow(-32769, 0, false)]
        [DataRow(0, -32769, false)]
        public void FitsCanvasTest(int dx, int dy, bool expected)
        {
            // Act
            bool actual = Program.FitsCanvas(A, dx, dy);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FitsCanvasTest_does_not_move_shape()
        {
            // Act
            Program.FitsCanvas(A, 10, 10);

            // Assert
            Assert.AreEqual((short)0, A.Points[0].x);
            Assert.AreEqual((short)1, A.Points[1].y);
        }

        /*     FormatRow     */

        [TestMethod]
        public void FormatRowTest_pads_values()
        {
            // Arrange
            string[] values = { "1", "Name" };
            int[] widths = { 3, 6 };

            // Act
            string actual = Program.FormatRow(values, 2, widths);

            // Assert
            Assert.AreEqual("1   | Name   |", actual);
        }

        [TestMethod]
        public void FormatRowTest_value_longer_than_width()
        {
            // Arrange
            string[] values = { "Long" };
            int[] widths = { 2 };

            // Act
            string actual = Program.FormatRow(values, 1, widths);

            // Assert
            Assert.AreEqual("Long |", actual);
        }

        /*     GetConstructorSignature     */

        [TestMethod]
        [DataRow(1, "()")]
        [DataRow(2, "(name)")]
        [DataRow(3, "(name; description)")]
        [DataRow(4, "(name; description; points)")]
        [DataRow(5, "(name; description; points; color)")]
        [DataRow(6, "(name; description; points; color; symbol)")]
        [DataRow(7, "(name; description; points; color; symbol; isCurve)")]
        public void GetConstructorSignatureTest_valid_choice(int choice, string expected)
        {
            // Act
            string actual = Program.GetConstructorSignature(choice);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(8)]
        public void GetConstructorSignatureTest_invalid_choice(int choice)
        {
            // Act
            string actual = Program.GetConstructorSignature(choice);

            // Assert
            Assert.AreEqual("Unknown constructor", actual);
        }

        /*     BuildTable     */

        [TestMethod]
        public void BuildTableTest_one_shape()
        {
            // Arrange
            List<Shape> list = new List<Shape> { new Shape() };
            string expected =
                $"Table | shape count: {Shape.ShapeCount} | (Scale: {Shape.Scale})\n" +
                "# | Name    | Description    | Points | Color | Curve | Visible | Symbol |\n" +
                "1 | Unnamed | No description | 2      | White | False | True    | #      |\n";

            // Act
            string actual = Program.BuildTable(list);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void BuildTableTest_empty_list()
        {
            // Arrange
            string expected =
                $"Table | shape count: {Shape.ShapeCount} | (Scale: {Shape.Scale})\n" +
                "# | Name | Description | Points | Color | Curve | Visible | Symbol |\n";

            // Act
            string actual = Program.BuildTable(new List<Shape>());

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void BuildTableTest_rows_have_equal_length()
        {
            // Arrange
            shapes.Add(new Shape("LongerName", "Some long description text"));

            // Act
            string[] lines = Program.BuildTable(shapes).Split('\n', StringSplitOptions.RemoveEmptyEntries);

            // Assert
            Assert.AreEqual(2 + shapes.Count, lines.Length);
            for (int i = 2; i < lines.Length; i++)
            {
                Assert.AreEqual(lines[1].Length, lines[i].Length);
            }
        }

        [TestMethod]
        public void BuildTableTest_ends_with_line_break()
        {
            // Act
            string actual = Program.BuildTable(shapes);

            // Assert
            Assert.IsTrue(actual.EndsWith('\n'));
            Assert.IsFalse(actual.EndsWith("\n\n"));
        }

        [TestMethod]
        public void BuildTableTest_row_numbers_follow_list_order()
        {
            // Act
            string[] lines = Program.BuildTable(shapes).Split('\n', StringSplitOptions.RemoveEmptyEntries);

            // Assert
            StringAssert.StartsWith(lines[2], "1 | A1");
            StringAssert.StartsWith(lines[3], "2 | B1");
            StringAssert.StartsWith(lines[4], "3 | C1");
        }
    }
}