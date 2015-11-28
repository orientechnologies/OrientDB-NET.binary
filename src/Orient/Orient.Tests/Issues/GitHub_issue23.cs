using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Issues
{
    
    public class GitHub_issue23 : IDisposable
    {
        TestDatabaseContext _context;
        ODatabase _database;
        
        public GitHub_issue23()
        {
            _context = new TestDatabaseContext();
            _database = new ODatabase(TestConnection.GlobalTestDatabaseAlias);
            _database.Create.Class<DemoEmployee>().Run();

        }

        public void Dispose()
        {
            _database.Dispose();
            _context.Dispose();
        }

        [Fact]
        public void ShouldSupportOrientDBObjectType()
        {
            var employee = new DemoEmployee();
            employee.SomeOtherId = Guid.NewGuid();
            employee.Name = "Janet";
            employee.Age = 33;
            employee.SomeVeryLongNumber = 12345678901234567;
            employee.BirthDate = new DateTime(2015, 1, 24, 2, 2, 2);
            employee.Salary = (decimal)23434.1234567891;
            employee.Commission = (double)23434.1234567891;
            employee.Allowance = (float)3434.1234567891;
            employee.IsMarried = true;
            employee.SomeIntegerList = new List<int>();
            employee.SomeIntegerList.Add(1);
            employee.SomeIntegerList.Add(2);
            employee.SomeIntegerList.Add(3);
            employee.SomeIntegerArrayList = new int[3] { 4, 2, 3 };
            employee.SomeDecimalList = new List<decimal>();
            employee.SomeDecimalList.Add((decimal)23434.1234567891);
            employee.SomeDecomeArray = new decimal[3] { (decimal)23434.1234567890, (decimal)23434.1234567890, (decimal)23434.1234567890 };
            employee.Kids = new List<DemoChild>();

            var kid = new DemoChild();
            kid.Name = "Janet";
            kid.Age = 33;
            kid.SomeVeryLongNumber = 12345678901234567;
            kid.BirthDate = new DateTime(2015, 1, 24, 2, 2, 2);
            kid.Salary = (decimal)23434.1234567891;
            kid.Commission = (double)23434.1234567891;
            kid.Allowance = (float)3434.1234567891;
            kid.IsMarried = true;

            kid.SomeIntegerList = new List<int>();
            kid.SomeIntegerList.Add(1);
            kid.SomeIntegerList.Add(2);
            kid.SomeIntegerList.Add(3);
            kid.SomeIntegerArrayList = new int[3] { 4, 2, 3 };

            kid.SomeDecimalList = new List<decimal>();
            kid.SomeDecimalList.Add((decimal)23434.1234567891);
            kid.SomeDecomeArray = new decimal[3] { (decimal)23434.1234567890, (decimal)23434.1234567890, (decimal)23434.1234567890 };
            employee.Kids.Add(kid);

            employee.SomeIListInteger = new List<int>();
            employee.SomeIListInteger.Add(7);
            employee.MyKeyValues = new Dictionary<string, long>();
            employee.MyKeyValues.Add("1", 578933315553);
            employee.MyKeyValues.Add("2", 123445555555555);

            employee.MyFavoriteColor = DemoEmployee.Color.Yellow;

            var document = _database.Insert<DemoEmployee>(employee).Run();
            Assert.NotNull(document);
            Assert.NotNull(document.ORID);
            Assert.Equal(employee.SomeOtherId, document.GetField<Guid>("SomeOtherId"));
            Assert.Equal(employee.Name, document.GetField<string>("Name"));
            Assert.Equal(employee.Age, document.GetField<int>("Age"));
            Assert.Equal(employee.SomeVeryLongNumber, document.GetField<long>("SomeVeryLongNumber"));
            Assert.Equal(employee.BirthDate, document.GetField<DateTime>("BirthDate"));
            Assert.Equal(employee.Salary, document.GetField<Decimal>("Salary"));
            Assert.Equal(employee.Commission, document.GetField<Double>("Commission"));

            Assert.Equal(employee.Allowance, document.GetField<float>("Allowance"));

            Assert.Equal(employee.IsMarried, document.GetField<bool>("IsMarried"));

            Assert.NotNull(employee.SomeIntegerList);
            Assert.True(document.GetField<List<int>>("SomeIntegerList").SequenceEqual(employee.SomeIntegerList));

            Assert.NotNull(employee.SomeDecimalList);
            Assert.True(document.GetField<List<decimal>>("SomeDecimalList").SequenceEqual(employee.SomeDecimalList));

            Assert.NotNull(employee.SomeDecomeArray);
            Assert.True(document.GetField<List<decimal>>("SomeDecomeArray").SequenceEqual(employee.SomeDecomeArray));

            var result = document.To<DemoEmployee>();

        }

        public class DemoEmployee
        {
            public Guid SomeOtherId { get; set; }

            public string Name { get; set; }

            public int Age { get; set; }

            public long SomeVeryLongNumber { get; set; }

            public DateTime BirthDate { get; set; }

            public decimal Salary { get; set; }

            public double Commission { get; set; }

            public float Allowance { get; set; }


            public bool IsMarried { get; set; }

            public List<int> SomeIntegerList { get; set; }

            public int[] SomeIntegerArrayList { get; set; }

            public List<decimal> SomeDecimalList { get; set; }

            public decimal[] SomeDecomeArray { get; set; }

            public List<DemoChild> Kids { get; set; }

            public IList<int> SomeIListInteger { get; set; }

            public Dictionary<string, long> MyKeyValues { get; set; }  // Map in orientdb

            public Color MyFavoriteColor { get; set; }

            public enum Color
            {
                Blue, Red, Green, Yellow
            }
        }

        public class DemoChild
        {
            public string Name { get; set; }

            public int Age { get; set; }

            public long SomeVeryLongNumber { get; set; }

            public decimal Salary { get; set; }

            public double Commission { get; set; }

            public float Allowance { get; set; }

            public DateTime BirthDate { get; set; }

            public bool IsMarried { get; set; }

            public List<int> SomeIntegerList { get; set; }

            public int[] SomeIntegerArrayList { get; set; }

            public List<decimal> SomeDecimalList { get; set; }

            public decimal[] SomeDecomeArray { get; set; }
        }

    }
}
