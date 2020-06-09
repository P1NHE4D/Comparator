using System.Collections.Generic;
using System.Linq;
using Comparator.Models;
using Comparator.Services;
using Xunit;

namespace ComparatorTest.Services {
    public class ClassifierTest {
        private readonly Classifier _classifier;
        private const string ObjA = "Linux";
        private const string ObjB = "Windows";

        public ClassifierTest() {
            _classifier = new Classifier();
        }

        [Fact]
        public void TestPosAdjSentence() {
            var prefersA = new DepccDataSet {Text = "Linux is faster than Windows"};
            var prefersB = new DepccDataSet {Text = "Windows is faster than Linux"};
            var classifiedData = _classifier.ClassifyData(new [] {prefersA, prefersB}, ObjA, ObjB);
            
            Assert.Contains(prefersA.Text, classifiedData.ObjAData);
            Assert.Contains(prefersB.Text, classifiedData.ObjBData);
            Assert.Equal(1, classifiedData.ObjAData.Count);
            Assert.Equal(1, classifiedData.ObjBData.Count);
            Assert.Equal(2, classifiedData.DataCount);
            Assert.Equal((double) 1 / 2, classifiedData.ObjATendency);
            Assert.Equal((double) 1 / 2, classifiedData.ObjBTendency);
        }

        [Fact]
        public void TestNegation() {
            var prefersA = new DepccDataSet {Text = "Windows is not faster than Linux"};
            var prefersB = new DepccDataSet {Text = "Linux is not better than Windows"};
            var classifiedData = _classifier.ClassifyData(new [] {prefersA, prefersB}, ObjA, ObjB);
            
            Assert.Contains(prefersA.Text, classifiedData.ObjAData);
            Assert.Contains(prefersB.Text, classifiedData.ObjBData);
            Assert.Equal(1, classifiedData.ObjAData.Count);
            Assert.Equal(1, classifiedData.ObjBData.Count);
            Assert.Equal(2, classifiedData.DataCount);
            Assert.Equal((double) 1 / 2, classifiedData.ObjATendency);
            Assert.Equal((double) 1 / 2, classifiedData.ObjBTendency);
        }

        [Fact]
        public void TestDoubleNegation() {
            var prefersA = new DepccDataSet {Text = "Linux is not worse than Windows"};
            var prefersB = new DepccDataSet {Text = "Windows is not inferior to Linux"};
            var classifiedData = _classifier.ClassifyData(new [] {prefersA, prefersB}, ObjA, ObjB);
            
            Assert.Contains(prefersA.Text, classifiedData.ObjAData);
            Assert.Contains(prefersB.Text, classifiedData.ObjBData);
            Assert.Equal(1, classifiedData.ObjAData.Count);
            Assert.Equal(1, classifiedData.ObjBData.Count);
            Assert.Equal(2, classifiedData.DataCount);
            Assert.Equal((double) 1 / 2, classifiedData.ObjATendency);
            Assert.Equal((double) 1 / 2, classifiedData.ObjBTendency);
        }

        [Fact]
        public void TestNegAdjSentence() {
            var prefersA = new DepccDataSet {Text = "Windows is worse than Linux"};
            var prefersB = new DepccDataSet {Text = "Linux is inferior to Windows"};
            var classifiedData = _classifier.ClassifyData(new [] {prefersA, prefersB}, ObjA, ObjB);
            
            Assert.Contains(prefersA.Text, classifiedData.ObjAData);
            Assert.Contains(prefersB.Text, classifiedData.ObjBData);
            Assert.Equal(1, classifiedData.ObjAData.Count);
            Assert.Equal(1, classifiedData.ObjBData.Count);
            Assert.Equal(2, classifiedData.DataCount);
            Assert.Equal((double) 1 / 2, classifiedData.ObjATendency);
            Assert.Equal((double) 1 / 2, classifiedData.ObjBTendency);
        }

        [Fact]
        public void TestUnclassifiableData() {
            var data = new [] {
                new DepccDataSet {Text = "Linux is faster"},
                new DepccDataSet {Text = "is better"},
                new DepccDataSet {Text = "faster than Windows"},
            };
            var classifiedData = _classifier.ClassifyData(data.ToList(), ObjA, ObjB);
            
            Assert.Equal(0, classifiedData.ObjAData.Count);
            Assert.Equal(0, classifiedData.ObjBData.Count);
            Assert.Equal(0, classifiedData.DataCount);
            Assert.Equal(0, classifiedData.ObjATendency);
            Assert.Equal(0, classifiedData.ObjBTendency);
        }
    }
}