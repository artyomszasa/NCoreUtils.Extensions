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
        [UnconditionalSuppressMessage("Trimming", "IL2111", Justification = "Handled via dynamic dependency")]
        private static readonly MethodInfo _gmCreateOfT1;

        [UnconditionalSuppressMessage("Trimming", "IL2111", Justification = "Handled via dynamic dependency")]
        private static readonly MethodInfo _gmGetOrCreateOfT0;

        [UnconditionalSuppressMessage("Trimming", "IL2111", Justification = "Handled via dynamic dependency")]
        private static readonly MethodInfo _gmGetOrCreateOfT1;

        private static readonly ConcurrentDictionary<Type, object> _cache = new();

        [UnconditionalSuppressMessage("Trimming", "IL2111", Justification = "Handled via dynamic dependency")]
        static ImmutableJsonConverterFactory()
        {
            _gmCreateOfT1 = typeof(ImmutableJsonConverterFactory)
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

            _gmGetOrCreateOfT0 = typeof(ImmutableJsonConverterFactory)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m =>
                {
                    if (!(m.Name == nameof(GetOrCreate) && m.IsGenericMethodDefinition))
                    {
                        return false;
                    }
                    return m.GetParameters().Length == 0;
                });

            _gmGetOrCreateOfT1 = typeof(ImmutableJsonConverterFactory)
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
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Handled via dynamic dependency.")]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ImmutableJsonConverter<>))]
        private static object Factory([DynamicallyAccessedMembers(D.CtorAndProps)] Type type, ImmutableJsonCoverterOptions options)
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
        [RequiresUnreferencedCode("Only root type is preserved, consider preserving relevant types of the properties.")]
        public static JsonConverter Create([DynamicallyAccessedMembers(D.CtorAndProps)] Type type)
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
        [RequiresUnreferencedCode("Only root type is preserved, consider preserving relevant types of the properties.")]
        public static JsonConverter Create<[DynamicallyAccessedMembers(D.CtorAndProps)] T>()
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
        [UnconditionalSuppressMessage("Trimming", "IL2111", Justification = "Handled via dynamic dependency.")]
        [UnconditionalSuppressMessage("Trimming", "IL2060", Justification = "Handled via dynamic dependency.")]
        [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Handled via dynamic dependency.")]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ImmutableJsonConverterFactory))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ImmutableJsonConverter<>))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ImmutableJsonCoverterOptionsBuilder<>))]
        [RequiresUnreferencedCode("Only root type is preserved, consider preserving relevant types of the properties.")]
        public static JsonConverter Create([DynamicallyAccessedMembers(D.CtorAndProps)] Type type, Action<ImmutableJsonCoverterOptionsBuilder> configure)
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
        [RequiresUnreferencedCode("Only root type is preserved, consider preserving relevant types of the properties.")]
        public static ImmutableJsonConverter<T> Create<[DynamicallyAccessedMembers(D.CtorAndProps)] T>(Action<ImmutableJsonCoverterOptionsBuilder<T>> configure)
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
        [UnconditionalSuppressMessage("Trimming", "IL2111", Justification = "Handled via dynamic dependency.")]
        [UnconditionalSuppressMessage("Trimming", "IL2060", Justification = "Handled via dynamic dependency.")]
        [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Handled via dynamic dependency.")]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ImmutableJsonConverterFactory))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ImmutableJsonConverter<>))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ImmutableJsonCoverterOptionsBuilder<>))]
        [RequiresUnreferencedCode("Only root type is preserved, consider preserving relevant types of the properties.")]
        public static JsonConverter GetOrCreate([DynamicallyAccessedMembers(D.CtorAndProps)] Type type)
            => (JsonConverter)_gmGetOrCreateOfT0.MakeGenericMethod(type).Invoke(default, Array.Empty<object>())!;

        private static ImmutableJsonConverter<T> DoCreateImmutableJsonConverter<[DynamicallyAccessedMembers(D.CtorAndProps)] T>(Type _) => new();

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
        [RequiresUnreferencedCode("Only root type is preserved, consider preserving relevant types of the properties.")]
        public static JsonConverter<T> GetOrCreate<[DynamicallyAccessedMembers(D.CtorAndProps)] T>()
            => (JsonConverter<T>)_cache.GetOrAdd(
                typeof(T),
                DoCreateImmutableJsonConverter<T>
            );

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
        [UnconditionalSuppressMessage("Trimming", "IL2111", Justification = "Handled via dynamic dependency.")]
        [UnconditionalSuppressMessage("Trimming", "IL2060", Justification = "Handled via dynamic dependency.")]
        [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Handled via dynamic dependency.")]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ImmutableJsonConverterFactory))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ImmutableJsonConverter<>))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ImmutableJsonCoverterOptionsBuilder<>))]
        [RequiresUnreferencedCode("Only root type is preserved, consider preserving relevant types of the properties.")]
        public static JsonConverter GetOrCreate([DynamicallyAccessedMembers(D.CtorAndProps)] Type type, Action<ImmutableJsonCoverterOptionsBuilder> configure)
            => (JsonConverter)_gmGetOrCreateOfT1.MakeGenericMethod(type).Invoke(default, new object[] { configure })!;

        private sealed class DoCreateImmutableJsonConverterWithOptions<[DynamicallyAccessedMembers(D.CtorAndProps)] T>
        {
            private ImmutableJsonCoverterOptions Options { get; }

            public DoCreateImmutableJsonConverterWithOptions(ImmutableJsonCoverterOptions options)
            {
                Options = options;
            }

            public ImmutableJsonConverter<T> Invoke(Type _)
                => new(Options);
        }


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
        [RequiresUnreferencedCode("Only root type is preserved, consider preserving relevant types of the properties.")]
        public static JsonConverter<T> GetOrCreate<[DynamicallyAccessedMembers(D.CtorAndProps)] T>(Action<ImmutableJsonCoverterOptionsBuilder<T>> configure)
        {
            if (_cache.TryGetValue(typeof(T), out var converter))
            {
                return (JsonConverter<T>)converter;
            }
            var builder = new ImmutableJsonCoverterOptionsBuilder<T>();
            configure(builder);
            return (JsonConverter<T>)_cache.GetOrAdd(
                typeof(T),
                new DoCreateImmutableJsonConverterWithOptions<T>(builder.Build()).Invoke
            );
        }
    }
}