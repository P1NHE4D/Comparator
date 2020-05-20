using System;
using System.Collections.Generic;
using System.Linq;
using Comparator.Models;
using Comparator.Utils.Logger;
using Nest;

namespace Comparator.Services {
    public class Classifier : IClassifier {
        private ILoggerManager _logger;
        
        public Classifier(ILoggerManager logger) {
            _logger = logger;
        }
        public ElasticSearchData ClassifyData(ISearchResponse<DepccDataSet> data, string objA, string objB, IEnumerable<string> aspects) {
            var objAData = new HashSet<string>(from doc in data.Documents
                                           where !IsQuestion(doc)
                                           where PrefersObject(objA, objB, doc) &&
                                                 !PrefersObject(objB, objA, doc)
                                           select doc.Text);
            var objBData = new HashSet<string>(from doc in data.Documents
                                           where !IsQuestion(doc)
                                           where PrefersObject(objB, objA, doc) &&
                                                 !PrefersObject(objA, objB, doc)
                                           select doc.Text);
            
            return new ElasticSearchData {
                Count = objAData.Count + objBData.Count,
                ObjADataSet = objAData,
                ObjBDataSet = objBData,
                UnclassifiedData = data.Documents
            };
        }

        private bool IsQuestion(DepccDataSet doc) => doc.Text.Contains("?");

        // Returns true, if user prefers the target object compared to another object
        private bool PrefersObject(string targetObj, string obj, DepccDataSet doc) {
            var targetObjPos = doc.Text.IndexOf(targetObj, StringComparison.InvariantCultureIgnoreCase);
            var objPos = doc.Text.IndexOf(obj, StringComparison.InvariantCultureIgnoreCase);
            if (targetObjPos == -1 || objPos == -1) return false;
            
            var posAdjPosAfterTarget = WordPos(doc.Text, targetObjPos, Constants.PosComparativeAdjectives);
            var negationPosAfterTarget = WordPos(doc.Text, targetObjPos, Constants.Negations);
            var negAdjPosAfterTarget = WordPos(doc.Text, targetObjPos, Constants.NegComparativeAdjectives);
            var posAdjPosAfterObj = WordPos(doc.Text, objPos, Constants.PosComparativeAdjectives);
            var negationPosAfterObj = WordPos(doc.Text, objPos, Constants.Negations);
            var negAdjPosAfterObj = WordPos(doc.Text, objPos, Constants.NegComparativeAdjectives);
            
            return (targetObjPos < posAdjPosAfterTarget && posAdjPosAfterTarget < objPos) ||
                   (targetObjPos < negationPosAfterTarget && negationPosAfterTarget < negAdjPosAfterTarget && negAdjPosAfterTarget < objPos) ||
                   (objPos < negationPosAfterObj && negationPosAfterObj < posAdjPosAfterObj && posAdjPosAfterObj < targetObjPos) ||
                   (objPos < negAdjPosAfterObj && negAdjPosAfterObj < targetObjPos);
        }

        private int WordPos(string text, int start, IEnumerable<string> wordList) {
            foreach (var word in wordList) {
                var wordPos = text.IndexOf(word, start, StringComparison.InvariantCultureIgnoreCase);
                if (wordPos != -1) {
                    return wordPos;
                }
            }

            return -1;
        }
    }
}