using System.Collections.Generic;

public class SerilogSettings
{
    public List<string> Using { get; set; }
    public MinimumLevel MinimumLevel { get; set; }
    public List<WriteTo> WriteTo { get; set; }
}

public class MinimumLevel
{
    public string Default { get; set; }
    public Dictionary<string, string> Override { get; set; }
}

public class WriteTo
{
    public string Name { get; set; }
    public WriteToArgs Args { get; set; }
}

public class WriteToArgs
{
    public string path { get; set; }
    public string rollingInterval { get; set; }
    public string outputTemplate { get; set; }
    public string connectionString { get; set; }
    public string tableName { get; set; }
    public string schemaName { get; set; }
    public bool autoCreateSqlTable { get; set; }
}
