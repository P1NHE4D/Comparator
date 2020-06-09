using System.Collections.Generic;
using IBM.Watson.NaturalLanguageUnderstanding.v1.Model;

namespace Comparator.Models {
    public class QueryResult {
        public int ProcessedDataSets { get; set; }
        public double ObjATendency { get; set; }
        public double ObjBTendency { get; set; }
        
        public EmotionScores ObjAEmotions { get; set; }
        public EmotionScores ObjBEmotions { get; set; }
        public IEnumerable<Keyword> ObjAKeywords { get; set; }
        public IEnumerable<Keyword> ObjBKeywords { get; set; }
        public double? ObjASentimentScore { get; set; }
        public double? ObjBSentimentScore { get; set; }
        public Dictionary<string, ClassifiedData> AspectResults { get; set; }
        public Dictionary<string, EmotionScores> ObjAAspectEmotions { get; set; }
        public Dictionary<string, EmotionScores> ObjBAspectEmotions { get; set; }
        public Dictionary<string, double?> ObjAAspectSentimentScores { get; set; }
        public Dictionary<string, double?> ObjBAspectSentimentScores { get; set; }
        public string Message { get; set; }
    }
}