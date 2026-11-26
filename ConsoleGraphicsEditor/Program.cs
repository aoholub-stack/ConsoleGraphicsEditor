using System.Text;

namespace ConsoleGraphicsEditor
{
    public class Program
    {
        // Межі полотна
        // (Поки що - константи для уникнення неправильного вводу)
        const short MIN_GRID_WIDTH = short.MinValue;
        const short MAX_GRID_WIDTH = short.MaxValue;
        const short MIN_GRID_HEIGHT = short.MinValue;
        const short MAX_GRID_HEIGHT = short.MaxValue;

        private static void Main(string[] args)
        {
            Console.WriteLine("Graphical Editor");

            // Визначення максимально допустимої кількості моделей/об'єктів
            int maxObjects = ReadIntInRange(1, short.MaxValue, "Enter maximum number of objects N (N > 0): ", "The number of objects");

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
                    AddShape(objects, maxObjects);
                else if (choice == 2)
                    ViewAll(objects);
                else if (choice == 3)
                    FindShape(objects);
                else if (choice == 4)
                    Demonstrate(objects);
                else if (choice == 5)
                    DeleteShape(objects);
                else if (choice == 6)
                    DemonstrateStaticMethods(objects);
                else
                    break;
            }

            Console.WriteLine("Program closed.");
            Console.ReadKey();
        }

