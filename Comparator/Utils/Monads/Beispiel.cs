using System;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Comparator.Utils.Monads {
    // Unsers
    public static class FileUser {
        public static int OpenFile(string filename) =>
            FileIOAbstract
                .OpenFile(filename)
                .Map(x => x * 2)
                .Map(x => x + 2)
                .Bind(x => Division(2, x))
                .Return(0);

        public static Capsule<int> Division(int a, int b) {
            if (b == 0)
                return new Failure<int>("Division by zero, du Lappen!!!");
            return new Success<int>(a / b);
        }
    }

    // Unsers
    public static class FileIOAbstract {
        public static Capsule<int> OpenFile(string filename) {
            try {
                var result = FileIO.OpenFile(filename);
                return new Success<int>(result);
            }
            catch (Exception e) {
                return new Failure<int>(e.Message);
            }
        }
    }


    // Fremd
    class FileIO {
        private string _value = "hallo";
    
    
        public static int OpenFile(string filename) {
            if (filename == "fehler")
                throw new Exception("Fehler!");
            return 0;
        }

        public string GetValue() => _value;

        public string Value2 {
            get {
                return _value;
            }
            set {
                _value = value;
            }
        }
        
        public string Value3 { get; set; }


        public void Func(FileIO a, FileIO b) {
            a.Value2 = b.Value2;
            a.SetValue(b.GetValue());
        }

        private void SetValue(string getValue) {
            throw new NotImplementedException();
        }
    }
}