using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace NCoreUtils;

public partial class GoogleCloudStorageUtils
{
    public sealed class GoogleCloudStorageEndpointFactory
    {
        [SuppressMessage("Performance", "CA1822")]
        public string ResumableUpload(string bucket)
            => $"https://www.googleapis.com/upload/storage/v1/b/{bucket}/o?uploadType=resumable";

        [SuppressMessage("Performance", "CA1822")]
        public string List(string bucket, string? prefix, bool? includeAcl, string? pageToken)
        {
            var buffer = ArrayPool<char>.Shared.Rent(8192);
            try
            {
                var builder = new SpanBuilder(buffer);
                builder.Append("https://storage.googleapis.com/storage/v1/b/");
                builder.Append(bucket);
                builder.Append("/o");
                bool first = true;
                if (!string.IsNullOrEmpty(prefix))
                {
                    builder.Append("?prefix=");
#if NET6_0_OR_GREATER
                    builder.AppendUriEscaped(prefix);
#else
                    builder.Append(Uri.EscapeDataString(prefix));
#endif
                    first = false;
                }
                if (includeAcl.HasValue)
                {
                    if (first)
                    {
                        builder.Append('?');
                        first = false;
                    }
                    else
                    {
                        builder.Append('&');
                    }
                    builder.Append("projection=");
                    builder.Append(includeAcl.Value ? "full" : "noAcl");
                }
                if (!string.IsNullOrEmpty(pageToken))
                {
                    if (first)
                    {
                        builder.Append('?');
                    }
                    else
                    {
                        builder.Append('&');
                    }
                    builder.Append("pageToken=");
#if NET6_0_OR_GREATER
                    builder.AppendUriEscaped(pageToken);
#else
                    builder.Append(Uri.EscapeDataString(pageToken));
#endif
                }
                return builder.ToString();
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
        }

        [SuppressMessage("Performance", "CA1822")]
        public string Copy(
            string sourceBucket,
            string sourceName,
            string destinationBucket,
            string destinationName)
#if NET6_0_OR_GREATER
        {
            var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
            try
            {
                var builder = new SpanBuilder(buffer);
                builder.Append("https://www.googleapis.com/storage/v1/b/");
                builder.AppendUriEscaped(sourceBucket);
                builder.Append("/o/");
                builder.AppendUriEscaped(sourceName);
                builder.Append("/copyTo/b/");
                builder.AppendUriEscaped(destinationBucket);
                builder.Append("/o/");
                builder.AppendUriEscaped(destinationName);
                return builder.ToString();
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer, clearArray: false);
            }
        }
#else
            => $"https://www.googleapis.com/storage/v1/b/{sourceBucket}/o/{Uri.EscapeDataString(sourceName)}/copyTo/b/{destinationBucket}/o/{Uri.EscapeDataString(destinationName)}";
#endif

        [SuppressMessage("Performance", "CA1822")]
        public string Delete(
            string bucket,
            string name)
#if NET6_0_OR_GREATER
        {
            var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
            try
            {
                var builder = new SpanBuilder(buffer);
                builder.Append("https://www.googleapis.com/storage/v1/b/");
                builder.AppendUriEscaped(bucket);
                builder.Append("/o/");
                builder.AppendUriEscaped(name);
                return builder.ToString();
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer, clearArray: false);
            }
        }
#else
            => $"https://www.googleapis.com/storage/v1/b/{bucket}/o/{Uri.EscapeDataString(name)}";
#endif

        [SuppressMessage("Performance", "CA1822")]
        public string Download(
            string bucket,
            string name)
#if NET6_0_OR_GREATER
        {
            var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
            try
            {
                var builder = new SpanBuilder(buffer);
                builder.Append("https://www.googleapis.com/storage/v1/b/");
                builder.AppendUriEscaped(bucket);
                builder.Append("/o/");
                builder.AppendUriEscaped(name);
                builder.Append("?alt=media");
                return builder.ToString();
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer, clearArray: false);
            }
        }
#else
            => $"https://www.googleapis.com/storage/v1/b/{bucket}/o/{Uri.EscapeDataString(name)}?alt=media";
#endif

        [SuppressMessage("Performance", "CA1822")]
        public string Get(
            string bucket,
            string name)
#if NET6_0_OR_GREATER
        {
            var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
            try
            {
                var builder = new SpanBuilder(buffer);
                builder.Append("https://www.googleapis.com/storage/v1/b/");
                builder.AppendUriEscaped(bucket);
                builder.Append("/o/");
                builder.AppendUriEscaped(name);
                return builder.ToString();
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer, clearArray: false);
            }
        }
#else
            => $"https://www.googleapis.com/storage/v1/b/{bucket}/o/{Uri.EscapeDataString(name)}";
#endif

        [SuppressMessage("Performance", "CA1822")]
        public string Patch(
            string bucket,
            string name,
            bool? includeAcl)
#if NET6_0_OR_GREATER
        {
            var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
            try
            {
                var builder = new SpanBuilder(buffer);
                builder.Append("https://www.googleapis.com/storage/v1/b/");
                builder.AppendUriEscaped(bucket);
                builder.Append("/o/");
                builder.AppendUriEscaped(name);
                if (includeAcl is bool acl)
                {
                    builder.Append("?projection=");
                    builder.Append(includeAcl.Value ? "full" : "noAcl");
                }
                return builder.ToString();
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer, clearArray: false);
            }
        }
#else
        {
            // FIXME: optimize
            var projection = includeAcl.HasValue
            ? $"?projection={(includeAcl.Value ? "full" : "noAcl")}"
            : string.Empty;
            return $"https://storage.googleapis.com/storage/v1/b/{bucket}/o/{Uri.EscapeDataString(name)}{projection}";
        }
#endif
    }

    public static GoogleCloudStorageEndpointFactory EndpointFactory { get; } = new();
}