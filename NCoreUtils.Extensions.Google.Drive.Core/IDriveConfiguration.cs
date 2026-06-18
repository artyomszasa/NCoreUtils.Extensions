using NCoreUtils.Google;

namespace NCoreUtils;

public interface IDriveConfiguration
{
    string AdjustScope(DriveApiV3Client.Methods method, string defaultScope);
}