using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;

namespace NCoreUtils.Collections
{
    static class SerializationHelper
    {
        static ConditionalWeakTable<object, SerializationInfo>? _serializationInfoTable;

        internal static ConditionalWeakTable<object, SerializationInfo> SerializationInfoTable
        {
            get
            {
                if (null == _serializationInfoTable)
                {
                    var newTable = new ConditionalWeakTable<object, SerializationInfo>();
                    Interlocked.CompareExchange(ref _serializationInfoTable, newTable, null);
                }
                return _serializationInfoTable;
            }
        }
    }
}