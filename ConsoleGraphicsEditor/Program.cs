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

            // Створюється модель
            GraphicModel item = new GraphicModel();

            // Заповнюються дані про дану модель
            Console.Write("Name: ");
            item.name = Console.ReadLine()!;
            if (item.name.Length < 2 || item.name.Length > 20)
            {
                Console.WriteLine("Name must contain from 2 to 20 characters.");
                return;
            }

            item.shapeType = (ShapeType)ReadIntInRange(0, 4, "Type (0-Rectangle, 1-Circle, 2-Triangle, 3-Line, 4-Text): ", "The value for the list of shapes");

            item.modelColour = (Colour)ReadIntInRange(0, 10, "Colour (0-White, 1-Pink, 2-Red, 3-Orange, 4-Yellow, 5-Lime, 6-Green, 7-Cyan, 8-Blue, 9-Purple, 10-Black): ",
                                                        "The value for the list of colours");

            item.symbol = SafeReadChar("Symbol: ");

            item.x = ReadIntInRange(0, gridWidth - 1, "X: ");

            item.y = ReadIntInRange(0, gridHeight - 1, "Y: ");

            item.width = ReadIntInRange(1, gridWidth - item.x, "Width: ");

            item.height = ReadIntInRange(1, gridHeight - item.y, "Height: ");

            item.SetIsVisible(SafeReadBool("Visible (0-false, 1-true): "));

            item.SetPrice(ReadPrice("Price: "));

            objects.Add(item);
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
                    if (item.x + item.width < gridWidth && item.y + item.width < gridHeight) item.Move(dx, dy);
                    else
                    {
                        Console.WriteLine("Model collides with a bound canvas");
                        return;
                    }
                    Console.WriteLine("Moved.");
                }
                else if (choice == 2)
                {
                    int w = SafeReadInt("Width: ");
                    int h = SafeReadInt("Height: ");
                    if (item.x + item.width < gridWidth && item.y + item.width < gridHeight) item.Resize(w, h);
                    else
                    {
                        Console.WriteLine("Model collides with a bound canvas");
                        return;
                    }
                    Console.WriteLine("Resized.");
                }
                else if (choice == 3)
                {
                    item.ChangeColour((Colour)SafeReadInt("New colour (0-10): "));
                    Console.WriteLine("Changed colour.");
                }
                else if (choice == 4)
                {
                    item.ToggleVisibility();
                    Console.WriteLine("Visibility toggled.");
                }
                else if (choice == 5)
                {
                    Console.WriteLine($"Area = {item.CalculateArea()}");
                }
                else if (choice == 6)
                {
                    Console.WriteLine(item);
                }
            }
        }

        // Фкнція, яка порівнює введене користувачем значення value та field з даними моделі й повертає чи відбулась зміна чи ні 
        static bool Matches(GraphicModel obj, int field, string value)
        {
            switch (field)
            {
                case 1:
                    return obj.name == value;
                case 2:
                    return int.TryParse(value, out int type) && obj.shapeType == (ShapeType)type;
                case 3:
                    return int.TryParse(value, out int x) && obj.x == x;
                case 4:
                    return int.TryParse(value, out int y) && obj.y == y;
                case 5:
                    return int.TryParse(value, out int width) && obj.width == width;
                case 6:
                    return int.TryParse(value, out int height) && obj.height == height;
                case 7:
                    return int.TryParse(value, out int colour) && obj.modelColour == (Colour)colour;
                case 8:
                    return bool.TryParse(value, out bool visible) && obj.GetIsVisible() == visible;
                case 9:
                    return value.Length > 0 && obj.symbol == value[0];
                case 10:
                    return decimal.TryParse(value, out decimal price) && obj.GetPrice() == price;
                default:
                    return false;
            }
        }

        // Вивід даних об'єкта у вигляді таблиці
        static void PrintTable(List<GraphicModel> objects)
        {
            string[] headers = { "#", "Name", "Type", "X", "Y", "Width", "Height", "Colour", "Visible", "Symbol", "Price"};
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
                    obj.name,
                    obj.shapeType.ToString(),
                    obj.x.ToString(),
                    obj.y.ToString(),
                    obj.width.ToString(),
                    obj.height.ToString(),
                    obj.modelColour.ToString(),
                    obj.GetIsVisible().ToString(),
                    obj.symbol.ToString(),
                    obj.GetPrice().ToString("N2")
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

        // Функція для відлову помилок при введенні числа в певному діапозоні
        // max - максимально допустиме значення, яке можна ввести
        // nameParam - Назва застосовуваного параметра
        // enterText - текст, який відображається перед користувацьким вводом.
        // nameParam - назва параметру, яка відобразиться в помилці
        static int ReadIntInRange(int min = 0, int max = 0, string enterText = "", string nameParam = "The value")
        {
            int value = 0;

            while (true)
            {
                value = SafeReadInt(enterText);
                if (value < min || value > max)
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
