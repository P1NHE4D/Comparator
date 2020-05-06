using System.IO;
using Comparator.Services;
using Comparator.Utils.Monads;
using Xunit;

namespace ComparatorTest.Services {
    public class ConfigurationLoaderTest {
        [Fact]
        public void FileExists_PositiveTest() {
            // Arrange
            /*if (File.Exists("config.json")) {
                File.Delete("config.json");
            }*/
            
            ConfigurationLoader loader = new ConfigurationLoader();
            const string failure = "Error loading config: configuration file not found";
            const string success = "test";

            // Act
            loader.KibanaUsername.Map(innerValue => {
                // Assert            
                Assert.Equal(success, innerValue);
                return success;
            }).Catch(msg => {
                Assert.Equal(success, msg);
                return msg;
            });
        }
    }
}