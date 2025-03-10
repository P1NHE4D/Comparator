using System;
using System.Collections.Generic;
using System.Linq;
using Comparator.Models;

namespace Comparator.Services {
    public class Classifier : IClassifier {

        /// <summary>
        /// Classifies documents contained in the data object according to predefined terms
        /// </summary>
        /// <param name="data">data object retrieved from elastic search</param>
        /// <param name="objA">first object</param>
        /// <param name="objB">second object</param>
        /// <returns>returns a ClassifiedData object containing the classified sentences</returns>
        public ClassifiedData ClassifyData(IEnumerable<DepccDataSet> data, string objA, string objB) {
            var filteredSentences = FilterSentences(
                data.Select(d => d.Text),
                objA,
                objB).ToList();

            var objAData = ClassifySentences(filteredSentences, objA, objB);
            var objBData = ClassifySentences(filteredSentences, objB, objA);
            var dataCount = objAData.Count + objBData.Count;
            var objATendency = dataCount == 0 ? 0 : (double) objAData.Count / dataCount;
            var objBTendency = dataCount == 0 ? 0 : (double) objBData.Count / dataCount;
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
        /// <param name="aspects">user defined aspects</param>
        /// <returns>returns a collection of ClassifiedData objects containing a ClassifiedData object for each user defined term</returns>
        public Dictionary<string, ClassifiedData> ClassifyAndSplitData(IEnumerable<DepccDataSet> data, string objA,
                                                                string objB, IEnumerable<string> aspects) {
            var filteredSentences = FilterSentences(
                data.Select(d => d.Text).ToList(),
                objA,
                objB).ToList();
            var objAOrigData = ClassifySentences(filteredSentences, objA, objB);
            var objBOrigData = ClassifySentences(filteredSentences, objB, objA);
            return new Dictionary<string, ClassifiedData>(from aspect in aspects
                                                          let objAData = FilterSentences(objAOrigData, aspect).ToList()
                                                          let objBData = FilterSentences(objBOrigData, aspect).ToList()
                                                          let dataCount = objAData.Count + objBData.Count
                                                          let objATendency = dataCount == 0 ? 0 : (double) objAData.Count / dataCount 
                                                          let objBTendency = dataCount == 0 ? 0 : (double) objBData.Count / dataCount
                                                          select new KeyValuePair<string, ClassifiedData>(aspect, new ClassifiedData {
                                                              ObjAData = objAData,
                                                              ObjBData = objBData,
                                                              ObjATendency = objATendency,
                                                              ObjBTendency = objBTendency,
                                                              DataCount = dataCount
                                                          }));
        }

        // classifies sentences into two groups, namely: pro objA and pro objB
        private static ICollection<string> ClassifySentences(IEnumerable<string> sentences, string objA, string objB) =>
            (from sentence in sentences.AsParallel()
             where PrefersObject(objA, objB, sentence) &&
                   !PrefersObject(objB, objA, sentence)
             group sentence by sentence
             into sentenceGroup
             select sentenceGroup.First()).ToList();

        // filters all sentences that are questions or than do not contain both objects
        private static IEnumerable<string> FilterSentences(IEnumerable<string> sentences, string objA, string objB) =>
            sentences.AsParallel()
                     .Where(s => !IsQuestion(s))
                     .Where(s => ContainsObjects(s, objA, objB));

        // filters all sentences that do not contain the given aspect
        private static IEnumerable<string> FilterSentences(IEnumerable<string> sentences, string aspect) =>
            sentences.AsParallel().Where(s => s.IndexOf(aspect, StringComparison.InvariantCultureIgnoreCase) >= 0);

        // returns true, if sentence is a question
        private static bool IsQuestion(string sentence) => sentence.Contains("?");

        // returns true, if sentence contains both objects
        private static bool ContainsObjects(string sentence, string objA, string objB) =>
            sentence.Contains(objA) && sentence.Contains(objB);

        // Returns true, if user prefers the target object compared to another object
        private static bool PrefersObject(string targetObj, string comparisonObj, string sentence) {
            var targetObjPos = sentence.IndexOf(targetObj, StringComparison.InvariantCultureIgnoreCase);
            var compObjPos = sentence.IndexOf(comparisonObj, StringComparison.InvariantCultureIgnoreCase);
            if (targetObjPos == -1 || compObjPos == -1) return false;

            var negationPosAfterTarget = WordPos(sentence, targetObjPos, Constants.Negations);
            var negationPosAfterObj = WordPos(sentence, compObjPos, Constants.Negations);
            var posAdjPosAfterTarget = WordPos(sentence, targetObjPos, Constants.PosComparativeAdjectives);
            var negAdjPosAfterTarget = WordPos(sentence, targetObjPos, Constants.NegComparativeAdjectives);
            var posAdjPosAfterObj = WordPos(sentence, compObjPos, Constants.PosComparativeAdjectives);
            var negAdjPosAfterObj = WordPos(sentence, compObjPos, Constants.NegComparativeAdjectives);
            
            return (targetObjPos < posAdjPosAfterTarget && posAdjPosAfterTarget < compObjPos || // objA better than objB
                    targetObjPos < negationPosAfterTarget && negationPosAfterTarget < negAdjPosAfterTarget &&
                    negAdjPosAfterTarget < compObjPos || // objA not worse than objB
                    compObjPos < negationPosAfterObj && negationPosAfterObj < posAdjPosAfterObj &&
                    posAdjPosAfterObj < targetObjPos || //objB not better than objA
                    compObjPos < negAdjPosAfterObj && negAdjPosAfterObj < targetObjPos) // objB worse than objA
                   &&
                   !(targetObjPos < negationPosAfterTarget && negationPosAfterTarget < posAdjPosAfterTarget &&
                     posAdjPosAfterObj < compObjPos || //obj A not better than objB
                     compObjPos < negationPosAfterObj && negationPosAfterObj < negAdjPosAfterObj &&
                     negAdjPosAfterObj < targetObjPos); // objB not worse than objA
        }

        // returns the position of the first word in a list of words that appears in a sentence
        private static int WordPos(string text, int start, IEnumerable<string> wordList) =>
            wordList.Select(word => WordPos(text, start, word))
                    .SkipWhile(wordPos => wordPos == -1)
                    .FirstOrDefault();

        // returns the position of the word in a sentence
        private static int WordPos(string text, int start, string word) =>
            text.IndexOf(word, start, StringComparison.InvariantCultureIgnoreCase);
    }
}