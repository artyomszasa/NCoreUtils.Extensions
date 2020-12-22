using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace NCoreUtils.Google
{
    internal static class HttpRequestMessageExtensions
    {
        private const string KeyRequiredScope = "RequiredGCSScope";

        public static void SetRequiredGSCScope(this HttpRequestMessage request, string scope)
            => request.Properties[KeyRequiredScope] = scope;

        public static bool TryGetRequiredGCSScope(this HttpRequestMessage request, [NotNullWhen(true)] out string? scope)
        {
            if (request.Properties.TryGetValue(KeyRequiredScope, out var boxed) && !(boxed is null) && boxed is string value)
            {
                scope = value;
                return true;
            }
            scope = default;
            return false;
        }
    }
}