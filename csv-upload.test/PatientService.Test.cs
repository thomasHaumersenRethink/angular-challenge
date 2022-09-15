using csv_upload.Models;
using csv_upload.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;

namespace csv_upload.test
{
    public class PatientServiceTest : IClassFixture<TestDatabaseFixture>
    {
        public PatientServiceTest(TestDatabaseFixture fixture)
            => Fixture = fixture;

        public TestDatabaseFixture Fixture { get; }

        [Fact]
        public void GetBlogSuccess_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            var service = new PatientService(context);

            // Act
            var superMan = service.Get(1);

            // Assert
            Assert.Equal(1, superMan.Id);
            Assert.Equal("Clark", superMan.FirstName);
            Assert.Equal("Kent", superMan.LastName);
            Assert.Equal(new DateOnly(1984, 02, 29), superMan.Birthday);
            Assert.Equal("M", superMan.Gender);
        }

        [Fact]
        public void GetBlogFail_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            var service = new PatientService(context);

            // Act
            var result = service.Get(100);

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public void GetAll_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            var service = new PatientService(context);

            // Act
            var result = service.GetAll(null);

            // Assert
            var list = result.ToList();
            Assert.Equal(4, list.Count);

            var superMan = list.First();
            Assert.Equal("Clark", superMan.FirstName);
            Assert.Equal("Kent", superMan.LastName);
            Assert.Equal(new DateOnly(1984, 02, 29), superMan.Birthday);
            Assert.Equal("M", superMan.Gender);
        }

        [Fact]
        public void UpdateExisting_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new PatientService(context);
            var tom = new Patient()
            {
                Id = 1,
                FirstName = "Thomas",
                LastName = "Haumersen",
                Birthday = new DateOnly(1995, 8, 17),
                Gender = "Ms",
            };

            // Act
            service.Upsert(tom);

            context.ChangeTracker.Clear();

            // Assert
            var result = context.Patients.Single(b => b.Id == 1);
            Assert.Equal("Thomas", result.FirstName);

        }
    }
}
