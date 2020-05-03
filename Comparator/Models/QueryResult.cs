using System;

namespace Comparator.Models
{
    public class QueryResult
    {
        // TODO: Specify content
        public Query Query { get; set; }
        public int ProcessedDataSets { get; set; }
        public double ComputationTime { get; set; }
    }
}