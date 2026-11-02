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

}
