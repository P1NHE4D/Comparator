using System;

namespace Comparator.Models
{
    public class QueryResult
    {
        public Query Query { get; set; }
        public int ProcessedDataSets { get; set; }
        public double ComputationTime { get; set; }
    }
}