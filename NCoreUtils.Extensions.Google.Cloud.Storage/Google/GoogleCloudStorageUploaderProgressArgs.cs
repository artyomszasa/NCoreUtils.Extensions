using System;

namespace NCoreUtils.Google
{
    public class GoogleCloudStorageUploaderProgressArgs : EventArgs
    {
        public long Sent { get; }

        public GoogleCloudStorageUploaderProgressArgs(long sent)
            => Sent = sent;
    }
}