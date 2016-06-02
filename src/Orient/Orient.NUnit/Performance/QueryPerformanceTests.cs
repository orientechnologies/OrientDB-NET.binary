using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Tests
{
    /// <summary>
    /// Test class to run some parallel queries and measure the execution duration.
    /// </summary>
    /// <remarks>
    /// Test class is ignored to save time.
    /// The operations are covered by other test cases.
    /// </remarks>
    [TestFixture]
    [Ignore]
    [Category("performance")]
    public class QueryPerformanceTests
    {
        private const int TestDocumentCount = 20000;        
        private const int TestCycles = 10;
        private static readonly Random Random = new Random();

        [TestCase]        
        public void TestParallelPerformanceForLongQuery()
        {
            var queryIterations = 500;
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    database
                        .Create.Class("TestClass")
                        .Run();
                    var transaction = database.CreateTransaction();

                    for (var i = 0; i < TestDocumentCount; i++)
                    {
                        var document = new ODocument { OClassName = "TestClass" };
                        document
                            .SetField("foo", "foo string value")
                            .SetField("bar", Random.Next(TestDocumentCount));
                        transaction.Add(document);
                    }

                    transaction.Commit();                    
                }

                Action<int> queryToRun = i =>
                {
                    using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                    {
                        var result = database.Query("select count(*) as countResult from TestClass where bar > " + i).Single();
                        Assert.GreaterOrEqual(result.GetField<long>("countResult"), 0);
                    }
                };

                for (var i = 0; i < TestCycles; i++)
                {
                    var timer = System.Diagnostics.Stopwatch.StartNew();
                    var parallelExecutionResult = Parallel.For(0, queryIterations, queryToRun);
                    timer.Stop();
                    Assert.IsTrue(parallelExecutionResult.IsCompleted);
                    System.Diagnostics.Debug.WriteLine(timer.ElapsedMilliseconds);
                }
            }
        }

        [TestCase]        
        public void TestParallelPerformanceForShortQuery()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    database
                        .Create.Class("TestClass")
                        .Run();

                    database.Create.Property("bar", OType.Integer).Class("TestClass").Run();
                    database.Command("CREATE INDEX TestClass.bar UNIQUE_HASH_INDEX");
                    var transaction = database.CreateTransaction();

                    for (var i = 0; i < TestDocumentCount; i++)
                    {
                        var document = new ODocument { OClassName = "TestClass" };
                        document
                            .SetField("foo", "foo string value")
                            .SetField("bar", i);
                        transaction.Add(document);
                    }

                    transaction.Commit();
                }

                Action<int> queryToRun = i =>
                {
                    using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                    {
                        var result = database.Query("select from TestClass where bar = " + i).Single();
                        Assert.AreEqual(i, result.GetField<int>("bar"));
                    }
                };

                for (var i = 0; i < TestCycles; i++)
                {
                    var timer = System.Diagnostics.Stopwatch.StartNew();
                    var parallelExecutionResult = Parallel.For(0, TestDocumentCount, queryToRun);
                    timer.Stop();
                    Assert.IsTrue(parallelExecutionResult.IsCompleted);
                    System.Diagnostics.Debug.WriteLine(timer.ElapsedMilliseconds);
                }
            }
        }
    }
}
