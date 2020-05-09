using Comparator.Models;

namespace Comparator.Services {
    public interface IDataAnalyser {
        QueryResult AnalyseQuery(Query query);
    }
}