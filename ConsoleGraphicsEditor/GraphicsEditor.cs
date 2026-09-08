using System;

namespace ConsoleGraphicsEditor
{
    internal class GraphicsEditor
    {
        // структура, яка зберігає координати x та y
        public struct Point
        {
            public int X { get; set; }
            public int Y { get; set; }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public Point() { X = 0; Y = 0; }
        }

        // Структура, яка відповідає за координати та розміри прямокутника.
        public struct Rect
        {
            public Point Position { get; set; }
            public Point Size { get; set; }

            public Rect(Point position, Point size)
            {
                Position = position;
                Size = size;
            }
        }

        //
        // ---
        //

        // Матриця, яка призначена для полотна
        private char[,] canvasMatrix = null!;
        
        // Змінна, яка зберігає дані про область полотна
        private Rect canvasRect;
        
        // Змінні, яка зберігають дані про положення курсору
        private Point cursor;
        private bool isCursorInsideCanvas = true;

        //
        // ---
        //

        // Метод для запуску й обробку циклу програми
        public void Run()
        {
            if (canvasMatrix == null)
                throw new Exception("Canvas is not initialized. Call Canvas(...) before Run().");

            Console.CursorVisible = true;
            Render();

            while (true)
            {
                if (!Console.KeyAvailable)
                    continue;

                if (HandleEvents())
                {
                    Update();
                    Render();
                }
            }
        }

        // Метод для обробки подій клавіатури та оновлення стану курсора та полотна (canvas).
        private bool HandleEvents()
        {
            if (!Console.KeyAvailable)
                return false;

            ConsoleKeyInfo key = Console.ReadKey(true);
            bool changed = false;

            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    MoveCursor(-1, 0);
                    changed = true;
                    break;
                case ConsoleKey.RightArrow:
                    MoveCursor(1, 0);
                    changed = true;
                    break;
                case ConsoleKey.UpArrow:
                    MoveCursor(0, -1);
                    changed = true;
                    break;
                case ConsoleKey.DownArrow:
                    MoveCursor(0, 1);
                    changed = true;
                    break;
                case ConsoleKey.Backspace:
                    WriteCurrentCell(' ');
                    changed = true;
                    break;
                case ConsoleKey.Spacebar:
                    WriteCurrentCell(' ');
                    changed = true;
                    break;
                default:
                    if (key.KeyChar != '\0' && !char.IsControl(key.KeyChar))
                    {
                        WriteCurrentCell(key.KeyChar);
                        changed = true;
                    }
                    break;
            }

            return changed;
        }

        // Метод, який оновлює стан курсора та перевіряє, чи знаходиться він всередині полотна (canvas).
        private void Update()
        {
            if (canvasMatrix == null)
                return;

            if (!IsInsideCanvas(cursor))
            {
                cursor = new Point(0, 0);
                isCursorInsideCanvas = false;
            }
        }

        // Метод для відображення полотна (canvas) та курсора на консолі.
        private void Render()
        {
            if (canvasMatrix == null)
                return;

            Console.Clear();

            for (int i = 0; i < canvasMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < canvasMatrix.GetLength(1); j++)
                {
                    Console.SetCursorPosition(j, i);
                    Console.Write(canvasMatrix[i, j]);
                }
            }

            Console.SetCursorPosition(cursor.X, cursor.Y);
        }

        //
        // ---
        // 

        // Метод, який перевіряє, чи знаходиться точка всередині полотна (canvas).
        private bool IsInsideCanvas(Point point)
        {
            if (canvasMatrix == null)
                return false;

            return point.Y >= 0
                && point.Y < canvasMatrix.GetLength(0)
                && point.X >= 0
                && point.X < canvasMatrix.GetLength(1);
        }

        // Метод, який рухає курсор по консолі й використовує так параметри:
        // dX - зміщення по горизонталі, dY - зміщення по вертикалі.
        private void MoveCursor(int dX, int dY)
        {
            var next = new Point(cursor.X + dX, cursor.Y + dY);

            if (!IsInsideCanvas(next))
                return;

            cursor = next;
            isCursorInsideCanvas = true;
        }

        // Метод для запису символу в поточну клітку під курсором.
        private void WriteCurrentCell(char c)
        {
            if (!IsInsideCanvas(cursor))
                return;

            canvasMatrix[cursor.Y, cursor.X] = c;
        }

        // Метод для очищення полотна (canvas), замінюючи всі символи на пробіли.
        public void ResetCanvas()
        {
            if (canvasMatrix == null)
                return;

            for (int y = 0; y < canvasMatrix.GetLength(0); y++)
            {
                for (int x = 0; x < canvasMatrix.GetLength(1); x++)
                {
                    canvasMatrix[y, x] = ' ';
                }
            }
        }

        // Метод для створення полотна (canvas) з заданими координатами та розмірами.
        public void Canvas(Rect rect)
        {
            canvasRect = rect;
            canvasMatrix = new char[rect.Position.Y + rect.Size.Y, rect.Position.X + rect.Size.X];
            cursor = rect.Position;
            ResetCanvas();
            DrawRectangle(rect, '.', false);
        }

        public void Canvas(int x, int y, int width, int height)
        {
            Canvas(new Rect(new Point(x, y), new Point(width, height)));
        }

        public void Canvas(int width, int height)
        {
            Canvas(new Rect(new Point(0, 0), new Point(width, height)));
        }

        // Очищає клітку під курсором, замінюючи її на пробіл.
        private void ClearCell()
        {
            WriteCurrentCell(' ');
        }

        // Метод для малювання прямокутника на полотні (canvas) з можливістю налаштування символів для заповнення та меж.
        private void DrawRectangle(Rect rect, char fillChar = '\0', bool isBorder = true, char horizontalChar = '\0', char verticalChar = '\0', char cornerChar = '\0')
        {
            int rows = canvasMatrix.GetLength(0);
            int cols = canvasMatrix.GetLength(1);

            int top = rect.Position.Y;
            int left = rect.Position.X;
            int bottom = top + rect.Size.Y - 1;
            int right = left + rect.Size.X - 1;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (!(i >= top && i <= bottom && j >= left && j <= right))
                        continue;

                    if (isBorder)
                    {
                        bool isHorizontalEdge = i == top || i == bottom;
                        bool isVerticalEdge = j == left || j == right;
                        bool isCorner = (i == top || i == bottom) && (j == left || j == right);

                        if (isCorner)
                        {
                            canvasMatrix[i, j] = cornerChar != '\0' ? cornerChar : '+';
                        }
                        else if (isHorizontalEdge)
                        {
                            canvasMatrix[i, j] = horizontalChar != '\0' ? horizontalChar : '-';
                        }
                        else if (isVerticalEdge)
                        {
                            canvasMatrix[i, j] = verticalChar != '\0' ? verticalChar : '|';
                        }
                        else
                        {
                            canvasMatrix[i, j] = fillChar != '\0' ? fillChar : ' ';
                        }
                    }
                    else
                    {
                        canvasMatrix[i, j] = fillChar != '\0' ? fillChar : ' ';
                    }
                }
            }
        }

        public override string ToString()
        {
            if (canvasMatrix == null)
                return string.Empty;

            string result = string.Empty;

            for (int i = 0; i < canvasMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < canvasMatrix.GetLength(1); j++)
                {
                    result += canvasMatrix[i, j];
                }

                result += "\n";
            }

            return result;
        }
    }
}