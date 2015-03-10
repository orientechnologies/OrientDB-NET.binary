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
    public class GitHub_issue54
    {
        [Test]
        public void GIVEN___EmployeeDictionaryType_class_with_common_data_type___WHEN___write_to_orientdb___THEN___should_be_able_to_read()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            using (var db = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                var employeeClassName = "EmployeeDictionaryType";

                var clusterid = db.Create.Class(employeeClassName).Run();

                var employee = new EmployeeDictionaryType();
                employee.Id = Guid.NewGuid();
                employee.Name = new Dictionary<string, string>();
                employee.Name.Add("Andrew", "Andrew");
                employee.Name.Add("Jack", "Jack");
                employee.Ids = new Dictionary<Guid, Guid>();
                employee.Ids.Add(Guid.NewGuid(), Guid.NewGuid());
                employee.Ids.Add(Guid.NewGuid(), Guid.NewGuid());
                employee.Age = new Dictionary<int, int>();
                employee.Age.Add(1, 2);
                employee.Age.Add(2, 4);
                employee.Height = new Dictionary<short, short>();
                employee.Height.Add(1, 2);
                employee.Height.Add(2, 4);
                employee.Tall = new Dictionary<long, long>();
                employee.Tall.Add(1234567890123456789, 987654321098765432);
                employee.Tall.Add(987654321098765432, 1234567890123456789);
                employee.Childs = new Dictionary<int, EmployeeCommonType>();
                employee.Childs.Add(1, new EmployeeCommonType() { Id = Guid.NewGuid() });
                employee.Childs.Add(2, new EmployeeCommonType() { Id = Guid.NewGuid() });
                employee.FavoriteColor = new Dictionary<Color, Color>();
                employee.FavoriteColor.Add(Color.Red, Color.Red);
                employee.FavoriteColor.Add(Color.Blue, Color.Blue);


                var document = db.Insert(employee).Into(employeeClassName).Run();

                var result = db.Query<EmployeeDictionaryType>("SELECT * FROM " + employeeClassName + " WHERE Id = '" + employee.Id + "'").SingleOrDefault();

                Assert.AreEqual(employee.Id, result.Id);
                Assert.True(result.Name.ContainsKey("Jack"));
                Assert.True(result.Name.ContainsKey("Andrew"));
                Assert.AreEqual(employee.Name["Jack"], result.Name["Jack"]);
                Assert.AreEqual(employee.Name["Andrew"], result.Name["Andrew"]);

                Assert.True(result.Ids.ContainsKey(employee.Ids.Keys.ToArray()[0]));
                Assert.True(result.Ids.ContainsKey(employee.Ids.Keys.ToArray()[1]));
                Assert.AreEqual(employee.Ids[employee.Ids.Keys.ToArray()[0]], result.Ids[employee.Ids.Keys.ToArray()[0]]);
                Assert.AreEqual(employee.Ids[employee.Ids.Keys.ToArray()[1]], result.Ids[employee.Ids.Keys.ToArray()[1]]);

                Assert.True(result.Age.ContainsKey(1));
                Assert.True(result.Age.ContainsKey(2));
                Assert.AreEqual(employee.Age[1], result.Age[1]);
                Assert.AreEqual(employee.Age[2], result.Age[2]);

                Assert.True(result.Height.ContainsKey(employee.Height.Keys.ToArray()[0]));
                Assert.True(result.Height.ContainsKey(employee.Height.Keys.ToArray()[1]));
                Assert.AreEqual(employee.Height[employee.Height.Keys.ToArray()[0]], result.Height[employee.Height.Keys.ToArray()[0]]);
                Assert.AreEqual(employee.Height[employee.Height.Keys.ToArray()[1]], result.Height[employee.Height.Keys.ToArray()[1]]);

                Assert.True(result.Tall.ContainsKey(employee.Tall.Keys.ToArray()[0]));
                Assert.True(result.Tall.ContainsKey(employee.Tall.Keys.ToArray()[1]));
                Assert.AreEqual(employee.Tall[employee.Tall.Keys.ToArray()[0]], result.Tall[employee.Tall.Keys.ToArray()[0]]);
                Assert.AreEqual(employee.Tall[employee.Tall.Keys.ToArray()[1]], result.Tall[employee.Tall.Keys.ToArray()[1]]);

                Assert.AreEqual(employee.Childs[1].Id, result.Childs[1].Id);
                Assert.AreEqual(employee.Childs[2].Id, result.Childs[2].Id);

                Assert.True(result.FavoriteColor.ContainsKey(Color.Red));
                Assert.True(result.FavoriteColor.ContainsKey(Color.Blue));
                Assert.AreEqual(employee.FavoriteColor[Color.Red], result.FavoriteColor[Color.Red]);
                Assert.AreEqual(employee.FavoriteColor[Color.Blue], result.FavoriteColor[Color.Blue]);

            }
        }

        public class EmployeeDictionaryType
        {
            public Guid Id { get; set; }
            public Dictionary<string, string> Name { get; set; }
            public Dictionary<Guid, Guid> Ids { get; set; }
            public Dictionary<int, int> Age { get; set; }
            public Dictionary<short, short> Height { get; set; }
            public Dictionary<long, long> Tall { get; set; }
            public Dictionary<Color, Color> FavoriteColor { get; set; }
            public Dictionary<int, EmployeeCommonType> Childs { get; set; }
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
