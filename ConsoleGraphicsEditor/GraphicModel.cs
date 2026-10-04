using System;
using System.ComponentModel;

namespace ConsoleGraphicsEditor
{
    class GraphicModel
    {

        /*     Конструктори     */

        public GraphicModel() : this("Unnamed", "No description", ShapeType.Rectangle, 0, 0, 1, 1, Colour.White, '*', 0m) { }

        public GraphicModel(string name) : this(name, "No description", ShapeType.Rectangle, 0, 0, 1, 1, Colour.White, '*', 0m) { }

        public GraphicModel(string name, string description)
            : this(name, description, ShapeType.Rectangle, 0, 0, 1, 1, Colour.White, '*', 0m) { }

        public GraphicModel(string name, string description, ShapeType shapeType, Colour modelColour) : this(name, description, shapeType, 0, 0, 1, 1, modelColour, '*', 0m) {}

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
        }

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
                IsPriceValid(value);
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

        /*     Методи для перевірок коректності значень у властивостях     */

        // Функція для відлову помилок при введенні рядка в певному діапазоні
        private bool IsStringInRange(int minLength, int maxLength, string input, string errorMessage = "Unknown error")
        {
            if (input.Length < minLength || input.Length > maxLength)
            {
                throw new ArgumentException(errorMessage);
            }
            return true;
        }

        // Функція для відлову помилок при введенні числа в певному діапозоні
        private bool IsIntInRange(int minLength, int maxLength, int input, string errorMessage = "Unknown error")
        {
            if (input < minLength || input > maxLength)
            {
                throw new ArgumentException(errorMessage);
            }
            return true;
        }

        // Перевірка того, чи ввів користувач правильну ціну.
        private bool IsPriceValid(decimal input)
        {
            if (input < 0)
            {
                throw new ArgumentException("Price cannot be negative.");
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

        public override string ToString()
        {
            return $"Name: {name}, Type: {shapeType}, Position: ({x}, {y}), Size: {width}x{height}, Colour: {modelColour}, Visible: {isVisible}, Symbol: {symbol}, Price: {price:C}";
        }
    }
}