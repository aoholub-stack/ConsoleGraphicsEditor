using System;

namespace ConsoleGraphicsEditor
{
    class GraphicModel
    {
        // Назва моделі
        public string Name = "\0";
        
        // Форма моделі
        public ShapeType Type;
        
        // Координати
        public int X;
        public int Y;
        
        // Ширина
        public int Width;
        public int Height;
        
        // Колір моделі
        public Colour Colour;
        
        // Символ, з якого відмальована модель
        public char Symbol;
        
        // Змінна, яка робить невидимою/видимою модель
        private bool IsVisible;
        
        // Ціна моделі
        private decimal Price;

        // Зміщення моделі
        public void Move(int offsetX, int offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        // Зміна розміру моделі
        public void Resize(int w, int h)
        {
            Width = w;
            Height = h;
        }

        // Зміна кольору моделі
        public void ChangeColour(Colour c)
        {
            Colour = c;
        }

        // Перемикач видимості моделі
        public void ToggleVisibility()
        {
            IsVisible = !IsVisible;
        }

        // Розрахунок площі, в якій поміщається модель
        public int CalculateArea()
        {
            return Width * Height;
        }

        // Визначити видимість 
        public void SetIsVisible(bool isVisible) { IsVisible = isVisible; }
        
        // Отримати значення видимості
        public bool GetIsVisible() { return IsVisible; }

        // Визначити ціну моделі
        public void SetPrice(decimal price) { Price = price; }
        
        // Отримати ціну моделі
        public decimal GetPrice() { return Price; }

        public override string ToString()
        {
            return $"Name: {Name}, Type: {Type}, Position: ({X}, {Y}), Size: {Width}x{Height}, Colour: {Colour}, Visible: {IsVisible}, Symbol: {Symbol}, Price: {Price:C}";
        }
    }
}
