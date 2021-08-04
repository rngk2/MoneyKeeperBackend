using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace DAL.Utils
{
    public class SqlQueryGenerator
    {
        public static string GenerateSelectQuerySecure(string tableName, object filters = null, string selectWhat = "*")
        {
            string sql = $"select {selectWhat} from " + tableName + " where 1=1 ";

            if (filters is not null)
            {
                Type type = filters.GetType();
                PropertyInfo[] pis = type.GetProperties();

                foreach (var pi in pis)
                {
                    sql += " and " + pi.Name + " = @" + pi.Name;
                }
            }
            return sql;
        }


        public static string GenerateInsertQuerySecure(object obj, string tableName, string[] remove = null)
        {
            string sqlBefore = "insert into " + tableName + "(";
            string sqlAfter = " values(";
            Type type = obj.GetType();
            PropertyInfo[] pis = type.GetProperties();
            bool toRemove = false;

            foreach (PropertyInfo pi in pis)
            {
                toRemove = false;
                if (remove is not null)
                {
                    foreach (string re in remove) //Exclude some fields (self-increasing fields)
                    {
                        if (pi.Name.ToLower().Equals(re.ToLower()))
                        {
                            toRemove = true;
                        }
                    }
                }

                if (!toRemove)
                {
                    sqlBefore += pi.Name + ",";
                    sqlAfter += "@" + pi.Name + ",";
                }
            }
            return sqlBefore[0..^1] + ") " + sqlAfter[0..^1] + ")";
        }

        public static string GenerateUpdateQuerySecure(object obj, string tableName, string[] whereFieldName)
        {
            if (whereFieldName == null)
            {
                throw new Exception("There is no condition for this statement");
            }

            string sqlSet = "update " + tableName + " set ";
            string sqlWhere = " where ";
            Type type = obj.GetType();
            PropertyInfo[] pis = type.GetProperties();

            foreach (PropertyInfo pi in pis)
            {
                bool blSetOrWhere = true; // true - set; false - where
                foreach (string whereFile in whereFieldName) //Whether it belongs to a condition field
                {
                    if (pi.Name.ToLower().Equals(whereFile.ToLower()))
                    {
                        blSetOrWhere = false;
                        break;
                    }
                }

                if (blSetOrWhere)
                {
                    sqlSet += pi.Name + " = @" + pi.Name + " ,";
                }
                else
                {
                    sqlWhere += pi.Name + " = @" + pi.Name + " and ";
                }
            }

            return sqlSet[0..^1] + sqlWhere[0..^4]; //Condition-4 means "and"
        }

        public static string GenerateDeleteQuerySecure(string tableName, string primaryKey) =>
            "delete from " + tableName + " where " + primaryKey + " = @" + primaryKey;

    }
}

