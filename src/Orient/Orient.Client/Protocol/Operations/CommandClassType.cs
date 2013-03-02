
namespace Orient.Client.Protocol.Operations
{
    internal enum CommandClassType
    {
        // com.orientechnologies.orient.core.sql.query.OSQLSynchQuery - e.g. select
        Idempotent = 0,
        // com.orientechnologies.orient.core.sql.OCommandSQL - e.g. insert
        NonIdempotent = 1,
        // com.orientechnologies.orient.core.command.script.OCommandScript - e.g. javascript
        Script = 2,
        Default = 3
    }
}
