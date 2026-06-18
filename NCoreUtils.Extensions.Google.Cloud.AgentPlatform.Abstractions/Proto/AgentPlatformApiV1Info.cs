using NCoreUtils.Proto;

namespace NCoreUtils.Google.Cloud.AgentPlatform.Proto;

[ProtoInfo(typeof(IAgentPlatformApiV1), Path = "")]
[ProtoMethodInfo(nameof(IAgentPlatformApiV1.GenerateContentAsync), HttpMethod = NCoreUtils.Proto.HttpMethod.Post, Input = InputType.Custom, Path = "v1")]
[ProtoMethodInfo(nameof(IAgentPlatformApiV1.PredictAsync), HttpMethod = NCoreUtils.Proto.HttpMethod.Post, Input = InputType.Custom, Path = "v1")]
[ProtoMethodInfo(nameof(IAgentPlatformApiV1.RunPredictAsync), HttpMethod = NCoreUtils.Proto.HttpMethod.Post, Input = InputType.Custom, Path = "v1")]
[ProtoMethodInfo(nameof(IAgentPlatformApiV1.GetOperationAsync), HttpMethod = NCoreUtils.Proto.HttpMethod.Get, Input = InputType.Custom, Path = "v1")]
public partial class AgentPlatformApiV1Info { }