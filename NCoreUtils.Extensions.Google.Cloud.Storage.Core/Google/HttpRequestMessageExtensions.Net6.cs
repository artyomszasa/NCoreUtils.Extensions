using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace NCoreUtils.Google
{
    internal static class HttpRequestMessageExtensions
    {
        private static HttpRequestOptionsKey<string> KeyRequiredScope  { get; }
            = new HttpRequestOptionsKey<string>("RequiredGCSScope");

        public static void SetRequiredGSCScope(this HttpRequestMessage request, string scope)
            => request.Options.Set(KeyRequiredScope, scope);

        public static bool TryGetRequiredGCSScope(this HttpRequestMessage request, [NotNullWhen(true)] out string? scope)
        {
            return request.Options.TryGetValue(KeyRequiredScope, out scope);
        }
    }
}