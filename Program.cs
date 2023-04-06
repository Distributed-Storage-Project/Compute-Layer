using Kusto.Data;
using Kusto.Data.Common;
using Kusto.Data.Net.Client;
using Kusto.Language;
using Kusto.Language.Syntax;

class Program
{
    static void Main(string[] args)
    {
        InMemoryLogDatabase logDatabase = new InMemoryLogDatabase();

        //only sample data
        logDatabase.Add(new LogEntry { Timestamp = new DateTime(2015, 8, 22, 5, 30, 0), Level = "e", Service = "Inferences.UnusualEvents_Main", Message = "Error 1" });
        logDatabase.Add(new LogEntry { Timestamp = new DateTime(2015, 8, 22, 5, 35, 0), Level = "e", Service = "Inferences.UnusualEvents_Main", Message = "Error 2" });
        logDatabase.Add(new LogEntry { Timestamp = new DateTime(2015, 8, 22, 5, 40, 0), Level = "w", Service = "Inferences.UnusualEvents_Main", Message = "Warning 1" });
        logDatabase.Add(new LogEntry { Timestamp = new DateTime(2015, 8, 22, 5, 45, 0), Level = "e", Service = "Inferences.UnusualEvents_Main", Message = "Error 3" });
        logDatabase.Add(new LogEntry { Timestamp = new DateTime(2015, 8, 22, 5, 50, 0), Level = "i", Service = "Inferences.UnusualEvents_Main", Message = "Information 1" });
        logDatabase.Add(new LogEntry { Timestamp = new DateTime(2015, 8, 22, 5, 55, 0), Level = "e", Service = "Inferences.UnusualEvents_Main", Message = "Information 1" });

        DateTime? timestamp = null, timestamp2 = null;
        string level = null, service = null;
        int? limit = null;

        Dictionary<string, List<string>> filterMap = new Dictionary<string, List<string>>();

        var query = "Logs | where Timestamp > datetime(2015-08-22 05:00) and Timestamp < datetime(2015-08-22 06:00) | where Level == \"e\" | where Service == \"Inferences.UnusualEvents_Main\" | project Level, Timestamp, Message| limit 10";
        var code = KustoCode.Parse(query);

        var FilterOpeators = code.Syntax.GetDescendants<FilterOperator>();

        foreach (var FilterOpeator in FilterOpeators)
        {
            var parsed = KustoCode.Parse(FilterOpeator.ToString());

            var filters = parsed.Syntax.GetDescendants<NameReference>();
            var vals = parsed.Syntax.GetDescendants<LiteralExpression>();

            string remove = "datetime(";

            int i = 0;
            foreach (var descendant in filters)
            {
                string s = descendant.ToString().Substring(1);
                if (s == "Timestamp" && filterMap.ContainsKey("Timestamp"))
                {
                    s = "Timestamp2";
                }
                if (!filterMap.ContainsKey(s))
                {
                    filterMap.Add(s, new List<string>());
                }
                filterMap[s].Add(vals.ElementAt(i).ToString().Substring(1).Replace("\"", "").Replace(remove, "").Replace(")", ""));

                Console.WriteLine(s + ": " + vals.ElementAt(i).ToString().Substring(1));
                i++;
            }
        }
        foreach (var kvp in filterMap)
        {
            Console.WriteLine(kvp.Key + ":");
            foreach (var value in kvp.Value)
            {
                Console.WriteLine(value);
            }
        }

        if (filterMap.ContainsKey("Timestamp"))
        {
            timestamp = DateTime.Parse(filterMap["Timestamp"][0]);
        }

        if (filterMap.ContainsKey("Timestamp2"))
        {
            timestamp2 = DateTime.Parse(filterMap["Timestamp2"][0]);
        }

        if (filterMap.ContainsKey("Service"))
        {
            service = filterMap["Service"][0];
        }

        if (filterMap.ContainsKey("Level"))
        {
            level = filterMap["Level"][0];
        }

        if (filterMap.ContainsKey("limit"))
        {
            limit = int.Parse(filterMap["limit"][0]);
        }
        Console.WriteLine(timestamp + "" + timestamp2 + "" + service + level);

        var results = logDatabase.QueryLogs(timestamp, timestamp2, level, service, limit);

        foreach (var entry in results)
        {
            Console.WriteLine($"{entry.Level}, {entry.Timestamp}, {entry.Message}");
        }

        Console.Read();

    }


}





public class LogEntry
{
    public DateTime Timestamp { get; set; }
    public string Level { get; set; }
    public string Service { get; set; }
    public string Message { get; set; }
}

public class InMemoryLogDatabase
{
    private List<LogEntry> _logs;

    public InMemoryLogDatabase()
    {
        _logs = new List<LogEntry>();
    }

    public void Add(LogEntry entry)
    {
        _logs.Add(entry);
    }

    public IEnumerable<LogEntry> GetLogs()
    {
        return _logs;
    }

    public IEnumerable<LogEntry> QueryLogs(DateTime? startTime = null, DateTime? endTime = null, string level = null, string service = null, int? limit = null)
    {
        var logs = GetLogs().AsQueryable();

        if (startTime.HasValue)
        {
            logs = logs.Where(log => log.Timestamp >= startTime.Value);
        }

        if (endTime.HasValue)
        {
            logs = logs.Where(log => log.Timestamp < endTime.Value);
        }

        if (!string.IsNullOrEmpty(level))
        {
            logs = logs.Where(log => log.Level == level);
        }

        if (!string.IsNullOrEmpty(service))
        {
            logs = logs.Where(log => log.Service == service);
        }

        if (limit.HasValue)
        {
            logs = logs.Take(limit.Value);
        }

        return logs.ToList();
    }
} 
