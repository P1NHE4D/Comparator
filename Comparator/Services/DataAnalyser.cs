using Comparator.Models;
using Comparator.Utils.Logger;

namespace Comparator.Services {
    public class DataAnalyser : IDataAnalyser {
        private readonly ILoggerManager _logger;
        private readonly IKibanaService _kibana;
        private readonly IWatsonService _watson;

        public DataAnalyser(ILoggerManager logger, IKibanaService kibana, IWatsonService watson) {
            _logger = logger;
            _kibana = kibana;
            _watson = watson;
        }
        
        public QueryResult AnalyseQuery(Query query) {
            throw new System.NotImplementedException();
        }
    }
}