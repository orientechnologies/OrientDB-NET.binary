﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Issues
{
    public class GitHub_issue55
    {
        [Fact]
        [Trait("type", "issue")]
        public void GIVEN___EmployeeSortedListType_class_with_common_data_type___WHEN___write_to_orientdb___THEN___should_be_able_to_read()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            using (var db = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                var employeeClassName = "EmployeeSortedListType";

                var clusterid = db.Create.Class(employeeClassName).Run();

                var employee = new EmployeeSortedListType();
                employee.Id = Guid.NewGuid();
                employee.Name = new SortedList<string, string>();
                employee.Name.Add("Andrew", "Andrew");
                employee.Name.Add("Jack", "Jack");
                employee.Ids = new SortedList<Guid, Guid>();
                employee.Ids.Add(Guid.NewGuid(), Guid.NewGuid());
                employee.Ids.Add(Guid.NewGuid(), Guid.NewGuid());
                employee.Age = new SortedList<int, int>();
                employee.Age.Add(1, 2);
                employee.Age.Add(2, 4);
                employee.Height = new SortedList<short, short>();
                employee.Height.Add(1, 2);
                employee.Height.Add(2, 4);
                employee.Tall = new SortedList<long, long>();
                employee.Tall.Add(1234567890123456789, 987654321098765432);
                employee.Tall.Add(987654321098765432, 1234567890123456789);
                employee.Childs = new SortedList<int, EmployeeCommonType>();
                employee.Childs.Add(1, new EmployeeCommonType() { Id = Guid.NewGuid() });
                employee.Childs.Add(2, new EmployeeCommonType() { Id = Guid.NewGuid() });
                employee.FavoriteColor = new SortedList<Color, Color>();
                employee.FavoriteColor.Add(Color.Red, Color.Red);
                employee.FavoriteColor.Add(Color.Blue, Color.Blue);


                var document = db.Insert(employee).Into(employeeClassName).Run();

                var result = db.Query<EmployeeSortedListType>("SELECT * FROM " + employeeClassName + " WHERE Id = '" + employee.Id + "'").SingleOrDefault();

                Assert.Equal(employee.Id, result.Id);
                Assert.True(result.Name.ContainsKey("Jack"));
                Assert.True(result.Name.ContainsKey("Andrew"));
                Assert.Equal(employee.Name["Jack"], result.Name["Jack"]);
                Assert.Equal(employee.Name["Andrew"], result.Name["Andrew"]);

                Assert.True(result.Ids.ContainsKey(employee.Ids.Keys.ToArray()[0]));
                Assert.True(result.Ids.ContainsKey(employee.Ids.Keys.ToArray()[1]));
                Assert.Equal(employee.Ids[employee.Ids.Keys.ToArray()[0]], result.Ids[employee.Ids.Keys.ToArray()[0]]);
                Assert.Equal(employee.Ids[employee.Ids.Keys.ToArray()[1]], result.Ids[employee.Ids.Keys.ToArray()[1]]);

                Assert.True(result.Age.ContainsKey(1));
                Assert.True(result.Age.ContainsKey(2));
                Assert.Equal(employee.Age[1], result.Age[1]);
                Assert.Equal(employee.Age[2], result.Age[2]);

                Assert.True(result.Height.ContainsKey(employee.Height.Keys.ToArray()[0]));
                Assert.True(result.Height.ContainsKey(employee.Height.Keys.ToArray()[1]));
                Assert.Equal(employee.Height[employee.Height.Keys.ToArray()[0]], result.Height[employee.Height.Keys.ToArray()[0]]);
                Assert.Equal(employee.Height[employee.Height.Keys.ToArray()[1]], result.Height[employee.Height.Keys.ToArray()[1]]);

                Assert.True(result.Tall.ContainsKey(employee.Tall.Keys.ToArray()[0]));
                Assert.True(result.Tall.ContainsKey(employee.Tall.Keys.ToArray()[1]));
                Assert.Equal(employee.Tall[employee.Tall.Keys.ToArray()[0]], result.Tall[employee.Tall.Keys.ToArray()[0]]);
                Assert.Equal(employee.Tall[employee.Tall.Keys.ToArray()[1]], result.Tall[employee.Tall.Keys.ToArray()[1]]);

                Assert.Equal(employee.Childs[1].Id, result.Childs[1].Id);
                Assert.Equal(employee.Childs[2].Id, result.Childs[2].Id);

                Assert.True(result.FavoriteColor.ContainsKey(Color.Red));
                Assert.True(result.FavoriteColor.ContainsKey(Color.Blue));
                Assert.Equal(employee.FavoriteColor[Color.Red], result.FavoriteColor[Color.Red]);
                Assert.Equal(employee.FavoriteColor[Color.Blue], result.FavoriteColor[Color.Blue]);

            }
        }

        public class EmployeeSortedListType
        {
            public Guid Id { get; set; }
            public SortedList<string, string> Name { get; set; }
            public SortedList<Guid, Guid> Ids { get; set; }
            public SortedList<int, int> Age { get; set; }
            public SortedList<short, short> Height { get; set; }
            public SortedList<long, long> Tall { get; set; }
            public SortedList<Color, Color> FavoriteColor { get; set; }
            public SortedList<int, EmployeeCommonType> Childs { get; set; }
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
