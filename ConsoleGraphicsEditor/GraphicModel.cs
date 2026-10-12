using System;
using System.ComponentModel;

namespace ConsoleGraphicsEditor
{
    class GraphicModel
    {

        /*     Конструктори     */

        public GraphicModel()
            : this("Unnamed", "No description", ShapeType.Rectangle, 0, 0, 1, 1, Colour.White, '*', 0m) { }

        public GraphicModel(string name)
            : this(name, "No description", ShapeType.Rectangle, 0, 0, 1, 1, Colour.White, '*', 0m) { }

        public GraphicModel(string name, string description)
            : this(name, description, ShapeType.Rectangle, 0, 0, 1, 1, Colour.White, '*', 0m) { }

        public GraphicModel(string name, string description, ShapeType shapeType, Colour modelColour)
            : this(name, description, shapeType, 0, 0, 1, 1, modelColour, '*', 0m) {}

        public GraphicModel(string name, string description, ShapeType shapeType, int x, int y, int width, int height, Colour modelColour)
            : this(name, description, shapeType, x, y, width, height, modelColour, '*', 0m) { }

        public GraphicModel(string name, string description, ShapeType shapeType, int x, int y, int width, int height, Colour modelColour, char symbol)
            : this(name, description, shapeType, x, y, width, height, modelColour, symbol, 0m) { }

        public GraphicModel(string name, string description, ShapeType shapeType, int x, int y, int width, int height, Colour modelColour, char symbol, decimal price)
        {
            Name = name;
            Description = description;
            ShapeType = shapeType;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            ModelColour = modelColour;
            Symbol = symbol;
            Price = price;

            // Підвищення лічильника створених моделей
            ModelCount++;
        }

        /*     Статичні private-поля     */

        // Лічильник створених моделей класу
        private static int modelCount;
        // Масштабування моделі при приближенні/віддаленні
        private static double scale = 1.0;

        /*     private-поля     */

        private string name = "\0";
        private ShapeType shapeType;
        // Координати
        private int x;
        private int y;
        // Розміри
        private int width;
        private int height;
        private Colour modelColour;
        // Символ, з якого відмальована модель
        private char symbol;
        private bool isVisible = true;
        // Ціна моделі
        private decimal price;

        /*     Публічні властивості для private-полів     */

