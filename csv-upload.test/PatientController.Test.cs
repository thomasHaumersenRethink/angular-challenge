using csv_upload.Controllers;
using csv_upload.Models;
using csv_upload.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

namespace csv_upload.test
{
    public class PatientControllerTest
    {
        [Fact]
        public void GetAllNoParams_Test()
        {
            //Arrange
            var mockPatientService = new Mock<IPatientService>();
            mockPatientService.Setup(repo => repo.GetAll(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool?>()))
                .Returns(SuperHeroes.AsEnumerable())
                .Verifiable();

            var controller = new PatientController(mockPatientService.Object);
            
            //Act
            var result = controller.GetAll(null, null, null, null, null);

            //Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            var patients = Assert.IsAssignableFrom<IEnumerable<Patient>>(okObjectResult.Value);
            var list = patients.ToList();
            Assert.Equal(4, list.Count);
            
            var superMan = patients.First();
            Assert.Equal("Clark", superMan.FirstName);
            Assert.Equal("Kent", superMan.LastName);
            Assert.Equal(new DateOnly(1984, 02, 29), superMan.Birthday);
            Assert.Equal("M", superMan.Gender);
            mockPatientService.Verify(x => x.GetAll(null, null, null, null, null), Times.Once());
        }

        [Fact]
        public void Parse_Test()
        {
            //arrange
            var mockDb = new Mock<DBContext>();
            var service = new PatientService(mockDb.Object);
            using var stream = GenerateStreamFromString(SuperherosCsv);
            
            //act
            var result = service.Parse(stream);
            var resultList = result.ToList();

            //Assert
            Assert.Equal(4, resultList.Count);
            
            var superMan = resultList[0];
            Assert.Equal("Clark",superMan.FirstName);
            Assert.Equal("Kent", superMan.LastName);
            Assert.Equal(new DateOnly(1984, 02, 29),superMan.Birthday);
            Assert.Equal("M", superMan.Gender);
        }

        [Fact]
        public void UploadFile_Test()
        {
            //arrange
            using var stream = GenerateStreamFromString(SuperherosCsv);
            var mockFile = new Mock<IFormFile>();
            mockFile
                .Setup(x => x.OpenReadStream())
                .Returns(stream)
                .Verifiable();
            var mockPatientService = new Mock<IPatientService>();
            mockPatientService.Setup(x => x.Parse(It.IsAny<Stream>()))
                .Returns(SuperHeroes.AsEnumerable())
                .Verifiable();
            mockPatientService.Setup(x => x.Upsert(It.IsAny<Patient>()))
                .Verifiable();
            var controller = new PatientController(mockPatientService.Object);
            
            //act
            var result = controller.UploadFile(mockFile.Object);

            //Assert
            var okResult = Assert.IsType<OkResult>(result);

            mockPatientService.Verify(x => x.Upsert(It.IsAny<Patient>()), Times.Exactly(4));
        }

        [Fact]
        public void GetPatientFound_Test()
        {
            //arrange
            var mockPatientService = new Mock<IPatientService>();
            mockPatientService.Setup(repo => repo.Get(It.IsAny<int>()))
                .Returns(SuperHeroes[0])
                .Verifiable();

            var controller = new PatientController(mockPatientService.Object);

            //act
            var result = controller.Get(1);

            //Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            var superMan = Assert.IsType<Patient>(okObjectResult.Value);
            Assert.Equal("Clark", superMan.FirstName);
            Assert.Equal("Kent", superMan.LastName);
            Assert.Equal(new DateOnly(1984, 02, 29), superMan.Birthday);
            Assert.Equal("M", superMan.Gender);

            mockPatientService.Verify(x => x.Get(1), Times.Once());
        }

        [Fact]
        public void GetPatientNotFound_Test()
        {
            //arrange
            var mockPatientService = new Mock<IPatientService>();
            mockPatientService.Setup(repo => repo.Get(It.IsAny<int>()))
                .Returns((Patient?)null)
                .Verifiable();

            var controller = new PatientController(mockPatientService.Object);

            //act
            var result = controller.Get(1);

            //Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            mockPatientService.Verify(x => x.Get(1), Times.Once());
        }

        [Fact]
        public void PatchPatientFound_Test()
        {
            //arrange
            var mockPatientService = new Mock<IPatientService>();
            mockPatientService.Setup(repo => repo.Upsert(It.IsAny<Patient>()))
                .Returns(SuperHeroes[0])
                .Verifiable();

            var controller = new PatientController(mockPatientService.Object);
            var superWoman = SuperHeroes[1];

            //act
            var result = controller.Patch(superWoman);

            //Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            var superMan = Assert.IsType<Patient>(okObjectResult.Value);
            Assert.Equal("Clark", superMan.FirstName);
            Assert.Equal("Kent", superMan.LastName);
            Assert.Equal(new DateOnly(1984, 02, 29), superMan.Birthday);
            Assert.Equal("M", superMan.Gender);

            mockPatientService.Verify(x => x.Upsert(It.IsAny<Patient>()), Times.Once());
        }

