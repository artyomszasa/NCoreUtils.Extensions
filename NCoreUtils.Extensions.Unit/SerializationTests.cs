using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using NCoreUtils.JsonSerialization;
using Xunit;

namespace NCoreUtils.Extensions.Unit
{
    public class SerializationTests
    {
        [JsonConverter(typeof(JsonImmutableConverter))]
        public class ImmutableBox<T>
        {
            public T Value { get; }

            public ImmutableBox(T value) => Value = value;
        }

        [Fact]
        public void Basic()
        {
            var input = new ImmutableBox<int>(2);
            var json = JsonSerializer.Serialize(input);
            Assert.Equal("{\"Value\":2}", json);
            var output = JsonSerializer.Deserialize<ImmutableBox<int>>(json);
            Assert.NotNull(output);
            Assert.Equal(input.Value, output.Value);
            json = JsonSerializer.Serialize(input, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            Assert.Equal("{\"value\":2}", json);
            output = JsonSerializer.Deserialize<ImmutableBox<int>>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            Assert.NotNull(output);
            Assert.Equal(input.Value, output.Value);
        }

        [Fact]
        public void Nested()
        {
            var input = new ImmutableBox<ImmutableBox<int>>(new ImmutableBox<int>(2));
            var json = JsonSerializer.Serialize(input);
            Assert.Equal("{\"Value\":{\"Value\":2}}", json);
            var output = JsonSerializer.Deserialize<ImmutableBox<ImmutableBox<int>>>(json);
            Assert.NotNull(output);
            Assert.Equal(input.Value.Value, output.Value.Value);
            json = JsonSerializer.Serialize(input, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            Assert.Equal("{\"value\":{\"value\":2}}", json);
            output = JsonSerializer.Deserialize<ImmutableBox<ImmutableBox<int>>>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            Assert.NotNull(output);
            Assert.Equal(input.Value.Value, output.Value.Value);
        }

        [Fact]
        public void ImmutableList()
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonImmutableListConverter() }
            };
            IReadOnlyList<int> input = ImmutableArray.Create(1, 2, 3);
            var json = JsonSerializer.Serialize(input, options);
            Assert.Equal("[1,2,3]", json);
            var output = JsonSerializer.Deserialize<IReadOnlyList<int>>(json, options);
            Assert.NotNull(output);
            Assert.True(input.SequenceEqual(output));
        }
    }
}