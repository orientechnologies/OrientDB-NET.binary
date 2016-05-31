using System;
namespace Orient.Client.API.Query.Interfaces
{
    public interface IOCreateDocument
    {
        IOCreateDocument Cluster(string clusterName);
        IOCreateDocument Cluster<T>();
        IOCreateDocument Document(string className);
        IOCreateDocument Document<T>();
        IOCreateDocument Document<T>(T obj);
        Orient.Client.ODocument Run();
        T Run<T>() where T : class, new();
        IOCreateDocument Set<T>(string fieldName, T fieldValue);
        IOCreateDocument Set<T>(T obj);
        string ToString();
    }
}
