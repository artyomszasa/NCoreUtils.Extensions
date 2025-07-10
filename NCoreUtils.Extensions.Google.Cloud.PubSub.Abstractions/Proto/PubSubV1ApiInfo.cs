using NCoreUtils.Proto;

namespace NCoreUtils.Google.Cloud.PubSub.Proto;

[ProtoInfo(typeof(IPubSubV1Api), Path = "v1")]
[ProtoMethodInfo(nameof(IPubSubV1Api.AcknowledgeAsync), Path = "projects", Input = InputType.Custom)]
[ProtoMethodInfo(nameof(IPubSubV1Api.PublishAsync), Path = "projects", Input = InputType.Custom)]
[ProtoMethodInfo(nameof(IPubSubV1Api.PullAsync), Path = "projects", Input = InputType.Custom)]
public partial class PubSubV1ApiInfo { }