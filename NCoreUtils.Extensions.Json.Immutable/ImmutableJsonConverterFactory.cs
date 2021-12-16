using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using NCoreUtils.Internal;

namespace NCoreUtils
{
    public static class ImmutableJsonConverterFactory
    {
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods, "NCoreUtils.ImmutableJsonConverterFactory", "NCoreUtils.Extensions.Json.Immutable")]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, "NCoreUtils.ImmutableJsonCoverterOptionsBuilder", "NCoreUtils.Extensions.Json.Immutable")]
        private static readonly MethodInfo _gmCreateOfT1 = typeof(ImmutableJsonConverterFactory)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m =>
            {
                if (!(m.Name == nameof(Create) && m.IsGenericMethodDefinition))
                {
                    return false;
                }
                var parameters = m.GetParameters();
                if (parameters.Length != 1)
                {
                    return false;
                }
                var p0 = parameters[0];
                var desiredP0Type = typeof(Action<>).MakeGenericType(
                  typeof(ImmutableJsonCoverterOptionsBuilder<>)
                    .MakeGenericType(m.GetGenericArguments()[0])
                );
                return p0.ParameterType == desiredP0Type;
            });

        private static readonly MethodInfo _gmGetOrCreateOfT0 = typeof(ImmutableJsonConverterFactory)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m =>
            {
                if (!(m.Name == nameof(GetOrCreate) && m.IsGenericMethodDefinition))
                {
                    return false;
                }
                return m.GetParameters().Length == 0;
            });

        private static readonly MethodInfo _gmGetOrCreateOfT1 = typeof(ImmutableJsonConverterFactory)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m =>
            {
                if (!(m.Name == nameof(GetOrCreate) && m.IsGenericMethodDefinition))
                {
                    return false;
                }
                var parameters = m.GetParameters();
                if (parameters.Length != 1)
                {
                    return false;
                }
                var p0 = parameters[0];
                var desiredP0Type = typeof(Action<>).MakeGenericType(
                  typeof(ImmutableJsonCoverterOptionsBuilder<>)
                    .MakeGenericType(m.GetGenericArguments()[0])
                );
                return p0.ParameterType == desiredP0Type;
            });

        private static readonly ConcurrentDictionary<Type, object> _cache = new ConcurrentDictionary<Type, object>();

        private static readonly Func<Type, object> _defFactory =
            type => Activator.CreateInstance(typeof(ImmutableJsonConverter<>).MakeGenericType(type), true)!;

        private static object Factory([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, ImmutableJsonCoverterOptions options)
        {
            var ctor = typeof(ImmutableJsonConverter<>)
                .MakeGenericType(type)
                .GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new [] { typeof(ImmutableJsonCoverterOptions) }, null);
            return ctor!.Invoke(new object[] { options });
        }

        /// <summary>
        /// Initializes new instance of <see cref="ImmutableJsonConverter{T}" /> with default settings. In contrast
        /// with the <see cref="GetOrCreate(Type)" /> this method always creates new converter instance, no caching is
        /// implied.
        /// </summary>
        /// <param name="type">Type of the target of the created converter.</param>
        /// <returns>
        /// Default json converter capable of serializing/deserializing immutable objects of type specified by
        /// <paramref name="type" /> parameter.
        /// </returns>
        public static JsonConverter Create(Type type)
        {
            var ctor = typeof(ImmutableJsonConverter<>)
                .MakeGenericType(type)
                .GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
            return (JsonConverter)ctor!.Invoke(Array.Empty<object>());
        }

        /// <summary>
        /// Initializes new instance of <see cref="ImmutableJsonConverter{T}" /> with default settings. In contrast
        /// with the <see cref="GetOrCreate{T}" /> this method always creates new converter instance, no caching is
        /// implied.
        /// </summary>
        /// <typeparam name="T">Type of the target of the created converter.</typeparam>
        /// <returns>
        /// Default json converter capable of serializing/deserializing immutable objects of type
        /// <typeparamref name="T" />.
        /// </returns>
        public static JsonConverter Create<T>()
            => new ImmutableJsonConverter<T>();


        /// <summary>
        /// Initializes new instance of <see cref="ImmutableJsonConverter{T}" /> using the the specified configuration
        /// function. In contrast with the <see cref="GetOrCreate(Type, Action{ImmutableJsonCoverterOptionsBuilder})" />
        /// this method always creates new converter instance, no caching is implied.
        /// </summary>
        /// <param name="type">Type of the target of the created converter.</param>
        /// <param name="configure">Configuration function.</param>
        /// <returns>
        /// Configured json converter capable of serializing/deserializing immutable objects of type specified by
        /// <paramref name="type" /> parameter.
        /// </returns>
        public static JsonConverter Create(Type type, Action<ImmutableJsonCoverterOptionsBuilder> configure)
            => (JsonConverter)_gmCreateOfT1.MakeGenericMethod(type).Invoke(default, new object[] { configure })!;

        /// <summary>
        /// Initializes new instance of <see cref="ImmutableJsonConverter{T}" /> using the the specified configuration
        /// function. In contrast with the <see cref="GetOrCreate{T}(Action{ImmutableJsonCoverterOptionsBuilder{T}})" />
        /// this method always creates new converter instance, no caching is implied.
        /// </summary>
        /// <param name="configure">Configuration function.</param>
        /// <typeparam name="T">Type of the target of the created converter.</typeparam>
        /// <returns>
        /// Configured json converter capable of serializing/deserializing immutable objects of type
        /// <typeparamref name="T" />.
        /// </returns>
        public static ImmutableJsonConverter<T> Create<T>(Action<ImmutableJsonCoverterOptionsBuilder<T>> configure)
        {
            var builder = new ImmutableJsonCoverterOptionsBuilder<T>();
            configure(builder);
            return new ImmutableJsonConverter<T>(builder.Build());
        }

        /// <summary>
        /// Either creates new instance of <see cref="ImmutableJsonConverter{T}" /> with the default settings or uses
        /// instance previously created by the <c>GetOrCreate</c> method group.
        /// <para>
        /// NOTE: Calling this method stores the created converter in the internal cache. Any further calls to the
        /// <c>GetOrCreate</c> method group will ignore any configuration options and return cached instance. If the
        /// converters are intended to be dynamically configured use <c>Create</c> method group instead.
        /// </para>
        /// </summary>
        /// <param name="type">Type of the target of the created converter.</param>
        /// <returns>
        /// Either default json converter capable of serializing/deserializing immutable objects of type specified by
        /// <paramref name="type" /> parameter or an instance configured by the previuos call to the <c>GetOrCreate</c>
        /// method group.
        /// </returns>
        public static JsonConverter GetOrCreate(Type type)
            => (JsonConverter)_gmGetOrCreateOfT0.MakeGenericMethod(type).Invoke(default, Array.Empty<object>())!;

        /// <summary>
        /// Either creates new instance of <see cref="ImmutableJsonConverter{T}" /> with default settings or uses
        /// instance previously created by the <c>GetOrCreate</c> method group.
        /// <para>
        /// NOTE: Calling this method stores the created converter in the internal cache. Any further calls to the
        /// <c>GetOrCreate</c> method group will ignore any configuration options and return cached instance. If the
        /// converters are intended to be dynamically configured use <c>Create</c> method group instead.
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of the target of the created converter.</typeparam>
        /// <returns>
        /// Either default json converter capable of serializing/deserializing immutable objects of type
        /// <typeparamref name="T" /> or an instance configured by the previuos call to the <c>GetOrCreate</c>
        /// method group.
        /// </returns>
        public static JsonConverter<T> GetOrCreate<T>()
            => (JsonConverter<T>)_cache.GetOrAdd(typeof(T), _defFactory);

        /// <summary>
        /// Either creates new instance of <see cref="ImmutableJsonConverter{T}" /> using the the specified
        /// configuration function or uses instance previously created by the <c>GetOrCreate</c> method group.
        /// <para>
        /// NOTE: Calling this method stores the created converter in the internal cache. Any further calls to the
        /// <c>GetOrCreate</c> method group will ignore any configuration options and return cached instance. If the
        /// converters are intended to be dynamically configured use <c>Create</c> method group instead.
        /// </para>
        /// </summary>
        /// <param name="type">Type of the target of the created converter.</param>
        /// <param name="configure">Configuration function.</param>
        /// <returns>
        /// Either default json converter capable of serializing/deserializing immutable objects of type specified by
        /// <paramref name="type" /> parameter or an instance configured by the previuos call to the <c>GetOrCreate</c>
        /// method group.
        /// </returns>
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods, "NCoreUtils.ImmutableJsonConverterFactory", "NCoreUtils.Extensions.Json.Immutable")]
        public static JsonConverter GetOrCreate([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, Action<ImmutableJsonCoverterOptionsBuilder> configure)
            => (JsonConverter)_gmGetOrCreateOfT1.MakeGenericMethod(type).Invoke(default, new object[] { configure })!;

        /// <summary>
        /// Either creates new instance of <see cref="ImmutableJsonConverter{T}" /> using the the specified
        /// configuration function or uses instance previously created by the <c>GetOrCreate</c> method group.
        /// <para>
        /// NOTE: Calling this method stores the created converter in the internal cache. Any further calls to the
        /// <c>GetOrCreate</c> method group will ignore any configuration options and return cached instance. If the
        /// converters are intended to be dynamically configured use <c>Create</c> method group instead.
        /// </para>
        /// </summary>
        /// <param name="configure">Configuration function.</param>
        /// <typeparam name="T">Type of the target of the created converter.</typeparam>
        /// <returns>
        /// Either default json converter capable of serializing/deserializing immutable objects of type
        /// <typeparamref name="T" /> or an instance configured by the previuos call to the <c>GetOrCreate</c>
        /// method group.
        /// </returns>
        public static JsonConverter<T> GetOrCreate<T>(Action<ImmutableJsonCoverterOptionsBuilder<T>> configure)
        {
            if (_cache.TryGetValue(typeof(T), out var converter))
            {
                return (JsonConverter<T>)converter;
            }
            var builder = new ImmutableJsonCoverterOptionsBuilder<T>();
            configure(builder);
            return (JsonConverter<T>)_cache.GetOrAdd(typeof(T), ty => Factory(ty, builder.Build()));
        }
    }
}