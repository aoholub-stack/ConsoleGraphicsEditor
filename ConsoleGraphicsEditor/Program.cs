using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleGraphicsEditor
{
    class Program
    {
        // Межі полотна
        static int gridWidth;
        static int gridHeight;
        static void Main(string[] args)
        {
            Console.WriteLine("Graphical Editor");

            // Визначення максимально допустимої кількості моделей/об'єктів
            int maxObjects = ReadIntInRange(1, 1000, "Enter maximum number of objects N (N > 0): ", "The number of objects");

            // Визначення розмірів полотна
            gridWidth = ReadIntInRange(1, 1000, "Enter Max width for canvas: ", "The width of the canvas");
            gridHeight = ReadIntInRange(1, 1000, "Enter Max Height for canvas: ", "The Height of the canvas");

            // Створення списку об'єктів
            List<GraphicModel> objects = new List<GraphicModel>();

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("1 - Add object");
                Console.WriteLine("2 - View all objects");
                Console.WriteLine("3 - Find object");
                Console.WriteLine("4 - Demonstrate behavior");
                Console.WriteLine("5 - Delete object");
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
                else
                    break;
            }

            Console.WriteLine("Program closed.");
        }

        // Функція, яка додає об'єкт до списку
        static void AddObject(List<GraphicModel> objects, int maxObjects)
        {
            if (objects.Count >= maxObjects)
            {
                Console.WriteLine("Storage is full.");
                return;
            }

            Console.WriteLine("Choose a constructor:");
            Console.WriteLine("1 - ()");
            Console.WriteLine("2 - (name)");
            Console.WriteLine("3 - (name; description)");
            Console.WriteLine("4 - (name; description; shapeType; colour)");
            Console.WriteLine("5 - (name; description; shapeType; coordinates: x, y; width; height; colour)");
            Console.WriteLine("6 - (name; description; shapeType; coordinates: x, y; width; height; colour; symbol)");
            Console.WriteLine("7 - (name; description; shapeType; coordinates: x, y; width; height; colour; symbol; price)");

            int constructorChoice = ReadIntInRange(1, 7, "Constructor number: ", "The constructor number");
            GraphicModel item = CreateObjectBySelectedConstructor(constructorChoice);

            objects.Add(item);
            Console.WriteLine($"Object created using constructor: {GetConstructorSignature(constructorChoice)}");
            Console.WriteLine("Object added.");
        }

        // Функція для виведення всього списку об'єктів у вигляді таблиці
        static void ViewAll(List<GraphicModel> objects)
        {
            if (objects.Count == 0)
            {
                Console.WriteLine("No objects found.");
                return;
            }

            PrintTable(objects);
        }

        // Знаходження об'єкта по двум його характеристикам
        static void FindObject(List<GraphicModel> objects)
        {
            if (objects.Count == 0)
            {
                Console.WriteLine("No objects found.");
                return;
            }

            Console.WriteLine("Choose 2 fields to search by:");
            Console.WriteLine("1-Name, 2-Type, 3-X, 4-Y, 5-Width, 6-Height, 7-Colour, 8-Visible, 9-Symbol, 10-Price");

            int field1 = SafeReadInt("First field: ");

            int field2 = SafeReadInt("Second field: ");

            Console.Write("Value for first field: ");
            string value1 = Console.ReadLine()!;

            Console.Write("Value for second field: ");
            string value2 = Console.ReadLine()!;

            List<GraphicModel> result = new List<GraphicModel>();
            foreach (GraphicModel obj in objects)
            {
                if (Matches(obj, field1, value1) && Matches(obj, field2, value2))
                    result.Add(obj);
            }

            if (result.Count == 0)
            {
                Console.WriteLine("No results found.");
                return;
            }

            PrintTable(result);
        }

        // Функція для видалення об'єкта/об'єктів по його характеристиці
        static void DeleteObject(List<GraphicModel> objects)
        {
            if (objects.Count == 0)
            {
                Console.WriteLine("No objects found.");
                return;
            }

            Console.WriteLine("1 - Delete by number");
            Console.WriteLine("2 - Delete by characteristic");
            int mode = ReadIntInRange(1, 2, "Choose: ", "The value for mode");

            if (mode == 1)
            {
                PrintTable(objects);
                int index = SafeReadInt("Delete object number: ");
                if (index > objects.Count || index <= 0)
                {
                    Console.WriteLine("No objects matched.");
                    return;
                }
                objects.RemoveAt(index - 1);
                Console.WriteLine("Object deleted.");
                return;
            }

            Console.WriteLine("Choose field to delete by:");
            Console.WriteLine("1-Name, 2-Type, 3-X, 4-Y, 5-Width, 6-Height, 7-Colour, 8-Visible, 9-Symbol, 10-Price");
            int field = ReadIntInRange(1, 10, "Field: ", "The value for field");

            Console.Write("Value: ");
            string value = Console.ReadLine()!;

            int count = 0;

            for (int i = objects.Count - 1; i >= 0; i--)
            {
                if (Matches(objects[i], field, value))
                {
                    objects.RemoveAt(i);
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

        // Функція, яка демонструє поведінку моделі
        static void Demonstrate(List<GraphicModel> objects)
        {
            if (objects.Count == 0)
            {
                Console.WriteLine("No objects found.");
                return;
            }

            PrintTable(objects);
            int index = ReadIntInRange(1, objects.Count, "Select object number: ", "The index for list of objects");
            GraphicModel item = objects[index - 1];

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("1 - Move");
                Console.WriteLine("2 - Resize");
                Console.WriteLine("3 - Change colour");
                Console.WriteLine("4 - Toggle visibility");
                Console.WriteLine("5 - Calculate area");
                Console.WriteLine("6 - Show info");
                Console.WriteLine("0 - Back");

                int choice = SafeReadInt("Choose: ");

                if (choice == 0)
                    return;
                else if (choice == 1)
                {
                    int dx = SafeReadInt("Horizontal shift: ");
                    int dy = SafeReadInt("Vertical shift: ");

                    int newX = item.X + dx;
                    int newY = item.Y + dy;

                    if (newX >= 0 && newY >= 0 &&
                        newX + item.Width <= gridWidth &&
                        newY + item.Height <= gridHeight)
                    {
                        item.MoveModel(dx, dy);
                        Console.WriteLine("Moved using MoveModel(int offsetX, int offsetY).");
                        DemonstrateMoveOverloads(item, dx, dy);
                    }
                    else
                    {
                        Console.WriteLine("Model collides with a bound canvas");
                    }
                }
                else if (choice == 2)
                {
                    int w = SafeReadInt("Width: ");
                    int h = SafeReadInt("Height: ");

                    if (w < 1 || h < 1)
                    {
                        Console.WriteLine("Size must be at least 1x1");
                    }
                    else if (item.X + w > gridWidth || item.Y + h > gridHeight)
                    {
                        Console.WriteLine("Model collides with a bound canvas");
                    }
                    else
                    {
                        item.ResizeModel(w, h);
                        Console.WriteLine("Resized.");
                    }
                }
                else if (choice == 3)
                {
                    item.ModelColour = (Colour)SafeReadInt("New colour (0-10): ");
                    Console.WriteLine("Changed colour.");
                }
                else if (choice == 4)
                {
                    item.ToggleVisibilityModel();
                    Console.WriteLine("Visibility toggled.");
                }
                else if (choice == 5)
                {
                    Console.WriteLine($"Area = {item.Area}");
                }
                else if (choice == 6)
                {
                    Console.WriteLine(item);
                }
            }
        }

        // Функція, яка створює об'єкт GraphicModel при виборі користувачем конструктора
        static GraphicModel CreateObjectBySelectedConstructor(int constructorChoice)
        {
            ReadConstructionData(constructorChoice, out string name, out string description, out ShapeType shapeType,
                out int x, out int y, out int width, out int height,
                out Colour colour, out char symbol, out decimal price);

            switch (constructorChoice)
            {
                case 1:
                    return new GraphicModel();
                case 2:
                    return new GraphicModel(name);
                case 3:
                    return new GraphicModel(name, description);
                case 4:
                    return new GraphicModel(name, description, shapeType, colour);
                case 5:
                    return new GraphicModel(name, description, shapeType, x, y, width, height, colour);
                case 6:
                    return new GraphicModel(name, description, shapeType, x, y, width, height, colour, symbol);
                case 7:
                    return new GraphicModel(name, description, shapeType, x, y, width, height, colour, symbol, price);
                default:
                    throw new ArgumentException("Unknown constructor choice.");
            }
        }

        // Функція, яка зчитує дані для створення об'єкта GraphicModel в залежності від вибраного конструктора
        static void ReadConstructionData(int constructorChoice, out string name, out string description, out ShapeType shapeType,
            out int x, out int y, out int width, out int height,
            out Colour colour, out char symbol, out decimal price)
        {
            name = default!;
            description = default!;
            shapeType = default;
            x = 0;
            y = 0;
            width = 0;
            height = 0;
            colour = default;
            symbol = '\0';
            price = 0m;

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
                shapeType = (ShapeType)ReadIntInRange(0, 4, "Type (0-Rectangle, 1-Circle, 2-Triangle, 3-Line, 4-Text): ", "The value for the list of shapes");
            }

            if (constructorChoice >= 4)
            {
                colour = (Colour)ReadIntInRange(0, 10, "Colour (0-White, 1-Pink, 2-Red, 3-Orange, 4-Yellow, 5-Lime, 6-Green, 7-Cyan, 8-Blue, 9-Purple, 10-Black): ",
                                               "The value for the list of colours");
            }

            if (constructorChoice >= 5)
            {
                x = ReadIntInRange(0, gridWidth - 1, "X: ");
                y = ReadIntInRange(0, gridHeight - 1, "Y: ");
                width = ReadIntInRange(1, gridWidth - x, "Width: ");
                height = ReadIntInRange(1, gridHeight - y, "Height: ");
            }

            if (constructorChoice >= 6)
            {
                symbol = SafeReadChar("Symbol: ");
            }

            if (constructorChoice == 7)
            {
                price = ReadPrice("Price: ");
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
                    return "(name; description; shapeType; coordinates: x, y; width, height; modelColour)";
                case 5:
                    return "(name; description; shapeType; coordinates: x, y; width, height; modelColour)";
                case 6:
                    return "(name; description; shapeType; coordinates: x, y; width, height; modelColour; symbol)";
                case 7:
                    return "(name; description; shapeType; coordinates: x, y; width, height; modelColour; symbol; price)";
                default:
                    return "Unknown constructor";
            }
        }

        // функція, яка демонструє перевантажені методи MoveModel
        static void DemonstrateMoveOverloads(GraphicModel item, int dx, int dy)
        {
            Console.WriteLine("Overloaded method demo for MoveModel:");

            Console.WriteLine("Calling MoveModel(int offsetX, int offsetY):");
            item.MoveModel(dx, dy);
            Console.WriteLine($"Result: ({item.X}, {item.Y})");

            Console.WriteLine("Calling MoveModel(int offsetY):");
            item.MoveModel(-dx, -dy);
            item.MoveModel(dy);
            Console.WriteLine($"Result: ({item.X}, {item.Y})");

            Console.WriteLine("Calling MoveModel():");
            item.MoveModel(-dy);
            item.MoveModel();
            Console.WriteLine($"Result: ({item.X}, {item.Y})");
        }

        // Фкнція, яка порівнює введене користувачем значення value та field з даними моделі й повертає чи відбулась зміна чи ні 
        static bool Matches(GraphicModel obj, int field, string value)
        {
            switch (field)
            {
                case 1:
                    return obj.Name == value;
                case 2:
                    return int.TryParse(value, out int type) && obj.ShapeType == (ShapeType)type;
                case 3:
                    return int.TryParse(value, out int x) && obj.X == x;
                case 4:
                    return int.TryParse(value, out int y) && obj.Y == y;
                case 5:
                    return int.TryParse(value, out int width) && obj.Width == width;
                case 6:
                    return int.TryParse(value, out int height) && obj.Height == height;
                case 7:
                    return int.TryParse(value, out int colour) && obj.ModelColour == (Colour)colour;
                case 8:
                    return bool.TryParse(value, out bool visible) && obj.IsVisible == visible;
                case 9:
                    return value.Length > 0 && obj.Symbol == value[0];
                case 10:
                    return decimal.TryParse(value, out decimal price) && obj.Price == price;
                default:
                    return false;
            }
        }

        // Вивід даних об'єкта у вигляді таблиці
        static void PrintTable(List<GraphicModel> objects)
        {
            string[] headers = { "#", "Name", "Description", "Type", "X", "Y", "Width", "Height", "Colour", "Visible", "Symbol", "Price"};
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
            for (int i = 0; i < objects.Count; i++)
            {
                GraphicModel obj = objects[i];
                rows.Add(new string[]
                {
                    (i + 1).ToString(),
                    obj.Name,
                    obj.Description,
                    obj.ShapeType.ToString(),
                    obj.X.ToString(),
                    obj.Y.ToString(),
                    obj.Width.ToString(),
                    obj.Height.ToString(),
                    obj.ModelColour.ToString(),
                    obj.IsVisible.ToString(),
                    obj.Symbol.ToString(),
                    obj.Price.ToString("N2")
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
            Console.WriteLine();
            Console.WriteLine(FormatRow(headers, countCols, colWidths));
            foreach (var row in rows)
                Console.WriteLine(FormatRow(row, countCols, colWidths));
            Console.WriteLine();
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
        // maxValue - максимально допустиме значення, яке можна ввести
        // minValue - мінімально допустиме значення, яке можна ввести
        // enterText - текст, який відображається перед користувацьким вводом.
        // nameParam - назва параметру, яка відобразиться в помилці
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

        // Перевірка того, чи ввів користувач правильну ціну.
        static decimal ReadPrice(string prompt)
        {
            decimal price = 0;
            while (true)
            {
                price = SafeReadDecimal(prompt);
                if (price >= 0)
                {
                    return price;
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
                if (input == "0")
                    return false;
                if (input == "1")
                    return true;
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
