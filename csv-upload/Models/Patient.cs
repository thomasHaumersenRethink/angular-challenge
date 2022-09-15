using System.Text.Json.Serialization;
using csv_upload.JsonConverters;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace csv_upload.Models
{
    
    public class Patient
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly Birthday { get; set; }
        public string Gender { get; set; } // don't want to limit myself to single character here
    }

    public class PatientMap : ClassMap<Patient>
    {
        public PatientMap()
        {
            Map(m => m.FirstName).Name("First Name");
            Map(m => m.LastName).Name("Last Name");
            Map(m => m.Birthday).Name("Birthday");
            Map(m => m.Gender).Name("Gender");
            Map(x => x.Id).Optional();
        }
    }
}
