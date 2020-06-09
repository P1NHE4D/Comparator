using Comparator.Utils.Monads;
using Xunit;

namespace ComparatorTest.Utils.Monads {
    public class CapsuleTest {
        [Fact]
        public void SuccessBindTest_ShouldReturnSameValue() {
            // Arrange
            const string value = "I am a value";
            var success = new Success<string>(value);

            // Act
            success.Bind(innerValue => {
                // Assert            
                Assert.Same(value, innerValue);
                return new Success<int>(808);
            });
        }

        [Fact]
        public void SuccessBindTest_ShouldPropagateValues() {
            // Arrange
            const string value = "I am a value";
            const int value2 = 808;
            var value3 = new Success<int>(value2);
            var success = new Success<string>(value);

            // Act & Assert
            success.Bind(innerValue => {
                Assert.Same(value, innerValue);
                return new Success<int>(value2);
            }).Bind(innerValue => {
                Assert.Equal(value2, innerValue);
                return new Success<Success<int>>(value3);
            }).Bind(innerValue => {
                Assert.Equal(value3, innerValue);
                return new Success<int>(0);
            });
        }

        [Fact]
        public void SuccessMapTest_ShouldReturnSameValue() {
            // Arrange
            const string value = "I am a value";
            var success = new Success<string>(value);

            // Act
            success.Map(innerValue => {
                // Assert            
                Assert.Same(value, innerValue);
                return 808;
            });
        }

        [Fact]
        public void SuccessMapTest_ShouldPropagateValues() {
            // Arrange
            const string value = "I am a value";
            const int value2 = 808;
            var value3 = new Success<int>(value2);
            var success = new Success<string>(value);

            // Act & Assert
            success.Map(innerValue => {
                Assert.Same(value, innerValue);
                return value2;
            }).Map(innerValue => {
                Assert.Equal(value2, innerValue);
                return value3;
            }).Map(innerValue => {
                Assert.Equal(value3, innerValue);
                return 0;
            });
        }

        [Fact]
        public void SuccessMapTest_ShouldIgnoreReturnDefault() {
            // Arrange
            const string value = "I am a value";
            const string value2 = "I am another value";
            var success = new Success<string>(value);

            // Act
            var result = success.Return(value2);

            // Assert
            Assert.Same(value, result);
            Assert.NotSame(value2, result);
        }

        [Fact]
        public void SuccessMapTest_ShouldIgnoreCatch() {
            // Arrange
            const string value = "I am a value";
            const string value2 = "I am another value";
            var success = new Success<string>(value);

            // Act
            var result = success.Catch(m => {
                // Assert
                Assert.Equal(0, 1);
                return value2;
            });

            // Assert
            Assert.Same(value, result);
            Assert.NotSame(value2, result);
        }


        [Fact]
        public void FailureBindTest_ShouldIgnoreLambda() {
            // Arrange
            const string message = "I am a value";
            const int value2 = 808;
            var value3 = new Success<int>(value2);
            var success = new Failure<string>(message);

            // Act & Assert
            success.Bind(innerValue => {
                Assert.Equal(1, 0);
                return new Success<int>(value2);
            }).Bind(innerValue => {
                Assert.Equal(1, 0);
                return new Success<Success<int>>(value3);
            }).Bind(innerValue => {
                Assert.Equal(1, 0);
                return new Success<int>(0);
            });
        }

        [Fact]
        public void FailureMapTest_ShouldIgnoreLambda() {
            // Arrange
            const string message = "I am a value";
            const int value2 = 808;
            var value3 = new Success<int>(value2);
            var success = new Failure<string>(message);

            // Act & Assert
            success.Map(innerValue => {
                Assert.Equal(1, 0);
                return value2;
            }).Map(innerValue => {
                Assert.Equal(1, 0);
                return value3;
            }).Map(innerValue => {
                Assert.Equal(1, 0);
                return 0;
            });
        }

        [Fact]
        public void FailureMapTest_ShouldFireCatch() {
            // Arrange
            const string message = "I am a value";
            const string value2 = "I am another value";
            var success = new Failure<string>(message);

            // Act
            var result = success.Catch(m => {
                // Assert
                Assert.Same(message, m);
                return value2;
            });

            // Assert
            Assert.NotSame(message, result);
            Assert.Same(value2, result);
        }
    }
}