using System.Collections.Generic;

namespace Comparator.Models {
    public class ClassifiedData {
        public int DataCount { get; set; }
        public double ObjATendency { get; set; }
        public double ObjBTendency { get; set; }
        public ICollection<string> ObjAData { get; set; }
        public ICollection<string> ObjBData { get; set; }
    }
}