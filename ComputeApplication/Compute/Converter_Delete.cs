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

public static class KustoToSqlConverter_Delete
{
    public static string Convert(Query query)
    {    //a method to decided calling logs table parser or request table parser, or none of them
         // Extract the table name
        string s = query.query;
        string table = s.Substring(0, s.IndexOf("|")).Trim();

        if (table != null && s.Contains("delete"))
        {
            string sql = KustoToSqlConverter.Convert(query);
            sql = sql.Replace("SELECT", "DELETE");
            sql = sql.Replace("*", "");

            return sql;
        }
        else
        {
            return "Error";
        }
    }
}
