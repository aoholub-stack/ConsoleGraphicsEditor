using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleGraphicsEditor.Tests
{
    [DoNotParallelize]
    [TestClass]
    public class ShapeTests
    {
        private Shape shape = null!;

        [TestInitialize]
        public void Setup()
        {
            Shape.Scale = 1.0;
            shape = new Shape("Test", "Test description",
                new List<(short x, short y)> { (0, 0), (1, 1) }, (Color)1, '*', false);
        }

        [TestCleanup]
        public void Cleanup()
        {
            shape = null!;
            Shape.Scale = 1.0;
        }

        /*     Конструктори     */

        [TestMethod]
        public void ConstructorTest_default()
        {
            // Act
            Shape actual = new Shape();

            // Assert
            Assert.AreEqual("Unnamed", actual.Name);
            Assert.AreEqual("No description", actual.Description);
            Assert.AreEqual(2, actual.Points.Count);
            Assert.AreEqual('#', actual.Symbol);
            Assert.IsFalse(actual.IsCurve);
        }

        [TestMethod]
        public void ConstructorTest_name()
        {
            // Act
            Shape actual = new Shape("Alpha");

            // Assert
            Assert.AreEqual("Alpha", actual.Name);
            Assert.AreEqual("No description", actual.Description);
        }

        [TestMethod]
        public void ConstructorTest_name_description()
        {
            // Act
            Shape actual = new Shape("Alpha", "Some text");

            // Assert
            Assert.AreEqual("Alpha", actual.Name);
            Assert.AreEqual("Some text", actual.Description);
        }

        [TestMethod]
        public void ConstructorTest_points()
        {
            // Arrange
            var points = new List<(short x, short y)> { (0, 0), (4, 0), (0, 3) };

            // Act
            Shape actual = new Shape("Tri", "Triangle", points);

            // Assert
            Assert.AreEqual(3, actual.Points.Count);
            Assert.AreEqual('#', actual.Symbol);
        }

        [TestMethod]
        public void ConstructorTest_color()
        {
            // Arrange
            var points = new List<(short x, short y)> { (0, 0), (4, 0), (0, 3) };

            // Act
            Shape actual = new Shape("Tri", "Triangle", points, (Color)5);

            // Assert
            Assert.AreEqual(5, (int)actual.Color);
        }

        [TestMethod]
        public void ConstructorTest_symbol()
        {
            // Arrange
            var points = new List<(short x, short y)> { (0, 0), (4, 0), (0, 3) };

            // Act
            Shape actual = new Shape("Tri", "Triangle", points, (Color)5, '@');

            // Assert
            Assert.AreEqual('@', actual.Symbol);
        }

        [TestMethod]
        public void ConstructorTest_all_parameters()
        {
            // Arrange
            var points = new List<(short x, short y)> { (0, 0), (3, 4) };

            // Act
            Shape actual = new Shape("Circ", "Circle", points, (Color)7, '%', true);

            // Assert
            Assert.AreEqual("Circ", actual.Name);
            Assert.AreEqual(7, (int)actual.Color);
            Assert.AreEqual('%', actual.Symbol);
            Assert.IsTrue(actual.IsCurve);
        }

        [TestMethod]
        public void ConstructorTest_increments_shape_count()
        {
            // Arrange
            int before = Shape.ShapeCount;

            // Act
            Shape actual = new Shape();

            // Assert
            Assert.AreEqual(before + 1, (int)Shape.ShapeCount);
        }

        [TestMethod]
        public void ConstructorTest_one_point()
        {
            // Arrange
            var points = new List<(short x, short y)> { (0, 0) };

            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => new Shape("Bad", "One point", points));
        }

        [TestMethod]
        public void ConstructorTest_name_too_short()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Shape("A"));
        }

        /*     Секції set властивостей     */

        [TestMethod]
        [DataRow("ab")]
        [DataRow("Middle")]
        [DataRow("12345678901234567890")]
        public void NameSetTest_valid_length(string name)
        {
            // Act
            shape.Name = name;

            // Assert
            Assert.AreEqual(name, shape.Name);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("a")]
        [DataRow("123456789012345678901")]
        public void NameSetTest_invalid_length(string name)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => { shape.Name = name; });
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(5)]
        [DataRow(10)]
        public void ColorSetTest_in_range(int color)
        {
            // Act
            shape.Color = (Color)color;

            // Assert
            Assert.AreEqual(color, (int)shape.Color);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(11)]
        public void ColorSetTest_out_of_range(int color)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => { shape.Color = (Color)color; });
        }

        [TestMethod]
        [DataRow(0.1)]
        [DataRow(1.0)]
        [DataRow(10.0)]
        public void ScaleSetTest_positive(double scale)
        {
            // Act
            Shape.Scale = scale;

            // Assert
            Assert.AreEqual(scale, Shape.Scale, 0.001);
        }

        [TestMethod]
        [DataRow(0.0)]
        [DataRow(-2.5)]
        public void ScaleSetTest_not_positive(double scale)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => { Shape.Scale = scale; });
        }

        [TestMethod]
        public void PointsSetTest_three_points()
        {
            // Arrange
            var points = new List<(short x, short y)> { (0, 0), (1, 1), (2, 2) };

            // Act
            shape.Points = points;

            // Assert
            Assert.AreEqual(3, shape.Points.Count);
        }

        [TestMethod]
        public void PointsSetTest_one_point()
        {
            // Arrange
            var points = new List<(short x, short y)> { (5, 5) };

            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => { shape.Points = points; });
        }

        [TestMethod]
        public void PointsSetTest_null()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => { shape.Points = null!; });
        }

        [TestMethod]
        public void PointsGetTest_returns_copy()
        {
            // Arrange
            var copy = shape.Points;

            // Act
            copy.Add((9, 9));

            // Assert
            Assert.AreEqual(2, shape.Points.Count);
            Assert.AreNotSame(copy, shape.Points);
        }

        [TestMethod]
        public void SimplePropertiesSetTest()
        {
            // Act
            shape.Description = "New text";
            shape.Symbol = '$';
            shape.IsVisible = true;
            shape.IsCurve = true;

            // Assert
            Assert.AreEqual("New text", shape.Description);
            Assert.AreEqual('$', shape.Symbol);
            Assert.IsTrue(shape.IsVisible);
            Assert.IsTrue(shape.IsCurve);
        }

        /*     Тип, площа та периметр     */

        [TestMethod]
        public void AreaTest_circle()
        {
            // Arrange
            var points = new List<(short x, short y)> { (0, 0), (3, 4) };
            Shape circle = new Shape("Circle", "Test", points, (Color)0, '#', true);

            // Act
            double area = circle.Area();
            double perimeter = circle.Perimeter;

            // Assert
            Assert.AreEqual("Circle", circle.TypeName);
            Assert.AreEqual(25 * Math.PI, area, 0.001);
            Assert.AreEqual(10 * Math.PI, perimeter, 0.001);
        }

        [TestMethod]
        public void AreaTest_ellipse()
        {
            // Arrange
            var points = new List<(short x, short y)> { (2, 0), (0, 0), (0, 1) };
            Shape ellipse = new Shape("Ellipse", "Test", points, (Color)0, '#', true);

            // Act
            double area = ellipse.Area();
            double perimeter = ellipse.Perimeter;

            // Assert
            Assert.AreEqual("Ellipse", ellipse.TypeName);
            Assert.AreEqual(2 * Math.PI, area, 0.001);
            Assert.AreEqual(2 * Math.PI * Math.Sqrt(2.5), perimeter, 0.001);
        }

        [TestMethod]
        public void AreaTest_triangle()
        {
            // Arrange
            var points = new List<(short x, short y)> { (0, 0), (4, 0), (0, 3) };
            Shape triangle = new Shape("Triangle", "Test", points);

            // Act
            double area = triangle.Area();
            double perimeter = triangle.Perimeter;

            // Assert
            Assert.AreEqual("Triangle", triangle.TypeName);
            Assert.AreEqual(6, area, 0.001);
            Assert.AreEqual(12, perimeter, 0.001);
        }

        [TestMethod]
        public void AreaTest_square()
        {
            // Arrange
            var points = new List<(short x, short y)> { (0, 0), (2, 0), (2, 2), (0, 2) };
            Shape square = new Shape("Square", "Test", points);

            // Act
            double area = square.Area();
            double perimeter = square.Perimeter;

            // Assert
            Assert.AreEqual("Square", square.TypeName);
            Assert.AreEqual(4, area, 0.001);
            Assert.AreEqual(8, perimeter, 0.001);
        }

        [TestMethod]
        public void AreaTest_rectangle()
        {
            // Arrange
            var points = new List<(short x, short y)> { (0, 0), (4, 0), (4, 2), (0, 2) };
            Shape rectangle = new Shape("Rectangle", "Test", points);

            // Act
            double area = rectangle.Area();
            double perimeter = rectangle.Perimeter;

            // Assert
            Assert.AreEqual("Rectangle", rectangle.TypeName);
            Assert.AreEqual(8, area, 0.001);
            Assert.AreEqual(12, perimeter, 0.001);
        }

        [TestMethod]
        public void AreaTest_rhombus()
        {
            // Arrange
            var points = new List<(short x, short y)> { (0, 0), (2, 1), (4, 0), (2, -1) };
            Shape rhombus = new Shape("Rhombus", "Test", points);

            // Act
            double area = rhombus.Area();
            double perimeter = rhombus.Perimeter;

            // Assert
            Assert.AreEqual("Rhombus", rhombus.TypeName);
            Assert.AreEqual(4, area, 0.001);
            Assert.AreEqual(4 * Math.Sqrt(5), perimeter, 0.001);
        }

        [TestMethod]
        public void AreaTest_parallelogram()
        {
            // Arrange
            var points = new List<(short x, short y)> { (0, 0), (4, 0), (5, 2), (1, 2) };
            Shape parallelogram = new Shape("Parallel", "Test", points);

            // Act
            double area = parallelogram.Area();
            double perimeter = parallelogram.Perimeter;

            // Assert
            Assert.AreEqual("Parallelogram", parallelogram.TypeName);
            Assert.AreEqual(8, area, 0.001);
            Assert.AreEqual(8 + 2 * Math.Sqrt(5), perimeter, 0.001);
        }

        [TestMethod]
        public void AreaTest_custom_shape()
        {
            // Arrange
            var points = new List<(short x, short y)> { (0, 0), (4, 0), (4, 3), (2, 5), (0, 3) };
            Shape custom = new Shape("Custom", "Test", points);

            // Act
            double area = custom.Area();
            double perimeter = custom.Perimeter;

            // Assert
            Assert.AreEqual("Custom shape", custom.TypeName);
            Assert.AreEqual(16, area, 0.001);
            Assert.AreEqual(10 + 4 * Math.Sqrt(2), perimeter, 0.001);
        }

        [TestMethod]
        public void AreaTest_unknown_curve()
        {
            // Arrange
            var points = new List<(short x, short y)> { (0, 0), (1, 0), (1, 1), (0, 1) };
            Shape curve = new Shape("Curve", "Test", points, (Color)0, '#', true);

            // Act
            double area = curve.Area();

            // Assert
            Assert.AreEqual("Unknown", curve.TypeName);
            Assert.AreEqual(-1, area, 0.001);
        }

        [TestMethod]
        public void AreaTest_empty_shape()
        {
            // Arrange
            shape.RemovePoint(0);
            shape.RemovePoint(0);

            // Act
            double area = shape.Area();

            // Assert
            Assert.AreEqual(-1, area, 0.001);
            Assert.AreEqual(0, shape.Perimeter, 0.001);
        }

        /*     Поведінка форми     */

        [TestMethod]
        public void ToggleVisibilityTest()
        {
            // Arrange
            bool initial = shape.IsVisible;

            // Act
            shape.ToggleVisibility();

            // Assert
            Assert.AreEqual(!initial, shape.IsVisible);
        }

        [TestMethod]
        public void ToggleCurveShapeTest()
        {
            // Act
            shape.ToggleCurveShape();

            // Assert
            Assert.IsTrue(shape.IsCurve);

            // Act
            shape.ToggleCurveShape();

            // Assert
            Assert.IsFalse(shape.IsCurve);
        }

        [TestMethod]
        public void ChangeShapeTest_valid_index()
        {
            // Act
            shape.ChangePoint(1, 3, 4);

            // Assert
            Assert.AreEqual((short)3, shape.Points[1].x);
            Assert.AreEqual((short)4, shape.Points[1].y);
            Assert.AreEqual(10, shape.Perimeter, 0.001);
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(10)]
        public void ChangeShapeTest_invalid_index(int id)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => shape.ChangePoint((ushort)id, 1, 1));
        }

        [TestMethod]
        public void MoveSelectedAreaTest_both_offsets()
        {
            // Act
            shape.MoveSelectedArea(2, 3);

            // Assert
            Assert.AreEqual((short)2, shape.Points[0].x);
            Assert.AreEqual((short)3, shape.Points[0].y);
            Assert.AreEqual((short)3, shape.Points[1].x);
            Assert.AreEqual((short)4, shape.Points[1].y);
        }

        [TestMethod]
        public void MoveSelectedAreaTest_only_offset_x()
        {
            // Act
            shape.MoveSelectedArea(5);

            // Assert
            Assert.AreEqual((short)5, shape.Points[0].x);
            Assert.AreEqual((short)0, shape.Points[0].y);
        }

        [TestMethod]
        public void MoveSelectedAreaTest_no_arguments()
        {
            // Act
            shape.MoveSelectedArea();

            // Assert
            Assert.AreEqual((short)0, shape.Points[0].x);
            Assert.AreEqual((short)0, shape.Points[0].y);
        }

        [TestMethod]
        public void AddPointTest()
        {
            // Act
            shape.AddPoint(5, 5);

            // Assert
            Assert.AreEqual(3, shape.Points.Count);
            Assert.AreEqual((short)5, shape.Points[2].x);
            Assert.AreEqual(10 * Math.Sqrt(2), shape.Perimeter, 0.001);
        }

        [TestMethod]
        public void RemovePointTest_valid_index()
        {
            // Arrange
            var points = new List<(short x, short y)> { (0, 0), (4, 0), (0, 3) };
            Shape triangle = new Shape("Tri", "Test", points);

            // Act
            triangle.RemovePoint(1);

            // Assert
            Assert.AreEqual(2, triangle.Points.Count);
            Assert.AreEqual(6, triangle.Perimeter, 0.001);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(2)]
        [DataRow(100)]
        public void RemovePointTest_invalid_index(int index)
        {
            // Act
            shape.RemovePoint((short)index);

            // Assert
            Assert.AreEqual(2, shape.Points.Count);
        }

        [TestMethod]
        public void IsEmptyTest_with_points()
        {
            // Act
            bool actual = shape.IsEmpty();

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsEmptyTest_all_points_removed()
        {
            // Arrange
            shape.RemovePoint(0);
            shape.RemovePoint(0);

            // Act
            bool actual = shape.IsEmpty();

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void GetMinPointTest()
        {
            // Arrange
            shape.Points = new List<(short x, short y)> { (3, -2), (-1, 5), (0, 0) };

            // Act
            var actual = shape.GetMinPoint(0, 0);

            // Assert
            Assert.AreEqual((short)-1, actual.x);
            Assert.AreEqual((short)-2, actual.y);
        }

        [TestMethod]
        public void GetMaxPointTest()
        {
            // Arrange
            shape.Points = new List<(short x, short y)> { (3, -2), (-1, 5), (0, 0) };

            // Act
            var actual = shape.GetMaxPoint(0, 0);

            // Assert
            Assert.AreEqual((short)3, actual.x);
            Assert.AreEqual((short)5, actual.y);
        }

        [TestMethod]
        public void GetMinPointTest_empty_shape()
        {
            // Arrange
            shape.RemovePoint(0);
            shape.RemovePoint(0);

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => shape.GetMinPoint(0, 0));
        }
        [TestMethod]
        public void GetMaxPointTest_empty_shape()
        {
            // Arrange
            shape.RemovePoint(0);
            shape.RemovePoint(0);

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => shape.GetMaxPoint(0, 0));
        }

        /*     Рядкове представлення та розбір рядка     */

        [TestMethod]
        public void ToStringTest()
        {
            // Arrange
            string expected = "Test;Test description;0,0,1,1;" + shape.Color + ";*;False";

            // Act
            string actual = shape.ToString();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ToStringTest_empty_shape()
        {
            // Arrange
            shape.RemovePoint(0);
            shape.RemovePoint(0);

            // Act
            string actual = shape.ToString();

            // Assert
            Assert.AreEqual("", actual);
        }

        [TestMethod]
        public void ParseTest_valid_string()
        {
            // Act
            Shape actual = Shape.Parse("Tri;Desc;0,0,4,0,0,3;2;*;true");

            // Assert
            Assert.AreEqual("Tri", actual.Name);
            Assert.AreEqual("Desc", actual.Description);
            Assert.AreEqual(3, actual.Points.Count);
            Assert.AreEqual(2, (int)actual.Color);
            Assert.AreEqual('*', actual.Symbol);
            Assert.IsTrue(actual.IsCurve);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("a;b;c")]
        [DataRow("Na;Desc;0,0,1;1;*;false")]
        [DataRow("Na;;0,0,1,1;1;*;false")]
        [DataRow("Na;Desc;0,0,1,1;1;**;false")]
        [DataRow("Na;Desc;0,0,1,1;1;*;maybe")]
        public void ParseTest_wrong_format(string input)
        {
            // Act + Assert
            Assert.ThrowsExactly<FormatException>(() => Shape.Parse(input));
        }

        [TestMethod]
        public void TryParseTest_valid_string()
        {
            // Act
            bool actual = Shape.TryParse("Tri;Desc;0,0,4,0,0,3;2;*;false", out Shape result);

            // Assert
            Assert.IsTrue(actual);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("a;b;c")]
        [DataRow("Na;Desc;0,0,1;1;*;false")]
        [DataRow("N;Desc;0,0,1,1;1;*;false")]
        [DataRow("Na;Desc;0,0;1;*;false")]
        [DataRow("Na;Desc;0,0,1,1;99;*;false")]
        public void TryParseTest_invalid_string(string input)
        {
            // Act
            bool actual = Shape.TryParse(input, out Shape result);

            // Assert
            Assert.IsFalse(actual);
            Assert.IsNull(result);
        }

        /*     Статичні методи     */

        [TestMethod]
        [DataRow(10, 2.5, 25.0)]
        [DataRow(-4, 0.5, -2.0)]
        [DataRow(0, 3.0, 0.0)]
        public void ScaleShapeTest(int value, double factor, double expected)
        {
            // Act
            double actual = Shape.ScaleShape(value, factor);

            // Assert
            Assert.AreEqual(expected, actual, 0.001);
        }

        [TestMethod]
        public void DecrementShapeCountTest()
        {
            // Arrange
            int before = Shape.ShapeCount;

            // Act
            int actual = Shape.DecrementShapeCount();

            // Assert
            Assert.AreEqual(before - 1, actual);
        }
    }
}