        public static int ModelCount
        {
            get { return modelCount; }
            private set { modelCount = value; }
        }
        public static double Scale
        {
            get { return scale; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Scale must be greater than 0.");
                }
                scale = value;
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                IsStringInRange(2, 20, value, "Name must be between 2 and 20 characters.");
                name = value;
            }
        }
        // Опис моделі
        public string Description { get; set; } = "No description";
        public ShapeType ShapeType
        {
            get { return shapeType; }
            set
            {
                IsIntInRange(0, 4, (int)value, "ShapeType must be between 0 and 4.");
                shapeType = value;
            }
        }
        public int X
        {
            get { return x; }
            set
            {
                IsIntInRange(0, 999, value, "X coordinate must be between 0 and 999.");
                x = value;
            }
        }
        public int Y
        {
            get { return y; }
            set
            {
                IsIntInRange(0, 999, value, "Y coordinate must be between 0 and 999.");
                y = value;
            }
        }
        public int Width
        {
            get { return width; }
            set
            {
                IsIntInRange(1, 1000 - X, value, "Width must be between 1 and 1000.");
                width = value;
            }
        }
        public int Height
        {
            get { return height; }
            set
            {
                IsIntInRange(1, 1000 - Y, value, "Height must be between 1 and 1000.");
                height = value;
            }
        }
        public Colour ModelColour
        {
            get { return modelColour; }
            set
            {
                IsIntInRange(0, 10, (int)value, "Colour must be between 0 and 10.");
                modelColour = value;
            }
        }
        public char Symbol
        {
            get { return symbol; }
            set
            {
                symbol = value;
            }
        }
        public bool IsVisible
        {
            get { return isVisible; }
            private set
            {
                if (value != true && value != false)
                {
                    throw new ArgumentException("IsVisible must be a boolean value.");
                }
                isVisible = value;
            }
        }
        public decimal Price
        {
            get { return price; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Price must be greater than or equal to 0.");
                }
                price = value;
            }
        }

        // Обчислювальна властивість, яка повертає площу, в якій поміщається модель
        public int Area
        {
            get { return width * height; }
        }

        /*     Методи класу     */

        // Зміщення моделі
        public void MoveModel(int offsetX, int offsetY = 0)
        {
            Move(offsetX, offsetY);
        }

        public void MoveModel(int offsetY)
        {
            Move(0, offsetY);
        }

        public void MoveModel()
        {
            Move(0, 0);
        }

        // Зміна розміру моделі
        public void ResizeModel(int w, int h)
        {
            Resize(w, h);
        }

        // Перемикач видимості моделі
        public void ToggleVisibilityModel()
        {
            ToggleVisibility();
        }

        /*     Статичні методи     */

        // Статичний метод, що масштабує будь-який параметр моделі в тому числі й розміри полотна,
        public static int ScaleObject(int value, double factor)
        {
            return (int)(value * factor);
        }

        // Статичний метод, що повертає кількість створених моделей класу
        public static int DecrementModelCount()
        {
            return --modelCount;
        }

        // Статичний метод, що повертає рядок з інформацією про модель
        public static GraphicModel Parse(string input)
        {
            var parts = input.Split(";");
            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part))
                {
                    // Оскільки ми використовуємо Split, то порожні частини можуть з'явитися, якщо вхідний рядок містить зайві коми або пробіли.
                    parts[Array.IndexOf(parts, part)] = part.Trim();
                }

                // Наприклад, якщо позаду числового значення є текст, то Parse має його ігнорувати.
                // Якщо це текст перед текстовим значенням, то Parse має його ігнорувати.

                // Наприклад, якщо вхідні рядки виглядають так:
                // "Name: Model1"
                // "Description: This is a model"
                // "(і так далі)"
                if (part.Contains(':'))
                {
                    var index = part.IndexOf(':');
                    parts[Array.IndexOf(parts, part)] = part.Substring(index + 1).Trim();
                }
            }
            if (parts.Length != 10)
            {
                throw new FormatException("Input string is not in the correct format.");
            }
            return new GraphicModel(
                parts[0].Trim(),
                parts[1].Trim(),
                (ShapeType)Enum.Parse(typeof(ShapeType), parts[2].Trim()),
                int.Parse(parts[3].Trim()),
                int.Parse(parts[4].Trim()),
                int.Parse(parts[5].Trim()),
                int.Parse(parts[6].Trim()),
                (Colour)Enum.Parse(typeof(Colour), parts[7].Trim()),
                char.Parse(parts[8].Trim()),
                decimal.Parse(parts[9].Trim())
            );
        }

        // Статичний метод, що намагається розпарсити рядок з інформацією про модель.
        // Повертає true, якщо розпарсити вдалося, і false, якщо ні.
        public static bool TryParse(string input, out GraphicModel model)
        {
            try
            {
                model = Parse(input);
                return true;
            }
            catch
            {
                model = null!;
                return false;
            }
        }

        /*     Методи для перевірок коректності значень     */

        private bool IsStringInRange(int minLength, int maxLength, string input, string errorMessage = "Unknown error")
        {
            if (input.Length < minLength || input.Length > maxLength)
            {
                throw new ArgumentException(errorMessage);
            }
            return true;
        }
        private bool IsIntInRange(int minLength, int maxLength, int input, string errorMessage = "Unknown error")
        {
            if (input < minLength || input > maxLength)
            {
                throw new ArgumentException(errorMessage);
            }
            return true;
        }

        /*     інкапсульовані методи класу     */

        // Метод для зміщення моделі по координатам x та y
        private void Move(int offsetX, int offsetY)
        {
            x += offsetX;
            y += offsetY;
        }

        // Метод для зміни розміру моделі
        private void Resize(int w, int h)
        {
            if (w > 0 && h > 0)
            {
                width = w;
                height = h;
            }
        }

        // Перемикач видимості моделі
        private void ToggleVisibility()
        {
            IsVisible = !IsVisible;
        }

        /*     Перевантажений метод для виводу інформації про модель     */

        // Воно підтримується Parse та TryParse, які дозволяють створювати об'єкти GraphicModel з рядків, що містять інформацію про модель.
        public override string ToString()
        {
            // Як це зробити з Parse?
            // Необхідно створити рядок у форматі, який підтримує Parse та TryParse, тобто розділити значення комами. Наприклад:

            return $"Name: {name}, " +
                   $"Description: {Description}, " +
                   $"Shape Type: {shapeType}, " +
                   $"X: {x}, " +
                   $"Y: {y}, " +
                   $"Width: {width}, " +
                   $"Height: {height}, " +
                   $"Colour: {modelColour}, " +
                   $"Visible: {isVisible}, " +
                   $"Symbol: {symbol}, " +
                   $"Price: {price:C}, " +
                   $"Scale: {scale}";
        }
    }
}