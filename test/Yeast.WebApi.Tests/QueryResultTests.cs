using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Yeast.WebApi.Tests
{
    public class QueryResultTests
    {
        [Fact]
        public void SerializesTotalResulsIfNotNull()
        {
            // Arrange
            var result = new QueryResult() {
                TotalResults = 1
            };

            // Act
            var serialized = JsonConvert.SerializeObject(result);
            var deserialized = JObject.Parse(serialized);

            // Assert
            Assert.Contains(deserialized.Properties(), prop => prop.Name.ToLower() == nameof(QueryResult.TotalResults).ToLower());
        }

        [Fact]
        public void DontSerializesTotalResulsIfNull()
        {
            // Arrange
            var result = new QueryResult();

            // Act
            var serialized = JsonConvert.SerializeObject(result);
            var deserialized = JObject.Parse(serialized);

            // Assert
            Assert.DoesNotContain(deserialized.Properties(), prop => prop.Name.ToLower() == nameof(QueryResult.TotalResults).ToLower());
        }
    }
}
