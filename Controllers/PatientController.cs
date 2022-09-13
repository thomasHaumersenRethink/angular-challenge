using System.Globalization;
using System.Text;
using csv_upload.Models;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore.Design;

namespace csv_upload.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientController : ControllerBase
    {
        private static readonly string[] firstNames = new[]
        {
            "Tom", "Chris", "Callum", "Aiden", "Bryce", "James"
        };
        private static readonly string[] lastNames = new[]
        {
            "Haumersen", "Haines", "Forbes", "Cheng", "Smith", "Russo"
        };
        private readonly ILogger<PatientController> _logger;
        private DBContext dbContext;
        public PatientController(ILogger<PatientController> logger, DBContext dbContext)
        {
            _logger = logger;
            this.dbContext = dbContext;
        }

        [HttpGet]
        [Route("/patients")]
        public IEnumerable<Patient> GetAll()
        {
            return dbContext.Patients.AsEnumerable();
        }

        [HttpPost]
        [Route("/patients")]
        public void UploadFile(IFormFile file)
        {
            // couldn't figure out how to directly stream file to a string
            //using (var stream = new MemoryStream(new byte[1024]))
            //{
            //    file.CopyTo(stream);
            //    using var reader = new StreamReader(stream, Encoding.UTF8, true);
            //    var txt = reader.ReadToEnd();
            //}

            var filePath = Path.GetTempFileName();

            using (var stream = System.IO.File.Create(filePath))
            {
                file.CopyTo(stream);
            }

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<PatientMap>();
         
                // background worker?
                foreach (var patient in csv.GetRecords<Patient>())
                {
                    UpsertPatient(patient);
                }
            }
        }

        [HttpGet]
        [Route("{id}")] 
        public Patient Get(int id)
        {
            return dbContext.Patients.Single(x => x.Id == id);
        }

        [HttpPatch]
        public Patient Patch(Patient patient)
        {
            if (patient.Id == default)
                throw new ArgumentException($"{nameof(Patient.Id)} must be defined");

            return UpsertPatient(patient);
        }

        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            var patient = dbContext.Patients.Single(x => x.Id == id);
            dbContext.Patients.Remove(patient);
            dbContext.SaveChanges();
        }

        private Patient UpsertPatient(Patient patient)
        {
            Patient? potentialMatch = null;
            if (patient.Id != default)
            {
                potentialMatch = dbContext.Patients.Single(x => x.Id == patient.Id);
            }

            if (potentialMatch == default)
            {
                potentialMatch = dbContext.Patients.SingleOrDefault(x =>
                    x.FirstName == patient.FirstName &&
                    x.LastName == patient.LastName &&
                    x.Birthday == patient.Birthday &&
                    x.Gender == patient.Gender);
            }

            if (potentialMatch != default)
            {
                //update
                potentialMatch.FirstName = patient.FirstName;
                potentialMatch.LastName = patient.LastName;
                potentialMatch.Birthday = patient.Birthday;
                potentialMatch.Gender = patient.Gender;
            }
            else
            {
                //insert
                dbContext.Patients.Add(patient);
            }
            
            dbContext.SaveChanges();
            return potentialMatch ?? patient;
        }
    }
}