        [Fact]
        public void PatchPatientNoId_Test()
        {
            //arrange
            var mockPatientService = new Mock<IPatientService>();
            mockPatientService.Setup(repo => repo.Upsert(It.IsAny<Patient>()))
                .Returns(SuperHeroes[0])
                .Verifiable();

            var controller = new PatientController(mockPatientService.Object);
            var superWoman = new Patient();
            //act
            var result = controller.Patch(superWoman);

            //Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            mockPatientService.Verify(x => x.Upsert(It.IsAny<Patient>()), Times.Never());
        }

        [Fact]
        public void PatchPatientIdNotFound_Test()
        {
            //arrange
            var mockPatientService = new Mock<IPatientService>();
            mockPatientService.Setup(repo => repo.Upsert(It.IsAny<Patient>()))
                .Returns((Patient?)null)
                .Verifiable();

            var controller = new PatientController(mockPatientService.Object);
            var superWoman = SuperHeroes[1];
            //act
            var result = controller.Patch(superWoman);

            //Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            mockPatientService.Verify(x => x.Upsert(It.IsAny<Patient>()), Times.Once());
        }

        [Fact]
        public void DeleteFound_Test()
        {
            //arrange
            var mockPatientService = new Mock<IPatientService>();
            mockPatientService.Setup(repo => repo.Delete(It.IsAny<int>()))
                .Returns(true)
                .Verifiable();

            var controller = new PatientController(mockPatientService.Object);

            //act
            var result = controller.Delete(1);

            //Assert
            var okResult = Assert.IsType<OkResult>(result);

            mockPatientService.Verify(x => x.Delete(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public void DeleteNotFound_Test()
        {
            //arrange
            var mockPatientService = new Mock<IPatientService>();
            mockPatientService.Setup(repo => repo.Delete(It.IsAny<int>()))
                .Returns(false)
                .Verifiable();

            var controller = new PatientController(mockPatientService.Object);
            var superWoman = SuperHeroes[1];

            //act
            var result = controller.Delete(1);

            //Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);

            mockPatientService.Verify(x => x.Delete(It.IsAny<int>()), Times.Once());
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private Patient[] relatives = {
            new ()
            {
                Id = 1,
                FirstName = "Andrew",
                LastName = "Haumersen",
                Birthday = new DateOnly(1990, 4, 24),
                Gender = "M",
            },
            new ()
            {
                Id = 2,
                FirstName = "John",
                LastName = "Haumersen",
                Birthday = new DateOnly(1992, 2, 4),
                Gender = "M",
            },
            new ()
            {
                Id = 3,
                FirstName = "Thomas",
                LastName = "Haumersen",
                Birthday = new DateOnly(1995, 8, 17),
                Gender = "M",
            },
            new ()
            {
                Id = 4,
                FirstName = "Thomas",
                LastName = "Edison",
                Birthday = new DateOnly(1847, 2, 11),
                Gender = "M",
            },
        };

        internal static string SuperherosCsv = @"First Name,Last Name,Birthday,Gender
Clark,Kent,1984-02-29,M
Diana,Prince,1976-03-22,F
Tony,Stark,1970-05-29,M
Carol,Denvers,1968-04-24,F";

        internal static Patient[] SuperHeroes = {
            new ()
            {
                Id = 1,
                FirstName = "Clark",
                LastName = "Kent",
                Birthday = new DateOnly(1984, 2, 29),
                Gender = "M",
            },
            new ()
            {
                Id = 2,
                FirstName = "Diana",
                LastName = "Prince",
                Birthday = new DateOnly(1976, 3, 22),
                Gender = "F",
            },
            new ()
            {
                Id = 3,
                FirstName = "Tony",
                LastName = "Stark",
                Birthday = new DateOnly(1970, 5, 29),
                Gender = "M",
            },
            new ()
            {
                Id = 4,
                FirstName = "Carol",
                LastName = "Denvers",
                Birthday = new DateOnly(1968, 4, 24),
                Gender = "F",
            },
        };

        internal static Patient[] SuperHeroesNoIds = {
            new ()
            {
                FirstName = "Clark",
                LastName = "Kent",
                Birthday = new DateOnly(1984, 2, 29),
                Gender = "M",
            },
            new ()
            {
                FirstName = "Diana",
                LastName = "Prince",
                Birthday = new DateOnly(1976, 3, 22),
                Gender = "F",
            },
            new ()
            {
                FirstName = "Tony",
                LastName = "Stark",
                Birthday = new DateOnly(1970, 5, 29),
                Gender = "M",
            },
            new ()
            {
                FirstName = "Carol",
                LastName = "Denvers",
                Birthday = new DateOnly(1968, 4, 24),
                Gender = "F",
            },
        };
    }
}