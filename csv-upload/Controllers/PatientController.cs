using csv_upload.Models;
using csv_upload.Services;

using Microsoft.AspNetCore.Mvc;

namespace csv_upload.Controllers
{
    [ApiController]
    [Route("patient")]
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
        /// <param name="skip">How many patients to skip, unspecified is none</param>
        /// <param name="take">How many rows to return, unspecified is unlimted</param>
        /// <param name="filter">Limits the patients returned based on first or last name</param>
        /// <param name="orderBy">which field to sort the patients by. Defaults to ID</param>
        /// <param name="ascending">whether or not the sort ascending. Defaults to true</param>
        /// <returns>the queried patients</returns>
        [HttpGet]
        [Route("/patients")]

        public ActionResult<IEnumerable<Patient>> GetAll(int? skip, int? take, string? filter, string? orderBy, bool? ascending)
        {
            return Ok(this._patientService.GetAll(skip, take, filter, orderBy, ascending));
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