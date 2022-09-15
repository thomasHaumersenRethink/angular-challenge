using csv_upload.Models;

namespace csv_upload.Services
{
    public interface IPatientService
    {

        IEnumerable<Patient> GetAll(int? skip, int? take, string? filter, string? orderBy, bool? ascending);

        Patient? Get(int id);

        bool Delete(int id);

        Patient? Upsert(Patient patient);

        IEnumerable<Patient> Parse(Stream stream);
    }
}
