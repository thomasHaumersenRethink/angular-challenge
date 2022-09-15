using csv_upload.Models;

namespace csv_upload.Services
{
    public interface IPatientService
    {
        IEnumerable<Patient> GetAll(string? filter);

        Patient? Get(int id);

        bool Delete(int id);

        Patient? Upsert(Patient patient);

        IEnumerable<Patient> Parse(Stream stream);
    }
}
