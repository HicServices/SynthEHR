using FAnsi;

namespace BadMedicine.Configuration
{
    /// <summary>
    /// Identify the target database and configuration for generated data
    /// </summary>
    public class TargetDatabase
    {
        /// <summary>
        /// Which RDBMS the database is (MySQL, Microsoft SQL Server, etc)
        /// </summary>
        public DatabaseType DatabaseType { get; set; }
        /// <summary>
        /// The ConnectionString containing the server name, credentials and other parameters for the connection
        /// </summary>
        public string ConnectionString { get; set; }
        
        /// <summary>
        /// The name of database
        /// </summary>
        public string DatabaseName { get; set; }
        
        /// <summary>
        /// Set to true to drop and recreate tables described in the Template
        /// </summary>
        public bool DropTables { get; set; }

    }
}
