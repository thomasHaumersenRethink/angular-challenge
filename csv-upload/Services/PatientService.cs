using csv_upload.Models;

using CsvHelper;

using System.Globalization;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace csv_upload.Services
{
    public class PatientService : IPatientService
    {
        private DBContext _dbContext;

        public PatientService(DBContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public IEnumerable<Patient> GetAll(int? skip, int? take, string? filter, string? orderBy, bool? ascending)
        {
            IQueryable<Patient> patients = _dbContext.Patients;
            
            if(filter != null)
            {
                patients = patients.Where(x => x.FirstName.Contains(filter) || x.LastName.Contains(filter));
            }

            IEnumerable<Patient> patientsEnum = patients;
            if(orderBy != null)
            {
                patientsEnum = Sort(patients, orderBy, ascending);
            }

            if(skip != null)
            {
                patientsEnum = patientsEnum.Skip(skip.Value);
            }

            if(take != null)
            {
                patientsEnum = patientsEnum.Take(take.Value);
            }

            return patientsEnum;
        }

        private IOrderedEnumerable<Patient> Sort(IQueryable<Patient> patients, string orderBy, bool? ascending)
        {
            switch (orderBy)
            {
                case "id":
                    return Sort(patients, x => x.Id, ascending);
                case "firstName":
                    return Sort(patients, x => x.FirstName, ascending);
                case "lastName":
                    return Sort(patients, x => x.LastName, ascending);
                case "birthday":
                    return Sort(patients, x => x.Birthday, ascending);
                case "gender":
                    return Sort(patients, x => x.Gender, ascending);
                default:
                    return Sort(patients, x => x.Id, ascending);
            }
        }

        private IOrderedEnumerable<Patient> Sort<T>(IQueryable<Patient> patients, Func<Patient, T> accessor, bool? ascending)
        {
            if(ascending == true)
            {
                return patients.OrderBy(accessor);
            }
            else
            {
                return patients.OrderByDescending(accessor);
            }
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
                if (potentialMatch == default)
                {
                    // if id is provide must use it to match
                    return null;
                }
            }

            if (potentialMatch == default)
            {
                potentialMatch = this._dbContext.Patients.SingleOrDefault(x =>
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
                this._dbContext.Patients.Add(patient);
            }

            this._dbContext.SaveChanges();
            return potentialMatch ?? patient;
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
