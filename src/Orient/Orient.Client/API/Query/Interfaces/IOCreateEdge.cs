using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orient.Client.API.Query.Interfaces
{
    public interface IOCreateEdge
    {
        IOCreateEdge Edge(string className);
        IOCreateEdge Edge<T>(T obj);
        IOCreateEdge Edge<T>();
        IOCreateEdge Cluster(string clusterName);
        IOCreateEdge Cluster<T>();
        IOCreateEdge From(ORID orid);
        IOCreateEdge From<T>(T obj);
        IOCreateEdge To(ORID orid);
        IOCreateEdge To<T>(T obj);
        IOCreateEdge Set<T>(string fieldName, T fieldValue);
        IOCreateEdge Set<T>(T obj);
        OEdge Run();
        T Run<T>() where T : class, new();
        string ToString();
    }
}
