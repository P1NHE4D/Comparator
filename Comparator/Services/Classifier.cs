using System;
using System.Collections.Generic;
using System.Linq;
using Comparator.Models;
using Nest;

namespace Comparator.Services {
    public class Classifier : IClassifier {
        /// <summary>
        /// Classifies documents contained in the data object according to predefined terms
        /// </summary>
        /// <param name="data">data object retrieved from elastic search</param>
        /// <param name="objA">first object</param>
        /// <param name="objB">second object</param>
        /// <returns>returns a ClassifiedData object containing the classified sentences</returns>
        public ClassifiedData ClassifyData(ISearchResponse<DepccDataSet> data, string objA, string objB) {
            var filteredSentences = FilterSentences(
                data.Documents.Select(d => d.Text),
                objA,
                objB).ToList();

            var objAData = ClassifySentences(filteredSentences, objA, objB);
            var objBData = ClassifySentences(filteredSentences, objB, objA);
            var dataCount = objAData.Count + objBData.Count;
            var objATendency = (double) objAData.Count / dataCount;
            var objBTendency = (double) objBData.Count / dataCount;
            return new ClassifiedData {
                ObjAData = objAData,
                ObjBData = objBData,
                DataCount = dataCount,
                ObjATendency = objATendency,
                ObjBTendency = objBTendency
            };
        }

        /// <summary>
        /// Classifies documents contained in the data object according to user defined terms
        /// </summary>
        /// <param name="data">data object retrieved from elastic search</param>
        /// <param name="objA">first object</param>
        /// <param name="objB">second object</param>
        /// <param name="terms">user defined terms</param>
        /// <returns>returns a collection of ClassifiedData objects containing a ClassifiedData object for each user defined term</returns>
        public Dictionary<string, ClassifiedData> ClassifyAndSplitData(ISearchResponse<DepccDataSet> data, string objA,
                                                                string objB, IEnumerable<string> terms) {
            var filteredSentences = FilterSentences(
                data.Documents.Select(d => d.Text).ToList(),
                objA,
                objB).ToList();
            return new Dictionary<string, ClassifiedData>(from term in terms
                                                          let objAData = ClassifySentences(filteredSentences, objA, objB, term)
                                                          let objBData = ClassifySentences(filteredSentences, objB, objA, term)
                                                          let dataCount = objAData.Count + objBData.Count
                                                          let objATendency = (double) objAData.Count / dataCount 
                                                          let objBTendency = (double) objBData.Count / dataCount
                                                          select new KeyValuePair<string, ClassifiedData>(term, new ClassifiedData {
                                                              ObjAData = objAData,
                                                              ObjBData = objBData,
                                                              ObjATendency = objATendency,
                                                              ObjBTendency = objBTendency,
                                                              DataCount = dataCount
                                                          }));
        }

        private static ICollection<string> ClassifySentences(IEnumerable<string> sentences, string objA, string objB,
                                                             string term = null) =>
            (from sentence in sentences
             where PrefersObject(objA, objB, sentence, term) &&
                   !PrefersObject(objB, objA, sentence, term)
             group sentence by sentence
             into sentenceGroup
             select sentenceGroup.First()).ToHashSet();

        private static IEnumerable<string> FilterSentences(IEnumerable<string> sentences, string objA, string objB) =>
            sentences
                .Where(s => !IsQuestion(s))
                .Where(s => ContainsObjects(s, objA, objB));

        private static bool IsQuestion(string sentence) => sentence.Contains("?");

        private static bool ContainsObjects(string sentence, string objA, string objB) =>
            sentence.Contains(objA) && sentence.Contains(objB);

        // Returns true, if user prefers the target object compared to another object
        private static bool PrefersObject(string targetObj, string comparisonObj, string sentence,
                                          string target = null) {
            var targetObjPos = sentence.IndexOf(targetObj, StringComparison.InvariantCultureIgnoreCase);
            var objPos = sentence.IndexOf(comparisonObj, StringComparison.InvariantCultureIgnoreCase);
            if (targetObjPos == -1 || objPos == -1) return false;

            var negationPosAfterTarget = WordPos(sentence, targetObjPos, Constants.Negations);
            var negationPosAfterObj = WordPos(sentence, objPos, Constants.Negations);
            if (target != null) {
                var termPos = WordPos(sentence, targetObjPos, target);
                return (targetObjPos < termPos && termPos < objPos || // objA adj objB
                        objPos < negationPosAfterObj && negationPosAfterObj < termPos &&
                        termPos < targetObjPos // objB not adj objA
                       )
                       &&
                       !(targetObjPos < negationPosAfterTarget && negationPosAfterTarget < termPos && termPos < objPos);
            }

            var posAdjPosAfterTarget = WordPos(sentence, targetObjPos, Constants.PosComparativeAdjectives);
            var negAdjPosAfterTarget = WordPos(sentence, targetObjPos, Constants.NegComparativeAdjectives);
            var posAdjPosAfterObj = WordPos(sentence, objPos, Constants.PosComparativeAdjectives);
            var negAdjPosAfterObj = WordPos(sentence, objPos, Constants.NegComparativeAdjectives);

            return (targetObjPos < posAdjPosAfterTarget && posAdjPosAfterTarget < objPos || // objA better than objB
                    targetObjPos < negationPosAfterTarget && negationPosAfterTarget < negAdjPosAfterTarget &&
                    negAdjPosAfterTarget < objPos || // objA not worse than objB
                    objPos < negationPosAfterObj && negationPosAfterObj < posAdjPosAfterObj &&
                    posAdjPosAfterObj < targetObjPos || //objB not better than objA
                    objPos < negAdjPosAfterObj && negAdjPosAfterObj < targetObjPos) // objB worse than objA
                   &&
                   !(targetObjPos < negationPosAfterTarget && negationPosAfterTarget < posAdjPosAfterTarget &&
                     posAdjPosAfterObj < objPos);
        }

        private static int WordPos(string text, int start, IEnumerable<string> wordList) =>
            wordList.Select(word => WordPos(text, start, word))
                    .SkipWhile(wordPos => wordPos == -1)
                    .FirstOrDefault();

        private static int WordPos(string text, int start, string word) =>
            text.IndexOf(word, start, StringComparison.InvariantCultureIgnoreCase);
    }
}