using System;

namespace ConsoleGraphicsEditor
{
    class GraphicModel
    {
        // Назва моделі
        public string name = "\0";
        
        // Форма моделі
        public ShapeType shapeType;
        
        // Координати
        public int x;
        public int y;
        
        // Ширина
        public int width;
        public int height;

        // Колір моделі
        public Colour modelColour;
        
        // Символ, з якого відмальована модель
        public char symbol;
        
        // Змінна, яка робить невидимою/видимою модель
        private bool isVisible;
        
        // Ціна моделі
        private decimal price;

        // Зміщення моделі
        public void Move(int offsetX, int offsetY)
        {
            x += offsetX;
            y += offsetY;
        }

        // Зміна розміру моделі
        public void Resize(int w, int h)
        {
            if (w > 0 && h > 0)
            {
                width = w;
                height = h;
            }
        }

        // Зміна кольору моделі
        public void ChangeColour(Colour c)
        {
            modelColour = c;
        }

        // Перемикач видимості моделі
        public void ToggleVisibility()
        {
            isVisible = !isVisible;
        }

        // Розрахунок площі, в якій поміщається модель
        public int CalculateArea()
        {
            return width * height;
        }

        // Визначити видимість 
        public void SetIsVisible(bool isVisible) { this.isVisible = isVisible; }
        
        // Отримати значення видимості
        public bool GetIsVisible() { return isVisible; }

        // Визначити ціну моделі
        public void SetPrice(decimal price) { this.price = price; }
        
        // Отримати ціну моделі
        public decimal GetPrice() { return this.price; }

        public override string ToString()
        {
            return $"Name: {name}, Type: {shapeType}, Position: ({x}, {y}), Size: {width}x{height}, Colour: {modelColour}, Visible: {isVisible}, Symbol: {symbol}, Price: {price:C}";
        }
    }
}
