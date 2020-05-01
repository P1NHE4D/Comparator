using System;

namespace Comparator.Utils.Monads
{
    // Unsers
    public static class FileUser
    {
        public static int OpenFile(string filename) =>
            FileIOAbstract
                .OpenFile(filename)
                .Map(x => x * 2)
                .Map(x => x + 2)
                .Bind(x => Division(2, x))
                .Return(0);

        public static Capsule<int> Division(int a, int b)
        {
            if (b == 0)
                return new Failure<int>("Division by zero, du Lappen!!!");
            return new Success<int>(a / b);
        }
    }
    
    // Unsers
    public static class FileIOAbstract
    {
        public static Capsule<int> OpenFile(string filename)
        {
            try
            {
                var result = FileIO.OpenFile(filename);
                return new Success<int>(result);
            }
            catch (Exception e)
            {
                return new Failure<int>(e.Message);
            }
        }
    }

    
    // Fremd
    static class FileIO
    {
        public static int  OpenFile(string filename)
        {
            if (filename == "fehler")
                throw new Exception("Fehler!");
            return 0;
        }
    }
}