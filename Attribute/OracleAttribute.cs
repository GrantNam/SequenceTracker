using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequenceTracker
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class OracleTableAttribute : Attribute
    {
        public string TableName { get; set; }
        public OracleTableAttribute(string tableName)
        { 
            TableName = tableName;
        }

    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    internal class OracleCountAttribute : Attribute
    {
        public string ColumnName { get; set; }

        public OracleCountAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    internal class  OracleColumnAttribute : Attribute
    {
        public string ColumnName { get; set; }

        public OracleColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    internal class OracleIndexAttribute : Attribute
    {
        public List<string> ColumnNames { get; set; } //0: Column Name || 1: CarTypeNo Index
        public string TableName;

        public OracleIndexAttribute(string Table, string columnName, string index)
        {
            ColumnNames = new List<string>();
            TableName = Table;
            ColumnNames.Add(index);
            ColumnNames.Add(columnName);
        }
    }


}
