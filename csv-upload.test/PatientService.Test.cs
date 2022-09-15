using csv_upload.Models;
using csv_upload.Services;

namespace csv_upload.test
{
    public class PatientServiceTest : IClassFixture<TestDatabaseFixture>
    {
        public PatientServiceTest(TestDatabaseFixture fixture)
            => Fixture = fixture;

        public TestDatabaseFixture Fixture { get; }

        [Fact]
        public void GetSuccess_Test()
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
        public void GetFail_Test()
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
            var result = service.GetAll(null, null, null, null, null);

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
            var result = service.GetAll(null, null, "e", null, null);

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
            var result = service.GetAll(null, null, "Diana", null, null);

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
            var result = service.GetAll(null, null, "Stark", null, null);

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
        public void DeleteExisting_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new PatientService(context);

            // Act
            var returnValue = service.Delete(1);

            context.ChangeTracker.Clear();

            // Assert
            Assert.True(returnValue);
            var patient = context.Patients.SingleOrDefault(b => b.Id == 1);
            Assert.Null(patient);
        }

        [Fact]
        public void DeleteNotExists_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new PatientService(context);

            // Act
            var returnValue = service.Delete(100);

            context.ChangeTracker.Clear();

            // Assert
            Assert.False(returnValue);
        }
        [Fact]
        public void Insert_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new PatientService(context);
            var tom = new Patient()
            {
                FirstName = "Thomas",
                LastName = "Haumersen",
                Birthday = new DateOnly(1995, 8, 17),
                Gender = "M",
            };

            // Act
            service.Upsert(tom);

            context.ChangeTracker.Clear();

            // Assert
            var result = context.Patients.Single(b => b.Id == 5);
            Assert.Equal("Thomas", result.FirstName);
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
                Gender = "M",
            };

            // Act
            service.Upsert(tom);

            context.ChangeTracker.Clear();

            // Assert
            var result = context.Patients.Single(b => b.Id == 1);
            Assert.Equal("Thomas", result.FirstName);
        }

        [Fact]
        public void UpdateNotExisting_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new PatientService(context);
            var tom = new Patient()
            {
                Id = 100,
                FirstName = "Thomas",
                LastName = "Haumersen",
                Birthday = new DateOnly(1995, 8, 17),
                Gender = "M",
            };

            // Act
            var result =service.Upsert(tom);

            context.ChangeTracker.Clear();

            // Assert
            Assert.Null(result);
            var doubleCheck = context.Patients.SingleOrDefault(x => x.FirstName == "Thomas");
            Assert.Null(doubleCheck);
        }

        [Fact]
        public void ServerSideBasic_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            var service = new PatientService(context);

            // Act
            var result = service.GetAll(0, 5, null, null, null);

            // Assert
            var list = result.ToList();
            Assert.Equal(4, list.Count);
        }

        [Fact]
        public void ServerSideSkip1_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            var service = new PatientService(context);

            // Act
            var result = service.GetAll(1, null, null, null, null);

            // Assert
            var list = result.ToList();
            Assert.Equal(3, list.Count);
        }


        [Fact]
        public void ServerSideSkip1Take1_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            var service = new PatientService(context);

            // Act
            var result = service.GetAll(1, 1, null, null, null);

            // Assert
            var list = result.ToList();
            Assert.Single(list);

            var wonderWoman = list.Single();
            Assert.Equal("Diana", wonderWoman.FirstName);
            Assert.Equal("Prince", wonderWoman.LastName);
            Assert.Equal(new DateOnly(1976, 03, 22), wonderWoman.Birthday);
            Assert.Equal("F", wonderWoman.Gender);
        }

        [Fact]
        public void ServerSideTooManySkips_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            var service = new PatientService(context);

            // Act
            var result = service.GetAll(5, 5, null, null, null);

            // Assert
            var list = result.ToList();
            Assert.Empty(list);
        }

        [Fact]
        public void ServerSideOrderByFirstNameAsecnding_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            var service = new PatientService(context);

            // Act
            var result = service.GetAll(0, 5, null, "firstName", true);

            // Assert
            var list = result.ToList();
            Assert.Equal(4, list.Count);

            var msMarvel = list.First();
            Assert.Equal("Carol", msMarvel.FirstName);
            Assert.Equal("Denvers", msMarvel.LastName);
            Assert.Equal(new DateOnly(1968, 04, 24), msMarvel.Birthday);
            Assert.Equal("F", msMarvel.Gender);
        }

        [Fact]
        public void ServerSideOrderByFirstNameDescending_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            var service = new PatientService(context);

            // Act
            var result = service.GetAll(0, 5, null, "firstName", false);

            // Assert
            var list = result.ToList();
            Assert.Equal(4, list.Count);
            
            var ironMan = list.First();
            Assert.Equal("Tony", ironMan.FirstName);
            Assert.Equal("Stark", ironMan.LastName);
            Assert.Equal(new DateOnly(1970, 05, 29), ironMan.Birthday);
            Assert.Equal("M", ironMan.Gender);
        }

        [Fact]
        public void ServerSideOrderByLastNameAscending_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            var service = new PatientService(context);

            // Act
            var result = service.GetAll(0, 5, null, "lastName", true);

            // Assert
            var list = result.ToList();
            Assert.Equal(4, list.Count);

            var msMarvel = list.First();
            Assert.Equal("Carol", msMarvel.FirstName);
            Assert.Equal("Denvers", msMarvel.LastName);
            Assert.Equal(new DateOnly(1968, 04, 24), msMarvel.Birthday);
            Assert.Equal("F", msMarvel.Gender);
        }

        [Fact]
        public void ServerSideOrderByLastNameDescending_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            var service = new PatientService(context);

            // Act
            var result = service.GetAll(0, 5, null, "lastName", false);

            // Assert
            var list = result.ToList();
            Assert.Equal(4, list.Count);

            var ironMan = list.First();
            Assert.Equal("Tony", ironMan.FirstName);
            Assert.Equal("Stark", ironMan.LastName);
            Assert.Equal(new DateOnly(1970, 05, 29), ironMan.Birthday);
            Assert.Equal("M", ironMan.Gender);
        }


        [Fact]
        public void ServerSideOrderByBirthdayAscending_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            var service = new PatientService(context);

            // Act
            var result = service.GetAll(0, 5, null, "birthday", true);

            // Assert
            var list = result.ToList();
            Assert.Equal(4, list.Count);

            var msMarvel = list.First();
            Assert.Equal("Carol", msMarvel.FirstName);
            Assert.Equal("Denvers", msMarvel.LastName);
            Assert.Equal(new DateOnly(1968, 04, 24), msMarvel.Birthday);
            Assert.Equal("F", msMarvel.Gender);
        }

        [Fact]
        public void ServerSideOrderByBirthdayDescending_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            var service = new PatientService(context);

            // Act
            var result = service.GetAll(0, 5, null, "birthday", false);

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
        public void ServerSideOrderByGenderAscending_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            var service = new PatientService(context);

            // Act
            var result = service.GetAll(0, 5, null, "gender", true);

            // Assert
            var list = result.ToList();
            Assert.Equal(4, list.Count);

            Assert.Equal("F", list[0].Gender);
            Assert.Equal("F", list[1].Gender);
            Assert.Equal("M", list[2].Gender);
            Assert.Equal("M", list[3].Gender);
        }

        [Fact]
        public void ServerSideOrderByGenderDescending_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            var service = new PatientService(context);

            // Act
            var result = service.GetAll(0, 5, null, "gender", false);

            // Assert
            var list = result.ToList();
            Assert.Equal(4, list.Count);

            Assert.Equal("M", list[0].Gender);
            Assert.Equal("M", list[1].Gender);
            Assert.Equal("F", list[2].Gender);
            Assert.Equal("F", list[3].Gender);
        }

        [Fact]
        public void ServerSideEverything_Test()
        {
            // Arrange
            using var context = Fixture.CreateContext();
            var service = new PatientService(context);

            // Act
            var result = service.GetAll(1, 2, "e", "firstName", true);

            // Assert
            var list = result.ToList();
            Assert.Equal(2, list.Count);

            var superMan = list[0];
            Assert.Equal("Clark", superMan.FirstName);
            Assert.Equal("Kent", superMan.LastName);
            Assert.Equal(new DateOnly(1984, 02, 29), superMan.Birthday);
            Assert.Equal("M", superMan.Gender);

            var wonderWoman = list[1];
            Assert.Equal("Diana", wonderWoman.FirstName);
            Assert.Equal("Prince", wonderWoman.LastName);
            Assert.Equal(new DateOnly(1976, 03, 22), wonderWoman.Birthday);
            Assert.Equal("F", wonderWoman.Gender);
        }
    }
}
