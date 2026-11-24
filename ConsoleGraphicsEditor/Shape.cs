using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace ConsoleGraphicsEditor
{
    // Клас Shape представляє базову форму моделі для графічного редактора
    internal class Shape
    {
        /*     Конструктори     */

        public Shape() : this("Unnamed", "No description", new List<(short x, short y)> { (0, 0), (1, 1) }, 0, '#', false) { }
        public Shape(string name)
            : this(name, "No description", new List<(short x, short y)> { (0, 0), (1, 1) }, 0, '#', false) { }
        public Shape(string name, string description)
            : this(name, description, new List<(short x, short y)> { (0, 0), (1, 1) }, 0, '#', false) { }
        public Shape(string name, string description, List<(short x, short y)> points)
            : this(name, description, points, 0, '#', false) { }
        public Shape(string name, string description, List<(short x, short y)> points, Color color)
            : this(name, description, points, color, '#', false) { }
        public Shape(string name, string description, List<(short x, short y)> points, Color color, char symbol)
            : this(name, description, points, color, symbol, false) { }
        public Shape(string name, string description, List<(short x, short y)> points, Color color, char symbol, bool isCurve)
        {
            Name = name;
            Description = description;
            Color = color;
            Symbol = symbol;
            IsCurve = isCurve;
            // Порожній список допускається (форма без точок), інакше діє перевірка у властивості Points
            if (points == null || points.Count > 0)
            {
                Points = points!;
            }

            shapeCount++;
        }

        /*     Статичні private-поля     */

        // Лічильник створених екземплярів класу
        private static ushort shapeCount = 0;
        private static double scale = 1.0;

        /*     private-поля     */
        private string name = ""; // Назва форми
        // Масив координат точок фігури (x, y)
        private List<(short x, short y)> points = [];
        // Масив відстаней між точками форми
        // Використано з метою оптимізації математичних обчислень форми.
        private List<double> distances = [];
        // Координати виділення об'єкта по визначеному масиву
        // З 2 точок (x1, y1), (x2, y2), тобто [x1, y1, x2, y2]
        private short[] selectionPoints = { 0, 0, 0, 0 };
        private Color color; // Колір форми
        private char symbol; // Символ, що використовується для відображення форми
        private bool isVisible = true; // Прапорець видимості форми
        private bool isCurve; // Прапорець кривизни форми

        /*     Публічні властивості для private-полів     */

        public static ushort ShapeCount
        {
            get { return shapeCount; }
            private set { shapeCount = value; }
        }
        public static double Scale
        {
            get { return scale; }
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("Scale must be a positive value.");
                }
                scale = value;
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                if (value.Length < 2 || value.Length > 20)
                {
                    throw new ArgumentOutOfRangeException("Name must be between 2 and 20 characters");
                }
                name = value;
            }
        }
        public string Description { get; set; } = "No description"; // Опис форми
        public List<(short x, short y)> Points
        {
            get { return points; }
            set
            {
                if (value == null || value.Count < 2)
                {
                    throw new ArgumentException("Points list must contain at least two point.");
                }

                points = value;
                UpdateDistances();
                UpdateSelectionArea();
            }
        }
        public Color Color
        {
            get { return color; }
            set
            {
                if ((int)value < 0 || (int)value > 10)
                {
                    throw new ArgumentOutOfRangeException("Color must be between 0 and 10");
                }
                color = value;
            }
        }
        public char Symbol
        {
            get { return symbol; }
            set { symbol = value; }
        }
        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }
        public bool IsCurve
        {
            get { return isCurve; }
            set { isCurve = value; }
        }

        // Обчислювальна властивість, яка повертає периметр фігури в залежності від її типу (див. IsTypeShape)
        public double Perimeter
        {
            get
            {
                switch (IsTypeShape())
                {
                    case 1:
                        // Коло: довжина кола, distances[0] - радіус
                        return 2 * Math.PI * distances[0];
                    case 2:
                        // Еліпс: наближення Рамануджана, distances[0] і distances[1] - півосі
                        double a = distances[0];
                        double b = distances[1];
                        return 2 * Math.PI * Math.Sqrt((Math.Pow(a, 2) + Math.Pow(b, 2)) / 2);
                    default:
                        // Фігури з прямими сторонами: сума довжин усіх сторін
                        double sum = 0;
                        for (short i = 0; i < distances.Count; i++)
                            sum += distances[i];
                        return sum;
                }
            }
        }
        // Обчислювальна властивість, яка повертає назву типу фігури (див. IsTypeShape)
        public string TypeName
        {
            get
            {
                switch (IsTypeShape())
                {
                    case 1: return "Circle";
                    case 2: return "Ellipse";
                    case 3: return "Triangle";
                    case 4: return "Square";
                    case 5: return "Rectangle";
                    case 6: return "Rhombus";
                    case 7: return "Parallelogram";
                    case 8: return "Custom shape";
                    default: return "Unknown";
                }
            }
        }

        /*     Методи класу     */

        // Повертає площу фігури (розрахунок виконує приватний метод CalculateArea)
        public double Area()
        {
            return CalculateArea();
        }
        // Змінює видимість форми на протилежну
        public void ToggleVisibility()
        {
            isVisible = !isVisible;
        }
        // Змінює наявність кривизни в фігури.
        public void ToggleCurveShape()
        {
            isCurve = !isCurve;
        }
        // Метод відповідає за зміну координати однієї точки фігури
        // Використовується при заданні змін в фігурі
        public void ChangePoint(ushort id, short x, short y)
        {
            if (id >= points.Count)
            {
                throw new ArgumentOutOfRangeException("Index must be a less than the size of the collection");
            }
            points[id] = (x, y);
            UpdateDistances();
            UpdateSelectionArea();
        }

        public void MoveSelectedArea(short offsetX, short offsetY = 0)
        {
            Move(offsetX, offsetY);
        }
        public void MoveSelectedArea(short offsetY)
        {
            Move(0, offsetY);
        }
        public void MoveSelectedArea()
        {
            Move(0, 0);
        }

        // Додає нову точку в кінець списку й оновлює відстані та область виділення
        public void AddPoint(short x, short y)
        {
            points.Add((x, y));
            UpdateDistances();
            UpdateSelectionArea();
        }
        // Видаляє точку за індексом (при некоректному індексі нічого не робить)
        public void RemovePoint(short ind)
        {
            if (IsEmpty() || ind >= points.Count || ind <= -1) return;

            points.RemoveAt(ind);
            UpdateDistances();
            UpdateSelectionArea();
        }

        // Повертає точку з мінімальними координатами x та y серед усіх точок форми
        public (short x, short y) GetMinPoint(short x, short y)
        {
            return GetMinMaxPoint(x, y, true);
        }
        // Повертає точку з максимальними координатами x та y серед усіх точок форми
        public (short x, short y) GetMaxPoint(short x, short y)
        {
            return GetMinMaxPoint(x, y, false);
        }

        // Знаходження або мінімальних (isMin = true) або максимальних координат (isMin = false) точки
        private (short x, short y) GetMinMaxPoint(short x, short y, bool isMin)
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Points list is empty.");
            }
            x = points[0].x;
            y = points[0].y;
            if (isMin)
            {
                foreach (var point in points)
                {
                    if (point.x < x) x = point.x;
                    if (point.y < y) y = point.y;
                }
            }
            else
            {
                foreach (var point in points)
                {
                    if (point.x > x) x = point.x;
                    if (point.y > y) y = point.y;
                }
            }
            return (x, y);
        }

        // Статичний метод: повертає значення, помножене на коефіцієнт масштабування
        public static double ScaleShape(int value, double factor)
        {
            return value * factor;
        }
        // Статичний метод: зменшує лічильник створених форм на 1 та повертає нове значення
        public static int DecrementShapeCount()
        {
            return --shapeCount;
        }
        // Створює форму з рядка формату: name;description;x1,y1,x2,y2;color;symbol;isCurve
        // При некоректному форматі викидає виняток
        public static Shape Parse(string input)
        {
            var parts = input.TrimEnd(';').Split(';');
            if (parts.Length != 6)
            {
                throw new FormatException("Input string is not in the correct format.");
            }
            foreach (var p in parts)
            {
                if (p == "")
                {
                    throw new FormatException("Input string is not in the correct format.");
                }
            }
            // Список з координатами
            var strCoords = parts[2].Split(',');
            if (strCoords.Length % 2 != 0)
            {
                throw new FormatException("Input string is not in the correct format.");
            }
            // Створення списку точок: кожні дві координати утворюють одну точку
            List<(short x, short y)> temp = [];
            for (int i = 0; i < strCoords.Length; i += 2)
            {
                temp.Add((short.Parse(strCoords[i].Trim()), short.Parse(strCoords[i + 1].Trim())));
            }
            return new Shape(
                parts[0],
                parts[1],
                temp,
                (Color)Enum.Parse(typeof(Color), parts[3].Trim()),
                char.Parse(parts[4]),
                bool.Parse(parts[5].Trim())
            );
        }
        // Безпечна версія Parse: повертає true і форму, якщо рядок коректний, інакше false
        public static bool TryParse(string input, out Shape shape)
        {
            try
            {
                shape = Parse(input);
                return true;
            }
            catch
            {
                shape = null!;
                return false;
            }
        }
        // Метод перевірки, чи є форма порожньою (не має точок)
        public bool IsEmpty()
        {
            return points.Count == 0;
        }
        // Метод перевірки типу фігури по ознакам.
        // Повертає -1, якщо не знайшов співпадіння
        private int IsTypeShape()
        {
            if (IsEmpty()) return -1;
            if (isCurve)
            {
                // Якщо це крива і дві точки - це коло.
                // Якщо це крива та три точки, відстань між якими неоднакова та
                // перша й остання точки розташовані пендикулярно один до одного, то це елліпс
                if (points.Count == 2) return 1;
                else if (points.Count == 3 && distances[0] != distances[1])
                {
                    if (distances[0] != distances[1])
                    {
                        // Повертаємо елліпс
                        return 2;
                    }
                    else
                    {
                        // Повертаємо коло
                        return 1;
                    }
                }
            }
            else
            {
                // Якщо три точки й не крива, то це трикутник
                if (points.Count == 3)
                {
                    // Звичайний трикутник (Без визначення прямого трикутника)
                    return 3;
                }
                else if (points.Count == 4)
                {
                    // Вектори двох суміжних сторін з першої вершини.
                    // Тип long захищає від переповнення при множенні координат
                    long ux = points[1].x - points[0].x;
                    long uy = points[1].y - points[0].y;
                    long vx = points[3].x - points[0].x;
                    long vy = points[3].y - points[0].y;

                    // Паралелограм: діагоналі діляться навпіл, тобто p0 + p2 = p1 + p3.
                    // Векторний добуток не повинен дорівнювати нулю (інакше всі точки лежать на одній прямій)
                    bool isParallelogram = points[0].x + points[2].x == points[1].x + points[3].x &&
                                           points[0].y + points[2].y == points[1].y + points[3].y &&
                                           ux * vy - uy * vx != 0;

                    if (isParallelogram)
                    {
                        bool equalSides = ux * ux + uy * uy == vx * vx + vy * vy; // суміжні сторони рівні
                        bool rightAngle = ux * vx + uy * vy == 0;                 // скалярний добуток = 0

                        if (equalSides && rightAngle) return 4; // Квадрат
                        if (rightAngle) return 5;               // Прямокутник
                        if (equalSides) return 6;               // Ромб
                        return 7;                               // Паралелограм
                    }
                }
                // Нестандартна форма
                return 8;
            }
            return -1;
        }

        // Розрахунок площі в залежності від типу фігури (див. IsTypeShape). Повертає -1, якщо тип не визначено
        private double CalculateArea()
        {
            if (IsEmpty()) return -1;

            double a, b, c, s;
            switch (IsTypeShape())
            {
                case 1:
                    // Площа кола
                    return (double)(distances[0] * distances[0] * Math.PI);
                case 2:
                    // Формула площі еліпсу
                    return (double)(distances[0] * distances[1] * Math.PI);
                case 3:
                    // Площа трикутника за формулою Герона (За трьома сторонами
                    a = distances[0];
                    b = distances[1];
                    c = distances[2];
                    s = (Perimeter) / 2;
                    // Math.Max захищає від від'ємного значення під коренем через похибку double
                    // (виникає, якщо три точки лежать на одній прямій)
                    return Math.Sqrt(Math.Max(0, s * (s - a) * (s - b) * (s - c)));
                case 4:
                    // Квадрат
                    return (double)(distances[0] * distances[0]);
                case 5:
                    // Прямокутник
                    return (double)(distances[0] * distances[1]);
                case 6:
                    // Ромб
                    return (double)(distances[0] * distances[0]);
                case 7:
                    // Паралелограм
                    return (double)(distances[0] * distances[1]);
                case 8:
                    // Довільний багатокутник, розрахований за формулою шнурків/Гаусса
                    double sum = 0;
                    for (int p = 0; p < points.Count; p++)
                    {
                        int next = (p + 1) % points.Count;
                        sum += ((double)points[p].x * points[next].y) - ((double)points[next].x * points[p].y);
                    }
                    return Math.Abs(sum) / 2;
                default:
                    return -1;
            }
        }

        private void UpdateDistances(List<(short x, short y)> value)
        {
            distances.Clear();
            if (value == null || value.Count == 0) return;
            // Відстані між сусідніми точками
            for (short p = 0; p < value.Count - 1; p++)
            {
                distances.Add(CalculateDistance(value[p], value[p + 1]));
            }
            // Відстань між останньою та першою точками (замикання контуру)
            distances.Add(CalculateDistance(value[value.Count - 1], value[0]));
        }
        private void UpdateDistances()
        {
            UpdateDistances(points);
        }

        // Оновлює координати виділення [x1, y1, x2, y2] за мінімальною та максимальною точками форми
        private void UpdateSelectionArea()
        {
            if (IsEmpty()) return;

            (short x, short y) min = GetMinPoint(0, 0);
            (short x, short y) max = GetMaxPoint(0, 0);
            selectionPoints[0] = min.x;
            selectionPoints[1] = min.y;
            selectionPoints[2] = max.x;
            selectionPoints[3] = max.y;
        }

        // Обчислює евклідову відстань між двома точками
        private double CalculateDistance((short x, short y) p1, (short x, short y) p2)
        {
            if (IsEmpty()) return -1;
            return Math.Sqrt(Math.Pow((p2.x - p1.x), 2) + Math.Pow((p2.y - p1.y), 2));
        }

        // Зміщення форми на певну координату.
        private void Move(short offsetX, short offsetY)
        {
            (short x, short y) point;
            for (short p = 0; p < points.Count; p++)
            {
                point = points[p];
                point.x += offsetX;
                point.y += offsetY;
                points[p] = point;
            }
            UpdateSelectionArea();
        }

        // Повертає форму у вигляді рядка формату Parse: name;description;x1,y1,x2,y2;color;symbol;isCurve
        public override string ToString()
        {
            if (IsEmpty()) return "";
            string coords = "";
            foreach (var p in points)
            {
                coords += $"{p.x},{p.y},";
            }
            coords = coords.TrimEnd(',');
            return $"{name};{Description};{coords};{color};{symbol};{isCurve}";
        }
    }
}