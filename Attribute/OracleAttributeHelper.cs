using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace SequenceTracker
{
    internal static class OracleAttributeHelper
    {
        public static string GetTableName<T>()
        {
            Type type = typeof(T);

            OracleTableAttribute attribute = type.GetCustomAttribute<OracleTableAttribute>();

            if(attribute == null)
            {
                throw new Exception($"OracleTable attribute not found on {type.Name}");
            }

            return attribute?.TableName;
        }

        public static string GetColumnName<T>(string memberName)
        {
            Type type = typeof(T);

            MemberInfo member = type.GetMember(memberName, BindingFlags.Public | BindingFlags.Instance).FirstOrDefault();

            if(member == null)
            {
                throw new Exception($"Member {memberName} not found on {type.Name}");
            }

            OracleColumnAttribute attribute = member.GetCustomAttribute<OracleColumnAttribute>();

            if(attribute == null)
            {
                throw new Exception($"OracleColumn attribute not found on member {memberName} of {type.Name}");
            }   

            return attribute?.ColumnName;
        }
    }
}
