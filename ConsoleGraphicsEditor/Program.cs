using System;
using System.Collections.Generic;

namespace ConsoleGraphicsEditor
{
    internal class Program
    {
        // Розміри полотна, на якому відбуваються операції над об'єктами
        const int GridWidth = 30;
        const int GridHeight = 15;

        // Список об'єктів
        static readonly List<GraphicalModel> Models = new List<GraphicalModel>();

        // Масив з кольорами від enum. Використовується для перерахування кольорів по циклу
        static readonly Colour[] ColourPalette =
        {
            Colour.White,
            Colour.Pink,
            Colour.Red,
            Colour.Orange,
            Colour.Yellow,
            Colour.Lime,
            Colour.Green,
            Colour.Cyan,
            Colour.Blue,
            Colour.Purple,
            Colour.Black
        };

        // Поточний режим роботи
        static ActionState _mode = ActionState.Commands;

        // Минулий режим роботи, до якого можна повернутись
        static ActionState _previousMode = ActionState.Commands;

        // Координати курсору
        static int _cursorX;
        static int _cursorY;

        // Змінна, яка застосовується для перевірку чи був обраний об'єкт чи ні
        static int _selectedIndex = -1;

        // Поточний колір, застосовуваний при створенні та зміні об'єктів
        static Colour _currentColour = Colour.White;

        static void Main(string[] args)
        {
            // Підготовка до запуску програмного циклу
            Console.CursorVisible = false;
            Console.Clear();
            Render();

            while (true)
            {
                // Якщо користувач знаходиться в режимі виконання команд - він буде здатен вводити команди
                if (_mode == ActionState.Commands)
                {
                    Console.Write("Command> ");
                    string? line = Console.ReadLine();

                    // Якщо пустий рядок - програма не відреагує на це.
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    // Якщо не виконується команда - програма попередить про це.
                    if (!ExecuteCommand(line))
                    {
                        break;
                    }

                    Render();
                    Update();

                    continue;
                }

                // Якщо доступність клавіатури обмежена - програма буде чекати дозволу
                if (!Console.KeyAvailable)
                {
                    continue;
                }

                // Обробка роботи з клавіатурою
                ConsoleKeyInfo keyInfo;

                try
                {
                    keyInfo = Console.ReadKey(true);
                }
                catch
                {
                    continue;
                }

                if (HandleKey(keyInfo))
                {
                    Update();
                    Render();
                }
            }
        }

        // Функція, яка призначена для обробки користувацького вводу
        static bool ExecuteCommand(string commandText)
        {
            string[] parts = commandText.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 0)
            {
                return true;
            }

            string command = parts[0].ToLowerInvariant();

            switch (command)
            {
                case "help":
                    PrintHelp();
                    Console.ReadKey();
                    return true;

                case "cursor":
                    _mode = ActionState.Cursor;
                    return true;

                case "color":
                    if (parts.Length > 1)
                    {
                        if (IsValidColour(parts[1]))
                        {
                            _currentColour = ParseColour(parts[1]);
                            Console.WriteLine("Current color: " + _currentColour);
                            return true;
                        }
                    }

                    Console.WriteLine("Available colors: " + string.Join(", ", ColourPalette));
                    Console.ReadKey();
                    return true;

                case "list":
                    ListModels();
                    return true;

                case "exit":
                case "quit":
                    return false;

                case "create":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("Usage: text ");
                        Console.ReadKey();
                        return true;
                    }

                    string kind = parts[1].ToLowerInvariant();

                    if (kind == "text")
                    {
                        CreateTextObjectAtCursor();
                        _mode = ActionState.Creating;
                        return true;
                    }

                    Console.WriteLine("Unknown shape type. Use square, circle, text or custom.");
                    return true;

                case "select":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("Usage: select <index>");
                        return true;
                    }

                    if (IsValidInt(parts[1]))
                    {
                        int index = ParseInt(parts[1]);
                        if (index >= 0 && index < Models.Count)
                        {
                            _selectedIndex = index;
                            _mode = ActionState.Selection;
                            GraphicalModel selected = Models[index];
                            _cursorX = selected.X;
                            _cursorY = selected.Y;
                            return true;
                        }
                    }

                    Console.WriteLine("Invalid object index.");
                    return true;

                case "delete":
                    if (parts.Length > 1)
                    {
                        if (IsValidInt(parts[1]))
                        {
                            int deleteIndex = ParseInt(parts[1]);
                            if (deleteIndex >= 0 && deleteIndex < Models.Count)
                            {
                                Models.RemoveAt(deleteIndex);
                                _selectedIndex = -1;
                                return true;
                            }
                        }
                    }

                    if (_selectedIndex >= 0)
                    {
                        DeleteSelectedModel();
                    }
                    return true;

