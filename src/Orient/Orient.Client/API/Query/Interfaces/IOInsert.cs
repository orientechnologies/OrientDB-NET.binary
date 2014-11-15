using System;
namespace Orient.Client.API.Query.Interfaces
{
    public interface IOInsert
    {
        IOInsert Cluster(string clusterName);
        IOInsert Cluster<T>();
        IOInsert Insert(string className);
        IOInsert Insert<T>();
        IOInsert Insert<T>(T obj);
        IOInsert Into(string className);
        IOInsert Into<T>();
        ODocument Run();
        T Run<T>() where T : class, new();
        IOInsert Set<T>(string fieldName, T fieldValue);
        IOInsert Set<T>(T obj);
        string ToString();
    }
}
