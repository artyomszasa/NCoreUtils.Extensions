using NCoreUtils.Google;

namespace NCoreUtils;

public interface IGmailConfiguration
{
    string AdjustScope(GmailApiV1Client.Methods method, string defaultScope);
}