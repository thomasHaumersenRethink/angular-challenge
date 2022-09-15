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
        public void GetAllEs_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            var service = new PatientService(context);

            // Act
            var result = service.GetAll("e");

            // Assert
            var list = result.ToList();
            Assert.Equal(3, list.Count);

            var msMarvel = list.Last();
            Assert.Equal("Carol", msMarvel.FirstName);
            Assert.Equal("Denvers", msMarvel.LastName);
            Assert.Equal(new DateOnly(1968, 04, 24), msMarvel.Birthday);
            Assert.Equal("F", msMarvel.Gender);
        }

        [Fact]
        public void GetAllDiana_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            var service = new PatientService(context);

            // Act
            var result = service.GetAll("Diana");

            // Assert
            var list = result.ToList();
            Assert.Single(list);

            var wonderWoman = list.Last();
            Assert.Equal("Diana", wonderWoman.FirstName);
            Assert.Equal("Prince", wonderWoman.LastName);
            Assert.Equal(new DateOnly(1976, 03, 22), wonderWoman.Birthday);
            Assert.Equal("F", wonderWoman.Gender);
        }

        [Fact]
        public void GetAllStark_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            var service = new PatientService(context);

            // Act
            var result = service.GetAll("Stark");

            // Assert
            var list = result.ToList();
            Assert.Single(list);

            var ironMan = list.Last();
            Assert.Equal("Tony", ironMan.FirstName);
            Assert.Equal("Stark", ironMan.LastName);
            Assert.Equal(new DateOnly(1970, 05, 29), ironMan.Birthday);
            Assert.Equal("M", ironMan.Gender);
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
