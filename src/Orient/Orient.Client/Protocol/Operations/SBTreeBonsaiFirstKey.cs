using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orient.Client.Protocol.Operations
{
    internal class SBTreeBonsaiFirstKey : IOperation
    {
        internal long FileId;
        internal long PageIndex;
        internal int PageOffset;

        public Request Request(int sessionID)
        {
            Request request = new Request();
            request.OperationMode = OperationMode.Synchronous;

            // standard request fields
            request.AddDataItem((byte)OperationType.SBTREE_BONSAI_FIRST_KEY);
            request.AddDataItem(sessionID);

            // collection pointer
            request.AddDataItem(FileId);
            request.AddDataItem(PageIndex);
            request.AddDataItem(PageOffset);

            return request;
        }

        public ODocument Response(Response response)
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

            document.SetField<ORID>("rid", new ORID(clusterId,clusterPosition));

            return document;
        }
    }
}
