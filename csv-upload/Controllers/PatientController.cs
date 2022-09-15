using System.Globalization;
using System.Text;
using csv_upload.Models;
using csv_upload.Services;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace csv_upload.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientController : ControllerBase
    {
        private IPatientService _patientService;
        public PatientController(IPatientService patientService)
        {
            this._patientService = patientService;
        }

        [HttpGet]
        [Route("/patients/")]
        public ActionResult<IEnumerable<Patient>> GetAll(string? filter)
        {
            return Ok(_patientService.GetAll(filter));
        }

        [HttpPost]
        [Route("/patients")]
        public ActionResult UploadFile(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                foreach (var patient in this._patientService.Parse(stream))
                {
                    this._patientService.Upsert(patient);
                }
            }

            return Ok();
        }

        [HttpGet]
        [Route("{id}")] 
        public ActionResult<Patient> Get(int id)
        {
            var patient =  this._patientService.Get(id);
            if (patient == default)
            {
                return BadRequest("Could not find patient");
            }

            return Ok(patient);
        }

        [HttpPatch]
        public ActionResult<Patient> Patch(Patient patient)
        {
            if (patient.Id == default)
            {
                return BadRequest($"{nameof(Patient.Id)} must be defined");
            }

            var result = this._patientService.Upsert(patient);
            if (result == default)
            {
                return BadRequest("Could not find patient");
            }

            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public ActionResult Delete(int id)
        {
            var successful = this._patientService.Delete(id);

            if (!successful)
            {
                return BadRequest("Could not find patient");
            }

            return Ok();
        }
    }
}