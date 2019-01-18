using FS.CodeBuilder.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FS.CodeBuilder
{
    /// <summary>
    /// 组织实体内容
    /// </summary>
    public  class TableEntityHelper
    {
        #region 生成实体
        /// <summary>
        /// 创建model实体
        /// </summary>
        /// <param name="tableName"></param>
        public static void CreateModel(string tableName)
        {
           var tableProperties = DapperHelper.GetList<TableProperties>(tableName);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("namespace TB.Trade.Entity.Activity");
            sb.AppendLine("{");
            sb.AppendLine("    public class "+ tableName);
            sb.AppendLine("    {");
            foreach (var item in tableProperties)
            {
                sb.AppendLine("        ///<summary>");
                sb.AppendLine("        ///"+item.ColumnDescription);
                sb.AppendLine("        ///<summary>");
                var columnType = ColumnTypeMapping(item);
                sb.AppendLine("       public "+ columnType + " "+item.ColumnName+" { get; set; }");
            }
            sb.AppendLine("    }");
            foreach (var item in tableProperties)
            {
                if (item.ColumnDescription != null && item.ColumnDescription.Contains("（") && item.ColumnDescription.Contains("）"))
                {
                    sb.AppendLine("     public enum"+" Enum" + item.TableName.Replace("TB_", "") + item.ColumnName);
                    sb.AppendLine("     {");
                    var startIndex = item.ColumnDescription.IndexOf("（");
                    var enumStr=item.ColumnDescription.Substring(startIndex+1,item.ColumnDescription.Length- startIndex - 2);
                    Console.WriteLine(enumStr);
                    var i = 1;
                    foreach (var enumitem in enumStr.Split('/'))
                    {
                        if (i > 1) sb.AppendLine(",");
                        sb.Append("         "+enumitem+"="+i);
                        i++;
                    }
                    sb.AppendLine("");
                    sb.AppendLine("     }");
                }
                else
                {
                    continue;
                }
               
            }
            sb.AppendLine("}");
            FileHelper.WriteInfoToFile(sb.ToString(), "E:\\willDeleteProject\\FS.net\\TB.CMS\\TB.Trade\\Entity\\Activity\\" + tableName + ".cs", Encoding.Default);
            Console.WriteLine(tableName+" Model已生成！");
        }

        /// <summary>
        /// sql类型映射C#类型
        /// </summary>
        /// <param name="columnType"></param>
        /// <returns></returns>
        private static string ColumnTypeMapping(TableProperties tableProperties)
        {
            string resultType = tableProperties.ColumnType;
            switch (tableProperties.ColumnType)
            {
                case "int":
                    resultType = "int";
                    if (tableProperties.ColumnDescription!=null&&tableProperties.ColumnDescription.Contains("（")&& tableProperties.ColumnDescription.Contains("）"))
                    {
                        resultType ="Enum"+ tableProperties.TableName.Replace("TB_","")+ tableProperties.ColumnName;
                    }
                    break;
                case "nvarchar":
                case "varchar":
                    resultType = "string";
                    break;
                case "datetime":
                    resultType = "DateTime";
                    break;
                default:
                    break;
            }

            return resultType;
        }

        /// <summary>
        /// sql类型映射C#类型
        /// </summary>
        /// <param name="columnType"></param>
        /// <returns></returns>
        private static string ColumnTypeMappingParam(TableProperties tableProperties)
        {
            string resultType = tableProperties.ColumnType;
            switch (tableProperties.ColumnType)
            {
                case "int":
                    resultType = "Int";
                    break;
                case "nvarchar":
                    resultType = "NVarChar";
                    break;
                case "varchar":
                    resultType = "VarChar";
                    break;
                case "datetime":
                    resultType = "DateTime";
                    break;
                default:
                    break;
            }

            return resultType;
        }

        #endregion
        #region 生成Dal
        //private string ModelTitle
        //{

        //}
        /// <summary>
        /// 生成 dll操作数据库 curd
        /// </summary>
        /// <param name="tableName"></param>
        public static void CreateTrade(string tableName)
        {
            var tableProperties = DapperHelper.GetList<TableProperties>(tableName);
            StringBuilder insertParameters = new StringBuilder();
            StringBuilder updateParameters = new StringBuilder();
            var insertSql = GetInsertSql(tableProperties,out insertParameters);
            var updateSql = GetUpdateSql(tableProperties,out updateParameters);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Data.SqlClient;");
            sb.AppendLine("using System.Data;");
            sb.AppendLine("using TB.General;");
            sb.AppendLine("using TB.Trade.Entity;");
            sb.AppendLine("using TB.Trade.Entity.Activity;");
            sb.AppendLine();
            sb.AppendLine("namespace TB.Trade.DAL");
            sb.AppendLine("{");
            sb.AppendLine("    public class "+tableName+"DAL");
            sb.AppendLine("    {");
            sb.AppendLine("        public static int Insert("+ tableName + " "+ tableName.ToLower() + ")");
            sb.AppendLine("        {");
                sb.AppendLine("            string sql = \""+insertSql+"\";");
                sb.AppendLine("            SqlCommand comm = new SqlCommand(sql);");
                sb.AppendLine("            "+ insertParameters);
                sb.AppendLine("             DBClient client = new DBClient();");
                sb.AppendLine("             object obj = client.Scalar(comm, TB.General.DBName.TideBuy);");
                sb.AppendLine("             if (obj == null || obj == DBNull.Value) return -1;");
                sb.AppendLine("             int id = Convert.ToInt32(obj);");
                sb.AppendLine("             TB.Trade.Base.SyncExecute(comm, \"\", TB.General.DBName.TideBuy, id);");
                sb.AppendLine("             return id;");
            sb.AppendLine("       }");
            sb.AppendLine("        public static bool Update(" + tableName + " " + tableName.ToLower() + ", DBName dbName)");
            sb.AppendLine("        {");
            sb.AppendLine("            string sql = \"" + updateSql + "\";");
            sb.AppendLine("            SqlCommand comm = new SqlCommand(sql);");
            sb.AppendLine("            "+ updateParameters);
            sb.AppendLine("             DBClient client = new DBClient();");
            sb.AppendLine("            if (client.Execute(comm, dbName))");
            sb.AppendLine("            {");
            sb.AppendLine("            Base.SyncData(comm, \"\", DBName.TideBuy, 0);");
            sb.AppendLine("            return true;");
            sb.AppendLine("            }");
            sb.AppendLine("            return false;");
            sb.AppendLine("       }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            FileHelper.WriteInfoToFile(sb.ToString(), "E:\\willDeleteProject\\FS.net\\TB.CMS\\TB.Trade\\Dal\\" + tableName + "DAL.cs", Encoding.Default);
            Console.WriteLine(tableName + "Trade已生成！");
        }

        private static string GetInsertSql(List<TableProperties> tablePropertiesList,out StringBuilder parameters)
        {
            var column = "";
            var param = "";
            int i = 0;
            parameters =new StringBuilder("");
            foreach (var item in tablePropertiesList)
            {
                if (item.ColumnName.ToLower() == "id") continue;
                var tableNameLower = tablePropertiesList[0].TableName.ToLower();
                if (i > 0)
                {
                    column += ",";
                    param += ",";
                }
                column += item.ColumnName;
                param +="@"+item.ColumnName;
                parameters.AppendLine("        comm.Parameters.Add(\"@"+item.ColumnName+"\", SqlDbType."+ ColumnTypeMappingParam(item) + ").Value = " + tableNameLower +"."+ item.ColumnName + ";");
                i++;
            }
            string sql = string.Format("insert into {0} ({1}) values({2});SELECT @@identity;", tablePropertiesList[0].TableName, column, param);
            return sql;
        }
        private static string GetUpdateSql(List<TableProperties> tablePropertiesList, out StringBuilder parameters)
        {
            var columnUpdate = "";
            var where = " siteID=@siteID and id=@ID";
            int i = 0;
            parameters = new StringBuilder("");
            var tableNameLower = tablePropertiesList[0].TableName.ToLower();
            foreach (var item in tablePropertiesList)
            {
                if (item.ColumnName.ToLower() == "id"|| item.ColumnName.ToLower() == "siteid"|| item.ColumnName.ToLower() == "createtime"|| item.ColumnName.ToLower() == "createname") continue;
                if (i > 0)
                {
                    columnUpdate += ",";
                }
                columnUpdate += item.ColumnName+"=@"+item.ColumnName;
                parameters.AppendLine("        comm.Parameters.Add(\"@" + item.ColumnName + "\", SqlDbType." + ColumnTypeMappingParam(item) + ").Value = " + tableNameLower + "."+ item.ColumnName + ";");
                i++;
            }
            string sql = string.Format("update {0} set {1} where {2}", tablePropertiesList[0].TableName, columnUpdate, where);
            return sql;
        }
        #endregion

        #region 生成Page
        /// <summary>
        /// 生成列表页
        /// </summary>
        /// <param name="tableName"></param>
        public static void CreatePageList(string tableName)
        {
            var tablename = "Manage"+tableName.Replace("TB_", "");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<%@ Page Language=\"C#\" AutoEventWireup=\"true\" CodeBehind=\""+tablename+ ".aspx.cs\" Inherits=\"TB.CMSUI.Activity." + tablename + "\" %>");
            sb.AppendLine("<%@ Import Namespace=\"System.Data\"%>");
            sb.AppendLine("<!DOCTYPE html PUBLIC \" -//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            sb.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            sb.AppendLine("<head>");
            sb.AppendLine("<title>"+ tablename + "</title>");
            sb.AppendLine("<link href=\"../js/layui-v2.4.5/layui/css/layui.css\" rel=\"stylesheet\" />");
            sb.AppendLine("<script src=\"../js/layui-v2.4.5/layui/layui.all.js\"></script>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");



            FileHelper.WriteInfoToFile(sb.ToString(), "E:\\willDeleteProject\\FS.net\\TB.CMS\\TB.CMS\\Activity\\" + tablename + ".aspx", Encoding.UTF8);


            StringBuilder sbcs = new StringBuilder();
            sbcs.AppendLine("using System;");
            sbcs.AppendLine("namespace TB.CMSUI.Activity");
            sbcs.AppendLine("{");
            sbcs.AppendLine("       public partial class "+ tablename + " : System.Web.UI.Page");
            sbcs.AppendLine("      {");
            sbcs.AppendLine("            protected void Page_Load(object sender, EventArgs e)");
            sbcs.AppendLine("            {");

            sbcs.AppendLine("            }");
            sbcs.AppendLine("      }");
            sbcs.AppendLine("}");
            FileHelper.WriteInfoToFile(sbcs.ToString(), "E:\\willDeleteProject\\FS.net\\TB.CMS\\TB.CMS\\Activity\\" + tablename + ".aspx.cs", Encoding.Default);


            StringBuilder sbdesigner = new StringBuilder();
            sbdesigner.AppendLine("namespace TB.CMSUI.Activity");
            sbdesigner.AppendLine("{");
            sbdesigner.AppendLine("   public partial class "+tablename);
            sbdesigner.AppendLine("   {");
          
            sbdesigner.AppendLine("   }");
            sbdesigner.AppendLine("}");
            FileHelper.WriteInfoToFile(sbdesigner.ToString(), "E:\\willDeleteProject\\FS.net\\TB.CMS\\TB.CMS\\Activity\\" + tablename + ".aspx.designer.cs", Encoding.Default);
            Console.WriteLine(tableName + "PageList已生成！");
        }
        #endregion

        public static void CreateAction()
        {

        }
    }
}
