using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FS.CodeBuilder
{
    public class DapperHelper
    {
        private readonly static string connString = "server=.;uid=test;pwd=test;database=tidebuy";

        public static int Execute<T>(string sql, List<T> list)
        {
            using (IDbConnection connection = new SqlConnection(connString))
            {
                return connection.Execute(sql, list);
            }
        }

        public static int Execute(string sql)
        {
            using (IDbConnection connection = new SqlConnection(connString))
            {
                return connection.Execute(sql);
            }
        }

        public static List<T> GetList<T>(string tableName)
        {
            var sql = @"
            SELECT
            A.name AS TableName,
            B.name AS ColumnName,
            C.value AS ColumnDescription,
            P.DATA_TYPE as ColumnType
            FROM sys.tables A
            INNER JOIN sys.columns B ON B.object_id = A.object_id
            LEFT JOIN sys.extended_properties C ON C.major_id = B.object_id AND C.minor_id = B.column_id
            LEFT JOIN information_schema.columns P on P.COLUMN_NAME=B.name and P.TABLE_NAME=A.Name
            WHERE A.name = '"+tableName+"'";
            using (IDbConnection connection = new SqlConnection(connString))
            {
                return connection.Query<T>(sql).ToList();
            }
        }
    }
}
