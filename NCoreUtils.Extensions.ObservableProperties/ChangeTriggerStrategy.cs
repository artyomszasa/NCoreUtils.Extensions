namespace NCoreUtils;

public enum ChangeTriggerStrategy
{
    /// <summary>
    /// Change event is only triggered when current value is not equal to the new value according to the selected
    /// equality comparison.
    /// </summary>
    Default = 0,
    /// <summary>
    /// Change event is triggered each time value is set (not depending in old/new value equality).
    /// </summary>
    Always = 1,
    /// <summary>
    /// Change event is triggered once value is set for the first time, then each time current value is not equal to the
    /// new value accoring to the selected equality comparison.
    /// </summary>
    InitialSet = 2
}