                default:
                    Console.WriteLine("Unknown command. Use 'help'.");
                    Console.ReadKey();
                    return true;
            }
        }

        // Функція, яка оброблює натискання клавіши
        static bool HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                if (_mode == ActionState.Selection)
                {
                    _mode = ActionState.Cursor;
                    return true;
                }

                if (_mode == ActionState.Cursor)
                {
                    _mode = ActionState.Commands;
                    return true;
                }

                if (_mode == ActionState.Creating || _mode == ActionState.Editing)
                {
                    _mode = ActionState.Commands;
                    return true;
                }
            }

            // Розподілення логіки роботи клавіш між режимами роботи
            switch (_mode)
            {
                case ActionState.Cursor:
                    return HandleCursorKey(keyInfo);
                case ActionState.Selection:
                    return HandleSelectionKey(keyInfo);
                case ActionState.Creating:
                    return HandleCreatingKey(keyInfo);
                case ActionState.Editing:
                    return HandleEditingKey(keyInfo);
                default:
                    return true;
            }
        }

        // Функція, яка оброблює керування курсором в режимі курсора
        static bool HandleCursorKey(ConsoleKeyInfo keyInfo)
        {
            int moveKey = IsMoveKey(keyInfo.Key);
            if (moveKey != 0)
            {
                int dx = 0;
                int dy = 0;
                switch (moveKey)
                {
                    case 1: dx = -1; break;
                    case 2: dx = 1; break;
                    case 3: dy = -1; break;
                    case 4: dy = 1; break;
                }

                _cursorX += dx;
                _cursorY += dy;
                return true;
            }

            switch (keyInfo.Key)
            {
                case ConsoleKey.Enter:
                    TrySelectModelAtCursor();
                    if (_selectedIndex >= 0)
                    {
                        _mode = ActionState.Selection;
                    }
                    return true;
                case ConsoleKey.C:
                    CreateTextObjectAtCursor();
                    _mode = ActionState.Creating;
                    return true;
                default:
                    return true;
            }
        }

        // Функція, яка оброблює керування курсором в режимі виділення об'єкта
        static bool HandleSelectionKey(ConsoleKeyInfo keyInfo)
        {
            // Тимчасово створена модель - бере посилання з обраного курсором об'єкта
            GraphicalModel model = GetSelectedModel();

            // Якщо об'єкт не був знайдений - програма повертається до режиму курсора
            if (model == null)
            {
                _mode = ActionState.Cursor;
                return true;
            }

            // Обробка переміщення курсора разом з виділеним об'єктом
            int moveKey = IsMoveKey(keyInfo.Key);
            if (moveKey != 0)
            {
                int dx = 0;
                int dy = 0;
                switch (moveKey)
                {
                    // Вліво
                    case 1: dx = -1; break;
                    // Вправо
                    case 2: dx = 1; break;
                    // Верх
                    case 3: dy = -1; break;
                    // Низ
                    case 4: dy = 1; break;
                }

                model.Move(dx, dy);
                _cursorX = model.X;
                _cursorY = model.Y;
                return true;
            }

            // Перемикання режимів для подальшої взаємодії з об'єктом
            switch (keyInfo.Key)
            {
                case ConsoleKey.D:
                    DeleteSelectedModel();
                    _mode = ActionState.Cursor;
                    return true;
                case ConsoleKey.E:
                    _mode = ActionState.Editing;
                    return true;
                case ConsoleKey.Enter:
                    _mode = ActionState.Editing;
                    return true;
                default:
                    return true;
            }
        }

        // Функція, яка оброблює керування курсором в режимі створення об'єкта
        static bool HandleCreatingKey(ConsoleKeyInfo keyInfo)
        {
            GraphicalModel model = GetSelectedModel();
            if (model == null)
            {
                return true;
            }

            // Обробка переміщення курсора
            int moveKey = IsMoveKey(keyInfo.Key);
            if (moveKey != 0)
            {
                int dx = 0;
                int dy = 0;
                switch (moveKey)
                {
                    case 1: dx = -1; break;
                    case 2: dx = 1; break;
                    case 3: dy = -1; break;
                    case 4: dy = 1; break;
                }

                _cursorX += dx;
                _cursorY += dy;
                return true;
            }

            // У разі підтвердження змін - збереження нового стану об'єкта й повернення до режиму команд
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                model.Update();
                _mode = ActionState.Commands;
                _selectedIndex = -1;
                return true;
            }

            // Функція дозволяє вводити користувачеві лише видимі символи
            if (!char.IsControl(keyInfo.KeyChar) && keyInfo.KeyChar != '\0')
            {
                int localX = _cursorX - model.X;
                int localY = _cursorY - model.Y;
                model.SetCell(localX, localY, keyInfo.KeyChar, _currentColour);
            }

            return true;
        }

        // Функція, яка оброблює керування курсором в режимі внесення змін у об'єкті
        static bool HandleEditingKey(ConsoleKeyInfo keyInfo)
        {
            GraphicalModel model = GetSelectedModel();
            if (model == null)
            {
                _mode = ActionState.Cursor;
                return true;
            }

            // Обробка переміщення курсора в межах об'єкта
            int moveKey = IsMoveKey(keyInfo.Key);
            if (moveKey != 0)
            {
                int dx = 0;
                int dy = 0;
                switch (moveKey)
                {
                    case 1: dx = -1; break;
                    case 2: dx = 1; break;
                    case 3: dy = -1; break;
                    case 4: dy = 1; break;
                }

                int nextX = _cursorX + dx;
                int nextY = _cursorY + dy;
                if (nextX >= model.X && nextX < model.X + model.Width && nextY >= model.Y && nextY < model.Y + model.Height)
                {
                    _cursorX = nextX;
                    _cursorY = nextY;
                }
                return true;
            }

            // Підтвердження внесених змін
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                model.Update();
                _mode = ActionState.Selection;
                return true;
            }

            // Вихід в режим виділення
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                _mode = ActionState.Selection;
                return true;
            }

            // Функція дозволяє вводити користувачеві лише видимі символи
            if (!char.IsControl(keyInfo.KeyChar) && keyInfo.KeyChar != '\0')
            {
                int localX = _cursorX - model.X;
                int localY = _cursorY - model.Y;
                model.SetCell(localX, localY, keyInfo.KeyChar, _currentColour);
            }

            return true;
        }

        // Функція для створення об'єкту
        static void CreateTextObjectAtCursor()
        {
            Console.Write("Width: ");
            int width = ParseInt(Console.ReadLine());
            if (width <= 0)
            {
                width = 1;
            }

            Console.Write("Height: ");
            int height = ParseInt(Console.ReadLine());
            if (height <= 0)
            {
                height = 1;
            }

            // Створюється об'єкт. Ініціалізується й додається до списку об'єктів
            GraphicalModel model = new GraphicalModel();
            model.Initialize(_cursorX, _cursorY, width, height, _currentColour);
            Models.Add(model);
            _selectedIndex = Models.Count - 1;
        }

        // Функція, яка намагається через курсор виділити об'єкт
        static bool TrySelectModelAtCursor()
        {
            for (int index = Models.Count - 1; index >= 0; index--)
            {
                GraphicalModel model = Models[index];
                // Перевірка на те, що курсор всередині об'єкта
                if (model.Contains(_cursorX, _cursorY))
                {
                    _selectedIndex = index;
                    _cursorX = model.X;
                    _cursorY = model.Y;
                    return true;
                }
            }

            _selectedIndex = -1;
            return false;
        }

        // Функція, яка видаляє виділений об'єкт зі списку
        static void DeleteSelectedModel()
        {
            if (_selectedIndex < 0 || _selectedIndex >= Models.Count)
            {
                return;
            }

            Models.RemoveAt(_selectedIndex);
            _selectedIndex = -1;
        }

        // Функція, яка повертає виділений об'єкт
        static GraphicalModel? GetSelectedModel()
        {
            if (_selectedIndex < 0 || _selectedIndex >= Models.Count)
            {
                return null;
            }

            return Models[_selectedIndex];
        }

        // Функція для виведення списку об'єктів з усіма їх характеристиками
        static void ListModels()
        {
            Console.Clear();
            Console.WriteLine("Objects:");

            if (Models.Count == 0)
            {
                Console.WriteLine("No objects created.");
            }
            else
            {
                for (int i = 0; i < Models.Count; i++)
                {
                    GraphicalModel model = Models[i];
                    Console.WriteLine("Object " + i + ":");

                    for (int row = 0; row < model.Height; row++)
                    {
                        for (int column = 0; column < model.Width; column++)
                        {
                            char ch = model.Data[row, column];
                            if (ch == ' ')
                            {
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write('.');
                            }
                            else
                            {
                                Console.ForegroundColor = ConvertColourToConsoleColor(model.Colours[row, column]);
                                Console.Write(ch);
                            }
                        }

                        Console.ResetColor();
                        Console.WriteLine();
                    }

                    Console.WriteLine("Pos: (" + model.X + ", " + model.Y + ") | Size: " + model.Width + "x" + model.Height);
                    Console.WriteLine();
                }
            }

            Console.ResetColor();
            Console.WriteLine("Press any key to continue...");
            try
            {
                Console.ReadKey(true);
            }
            catch
            {
            }
        }

        // Функція, яка оновлює стан об'єктів в програмному циклі
        static void Update()
        {
            foreach (GraphicalModel model in Models)
            {
                model.Update();
            }
        }

        // Функція для відмалювання всього полотна та користувацького інтерфейсу
        static void Render()
        {
            try
            {
                // Створення полотна з фіксованими розмірами
                char[,] renderBuffer = new char[GridHeight, GridWidth];
                for (int row = 0; row < GridHeight; row++)
                {
                    for (int column = 0; column < GridWidth; column++)
                    {
                        renderBuffer[row, column] = ' ';
                    }
                }

                // Відтворення об'єктів всередині полотна
                foreach (GraphicalModel model in Models)
                {
                    model.Draw(renderBuffer);
                }

                // Очищення всього вмісту консолі
                // Підготовка до відмалювання нового кадру
                Console.Clear();

                // Відмалювання нового кадру
                Console.WriteLine("Mode: " + _mode + " | Selected: " + _selectedIndex + " | Active colour: " + _currentColour + " | Objects: " + Models.Count);
                Console.WriteLine(new string('-', GridWidth));

                // Виведення на консоль курсору, об'єктів й полотна
                for (int row = 0; row < GridHeight; row++)
                {
                    for (int column = 0; column < GridWidth; column++)
                    {
                        char current = renderBuffer[row, column];
                        if (column == _cursorX && row == _cursorY)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write('█');
                        }
                        else if (current != ' ')
                        {
                            Console.ForegroundColor = GetColourAtWorldPosition(column, row);
                            Console.Write(current);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write('.');
                        }
                    }

                    Console.WriteLine();
                }

                Console.ResetColor();
                Console.WriteLine(new string('-', GridWidth));
                Console.WriteLine("Commands: cursor | select <index> | color <name> | coloring | list | delete | exit");
                Console.WriteLine("Tip: use Esc to go back to command mode; Enter confirms; arrow keys move the cursor and selected object.");
            }
            catch
            {
            }
        }

        // Отримання кольору від об'єктів всередині полотна
        static ConsoleColor GetColourAtWorldPosition(int worldX, int worldY)
        {
            for (int i = Models.Count - 1; i >= 0; i--)
            {
                GraphicalModel model = Models[i];
                if (model.Contains(worldX, worldY))
                {
                    int localX = worldX - model.X;
                    int localY = worldY - model.Y;
                    
                    if (localX >= 0 && localY >= 0 && localX < model.Width && localY < model.Height)
                    {
                        return ConvertColourToConsoleColor(model.Colours[localY, localX]);
                    }
                }
            }

            return ConsoleColor.Gray;
        }

        // Функція по конвертації доступних кольорів з enum Colour в доступні консольні кольори для символу
        static ConsoleColor ConvertColourToConsoleColor(Colour colour)
        {
            switch (colour)
            {
                case Colour.White: return ConsoleColor.White;
                case Colour.Pink: return ConsoleColor.Magenta;
                case Colour.Red: return ConsoleColor.Red;
                case Colour.Orange: return ConsoleColor.DarkYellow;
                case Colour.Yellow: return ConsoleColor.Yellow;
                case Colour.Lime: return ConsoleColor.Green;
                case Colour.Green: return ConsoleColor.DarkGreen;
                case Colour.Cyan: return ConsoleColor.Cyan;
                case Colour.Blue: return ConsoleColor.Blue;
                case Colour.Purple: return ConsoleColor.DarkMagenta;
                case Colour.Black: return ConsoleColor.Black;
                default: return ConsoleColor.Gray;
            }
        }

        // Функція, яка звіряє введений користувачем текст з набором списку кольорів
        static bool IsValidColour(string value)
        {
            string normalized = value.Trim();
            foreach (Colour item in ColourPalette)
            {
                if (item.ToString().Equals(normalized, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        // Функція для перетворення введенного користувачем тексту у колір з доступного списку кольорів
        static Colour ParseColour(string value)
        {
            string normalized = value.Trim();
            foreach (Colour item in ColourPalette)
            {
                if (item.ToString().Equals(normalized, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }

            return Colour.White;
        }

        // Функція для керування рухом об'єкту чи курсора
        static int IsMoveKey(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    return 1;
                case ConsoleKey.RightArrow:
                    return 2;
                case ConsoleKey.UpArrow:
                    return 3;
                case ConsoleKey.DownArrow:
                    return 4;
                default:
                    return 0;
            }
        }

        // Функція, яка перевіряє чи правильно введений тип даних
        static bool IsValidInt(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            try
            {
                int.Parse(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Функція, яка безпечно перетворює текстове значення в числове
        static int ParseInt(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }

            try
            {
                return int.Parse(value);
            }
            catch
            {
                return 0;
            }
        }

        // Функція для виведення опису існуючих команд.
        static void PrintHelp()
        {
            Console.WriteLine("Console Graphics Editor");
            Console.WriteLine("Commands: cursor | select <index> | list | color <name> | coloring | delete | exit");
            Console.WriteLine("Use Esc to step back and keep the model data as the render source. Colors are stored per character.");
        }
    }
}