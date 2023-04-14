using System;
using System.Collections;
using System.Collections.Generic;
using Kusto.Data;
using Kusto.Data.Common;
using Kusto.Data.Linq;
using Kusto.Data.Net.Client;
using Kusto.Language;
using Kusto.Language.Syntax;
using static Kusto.Data.Security.WellKnownAadResourceIds;

public static class KustoToSqlConverter
{
    public static string Convert(Query query)
    {    //a method to decided calling logs table parser or request table parser, or none of them
         // Extract the table name
        string s = query.query;
        var table = s.Substring(0, s.IndexOf("|")).Trim();

        if (table == "Logs")
        {
            return logsToSql(s);
        }
        else if (table == "Requests")
        {
            return reqToSql(s);
        }
        else
        {
            return errToSql(table);
        }
    }

    static string reqToSql(string kustoQuery)
    {
        int? limit = null;
        Dictionary<string, List<string>> filterMap = new Dictionary<string, List<string>>();

        var code = KustoCode.Parse(kustoQuery);
        // Get descendants of the root node that are of type ProjectOperator
        var FilterOperators = code.Syntax.GetDescendants<FilterOperator>();
        var TakeOperators = code.Syntax.GetDescendants<TakeOperator>();
        foreach (var TakeOperator in TakeOperators)
        {
            var parsed = KustoCode.Parse(TakeOperator.ToString());
            var vals = parsed.Syntax.GetDescendants<LiteralExpression>();

            if (!filterMap.ContainsKey("limit"))
            {
                filterMap.Add("limit", new List<string>());
            }
            filterMap["limit"].Add(vals.First().ToString());
        }
        if (filterMap.ContainsKey("limit"))
        {
            limit = int.Parse(filterMap["limit"][0]);
        }

        string sql = $"SELECT * FROM Requests WHERE ";
        // Construct the SQL query using the extracted components
        foreach (var descendant in FilterOperators)
        {
            sql += descendant.ToString().Substring(1).Replace("where", "").Substring(1) + " AND ";
        }
        if (sql.EndsWith(" AND "))
        {
            sql = sql.Substring(0, sql.Length - 5);
        }

        if (limit != null)
        {
            sql += $" LIMIT {limit}";
        }

        return sql;
    }

    static String errToSql(String table)
    {
        String s = "Table: \"" + table + "\" not accessable or table doesn't exist.";
        return s;
    }

    static string logsToSql(string kustoQuery)
    {
        Dictionary<string, List<string>> filterMap = new Dictionary<string, List<string>>();
        DateTime? timestamp = null;
        string level = null, service = null;
        int? limit = null;

        var code = KustoCode.Parse(kustoQuery);

        var FilterOperators = code.Syntax.GetDescendants<FilterOperator>();
        var TakeOperators = code.Syntax.GetDescendants<TakeOperator>();

        foreach (var FilterOperator in FilterOperators)
        {
            var parsed = KustoCode.Parse(FilterOperator.ToString());

            var filters = parsed.Syntax.GetDescendants<NameReference>();
            var vals = parsed.Syntax.GetDescendants<LiteralExpression>();

            string remove = "datetime(";

            int i = 0;
            foreach (var descendant in filters)
            {
                string s = descendant.ToString().Substring(1);
                if (!filterMap.ContainsKey(s))
                {
                    filterMap.Add(s, new List<string>());
                }
                filterMap[s].Add(vals.ElementAt(i).ToString().Substring(1).Replace("\"", "").Replace(remove, "").Replace(")", ""));

                i++;
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
        }

        foreach (var TakeOperator in TakeOperators)
        {
            var parsed = KustoCode.Parse(TakeOperator.ToString());
            var vals = parsed.Syntax.GetDescendants<LiteralExpression>();

            if (!filterMap.ContainsKey("limit"))
            {
                filterMap.Add("limit", new List<string>());
            }
            filterMap["limit"].Add(vals.First().ToString());
        }

        if (filterMap.ContainsKey("Timestamp"))
        {
            timestamp = DateTime.Parse(filterMap["Timestamp"][0]);
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

        string sqlQuery = $"SELECT * FROM Logs WHERE ";
        // Construct the SQL query using the extracted components


        if (timestamp != null)
        {
            sqlQuery += $"Timestamp >= '{timestamp}' AND ";
        }

        if (service != null)
        {
            sqlQuery += $"Service = '{service}' AND ";
        }

        if (level != null)
        {
            sqlQuery += $"Level = '{level}' AND ";
        }

        if (sqlQuery.EndsWith(" AND "))
        {
            sqlQuery = sqlQuery.Substring(0, sqlQuery.Length - 5);
        }

        if (limit != null)
        {
            sqlQuery += $" LIMIT {limit}";
        }
        return sqlQuery;
    }
}


