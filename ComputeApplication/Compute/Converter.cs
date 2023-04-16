using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
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
        string table = s.Substring(0, s.IndexOf("|")).Trim();

        if (table != null)
        {
            return toSql(s, table);
        }
        else
        {
            return errToSql(table);
        }
    }

    static string toSql(string kustoQuery, string table)
    {
        int? limit = null;
        DateTime? timestamp = null;

        string sql = $"SELECT * FROM {table} WHERE ";
        // Construct the SQL query using the extracted components

        var code = KustoCode.Parse(kustoQuery);
        Dictionary<string, List<string>> filterMap = new Dictionary<string, List<string>>();

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

        }

        if (filterMap.ContainsKey("Timestamp"))
        {
            timestamp = DateTime.Parse(filterMap["Timestamp"][0]);
        }
        if (timestamp != null)
        {
            sql += $" Timestamp >= '{timestamp.Value.ToString("yyyy-MM-dd HH:mm:ss")}' AND ";
        }
        ArrayList list = new ArrayList();


        // print out the remaining key-value pairs
        foreach (var entry in filterMap)
        {
            string key = entry.Key;
            List<string> values = entry.Value;
            if (key != "limit" && key != "Timestamp")
            {
                list.Add($"{key} = '{string.Join(", ", values)}'");
            }


        }



        foreach (object element in list)
        {
            sql += $" {element} AND ";

        }



        if (sql.EndsWith(" AND "))
        {
            sql = sql.Substring(0, sql.Length - 5);
        }
        if (sql.EndsWith(" WHERE "))
        {
            sql = sql.Substring(0, sql.Length - 7);
        }

        if (limit != null)
        {
            sql += $" LIMIT {limit} ";
        }


        return sql;
    }



    static String errToSql(String table)
    {
        String s = "Table: \"" + table + "\" not accessable or table doesn't exist.";
        return s;
    }
}

public class Query
{
    //public string name { get; set; }
    public string query { get; set; }
}