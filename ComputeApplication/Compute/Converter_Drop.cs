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

public static class KustoToSqlConverter_Drop
{
    public static string Convert(Query query)
    {    //a method to decided calling logs table parser or request table parser, or none of them
         // Extract the table name
        string s = query.query;

        if (s.StartsWith(".drop table"))
        {
            string table = "";
            // extract table name from create table queryC
            int startIndex = s.IndexOf("table") + 5;

            table = s.Substring(startIndex).Replace("|", "").Trim();
            return Drop(s, table);
        }
        else
        {
            return "Error";
        }

    }

    public static string Drop(string kustoQuery, string table)
    {
        String sql = "DROP TABLE " + table;

        return sql;
    }

}
