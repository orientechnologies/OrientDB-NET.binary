using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Tests.Issues
{
    [TestFixture(Category = "issues")]
    public class GitHub_issue46
    {

        TestDatabaseContext _context;
        ODatabase _database;

        [SetUp]
        public void Init()
        {
            _context = new TestDatabaseContext();
            _database = new ODatabase(TestConnection.GlobalTestDatabaseAlias);
            _database.Create.Class<DemoEmployee>().Run();
        }

        [TearDown]
        public void Cleanup()
        {
            _database.Dispose();
            _context.Dispose();
        }

        [Test]
        public void ShouldSupportCommonDataType()
        {
            for (int index = 1; index <= 1; index++)
            {
                var guid = Guid.NewGuid();
                var employee = new DemoEmployee();
                employee.MyFavoriteColor = DemoEmployee.Color.Red;
                employee.SomeOtherId = guid;
                employee.Name = "Janet";
                employee.Age = index;
                employee.WorkingDays = new List<DayOfWeek>();
                employee.WorkingDays.Add(DayOfWeek.Monday);
                employee.WorkingDays.Add(DayOfWeek.Tuesday);
                employee.WorkingDays.Add(DayOfWeek.Wednesday);

                var document = _database.Insert<DemoEmployee>(employee).Run();
                var result = document.To<DemoEmployee>();

                Assert.That(result, Is.Not.Null);
                Assert.That(result.MyFavoriteColor, Is.EqualTo(DemoEmployee.Color.Red));
                Assert.That(result.SomeOtherId, Is.EqualTo(guid));
                Assert.That(result.Name, Is.EqualTo("Janet"));
                Assert.That(result.Age, Is.EqualTo(index));
                Assert.That(result.WorkingDays, Is.EquivalentTo(new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday }));
            }
        }

        public class DemoEmployee
        {
            public Guid SomeOtherId { get; set; }

            public string Name { get; set; }

            public int Age { get; set; }

            public long SomeVeryLongNumber { get; set; }

            public decimal Salary { get; set; }

            public List<DayOfWeek> WorkingDays { get; set; }
            public DateTime? IncrementDate { get; set; }

            public int? Height { get; set; }
            public double Commission { get; set; }

            public float Allowance { get; set; }

            public DateTime BirthDate { get; set; }

            public bool IsMarried { get; set; }

            public List<int> SomeIntegerList { get; set; }

            public int[] SomeIntegerArrayList { get; set; }

            public List<decimal> SomeDecimalList { get; set; }

            public decimal[] SomeDecomeArray { get; set; }

            public IList<int> SomeIListInteger { get; set; }

            public Dictionary<int, string> MyKeyValues { get; set; }

            public SortedList<int, string> YearlyAchievement { get; set; }

            public Color MyFavoriteColor { get; set; }

            public enum Color
            {
                Blue, Red
            }
        }
    }
}
