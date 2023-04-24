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

public static class KustoToSqlConverter_Insert
{
    public static string Convert(Query query)
    {    //a method to decided calling logs table parser or request table parser, or none of them
         // Extract the table name
        string s = query.query;

        if (s.StartsWith(".append"))
        {
            string table = "";
            // extract table name from create table queryC

            table = s.Substring(0, s.IndexOf('[')).Replace(".append", "").Trim();
            return insert(s, table);
        }
        else
        {
            return "Error";
        }

    }

    public static string insert(string kustoQuery, string table)
    {
        String sql = "";

        var code = KustoCode.Parse(kustoQuery);

        var CustomNodes = code.Syntax.GetDescendants<SeparatedElement>();

        foreach (var CustomNode in CustomNodes)
        {



            if (CustomNode != CustomNodes.Last())
            {
                String add = CustomNode.ToString().Replace("\"", "\'").Trim();



                sql += add;
            }
        }

        sql = "INSERT INTO " + table + " VALUES " + sql.Substring(sql.IndexOf('[')).Replace("[", "").Replace("]", "").Trim();

        return sql;
    }

}
