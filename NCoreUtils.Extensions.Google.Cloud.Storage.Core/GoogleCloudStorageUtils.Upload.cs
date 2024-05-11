using System.Buffers;
using NCoreUtils.Google;

namespace NCoreUtils;

public partial class GoogleCloudStorageUtils
{
    public async Task<GoogleObjectData> UploadAsync(
        string bucket,
        string name,
        Stream source,
        string? contentType = default,
        string? cacheControl = default,
        IEnumerable<GoogleAccessControlEntry>? acl = default,
        string? accessToken = default,
        Action<long>? progress = default,
        IMemoryOwner<byte>? buffer = default,
        CancellationToken cancellationToken = default)
    {
        // prepare data
        var acl1 = acl is null ? default : acl.ToList();
        // initialize resumable upload
        var endpoint = await InitializeResumableUploadAsync(
            bucket: bucket,
            name: name,
            contentType: contentType,
            cacheControl: cacheControl,
            origin: default,
            acl: acl1,
            accessToken: accessToken,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        // configure uploader
        await using var uploader = new GoogleCloudStorageUploader(
            CreateHttpClient(),
            endpoint,
            contentType: contentType,
            buffer
        );
        // configure progress observere
        var size = 0L;
        uploader.Progress += (_, e) =>
        {
            progress?.Invoke(e.Sent);
            size = e.Sent;
        };
        // upload object
        await uploader.UploadAsync(source, true, cancellationToken).ConfigureAwait(false);
        // create result objects
        var obj = new GoogleObjectData
        {
            BucketName = bucket,
            Name = name,
            ContentType = contentType,
            CacheControl = cacheControl ?? DefaultCacheControl,
            Size = unchecked((ulong)size),
        };
        if (acl1 is not null)
        {
            obj.Acl.AddRange(acl1);
        }
        return obj;
    }

    public Task<GoogleObjectData> UploadAsync(
        string bucket,
        string name,
        Stream source,
        string? contentType = default,
        string? cacheControl = default,
        bool isPublic = true,
        string? accessToken = default,
        Action<long>? progress = default,
        IMemoryOwner<byte>? buffer = default,
        CancellationToken cancellationToken = default)
        => UploadAsync(
            bucket,
            name,
            source,
            contentType,
            cacheControl,
            isPublic ? _publicRead : default,
            accessToken,
            progress,
            buffer,
            cancellationToken
        );
}