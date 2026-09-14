namespace ConsoleGraphicsEditor
{
    internal class GraphicalModel
    {
        // Властивості об'єкта
        // Координати
        public int X;
        public int Y;
        
        // Розміри
        public int Width;
        public int Height;

        // Дані про внутрішній вміст об'єкта
        // Символи
        public char[,] Data;
        // Кольори символів
        public Colour[,] Colours;

        // Метод ініціалізації об'єкту
        public void Initialize(int x, int y, int width, int height, Colour colour)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Data = new char[height, width];
            Colours = new Colour[height, width];

            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    Data[row, column] = ' ';
                    Colours[row, column] = colour;
                }
            }
        }
        
        // Метод, який відповідає за зміну одного символу в матриці
        // Використовується при створенні й зміні об'єкта
        public void SetCell(int localX, int localY, char value, Colour colour)
        {
            if (localX < 0 || localY < 0 || localX >= Width || localY >= Height)
            {
                return;
            }

            Data[localY, localX] = value;
            Colours[localY, localX] = colour;
        }

        // Метод, який перевіряє чи знаходиться точка в межах самого об'єкту.
        public bool Contains(int x, int y)
        {
            return x >= X && x < X + Width && y >= Y && y < Y + Height;
        }

        // Метод, який зміщує об'єкт на певну відстань.
        public void Move(int offsetX, int offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        // Метод, який перевіряє та оновлює вміст об'єкта
        public void Update()
        {
            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    if (Data[row, column] == '\0')
                    {
                        Data[row, column] = ' ';
                    }

                    if (Colours[row, column] == default)
                    {
                        Colours[row, column] = Colour.White;
                    }
                }
            }
        }

        // Метод, який відповідає за відмалювання об'єкту всередині іншої матриці
        public void Draw(char[,] m)
        {
            if (m == null)
            {
                return;
            }

            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    int mX = X + column;
                    int mY = Y + row;

                    if (mY >= 0 && mY < m.GetLength(0)
                        && mX >= 0 && mX < m.GetLength(1)
                        && Data[row, column] != '\0'
                        && Data[row, column] != ' ')
                    {
                        m[mY, mX] = Data[row, column];
                    }
                }
            }
        }


        // Перевантажений метод, який застосовується при виведенні списку об'єктів
        public override string ToString()
        {
            string rows = "";
            for (int row = 0; row < Height; row++)
            {
                rows += "\n";
                for (int column = 0; column < Width; column++)
                {
                    rows += Data[row, column] == ' ' ? '.' : Data[row, column];
                }
            }

            return string.Format("Appearance:{0} | Pos: ({1}, {2}) | Size: {3}x{4}",
                rows,
                X,
                Y,
                Width,
                Height);
        }
    }
}
