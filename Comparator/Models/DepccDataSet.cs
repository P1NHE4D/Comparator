using Nest;

namespace Comparator.Models {
    public class DepccDataSet {
        
        [Text(Name="document_id")]
        public string DocumentId { get; set; }
        
        [Text(Name="text")]
        public string Text { get; set; }
        
    }
}