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
        private readonly IPatientService _patientService;
        public PatientController(IPatientService patientService)
        {
            this._patientService = patientService;
        }
        /// <summary>
        /// Gets all of the patients uploaded
        /// </summary>
        /// <param name="filter">Limits the patients returned based on first or last name</param>
        /// <returns>All the relevant patients</returns>
        [HttpGet]
        [Route("/patients/")]
        public ActionResult<IEnumerable<Patient>> GetAll(string? filter)
        {
            return Ok(_patientService.GetAll(filter));
        }

        /// <summary>
        /// Add patients in bulk to the application
        /// </summary>
        /// <remarks>Data is to be uploaded as a text file csv with the following headers in the first line: First Name, Last Name, Birthday, Gender</remarks>
        /// <param name="file"></param>
        /// <returns>Ok on successful addition to the application</returns>
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

        /// <summary>
        /// Return the specified patient
        /// </summary>
        /// <param name="id">Id of the patient to be returned</param>
        /// <returns>Specified patient if successfully found, BadRequest otherwise</returns>
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

        /// <summary>
        /// Updates all the values in the patient with the matching id
        /// </summary>
        /// <param name="patient">The fully detailed patient information</param>
        /// <returns>the same patient if they were successfully updated, BadRequest otherwise</returns>
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

        //
        /// <summary>
        /// Remove the patient with the corresponding id from the application
        /// </summary>
        /// <param name="id">id of the patient to remove</param>
        /// <returns>ok if patient was successfully removed, BadRequest otherwise</returns>
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