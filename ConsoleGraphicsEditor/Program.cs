namespace ConsoleGraphicsEditor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GraphicsEditor GE = new GraphicsEditor();
            GE.Canvas(8, 4, 40, 20);
            GE.Run();
        }
    }
}
