using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Issues
{
    public class GitHub_issue53
    {
        [Fact]
        [Trait("type", "issue")]
        public void GIVEN___EmployeeArrayType_class_with_common_data_type___WHEN___write_to_orientdb___THEN___should_be_able_to_read()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            using (var db = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                var employeeClassName = "EmployeeArrayType";

                var clusterid = db.Create.Class(employeeClassName).Run();

                var employee = new EmployeeArrayType();
                employee.Id = Guid.NewGuid();
                employee.Name = new string[2];
                employee.Name[0] = "Andrew";
                employee.Name[1] = ("Jack");
                employee.Age = new int[2];
                employee.Age[0] = (1);
                employee.Age[1] = (2);
                employee.BirthDate = new DateTime[2];
                employee.BirthDate[0] = (DateTime.Now);
                employee.BirthDate[1] = (DateTime.Now.AddDays(-3));
                employee.Childs = new EmployeeCommonType[2];
                employee.Childs[0] = (new EmployeeCommonType() { Id = Guid.NewGuid() });
                employee.Childs[1] = (new EmployeeCommonType() { Id = Guid.NewGuid() });
                employee.FavoriteColor = new Color[2];
                employee.FavoriteColor[0] = (Color.Red);
                employee.FavoriteColor[1] = (Color.Blue);
                employee.Height = new short[2];
                employee.Height[0] = (323);
                employee.Height[1] = (333);
                employee.Ids = new Guid[2];
                employee.Ids[0] = (Guid.NewGuid());
                employee.Ids[1] = (Guid.NewGuid());
                employee.IsMarried = new bool[2];
                employee.IsMarried[0] = (true);
                employee.IsMarried[1] = (false);
                employee.Salary = new decimal[2];
                employee.Salary[0] = ((decimal)1234567890.123456789);
                employee.Salary[1] = ((decimal)1234567890.123456799);
                employee.Tall = new long[2];
                employee.Tall[0] = (3233);
                employee.Tall[1] = (3234);
                employee.YearlyIncome = new double[2];
                employee.YearlyIncome[0] = (3233);
                employee.YearlyIncome[1] = (1234);

                var document = db.Insert<EmployeeArrayType>(employee).Run();

                var result = db.Query<EmployeeArrayType>("SELECT * FROM " + employeeClassName + " WHERE Id = '" + employee.Id + "'").SingleOrDefault();

                Assert.Equal(employee.Id, result.Id);
                Assert.Equal(employee.Ids[0], result.Ids[0]);
                Assert.Equal(employee.Ids[1], result.Ids[1]);
                Assert.Equal(employee.Name[0], result.Name[0]);
                Assert.Equal(employee.Name[1], result.Name[1]);
                Assert.Equal(employee.Age[0], result.Age[0]);
                Assert.Equal(employee.Age[1], result.Age[1]);
                Assert.Equal(employee.Age[0], result.Age[0]);
                Assert.Equal(employee.Salary[0], result.Salary[0]);
                Assert.Equal(employee.Salary[1], result.Salary[1]);
                Assert.Equal(employee.IsMarried[0], result.IsMarried[0]);
                Assert.Equal(employee.IsMarried[1], result.IsMarried[1]);
                Assert.Equal(employee.Childs[0].Id, result.Childs[0].Id);
                Assert.Equal(employee.Childs[1].Id, result.Childs[1].Id);
                Assert.Equal(employee.BirthDate[0].ToLongDateString(), result.BirthDate[0].ToLongDateString());
                Assert.Equal(employee.BirthDate[1].ToLongDateString(), result.BirthDate[1].ToLongDateString());
                Assert.Equal(employee.YearlyIncome[0], result.YearlyIncome[0]);
                Assert.Equal(employee.YearlyIncome[1], result.YearlyIncome[1]);
                Assert.Equal(employee.FavoriteColor[0], result.FavoriteColor[0]);
                Assert.Equal(employee.FavoriteColor[1], result.FavoriteColor[1]);
                Assert.Equal(employee.Height[0], result.Height[0]);
                Assert.Equal(employee.Height[1], result.Height[1]);
                Assert.Equal(employee.Tall[0], result.Tall[0]);
                Assert.Equal(employee.Tall[1], result.Tall[1]);

            }
        }

        public class EmployeeArrayType
        {
            public Guid Id { get; set; }

            public Guid[] Ids { get; set; }
            public string[] Name { get; set; }

            public int[] Age { get; set; }

            public decimal[] Salary { get; set; }

            public bool[] IsMarried { get; set; }

            public DateTime[] BirthDate { get; set; }

            public double[] YearlyIncome { get; set; }

            public short[] Height { get; set; }

            public long[] Tall { get; set; }

            public Color[] FavoriteColor { get; set; }

            public EmployeeCommonType[] Childs { get; set; }
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
