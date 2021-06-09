using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils
{
    public static class JsonSerializerOptionsExtensions
    {
        /// <summary>
        /// Copies the options from a <see cref="JsonSerializerOptions" /> instance to a new instance.
        /// <para>
        /// NOTE: This method intended to be used with <c>System.Text.Json</c> &lt; <c>5.0.0</c>. Any properties
        /// introduced in the following versions will not be copied. If you are using <c>System.Text.Json</c> &gt;=
        /// <c>5.0.0</c> use the overloaded constructor instead!
        /// </para>
        /// </summary>
        /// <param name="options">The options instance to copy options from.</param>
        /// <returns>New instance of the <see cref="JsonSerializerOptions" />.</returns>
        public static JsonSerializerOptions Clone(this JsonSerializerOptions options)
        {
            var clone = new JsonSerializerOptions
            {
                AllowTrailingCommas = options.AllowTrailingCommas,
                DefaultBufferSize = options.DefaultBufferSize,
                DictionaryKeyPolicy = options.DictionaryKeyPolicy,
                Encoder = options.Encoder,
                IgnoreNullValues = options.IgnoreNullValues,
                IgnoreReadOnlyProperties = options.IgnoreReadOnlyProperties,
                MaxDepth = options.MaxDepth,
                PropertyNameCaseInsensitive = options.PropertyNameCaseInsensitive,
                PropertyNamingPolicy = options.PropertyNamingPolicy,
                ReadCommentHandling = options.ReadCommentHandling,
                WriteIndented = options.WriteIndented
            };
            foreach (var converter in options.Converters)
            {
                clone.Converters.Add(converter);
            }
            return clone;
        }

        public static JsonSerializerOptions AddConverter(this JsonSerializerOptions options, JsonConverter converter)
        {
            options.Converters.Add(converter);
            return options;
        }

        public static JsonSerializerOptions RemoveConverter(
            this JsonSerializerOptions options,
            Func<JsonConverter, bool> predicate)
        {
            for (var index = FindIndex(options.Converters, predicate);
                index != -1;
                index = FindIndex(options.Converters, predicate))
            {
                options.Converters.RemoveAt(index);
            }
            return options;

            static int FindIndex(IList<JsonConverter> converters, Func<JsonConverter, bool> predicate)
            {
                for (var i = 0; i < converters.Count; ++i)
                {
                    if (predicate(converters[i]))
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        public static JsonSerializerOptions ReplaceConverter(
            this JsonSerializerOptions options,
            Func<JsonConverter, bool> predicate,
            JsonConverter converter)
            => options
                .RemoveConverter(predicate)
                .AddConverter(converter);
    }
}