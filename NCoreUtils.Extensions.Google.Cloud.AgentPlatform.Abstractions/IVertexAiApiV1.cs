namespace NCoreUtils.Google.Cloud.AgentPlatform;

public interface IAgentPlatformApiV1
{
    Task<PredictResponse> PredictAsync(
        string project,
        string location,
        string publisher,
        string model,
        IReadOnlyList<string> prompts,
        PredictRequestParameters parameters,
        CancellationToken cancellationToken = default
    );

    Task<GenerateContentResponse> GenerateContentAsync(
        string project,
        string location,
        string publisher,
        string model,
        GenerateContentRequest request,
        CancellationToken cancellationToken = default
    );

    Task<Operation> RunPredictAsync(
        string project,
        string location,
        string publisher,
        string model,
        IReadOnlyList<string> prompts,
        PredictRequestParameters parameters,
        CancellationToken cancellationToken = default
    );

    Task<Operation> GetOperationAsync(string name, CancellationToken cancellationToken = default);
}