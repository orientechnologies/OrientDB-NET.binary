using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orient.Client.Protocol.Operations
{
    internal class SBTreeBonsaiFirstKey : BaseOperation
    {
        internal long FileId;
        internal long PageIndex;
        internal int PageOffset;

        public SBTreeBonsaiFirstKey(ODatabase database)
            : base(database)
        {

        }
        public override Request Request(Request request)
        {
            request.OperationMode = OperationMode.Synchronous;

            // standard request fields
            request.AddDataItem((byte)OperationType.SBTREE_BONSAI_FIRST_KEY);
            request.AddDataItem(request.SessionId);

            // collection pointer
            request.AddDataItem(FileId);
            request.AddDataItem(PageIndex);
            request.AddDataItem(PageOffset);

            return request;
        }

        public override ODocument Response(Response response)
        {
            ODocument document = new ODocument();
            if (response == null)
            {
                return document;
            }

            var reader = response.Reader;

            // (keySerializerId:byte)(key:binary)
            var bytesLength = reader.ReadInt32EndianAware();
            byte keySerializerId = reader.ReadByte();
            short clusterId = reader.ReadInt16EndianAware();
            long clusterPosition = reader.ReadInt64EndianAware();

            document.SetField<ORID>("rid", new ORID(clusterId, clusterPosition));

            return document;
        }
    }
}
