using Hazelcast.IO.Serialization;
using Hazelcast.Serialization.Hook;

namespace Hazelcast.Client.Request.Collection
{
    internal class ListIndexOfRequest : CollectionRequest
    {
        internal bool last;
        internal Data value;


        public ListIndexOfRequest(string name, Data value, bool last) : base(name)
        {
            this.value = value;
            this.last = last;
        }

        public override int GetClassId()
        {
            return CollectionPortableHook.ListIndexOf;
        }

        /// <exception cref="System.IO.IOException"></exception>
        public override void WritePortable(IPortableWriter writer)
        {
            base.WritePortable(writer);
            writer.WriteBoolean("l", last);
            value.WriteData(writer.GetRawDataOutput());
        }

    }
}