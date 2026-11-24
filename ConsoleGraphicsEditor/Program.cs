using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ConsoleGraphicsEditor
{
    class Program
    {
        // Межі полотна
        // (Поки що - константи для уникнення неправильного вводу)
        const short MIN_GRID_WIDTH = short.MinValue;
        const short MAX_GRID_WIDTH = short.MaxValue;
        const short MIN_GRID_HEIGHT = short.MinValue;
        const short MAX_GRID_HEIGHT = short.MaxValue;

        static void Main(string[] args)
        {
            Console.WriteLine("Graphical Editor");

            // Визначення максимально допустимої кількості моделей/об'єктів
            int maxObjects = ReadIntInRange(0, short.MaxValue, "Enter maximum number of objects N (N > 0): ", "The number of objects");

            // Створення списку об'єктів
            List<Shape> objects = new List<Shape>();

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("1 - Add object");
                Console.WriteLine("2 - View all objects");
                Console.WriteLine("3 - Find object");
                Console.WriteLine("4 - Demonstrate behavior");
                Console.WriteLine("5 - Delete object");
                Console.WriteLine("6 - Demonstrate static methods");
                Console.WriteLine("0 - Exit");

                int choice = SafeReadInt("Choose: ");

                if (choice == 1)
                    AddObject(objects, maxObjects);
                else if (choice == 2)
                    ViewAll(objects);
                else if (choice == 3)
                    FindObject(objects);
                else if (choice == 4)
                    Demonstrate(objects);
                else if (choice == 5)
                    DeleteObject(objects);
                else if (choice == 6)
                    DemonstrateStaticMethods(objects);
                else
                    break;
            }

            Console.WriteLine("Program closed.");
            Console.ReadKey();
        }

        // Функція, яка додає об'єкт до списку
        static void AddObject(List<Shape> objects, int maxObjects)
        {
            if (objects.Count >= maxObjects)
            {
                Console.WriteLine("Storage is full.");
                return;
            }

            Console.WriteLine("1. Add shape using TryParse");
            Console.WriteLine("2. Add shape using constructor");
            int choose = ReadIntInRange(1, 2, "Choose: ", "The value for mode");

            switch (choose)
            {
                case 1:
                    Console.WriteLine("Enter object data in the format: name;description;x1,y1,x2,y2;color;symbol;isCurve");
                    Console.WriteLine("\nTip: You can enter as many points as you like in the format x1, y1, x2, y2, and so on.");
                    Console.WriteLine("Warning: Do not leave any field blank to ensure correct input, and always enter a point with two coordinates.\n");
                    Console.WriteLine("Color: (White - 0; Pink - 1; Red - 2; Orange - 3; Yellow - 4; Lime - 5; Green - 6; Cyan - 7; Blue - 8; Purple - 9; Black - 10)");
                    string input = Console.ReadLine()!;
                    if (Shape.TryParse(input, out Shape parsedObject))
                    {
                        objects.Add(parsedObject);
                        Console.WriteLine("Shape created using TryParse.");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Failed to parse shape. Please check the input format.");
                        return;
                    }
                case 2:
                    Console.WriteLine("1 - ()");
                    Console.WriteLine("2 - (name)");
                    Console.WriteLine("3 - (name;description)");
                    Console.WriteLine("4 - (name;description;points)");
                    Console.WriteLine("5 - (name;description;points;color)");
                    Console.WriteLine("6 - (name;description;points;color;symbol)");
                    Console.WriteLine("7 - (name;description;points;color;symbol;isCurve)");

                    int constructorChoice = ReadIntInRange(1, 7, "Constructor number: ", "The constructor number");
                    Shape item = CreateObjectBySelectedConstructor(constructorChoice);

                    objects.Add(item);
                    Console.WriteLine($"Shape created using constructor: {GetConstructorSignature(constructorChoice)}");
                    Console.WriteLine("Shape added.");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please select a valid option.");
                    return;
            }
        }
        // Функція, яка створює об'єкт Shape при виборі користувачем конструктора
        private static Shape CreateObjectBySelectedConstructor(int constructorChoice)
        {
            ReadConstructionData(constructorChoice, out string name, out string description, out List<(short x, short y)> points,
                out Color color, out char symbol, out bool isCurve);

            switch (constructorChoice)
            {
                case 1:
                    return new Shape();
                case 2:
                    return new Shape(name);
                case 3:
                    return new Shape(name, description);
                case 4:
                    return new Shape(name, description, points);
                case 5:
                    return new Shape(name, description, points, color);
                case 6:
                    return new Shape(name, description, points, color, symbol);
                case 7:
                    return new Shape(name, description, points, color, symbol, isCurve);
                default:
                    throw new ArgumentException("Unknown constructor choice.");
            }
        }
        // Функція, яка зчитує дані для створення об'єкта Shape в залежності від вибраного конструктора
        private static void ReadConstructionData(int constructorChoice, out string name, out string description, out List<(short x, short y)> points,
                out Color color, out char symbol, out bool isCurve)
        {
            name = default!;
            description = default!;
            points = [];
            color = 0;
            symbol = '\0';
            isCurve = false;

            if (constructorChoice >= 2)
            {
                name = ReadStringInRange(2, 20, "Name: ", "Name must contain from 2 to 20 characters.");
            }

            if (constructorChoice >= 3)
            {
                description = ReadStringInRange(0, 100, "Description: ", "Description must contain up to 100 characters.");
            }

            if (constructorChoice >= 4)
            {
                // Скільки точок створювати (Shape.Points вимагає щонайменше 2 точки)
                int countPoints = ReadIntInRange(2, short.MaxValue, "Count points in shape (at least 2): ", "The value for the list points");
                short x, y;
                for (int i = 0; i < countPoints; i++)
                {
                    x = (short)ReadIntInRange(0, MAX_GRID_WIDTH, "X: ");
                    y = (short)ReadIntInRange(0, MAX_GRID_HEIGHT, "Y: ");
                    points.Add((x, y));
                }
            }

            if (constructorChoice >= 5)
            {
                // Діапазон 0-10 відповідає перевірці у властивості Shape.Color
                color = (Color)ReadIntInRange(0, 10, "Color: (White - 0; Pink - 1; Red - 2; Orange - 3; Yellow - 4; Lime - 5; Green - 6; Cyan - 7; Blue - 8; Purple - 9; Black - 10): ", "The value for color");
            }

            if (constructorChoice >= 6)
            {
                symbol = SafeReadChar("Symbol: ");
            }

            if (constructorChoice == 7)
            {
                isCurve = SafeReadBool("Is curve (0/1): ");
            }
        }
        static string GetConstructorSignature(int constructorChoice)
        {
            switch (constructorChoice)
            {
                case 1:
                    return "()";
                case 2:
                    return "(name)";
                case 3:
                    return "(name; description)";
                case 4:
                    return "(name; description; points)";
                case 5:
                    return "(name; description; points; color)";
                case 6:
                    return "(name; description; points; color; symbol)";
                case 7:
                    return "(name; description; points; color; symbol; isCurve)";
                default:
                    return "Unknown constructor";
            }
        }

        // Функція для виведення всього списку об'єктів у вигляді таблиці
        static void ViewAll(List<Shape> shapes)
        {
            if (IsEmptyListShapes(shapes, "No shapes found.")) return;

            PrintTable(shapes);
        }
        // Вивід даних об'єкта у вигляді таблиці
        static void PrintTable(List<Shape> shapes)
        {
            string[] headers = { "#", "Name", "Description", "Points", "Color", "Curve", "Visible", "Symbol" };
            int countCols = headers.Length;

            // Даний параметр визначає, скільки простору займає заголовки або дані об'єкту в колонці таблиці
            int fixedWidth = 0;
            for (int i = 0; i < countCols; i++)
            {
                if (headers[i].Length > fixedWidth)
                {
                    fixedWidth = headers[i].Length;
                }
            }

            // Дані кожного об'єкта у вигляді рядка (порядок відповідає headers)
            List<string[]> rows = new List<string[]>();
            for (int i = 0; i < shapes.Count; i++)
            {
                Shape obj = shapes[i];
                rows.Add(new string[]
                {
                    (i + 1).ToString(),
                    obj.Name,
                    obj.Description,
                    obj.Points.Count.ToString(),
                    obj.Color.ToString(),
                    obj.IsCurve.ToString(),
                    obj.IsVisible.ToString(),
                    obj.Symbol.ToString()
                });
            }

            // Розрахунок розміру для колонок таблиці
            int[] colWidths = new int[countCols];
            for (int c = 0; c < countCols; c++)
            {
                int max = headers[c].Length;
                foreach (var row in rows)
                    if (row[c].Length > max) max = row[c].Length;
                colWidths[c] = max;
            }

            // Виведення таблиці
            Console.WriteLine($"Table | shape count: {Shape.ShapeCount} | (Scale: {Shape.Scale})");
            Console.WriteLine(FormatRow(headers, countCols, colWidths));
            foreach (var row in rows)
                Console.WriteLine(FormatRow(row, countCols, colWidths));
        }
        // Функція для форматування даних таблиці
        // countCols - кількість колонок таблиці
        // colWidths - масив, який містить ширину кожної колонки таблиці
        static string FormatRow(string[] values, int countCols, int[] colWidths)
        {
            var sb = new StringBuilder();
            for (int c = 0; c < countCols; c++)
            {
                sb.Append(values[c].PadRight(colWidths[c])).Append(" | ");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        // Знаходження об'єкта по двум його характеристикам
        static void FindObject(List<Shape> shapes)
        {
            if (IsEmptyListShapes(shapes, "No objects found")) return;

            Console.WriteLine("Choose 2 fields to search by:");
            Console.WriteLine("1-Name, 2-Description, 3-Points count, 4-Color, 5-Curve, 6-Visible, 7-Symbol");

            int field1 = SafeReadInt("First field: ");

            int field2 = SafeReadInt("Second field: ");

            string value1 = "";
            string value2 = "";

            List<Shape> result = new List<Shape>();

            if (!(field1 == field2))
            {
                Console.Write("Value for first field: ");
                value1 = Console.ReadLine()!;

                Console.Write("Value for second field: ");
                value2 = Console.ReadLine()!;
            }
            else
            {
                Console.WriteLine("Please, write one value for find.");
                Console.Write("Value for first field: ");
                value1 = Console.ReadLine()!;
                value2 = value1;
            }


            foreach (Shape sh in shapes)
            {
                if (Matches(sh, field1, value1) && Matches(sh, field2, value2))
                    result.Add(sh);
            }

            if (IsEmptyListShapes(result, "No results found")) return;

            PrintTable(result);
        }
        // Функція для видалення об'єкта/об'єктів по його характеристиці
        static void DeleteObject(List<Shape> shapes)
        {
            // При видаленні об'єкту - зменшуємо лічильник об'єктів на 1.
            if (IsEmptyListShapes(shapes, "No shapes found.")) return;

            Console.WriteLine("1 - Delete by number");
            Console.WriteLine("2 - Delete by characteristic");
            int mode = ReadIntInRange(1, 2, "Choose: ", "The value for mode");

            if (mode == 1)
            {
                PrintTable(shapes);
                int index = SafeReadInt("Delete object number: ");
                if (index > shapes.Count || index <= 0)
                {
                    Console.WriteLine("No objects matched.");
                    return;
                }
                shapes.RemoveAt(index - 1);
                Shape.DecrementShapeCount();
                Console.WriteLine("Object deleted.");
                return;
            }

            Console.WriteLine("Choose field to delete by:");
            Console.WriteLine("1-Name, 2-Description, 3-Points count, 4-Color, 5-Curve, 6-Visible, 7-Symbol");
            int field = ReadIntInRange(1, 7, "Field: ", "The value for field");

            Console.Write("Value: ");
            string value = Console.ReadLine()!;

            int count = 0;

            for (int i = shapes.Count - 1; i >= 0; i--)
            {
                if (Matches(shapes[i], field, value))
                {
                    shapes.RemoveAt(i);
                    Shape.DecrementShapeCount();
                    count++;
                }
            }

            if (count == 0)
            {
                Console.WriteLine("No objects matched.");
                return;
            }

            Console.WriteLine($"Deleted {count} object(s).");
        }
        // Функція, яка порівнює введене користувачем значення value та field з даними фігури й повертає чи відбувся збіг
        static bool Matches(Shape obj, int field, string value)
        {
            switch (field)
            {
                case 1:
                    return obj.Name == value;
                case 2:
                    return obj.Description == value;
                case 3:
                    return int.TryParse(value, out int pointsCount) && obj.Points.Count == pointsCount;
                case 4:
                    return int.TryParse(value, out int colour) && obj.Color == (Color)colour;
                case 5:
                    return bool.TryParse(value, out bool curve) && obj.IsCurve == curve;
                case 6:
                    return bool.TryParse(value, out bool visible) && obj.IsVisible == visible;
                case 7:
                    return value.Length > 0 && obj.Symbol == value[0];
                default:
                    return false;
            }
        }

        // Функція, яка демонструє поведінку фігури
        static void Demonstrate(List<Shape> shapes)
        {
            if (IsEmptyListShapes(shapes, "No shapes found.")) return;

            FindingShapeFromList(shapes, out Shape item);

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("1 - Move");
                Console.WriteLine("2 - Change point");
                Console.WriteLine("3 - Add point");
                Console.WriteLine("4 - Remove point");
                Console.WriteLine("5 - Toggle visibility");
                Console.WriteLine("6 - Toggle curve");
                Console.WriteLine("7 - Calculate area");
                Console.WriteLine("8 - Calculate perimeter");
                Console.WriteLine("9 - Show info");
                Console.WriteLine("0 - Back");

                int choice = SafeReadInt("Choose: ");

                if (choice == 0)
                    return;
                else if (choice == 1)
                {
                    // Межі -short.MaxValue..short.MaxValue гарантують, що зміщення можна безпечно інвертувати
                    int dx = ReadIntInRange(-short.MaxValue, short.MaxValue, "Horizontal shift: ", "Horizontal shift");
                    int dy = ReadIntInRange(-short.MaxValue, short.MaxValue, "Vertical shift: ", "Vertical shift");

                    // Усі точки фігури мають залишитись у межах полотна
                    bool fitsCanvas = true;
                    foreach (var p in item.Points)
                    {
                        int newX = p.x + dx;
                        int newY = p.y + dy;
                        if (newX < MIN_GRID_WIDTH || newX > MAX_GRID_WIDTH ||
                            newY < MIN_GRID_HEIGHT || newY > MAX_GRID_HEIGHT)
                        {
                            fitsCanvas = false;
                            break;
                        }
                    }

                    if (fitsCanvas)
                    {
                        item.MoveSelectedArea((short)dx, (short)dy);
                        Console.WriteLine("Moved using MoveSelectedArea(short offsetX, short offsetY).");
                        DemonstrateMoveOverloads(item, dx, dy);
                    }
                    else
                    {
                        Console.WriteLine("Shape collides with a bound canvas");
                    }
                }
                else if (choice == 2)
                {
                    for (int i = 0; i < item.Points.Count; i++)
                    {
                        Console.WriteLine($"{i}: ({item.Points[i].x}, {item.Points[i].y})");
                    }

                    int id = ReadIntInRange(0, item.Points.Count - 1, "Point index: ", "The point index");
                    short x = (short)ReadIntInRange(MIN_GRID_WIDTH, MAX_GRID_WIDTH, "X: ");
                    short y = (short)ReadIntInRange(MIN_GRID_HEIGHT, MAX_GRID_HEIGHT, "Y: ");

                    item.ChangePoint((ushort)id, x, y);
                    Console.WriteLine("Point changed.");
                }
                else if (choice == 3)
                {
                    short x = (short)ReadIntInRange(MIN_GRID_WIDTH, MAX_GRID_WIDTH, "X: ");
                    short y = (short)ReadIntInRange(MIN_GRID_HEIGHT, MAX_GRID_HEIGHT, "Y: ");

                    item.AddPoint(x, y);
                    Console.WriteLine("Point added.");
                }
                else if (choice == 4)
                {
                    for (int i = 0; i < item.Points.Count; i++)
                    {
                        Console.WriteLine($"{i}: ({item.Points[i].x}, {item.Points[i].y})");
                    }

                    int id = ReadIntInRange(0, item.Points.Count - 1, "Point index: ", "The point index");

                    item.RemovePoint((short)id);
                    Console.WriteLine("Point removed.");

                    // Форма з менш ніж двома точками некоректна, тому видаляється автоматично
                    if (item.Points.Count < 2)
                    {
                        shapes.Remove(item);
                        Shape.DecrementShapeCount();
                        Console.WriteLine("Shape has fewer than 2 points and was deleted automatically.");
                        return;
                    }
                }
                else if (choice == 5)
                {
                    item.ToggleVisibility();
                    Console.WriteLine("Visibility toggled.");
                }
                else if (choice == 6)
                {
                    item.ToggleCurveShape();
                    Console.WriteLine("Curve shape toggled.");
                }
                else if (choice == 7)
                {
                    Console.WriteLine($"Area = {item.Area().ToString("F2")}");
                    Console.WriteLine($"Type: {item.TypeName}");
                }
                else if (choice == 8)
                {
                    Console.WriteLine($"Area = {item.Perimeter.ToString("F2")}");
                    Console.WriteLine($"Type: {item.TypeName}");
                }
                else if (choice == 9)
                {
                    Console.WriteLine(item);
                }

            }
        }
        // функція, яка демонструє перевантажені методи MoveSelectedArea
        static void DemonstrateMoveOverloads(Shape item, int dx, int dy)
        {
            Console.WriteLine("Overloaded method demo for MoveSelectedArea:");

            Console.WriteLine("Calling MoveSelectedArea(short offsetX, short offsetY):");
            item.MoveSelectedArea((short)dx, (short)dy);
            Console.WriteLine($"Result (first point): ({item.Points[0].x}, {item.Points[0].y})");

            Console.WriteLine("Calling MoveSelectedArea(short offsetY):");
            item.MoveSelectedArea((short)-dx, (short)-dy);
            item.MoveSelectedArea((short)dy);
            Console.WriteLine($"Result (first point): ({item.Points[0].x}, {item.Points[0].y})");

            Console.WriteLine("Calling MoveSelectedArea():");
            item.MoveSelectedArea((short)-dy);
            item.MoveSelectedArea();
            Console.WriteLine($"Result (first point): ({item.Points[0].x}, {item.Points[0].y})");
        }

        // Функція, яка демонструє статичні методи класу Shape
        // Примітка - це окрема опція в меню - не частина функції Demonstrate.
        static void DemonstrateStaticMethods(List<Shape> shapes)
        {
            if (IsEmptyListShapes(shapes, "No shapes found.")) return;

            FindingShapeFromList(shapes, out Shape item);

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("1 - Get current scale");
                Console.WriteLine("2 - Set next scale");
                Console.WriteLine("3 - Get shape count");
                Console.WriteLine("0 - Back");
                int choice = SafeReadInt("Choose: ");
                if (choice == 0)
                    return;
                else if (choice == 1)
                {
                    Console.WriteLine($"Current scale: {Shape.Scale}");
                    DemonstrateScaledShape(item, Shape.Scale);
                }
                else if (choice == 2)
                {
                    double newScale = ReadDoubleInRange(0.1, 10.0, "New scale: ", "Scale");
                    Shape.Scale = newScale;
                    Console.WriteLine($"Scale set to: {Shape.Scale}");
                    DemonstrateScaledShape(item, newScale);
                }
                else if (choice == 3)
                {
                    Console.WriteLine($"Shape count: {Shape.ShapeCount}");
                }
            }
        }
        // Просте виведення збільшених значень
        static void DemonstrateScaledShape(Shape shape, double Scale)
        {
            Console.WriteLine($"Scaled canvas bounds: X = [{Shape.ScaleShape(MIN_GRID_WIDTH, Scale)}; {Shape.ScaleShape(MAX_GRID_WIDTH, Scale)}], " +
                              $"Y = [{Shape.ScaleShape(MIN_GRID_HEIGHT, Scale)}; {Shape.ScaleShape(MAX_GRID_HEIGHT, Scale)}]");
            Console.WriteLine("Scaled coordinates of shape points:");
            for (int i = 0; i < shape.Points.Count; i++)
            {
                Console.WriteLine($"{i}: X = {Shape.ScaleShape(shape.Points[i].x, Scale)}, " +
                                  $"Y = {Shape.ScaleShape(shape.Points[i].y, Scale)}");
            }
            Console.WriteLine("This is a demonstration of the static method ScaleShape(int value, double factor), " +
                              "not for actual scaling of the canvas.");
        }
        // Функція, яка відповідає за вибір фігури з представленого списку
        static void FindingShapeFromList(List<Shape> shapes, out Shape item)
        {
            PrintTable(shapes);
            int index = ReadIntInRange(1, shapes.Count, "Select shape number: ", "The index for list of shapes");
            item = shapes[index - 1];
        }
        // Функція, яка перевіряє чи нема елементів в списку фігур
        static bool IsEmptyListShapes(List<Shape> shapes, string message = "")
        {
            if (shapes.Count == 0)
            {
                Console.WriteLine(message);
                return true;
            }
            return false;
        }

        // Функція для відлову помилок при введенні рядка в певному діапозоні
        // enterText - текст, який відображається перед користувацьким вводом.
        static string ReadStringInRange(int minLength, int maxLength, string enterText = "", string errorMessage = "Invalid input. Please enter a valid string.")
        {
            while (true)
            {
                Console.Write(enterText);
                string input = Console.ReadLine()!;
                if (input.Length >= minLength && input.Length <= maxLength)
                {
                    return input;
                }
                Console.WriteLine(errorMessage);
            }
        }
        // Функція для відлову помилок при введенні числа в певному діапозоні
        static int ReadIntInRange(int minValue = 0, int maxValue = 0, string enterText = "", string nameParam = "The value")
        {
            int value = 0;

            while (true)
            {
                value = SafeReadInt(enterText);
                if (value < minValue || value > maxValue)
                {
                    Console.WriteLine($"Error! {nameParam} out of bounds!");
                }
                else
                {
                    return value;
                }
            }
        }
        // Функція для відлову помилок при введенні числа з плаваючою комою в певному діапозоні
        static double ReadDoubleInRange(double minValue = 0, double maxValue = 0, string enterText = "", string nameParam = "The value")
        {
            double value = 0;
            while (true)
            {
                value = SafeReadDouble(enterText);
                if (value < minValue || value > maxValue)
                {
                    Console.WriteLine($"Error! {nameParam} out of bounds!");
                }
                else
                {
                    return value;
                }
            }
        }

        // Безпечна конвертація текстового формату в числовий
        static int SafeReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int value))
                    return value;
                Console.WriteLine("Invalid input. Please enter a valid integer.");
            }
        }
        static double SafeReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), out double value))
                    return value;
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }
        // Безпечна конвертація текстового формату в грошовий формат
        static decimal SafeReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (decimal.TryParse(Console.ReadLine(), out decimal value))
                    return value;
                Console.WriteLine("Invalid input. Please enter a valid decimal number.");
            }
        }
        // Безпечна конвертація текстового формату в булове значення
        static bool SafeReadBool(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()!;
                if (input == "0" || input == "false" || input == "f") return false;
                if (input == "1" || input == "true" || input == "t") return true;
                Console.WriteLine("Invalid input. Please enter 0 for false or 1 for true.");
            }
        }
        // Безпечна конвертація текстового формату в символ
        static char SafeReadChar(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()!;
                if (input.Length == 1)
                    return input[0];
                Console.WriteLine("Invalid input. Please enter a valid character.");
            }
        }
    }
}