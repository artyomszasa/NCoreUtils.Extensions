using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.Monitoring;

public class MonitoredResource(
    string type,
    IReadOnlyDictionary<string, string>? labels = default)
{
    public static MonitoredResource K8sContainer(
        string projectId,
        string location,
        string clusterName,
        string namespaceName,
        string podName,
        string containerName)
        => new(
            type: "k8s_container",
            labels: new Dictionary<string, string>()
            {
                { "project_id", projectId },
                { "location", location },
                { "cluster_name", clusterName },
                { "namespace_name", namespaceName },
                { "pod_name", podName },
                { "container_name", containerName }
            }
        );

    [JsonPropertyName("type")]
    public string Type { get; } = type;

    [JsonPropertyName("labels")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyDictionary<string, string>? Labels { get; } = labels;
}
