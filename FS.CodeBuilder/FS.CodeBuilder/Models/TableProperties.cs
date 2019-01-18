using System;
using System.Collections.Generic;
using System.Text;

namespace FS.CodeBuilder.Models
{
    /// <summary>
    /// 表属性映射
    /// </summary>
    public class TableProperties
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string ColumnDescription { get; set; }
        public string ColumnType { get; set; }
    }
}
