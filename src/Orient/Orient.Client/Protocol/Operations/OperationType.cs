namespace Orient.Client.Protocol.Operations
{
    internal enum OperationType
    {
        SHUTDOWN = 1,
        CONNECT = 2,
        DB_OPEN = 3,
        DB_CREATE = 4,
        DB_CLOSE = 5,
        DB_EXIST = 6,
        DB_DROP = 7,
        DB_SIZE = 8,
        DB_COUNTRECORDS = 9,
        DATACLUSTER_ADD = 10,
        DATACLUSTER_DROP = 11,
        DATACLUSTER_COUNT = 12,
        DATACLUSTER_DATARANGE = 13,
        DATASEGMENT_ADD = 20,
        DATASEGMENT_REMOVE = 21,
        RECORD_METADATA = 29,
        RECORD_LOAD = 30,
        RECORD_CREATE = 31,
        RECORD_UPDATE = 32,
        RECORD_DELETE = 33,
        COUNT = 40,
        COMMAND = 41,
        TX_COMMIT = 60,
        CONFIG_GET = 70,
        CONFIG_SET = 71,
        CONFIG_LIST = 72,
        DB_RELOAD = 73,
        DB_LIST = 74,
        CREATE_SBTREE_BONSAI = 110,
        SBTREE_BONSAI_GET = 111,
        SBTREE_BONSAI_FIRST_KEY = 112,
        SBTREE_BONSAI_GET_ENTRIES_MAJOR = 113,
        RIDBAG_GET_SIZE = 114
    }
}