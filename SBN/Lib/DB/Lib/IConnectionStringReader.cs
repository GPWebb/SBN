namespace SBN.Lib.DB
{
    public interface IConnectionStringReader
    {
        string ConnectionString(string connectionString, string host);
        string ConnectionString();
    }
}