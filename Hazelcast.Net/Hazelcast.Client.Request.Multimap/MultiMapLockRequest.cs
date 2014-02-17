using Hazelcast.Client.Request.Concurrent.Lock;
using Hazelcast.IO.Serialization;
using Hazelcast.Serialization.Hook;

namespace Hazelcast.Client.Request.Multimap
{
    internal class MultiMapLockRequest : AbstractLockRequest
    {
        internal string name;

        public MultiMapLockRequest()
        {
        }

        public MultiMapLockRequest(Data key, long threadId, string name)
            : base(key, threadId)
        {
            this.name = name;
        }

        public MultiMapLockRequest(Data key, long threadId, long ttl, long timeout, string name)
            : base(key, threadId, ttl, timeout)
        {
            this.name = name;
        }

        /// <exception cref="System.IO.IOException"></exception>
        public override void WritePortable(IPortableWriter writer)
        {
            writer.WriteUTF("n", name);
            base.WritePortable(writer);
        }

        public override int GetFactoryId()
        {
            return MultiMapPortableHook.FId;
        }

        public override int GetClassId()
        {
            return MultiMapPortableHook.Lock;
        }
    }
}