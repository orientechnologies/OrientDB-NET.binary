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
            _operationType = OperationType.SBTREE_BONSAI_FIRST_KEY;
        }
        public override Request Request(Request request)
        {
            base.Request(request);

            request.OperationMode = OperationMode.Synchronous;

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
            if (response.Connection.ProtocolVersion > 26 && response.Connection.UseTokenBasedSession)
                ReadToken(reader); 

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
