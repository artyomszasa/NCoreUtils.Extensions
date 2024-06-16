using NCoreUtils.Google;

namespace NCoreUtils;

public interface IGoogleStorageAdminClient
{
    /// <summary>
    /// eturns metadata for the specified bucket.
    /// </summary>
    /// <param name="bucket"></param>
    /// <param name="projection"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<GoogleBucket> GetBucketAsync(
        string bucket,
        string projection = "noAcl",
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Creates a new bucket.
    /// </summary>
    /// <remarks>
    /// Google Cloud Storage uses a flat namespace, so you can't create a bucket with a name that is already in use. For
    /// more information, see the bucket naming guidelines.
    /// </remarks>
    /// <param name="request">A bucket resource.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A resource representing a newly created bucket.</returns>
    Task<GoogleBucket> InsertBucketAsync(
        CreateBucketRequest request,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Permanently deletes an empty bucket. The request fails if there are any live or noncurrent objects in the
    /// bucket, but the request succeeds if the bucket only contains soft-deleted objects or incomplete uploads,
    /// such as ongoing XML API multipart uploads. Does not delete soft-deleted objects.
    /// </summary>
    /// <param name="bucket">Bucket name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteBucketAsync(string bucket, CancellationToken cancellationToken = default);
}