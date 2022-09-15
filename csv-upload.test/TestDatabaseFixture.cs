using Microsoft.EntityFrameworkCore;

namespace csv_upload.test
{
    public class TestDatabaseFixture
    {
        private const string ConnectionString = @"Server=localhost;Database=csvUploadApp;Trusted_Connection=True";

        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        public TestDatabaseFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();
                        context.Patients.AddRange(PatientControllerTest.SuperHeroesNoIds);
                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public DBContext CreateContext()
            => new DBContext(
                new DbContextOptionsBuilder<DBContext>()
                    .UseSqlServer(ConnectionString)
                    .Options);
    }

}
