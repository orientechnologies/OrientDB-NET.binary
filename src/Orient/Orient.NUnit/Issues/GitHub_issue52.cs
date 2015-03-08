using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Tests.Issues
{
    [TestFixture(Category = "issue")]
    public class GitHub_issue52
    {
        [Test]
        public void ShouldInsertClassWithEmbededList()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            using (var db = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                var employeeListTypeClassName = "EmployeeListType";

                var clusterid = db.Create.Class(employeeListTypeClassName).Run();

                var employee = new EmployeeListType();
                employee.Id = Guid.NewGuid();
                employee.Name = new List<string>();
                employee.Name.Add("Andrew");
                employee.Name.Add("Jack");
                employee.Age = new List<int>();
                employee.Age.Add(1);
                employee.Age.Add(2);
                employee.BirthDate = new List<DateTime>();
                employee.BirthDate.Add(DateTime.Now);
                employee.BirthDate.Add(DateTime.Now.AddDays(-3));
                employee.Childs = new List<EmployeeCommonType>();
                employee.Childs.Add(new EmployeeCommonType() { Id = Guid.NewGuid() });
                employee.Childs.Add(new EmployeeCommonType() { Id = Guid.NewGuid() });
                employee.FavoriteColor = new List<Color>();
                employee.FavoriteColor.Add(Color.Red);
                employee.FavoriteColor.Add(Color.Blue);
                employee.Height = new List<short>();
                employee.Height.Add(323);
                employee.Height.Add(333);
                employee.Ids = new List<Guid>();
                employee.Ids.Add(Guid.NewGuid());
                employee.Ids.Add(Guid.NewGuid());
                employee.IsMarried = new List<bool>();
                employee.IsMarried.Add(true);
                employee.IsMarried.Add(false);
                employee.Salary = new List<decimal>();
                employee.Salary.Add((decimal)1234567890.123456789);
                employee.Salary.Add((decimal)1234567890.123456799);
                employee.Tall = new List<long>();
                employee.Tall.Add(3233);
                employee.Tall.Add(3234);
                employee.YearlyIncome = new List<double>();
                employee.YearlyIncome.Add(3233);
                employee.YearlyIncome.Add(1234);

                var document = db.Insert<EmployeeListType>(employee).Run();

                var result = db.Query<EmployeeListType>("SELECT * FROM " + employeeListTypeClassName + " WHERE Id = '" + employee.Id + "'").SingleOrDefault();

                Assert.AreEqual(employee.Id, result.Id);
                Assert.AreEqual(employee.Ids[0], result.Ids[0]);
                Assert.AreEqual(employee.Ids[1], result.Ids[1]);
                Assert.AreEqual(employee.Name[0], result.Name[0]);
                Assert.AreEqual(employee.Name[1], result.Name[1]);
                Assert.AreEqual(employee.Age[0], result.Age[0]);
                Assert.AreEqual(employee.Age[1], result.Age[1]);
                Assert.AreEqual(employee.Age[0], result.Age[0]);
                Assert.AreEqual(employee.Salary[0], result.Salary[0]);
                Assert.AreEqual(employee.Salary[1], result.Salary[1]);
                Assert.AreEqual(employee.IsMarried[0], result.IsMarried[0]);
                Assert.AreEqual(employee.IsMarried[1], result.IsMarried[1]);

                // Error happen here.
                Assert.AreEqual(employee.Childs[0].Id, result.Childs[0].Id);
                Assert.AreEqual(employee.Childs[1].Id, result.Childs[1].Id);
                Assert.AreEqual(employee.BirthDate[0].ToLongDateString(), result.BirthDate[0].ToLongDateString());
                Assert.AreEqual(employee.BirthDate[1].ToLongDateString(), result.BirthDate[1].ToLongDateString());
                Assert.AreEqual(employee.YearlyIncome[0], result.YearlyIncome[0]);
                Assert.AreEqual(employee.YearlyIncome[1], result.YearlyIncome[1]);
                Assert.AreEqual(employee.FavoriteColor[0], result.FavoriteColor[0]);
                Assert.AreEqual(employee.FavoriteColor[1], result.FavoriteColor[1]);
                Assert.AreEqual(employee.Height[0], result.Height[0]);
                Assert.AreEqual(employee.Height[1], result.Height[1]);
                Assert.AreEqual(employee.Tall[0], result.Tall[0]);
                Assert.AreEqual(employee.Tall[1], result.Tall[1]);

            }
        }

        public class EmployeeListType
        {
            public Guid Id { get; set; }

            public List<Guid> Ids { get; set; }
            public List<string> Name { get; set; }

            public List<int> Age { get; set; }

            public List<decimal> Salary { get; set; }

            public List<bool> IsMarried { get; set; }

            public List<DateTime> BirthDate { get; set; }

            public List<double> YearlyIncome { get; set; }

            public List<short> Height { get; set; }

            public List<long> Tall { get; set; }

            public List<Color> FavoriteColor { get; set; }

            public List<EmployeeCommonType> Childs { get; set; }
        }

        public class EmployeeCommonType
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public int Age { get; set; }

            public decimal Salary { get; set; }

            public bool IsMarried { get; set; }

            public DateTime BirthDate { get; set; }

            public double YearlyIncome { get; set; }

            public short Height { get; set; }

            public long Tall { get; set; }

            public Color FavoriteColor { get; set; }
        }

        public enum Color
        {
            Blue, Red, Green, Yellow
        }
    }

}
