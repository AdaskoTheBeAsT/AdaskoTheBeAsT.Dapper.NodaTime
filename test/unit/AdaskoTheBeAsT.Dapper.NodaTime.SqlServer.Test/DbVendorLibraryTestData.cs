using Xunit;

namespace AdaskoTheBeAsT.Dapper.NodaTime.SqlServer.Test
{
    public sealed class DbVendorLibraryTestData
        : TheoryData<DbVendorLibrary>
    {
        public DbVendorLibraryTestData()
        {
            Add(DbVendorLibrary.MicrosoftSqlServer);
#if NET462_OR_GREATER
            Add(DbVendorLibrary.SystemSqlServer);
#endif
        }
    }
}
