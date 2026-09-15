using System;
using System.Collections.Generic;

namespace ConsoleGraphicsEditor
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Graphical Editor");
            
            // Визначення максимально допустимої кількості моделей/об'єктів
            int maxObjects = SafeReadInt("Enter maximum number of objects N (N > 0): ");
            if (maxObjects <= 0) return;

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
            item.Name = Console.ReadLine()!;
            if (item.Name.Length < 2 || item.Name.Length > 20)
            {
                Console.WriteLine("Name must contain from 2 to 20 characters.");
                return;
            }

            item.Type = (ShapeType)SafeReadInt("Type (0-Rectangle, 1-Circle, 2-Triangle, 3-Line, 4-Text): ");

            item.X = SafeReadInt("X: ");

            item.Y = SafeReadInt("Y: ");

            item.Width = SafeReadInt("Width: ");

            item.Height = SafeReadInt("Height: ");

            item.Colour = (Colour)SafeReadInt("Colour (0-White, 1-Pink, 2-Red, 3-Orange, 4-Yellow, 5-Lime, 6-Green, 7-Cyan, 8-Blue, 9-Purple, 10-Black): ");

            item.SetIsVisible(SafeReadBool("Visible (0-false, 1-true): "));

            item.Symbol = SafeReadChar("Symbol: ");

            item.SetPrice(SafeReadDecimal("Price: "));

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
            Console.WriteLine("1-Name, 2-Type, 3-X, 4-Y, 5-Width, 6-Height, 7-Colour, 8-Visible, 9-Symbol, 10-Price, 11-Scale");

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
            int mode = SafeReadInt("Choose: ");

            if (mode == 1)
            {
                PrintTable(objects);
                int index = SafeReadInt("Delete object number: ");
                objects.RemoveAt(index - 1);
                Console.WriteLine("Object deleted.");
                return;
            }

            Console.WriteLine("Choose field to delete by:");
            Console.WriteLine("1-Name, 2-Type, 3-X, 4-Y, 5-Width, 6-Height, 7-Colour, 8-Visible, 9-Symbol, 10-Price, 11-Scale");
            int field = SafeReadInt("Field: ");

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
            int index = SafeReadInt("Select object number: ");
            if (index > objects.Count) return;
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
                    item.Move(dx, dy);
                    Console.WriteLine("Moved.");
                }
                else if (choice == 2)
                {
                    int w = SafeReadInt("Width: ");
                    int h = SafeReadInt("Height: ");
                    item.Resize(w, h);
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
                    return obj.Name.Equals(value, StringComparison.OrdinalIgnoreCase);
                case 2:
                    return int.TryParse(value, out int type) && obj.Type == (ShapeType)type;
                case 3:
                    return int.TryParse(value, out int x) && obj.X == x;
                case 4:
                    return int.TryParse(value, out int y) && obj.Y == y;
                case 5:
                    return int.TryParse(value, out int width) && obj.Width == width;
                case 6:
                    return int.TryParse(value, out int height) && obj.Height == height;
                case 7:
                    return int.TryParse(value, out int colour) && obj.Colour == (Colour)colour;
                case 8:
                    return bool.TryParse(value, out bool visible) && obj.GetIsVisible() == visible;
                case 9:
                    return value.Length > 0 && obj.Symbol == value[0];
                case 10:
                    return decimal.TryParse(value, out decimal price) && obj.GetPrice() == price;
                default:
                    return false;
            }
        }

        // Вивід даних об'єкта у вигляді таблиці
        static void PrintTable(List<GraphicModel> objects)
        {
            Console.WriteLine();
            Console.WriteLine("#  Name        Type     X    Y    Width  Height  Colour  Visible  Symbol  Price");
            Console.WriteLine(new string('-', 110));

            for (int i = 0; i < objects.Count; i++)
            {
                GraphicModel obj = objects[i];
                Console.WriteLine($"{i + 1, 2}|{obj.Name, -10}|{obj.Type, -8}|{obj.X, 4}|{obj.Y, 4}|{obj.Width, 6}|{obj.Height, 7}|{obj.Colour, -8}|{obj.GetIsVisible(), 8}|{obj.Symbol, 7}|{obj.GetPrice(), 7}");
            }
            Console.WriteLine();
        }

        // Безпечна конвертація текстового формату в числовий
        static int SafeReadInt(string prompt)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value))
                return value;
            Console.WriteLine("Invalid input. Please enter a valid integer.");
            return 0;
        }

        // Безпечна конвертація текстового формату в грошовий формат
        static decimal SafeReadDecimal(string prompt)
        {
            Console.Write(prompt);
            if (decimal.TryParse(Console.ReadLine(), out decimal value))
                return value;
            Console.WriteLine("Invalid input. Please enter a valid decimal number.");
            return 0;
        }

        // Безпечна конвертація текстового формату в булове значення
        static bool SafeReadBool(string prompt)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()!;
            if (input == "0")
                return false;
            if (input == "1")
                return true;
            Console.WriteLine("Invalid input. Please enter 0 for false or 1 for true.");
            return false;
        }

        // Безпечна конвертація текстового формату в символ
        static char SafeReadChar(string prompt)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()!;
            if (input.Length > 0)
                return input[0];
            Console.WriteLine("Invalid input. Please enter a character.");
            return '\0';
        }
    }
}
