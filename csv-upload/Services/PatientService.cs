using System.Globalization;
using System.Runtime.CompilerServices;
using csv_upload.Models;
using CsvHelper;

namespace csv_upload.Services
{
    public class PatientService : IPatientService
    {
        private DBContext _dbContext;

        public PatientService(DBContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public IEnumerable<Patient> GetAll(string? filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return _dbContext.Patients.AsEnumerable();
            }
            
            return _dbContext.Patients
                .Where(x => x.FirstName.Contains(filter) || x.LastName.Contains(filter))
                .AsEnumerable();
        }

        public Patient? Get(int id)
        {
            return _dbContext.Patients.SingleOrDefault(x => x.Id == id);
        }

        public bool Delete(int id)
        {
            var patient = this.Get(id);

            if (patient == default)
            {
                return false;
            }

            this._dbContext.Patients.Remove(patient);
            this._dbContext.SaveChanges();

            return true;
        }


        public Patient? Upsert(Patient patient)
        {
            Patient? potentialMatch = null;
            if (patient.Id != default)
            {
                potentialMatch = this.Get(patient.Id);
                return null;
            }

            if (potentialMatch == default)
            {
                potentialMatch = _dbContext.Patients.SingleOrDefault(x =>
                    x.FirstName == patient.FirstName &&
                    x.LastName == patient.LastName &&
                    x.Birthday == patient.Birthday &&
                    x.Gender == patient.Gender);
            }

            if (potentialMatch != default)
            {
                patient.Id = potentialMatch.Id;
            }

            // should insert or update
            _dbContext.Patients.Update(patient);
            _dbContext.SaveChanges();

            return patient;
        }

        public IEnumerable<Patient> Parse(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<PatientMap>();

                foreach (var patient in csv.GetRecords<Patient>())
                {
                    yield return patient;
                }
            }
        }
    }
}
