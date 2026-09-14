namespace ConsoleGraphicsEditor
{
    // Доступні кольори, що застосовувані до символа
    public enum Colour
    {
        White,
        Pink,
        Red,
        Orange,
        Yellow,
        Lime,
        Green,
        Cyan,
        Blue,
        Purple,
        Black
    }

    // Відповідає за режими роботи в терміналі.
    public enum ActionState
    {
        // Режим введення команд
        Commands,
        // Режим курсору
        Cursor,
        // Режим додавання нового об'єкта
        Creating,
        // Виділення об'єкта
        Selection,
        // Редагування властивостей об'єкта (З використанням режиму виділення)
        Editing
    }
}
