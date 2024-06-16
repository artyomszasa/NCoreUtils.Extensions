
namespace NCoreUtils.Google;

public class GoogleStorageAdminClient(IGoogleStorageAdminApiV1 api, string projectId) : IGoogleStorageAdminClient
{
    public IGoogleStorageAdminApiV1 Api { get; } = api ?? throw new ArgumentNullException(nameof(api));

    public string ProjectId { get; } = projectId ?? throw new ArgumentNullException(nameof(projectId));

    public Task DeleteBucketAsync(string bucket, CancellationToken cancellationToken = default)
        => Api.DeleteBucketAsync(bucket, cancellationToken);

    public Task<GoogleBucket> GetBucketAsync(string bucket, string projection = "noAcl", CancellationToken cancellationToken = default)
        => Api.GetBucketAsync(bucket, projection, cancellationToken);

    public Task<GoogleBucket> InsertBucketAsync(CreateBucketRequest request, CancellationToken cancellationToken = default)
        => Api.InsertBucketAsync(ProjectId, request, cancellationToken);
}