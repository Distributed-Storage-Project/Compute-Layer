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

public static class KustoToSqlConverter_Create
{
    public static string Convert(Query query)
    {    //a method to decided calling logs table parser or request table parser, or none of them
         // Extract the table name
        string s = query.query;

        if (s.StartsWith(".create table"))
        {
            string table = "";
            // extract table name from create table queryC
            int startIndex = s.IndexOf("table") + 6;
            int endIndex = s.IndexOf("(", startIndex);
            table = s.Substring(startIndex, endIndex - startIndex).Trim();

            return create(s, table);
        }
        else
        {
            return "Error";
        }

    }

    public static string create(string kustoQuery, string table)
    {
        String sql = "";

        var code = KustoCode.Parse(kustoQuery);

        var CustomNodes = code.Syntax.GetDescendants<SeparatedElement>();

        foreach (var CustomNode in CustomNodes)
        {



            if (CustomNode != CustomNodes.Last())
            {
                String add = CustomNode.ToString().Replace(':', ' ');
                if (add.Contains("int"))
                {
                    add = add.Replace("int", "INTEGER");
                }
                if (add.Contains("string"))
                {
                    add = add.Replace("string", "VARCHAR");
                }
                if (add.Contains("datetime"))
                {
                    add = add.Replace("datetime", "DATETIME");
                }
                if (add.Contains("decimal"))
                {
                    add = add.Replace("decimal", "TEXT");
                }
                if (add.Contains("real"))
                {
                    add = add.Replace("real", "REAL");
                }


                sql += add;
            }
        }

        sql = "CREATE TABLE" + sql.Substring(13);

        return sql.Substring(0, sql.IndexOf(')') + 1);
    }

}
