using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using NCoreUtils.Google.Drive.Proto;
using NCoreUtils.Proto;
using NCoreUtils.Proto.Internal;
using HttpMethod = System.Net.Http.HttpMethod;

namespace NCoreUtils.Google;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(JsonRootDriveApiV3Info))]
internal sealed partial class DriveApiV3SerializerContext : JsonSerializerContext { }

internal static class DriveApiV1ClientSpanBuilderExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendQueryParameter(this ref SpanBuilder builder, ref bool first, string name, string value)
    {
        char separator;
        if (first)
        {
            separator = '?';
            first = false;
        }
        else
        {
            separator = '&';
        }
        builder.Append(separator);
        builder.Append(name);
        builder.Append('=');
        builder.AppendUriEscaped(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendQueryParameter(this ref SpanBuilder builder, ref bool first, string name, bool value)
        => builder.AppendQueryParameter(ref first, name, value ? "true" : "false");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendQueryParameter(this ref SpanBuilder builder, ref bool first, string name, uint value)
    {
        char separator;
        if (first)
        {
            separator = '?';
            first = false;
        }
        else
        {
            separator = '&';
        }
        builder.Append(separator);
        builder.Append(name);
        builder.Append('=');
        builder.Append(value);
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static void AppendQueryParameter<T>(this ref SpanBuilder builder, ref bool first, string name, T value, IEmplacer<T> emplacer)
    //     where T : struct
    // {
    //     char separator;
    //     if (first)
    //     {
    //         separator = '?';
    //         first = false;
    //     }
    //     else
    //     {
    //         separator = '&';
    //     }
    //     builder.Append(separator);
    //     builder.Append(name);
    //     builder.Append('=');
    //     builder.Append(value, emplacer);
    // }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendOptionalQueryParameter(this ref SpanBuilder builder, ref bool first, string name, string? value)
    {
        if (value is not null)
        {
            AppendQueryParameter(ref builder, ref first, name, value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendOptionalQueryParameter(this ref SpanBuilder builder, ref bool first, string name, bool? value)
    {
        if (value is bool v)
        {
            AppendQueryParameter(ref builder, ref first, name, v);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendOptionalQueryParameter(this ref SpanBuilder builder, ref bool first, string name, uint? value)
    {
        if (value is uint v)
        {
            AppendQueryParameter(ref builder, ref first, name, v);
        }
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static void AppendOptionalQueryParameter<T>(this ref SpanBuilder builder, ref bool first, string name, T? value, IEmplacer<T> emplacer)
    //     where T : struct
    // {
    //     if (value is T v)
    //     {
    //         AppendQueryParameter(ref builder, ref first, name, v, emplacer);
    //     }
    // }
}

[ProtoClient(typeof(DriveApiV3Info), typeof(DriveApiV3SerializerContext))]
[ProtoClientConstructorParameter(typeof(IDriveConfiguration), "DriveConfiguration")]
public partial class DriveApiV3Client
{
    public const string DriveFileScope = "https://www.googleapis.com/auth/drive.file";

    public const string DriveMetadataScope = "https://www.googleapis.com/auth/drive.metadata";

    public const string DriveMetadataReadOnlyScope = "https://www.googleapis.com/auth/drive.metadata.readonly";

    public const string HttpClientConfigurationName = nameof(DriveApiV3Client);

    private HttpRequestMessage CreateListMinimalFilesRequest(
        string? corpora,
        string? driveId,
        bool? includeItemsFromAllDrives,
        string? orderBy,
        uint? pageSize,
        string? pageToken,
        string? q,
        string? spaces,
        bool? supportsAllDrives,
        string? includePermissionsForView,
        string? includeLabels)
    {
        var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
        try
        {
            var basePath = GetCachedMethodPath(Methods.ListMinimalFiles);
            string path;
            {
                var firstArg = true;
                var builder = new SpanBuilder(buffer);
                builder.Append(basePath);
                builder.AppendOptionalQueryParameter(ref firstArg, "corpora", corpora);
                builder.AppendOptionalQueryParameter(ref firstArg, "driveId", driveId);
                builder.AppendOptionalQueryParameter(ref firstArg, "includeItemsFromAllDrives", includeItemsFromAllDrives);
                builder.AppendOptionalQueryParameter(ref firstArg, "orderBy", orderBy);
                builder.AppendOptionalQueryParameter(ref firstArg, "pageSize", pageSize);
                builder.AppendOptionalQueryParameter(ref firstArg, "pageToken", pageToken);
                builder.AppendOptionalQueryParameter(ref firstArg, "q", q);
                builder.AppendOptionalQueryParameter(ref firstArg, "spaces", spaces);
                builder.AppendOptionalQueryParameter(ref firstArg, "supportsAllDrives", supportsAllDrives);
                builder.AppendOptionalQueryParameter(ref firstArg, "includePermissionsForView", includePermissionsForView);
                builder.AppendOptionalQueryParameter(ref firstArg, "includeLabels", includeLabels);
                builder.AppendQueryParameter(ref firstArg, "fields", "files(id,name,mimeType,trashed,createdTime,modifiedTime)");
                path = builder.ToString();
            }
            var req = new HttpRequestMessage(HttpMethod.Get, path);
            req.SetRequiredGcpScope(DriveConfiguration.AdjustScope(Methods.ListMinimalFiles, DriveMetadataReadOnlyScope ));
            return req;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private HttpRequestMessage CreateCreateMetadataFileRequest(
        bool? ignoreDefaultVisibility,
        bool? keepRevisionForever,
        bool? supportsAllDrives,
        string? includePermissionsForView,
        string? includeLabels,
        Drive.File file)
    {
        var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
        try
        {
            var basePath = GetCachedMethodPath(Methods.CreateMetadataFile);
            string path;
            {
                var firstArg = true;
                var builder = new SpanBuilder(buffer);
                builder.Append(basePath);
                builder.AppendOptionalQueryParameter(ref firstArg, "ignoreDefaultVisibility", ignoreDefaultVisibility);
                builder.AppendOptionalQueryParameter(ref firstArg, "keepRevisionForever", keepRevisionForever);
                builder.AppendOptionalQueryParameter(ref firstArg, "supportsAllDrives", supportsAllDrives);
                builder.AppendOptionalQueryParameter(ref firstArg, "includePermissionsForView", includePermissionsForView);
                builder.AppendOptionalQueryParameter(ref firstArg, "includeLabels", includeLabels);
                path = builder.ToString();
            }
            var req = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = ProtoJsonContent.Create(file, DriveApiV3SerializerContext.Default.File)
            };
            req.SetRequiredGcpScope(DriveConfiguration.AdjustScope(Methods.CreateMetadataFile, DriveFileScope));
            return req;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private HttpRequestMessage CreateSimpleUploadFileRequest(
        bool? ignoreDefaultVisibility,
        bool? keepRevisionForever,
        string? ocrLanguage,
        bool? supportsAllDrives,
        bool? useContentAsIndexableText,
        string? includePermissionsForView,
        string? includeLabels,
        Stream stream)
    {
        var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
        try
        {
            var basePath = GetCachedMethodPath(Methods.SimpleUploadFile);
            string path;
            {
                var firstArg = true;
                var builder = new SpanBuilder(buffer);
                builder.Append(basePath);
                builder.AppendOptionalQueryParameter(ref firstArg, "ignoreDefaultVisibility", ignoreDefaultVisibility);
                builder.AppendOptionalQueryParameter(ref firstArg, "keepRevisionForever", keepRevisionForever);
                builder.AppendOptionalQueryParameter(ref firstArg, "ocrLanguage", ocrLanguage);
                builder.AppendOptionalQueryParameter(ref firstArg, "supportsAllDrives", supportsAllDrives);
                builder.AppendOptionalQueryParameter(ref firstArg, "uploadType", "media");
                builder.AppendOptionalQueryParameter(ref firstArg, "useContentAsIndexableText", useContentAsIndexableText);
                builder.AppendOptionalQueryParameter(ref firstArg, "includePermissionsForView", includePermissionsForView);
                builder.AppendOptionalQueryParameter(ref firstArg, "includeLabels", includeLabels);
                path = builder.ToString();
            }
            var req = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = new StreamContent(stream)
            };
            req.SetRequiredGcpScope(DriveConfiguration.AdjustScope(Methods.CreateMetadataFile, DriveFileScope));
            return req;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private HttpRequestMessage CreateMultipartUploadFileRequest(
        bool? ignoreDefaultVisibility,
        bool? keepRevisionForever,
        string? ocrLanguage,
        bool? supportsAllDrives,
        bool? useContentAsIndexableText,
        string? includePermissionsForView,
        string? includeLabels,
        Drive.File file,
        Stream stream)
    {
        var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
        try
        {
            var basePath = GetCachedMethodPath(Methods.SimpleUploadFile);
            string path;
            {
                var firstArg = true;
                var builder = new SpanBuilder(buffer);
                builder.Append(basePath);
                builder.AppendOptionalQueryParameter(ref firstArg, "ignoreDefaultVisibility", ignoreDefaultVisibility);
                builder.AppendOptionalQueryParameter(ref firstArg, "keepRevisionForever", keepRevisionForever);
                builder.AppendOptionalQueryParameter(ref firstArg, "ocrLanguage", ocrLanguage);
                builder.AppendOptionalQueryParameter(ref firstArg, "supportsAllDrives", supportsAllDrives);
                builder.AppendOptionalQueryParameter(ref firstArg, "uploadType", "media");
                builder.AppendOptionalQueryParameter(ref firstArg, "useContentAsIndexableText", useContentAsIndexableText);
                builder.AppendOptionalQueryParameter(ref firstArg, "includePermissionsForView", includePermissionsForView);
                builder.AppendOptionalQueryParameter(ref firstArg, "includeLabels", includeLabels);
                path = builder.ToString();
            }
            var req = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = new MultipartContent("related")
                {
                    ProtoJsonContent.Create(file, DriveApiV3SerializerContext.Default.File),
                    new StreamContent(stream)
                }
            };
            req.SetRequiredGcpScope(DriveConfiguration.AdjustScope(Methods.CreateMetadataFile, DriveFileScope));
            return req;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private HttpRequestMessage CreateInitializeResumableUploadRequest(
        bool? ignoreDefaultVisibility,
        bool? keepRevisionForever,
        string? ocrLanguage,
        bool? supportsAllDrives,
        bool? useContentAsIndexableText,
        string? includePermissionsForView,
        string? includeLabels,
        Drive.File? file)
    {
        var buffer = ArrayPool<char>.Shared.Rent(8 * 1024);
        try
        {
            var basePath = GetCachedMethodPath(Methods.InitializeResumableUpload);
            string path;
            {
                var firstArg = true;
                var builder = new SpanBuilder(buffer);
                builder.Append(basePath);
                builder.AppendOptionalQueryParameter(ref firstArg, "ignoreDefaultVisibility", ignoreDefaultVisibility);
                builder.AppendOptionalQueryParameter(ref firstArg, "keepRevisionForever", keepRevisionForever);
                builder.AppendOptionalQueryParameter(ref firstArg, "ocrLanguage", ocrLanguage);
                builder.AppendOptionalQueryParameter(ref firstArg, "supportsAllDrives", supportsAllDrives);
                builder.AppendOptionalQueryParameter(ref firstArg, "uploadType", "resumable");
                builder.AppendOptionalQueryParameter(ref firstArg, "useContentAsIndexableText", useContentAsIndexableText);
                builder.AppendOptionalQueryParameter(ref firstArg, "includePermissionsForView", includePermissionsForView);
                builder.AppendOptionalQueryParameter(ref firstArg, "includeLabels", includeLabels);
                path = builder.ToString();
            }
            var req = new HttpRequestMessage(HttpMethod.Post, path);
            if (file is not null)
            {
                req.Content = ProtoJsonContent.Create(file, DriveApiV3SerializerContext.Default.File);
                if (file.MimeType is string { Length: > 0 } mimeType)
                {
                    req.Headers.Add("X-Upload-Content-Type", mimeType);
                }
            }
            req.SetRequiredGcpScope(DriveConfiguration.AdjustScope(Methods.CreateMetadataFile, DriveFileScope));
            return req;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private ValueTask<string> ReadInitializeResumableUploadResponse(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.Headers.Location is not Uri location)
        {
            throw new GoogleCloudException("Drive resumable upload endpoint returned no location.");
        }
        return new(location.AbsoluteUri);
    }


    protected override ValueTask HandleErrors(HttpResponseMessage response, CancellationToken cancellationToken)
        => response.HandleGoogleCloudErrorResponseAsync(cancellationToken);
}