        // Функція, яка додає об'єкт до списку
        private static void AddShape(List<Shape> shapes, int maxObjects)
        {
            if (IsStorageFull(shapes, maxObjects))
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
                        TryAddShape(shapes, parsedObject, maxObjects);
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

                    TryAddShape(shapes, item, maxObjects);
                    Console.WriteLine($"Shape created using constructor: {GetConstructorSignature(constructorChoice)}");
                    Console.WriteLine("Shape added.");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please select a valid option.");
                    return;
            }
        }
        public static bool TryAddShape(List<Shape> shapes, Shape shape, int maxObjects)
        {
            if (IsStorageFull(shapes, maxObjects)) return false;

            shapes.Add(shape);
            return true;
        }
        public static bool IsStorageFull(List<Shape> shapes, int maxObjects)
        {
            return shapes.Count >= maxObjects;
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
        public static string GetConstructorSignature(int constructorChoice)
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
        private static void ViewAll(List<Shape> shapes)
        {
            if (IsEmptyListShapes(shapes, "No shapes found.")) return;

            PrintTable(shapes);
        }
        // Вивід даних об'єкта у вигляді таблиці
        private static void PrintTable(List<Shape> shapes)
        {
            Console.Write(BuildTable(shapes));
        }
        // Формує таблицю фігур у вигляді тексту (без виводу в консоль)
        public static string BuildTable(List<Shape> shapes)
        {
            string[] headers = { "#", "Name", "Description", "Points", "Color", "Curve", "Visible", "Symbol" };
            int countCols = headers.Length;

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

            // Розрахунок ширини колонок
            int[] colWidths = new int[countCols];
            for (int c = 0; c < countCols; c++)
            {
                int max = headers[c].Length;
                foreach (var row in rows)
                    if (row[c].Length > max) max = row[c].Length;
                colWidths[c] = max;
            }

            List<string> lines = new List<string>
            {
                $"Table | shape count: {Shape.ShapeCount} | (Scale: {Shape.Scale})",
                FormatRow(headers, countCols, colWidths)
            };
            
            foreach (var row in rows)
            {
                lines.Add(FormatRow(row, countCols, colWidths));
            }

            string res = "";
            for (int i = 0; i < lines.Count; i++)
            {
                res += lines[i] + '\n';
            }

            return res;
        }
        // Функція для форматування даних таблиці
        // countCols - кількість колонок таблиці
        // colWidths - масив, який містить ширину кожної колонки таблиці
        public static string FormatRow(string[] values, int countCols, int[] colWidths)
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
        private static void FindShape(List<Shape> shapes)
        {
            if (IsEmptyListShapes(shapes, "No objects found")) return;

            Console.WriteLine("Choose 2 fields to search by:");
            Console.WriteLine("1-Name, 2-Description, 3-Points count, 4-Color, 5-Curve, 6-Visible, 7-Symbol");

            int field1 = SafeReadInt("First field: ");

            int field2 = SafeReadInt("Second field: ");

            string value1 = "";
            string value2 = "";

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

            List<Shape> result = FindShapes(shapes, field1, value1, field2, value2);

            if (IsEmptyListShapes(result, "No results found")) return;

            PrintTable(result);
        }
        public static List<Shape> FindShapes(List<Shape> shapes, int field1, string value1, int field2, string value2)
        {
            return shapes.FindAll(s => Matches(s, field1, value1) && Matches(s, field2, value2));
        }
        // Функція для видалення об'єкта/об'єктів по його характеристиці
        private static void DeleteShape(List<Shape> shapes)
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
                if (!DeleteByNumber(shapes, index))
                {
                    Console.WriteLine("No objects matched.");
                    return;
                }
                Console.WriteLine("Object deleted.");
                return;
            }

            Console.WriteLine("Choose field to delete by:");
            Console.WriteLine("1-Name, 2-Description, 3-Points count, 4-Color, 5-Curve, 6-Visible, 7-Symbol");
            int field = ReadIntInRange(1, 7, "Field: ", "The value for field");

            Console.Write("Value: ");
            string value = Console.ReadLine()!;

            int count = DeleteByField(shapes, field, value);

            if (count == 0)
            {
                Console.WriteLine("No objects matched.");
                return;
            }

            Console.WriteLine($"Deleted {count} object(s).");
        }
        // Видаляє фігуру за порядковим номером (з 1). Повертає false, якщо номер некоректний
        public static bool DeleteByNumber(List<Shape> shapes, int number)
        {
            if (number <= 0 || number > shapes.Count) return false;

            shapes.RemoveAt(number - 1);
            Shape.DecrementShapeCount();
            return true;
        }

        // Видаляє всі фігури за характеристикою. Повертає кількість видалених
        public static int DeleteByField(List<Shape> shapes, int field, string value)
        {
            int removed = shapes.RemoveAll(s => Matches(s, field, value));
            for (int i = 0; i < removed; i++)
            {
                Shape.DecrementShapeCount();
            }
            return removed;
        }
        // Функція, яка порівнює введене користувачем значення value та field з даними фігури й повертає чи відбувся збіг
        public static bool Matches(Shape shape, int field, string value)
        {
            switch (field)
            {
                case 1:
                    return shape.Name == value;
                case 2:
                    return shape.Description == value;
                case 3:
                    return int.TryParse(value, out int pointsCount) && shape.Points.Count == pointsCount;
                case 4:
                    return int.TryParse(value, out int colour) && shape.Color == (Color)colour;
                case 5:
                    return bool.TryParse(value, out bool curve) && shape.IsCurve == curve;
                case 6:
                    return bool.TryParse(value, out bool visible) && shape.IsVisible == visible;
                case 7:
                    return value.Length > 0 && shape.Symbol == value[0];
                default:
                    return false;
            }
        }

        // Функція, яка демонструє поведінку фігури
        private static void Demonstrate(List<Shape> shapes)
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
                    if (FitsCanvas(item, dx, dy))
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
                    Console.WriteLine($"Perimeter = {item.Perimeter.ToString("F2")}");
                    Console.WriteLine($"Type: {item.TypeName}");
                }
                else if (choice == 9)
                {
                    Console.WriteLine(item);
                }

            }
        }
        // Перевіряє, чи залишаться всі точки фігури в межах полотна після зміщення
        public static bool FitsCanvas(Shape shape, int dx, int dy)
        {
            foreach (var p in shape.Points)
            {
                int newX = p.x + dx;
                int newY = p.y + dy;
                if (newX < MIN_GRID_WIDTH || newX > MAX_GRID_WIDTH ||
                    newY < MIN_GRID_HEIGHT || newY > MAX_GRID_HEIGHT)
                {
                    return false;
                }
            }
            return true;
        }
        // функція, яка демонструє перевантажені методи MoveSelectedArea
        private static void DemonstrateMoveOverloads(Shape item, int dx, int dy)
        {
            Console.WriteLine("Overloaded method demo for MoveSelectedArea:");

            Console.WriteLine("Calling MoveSelectedArea(short offsetX, short offsetY):");
            item.MoveSelectedArea((short)dx, (short)dy);
            Console.WriteLine($"Result (first point): ({item.Points[0].x}, {item.Points[0].y})");

            Console.WriteLine("Calling MoveSelectedArea(short offsetX):");
            item.MoveSelectedArea((short)-dx, (short)-dy);
            item.MoveSelectedArea((short)dx);
            Console.WriteLine($"Result (first point): ({item.Points[0].x}, {item.Points[0].y})");

            Console.WriteLine("Calling MoveSelectedArea():");
            item.MoveSelectedArea((short)-dy);
            item.MoveSelectedArea();
            Console.WriteLine($"Result (first point): ({item.Points[0].x}, {item.Points[0].y})");
        }

        // Функція, яка демонструє статичні методи класу Shape
        // Примітка - це окрема опція в меню - не частина функції Demonstrate.
        private static void DemonstrateStaticMethods(List<Shape> shapes)
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
        private static void DemonstrateScaledShape(Shape shape, double Scale)
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
        private static void FindingShapeFromList(List<Shape> shapes, out Shape item)
        {
            PrintTable(shapes);
            int index = ReadIntInRange(1, shapes.Count, "Select shape number: ", "The index for list of shapes");
            item = shapes[index - 1];
        }
        // Функція, яка перевіряє чи нема елементів в списку фігур
        private static bool IsEmptyListShapes(List<Shape> shapes, string message = "")
        {
            if (shapes.Count == 0 || shapes == null)
            {
                Console.WriteLine(message);
                return true;
            }
            return false;
        }

        // Функція для відлову помилок при введенні рядка в певному діапозоні
        // enterText - текст, який відображається перед користувацьким вводом.
        private static string ReadStringInRange(int minLength, int maxLength, string enterText = "", string errorMessage = "Invalid input. Please enter a valid string.")
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
        private static int ReadIntInRange(int minValue = 0, int maxValue = 0, string enterText = "", string nameParam = "The value")
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
        private static double ReadDoubleInRange(double minValue = 0, double maxValue = 0, string enterText = "", string nameParam = "The value")
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
        private static int SafeReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int value))
                    return value;
                Console.WriteLine("Invalid input. Please enter a valid integer.");
            }
        }
        private static double SafeReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), out double value))
                    return value;
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }
        // Безпечна конвертація текстового формату в булове значення
        private static bool SafeReadBool(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()!.ToLower();
                if (input == "0" || input == "false" || input == "f") return false;
                if (input == "1" || input == "true" || input == "t") return true;
                Console.WriteLine("Invalid input. Please enter 0 for false or 1 for true.");
            }
        }
        // Безпечна конвертація текстового формату в символ
        private static char SafeReadChar(string prompt)
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