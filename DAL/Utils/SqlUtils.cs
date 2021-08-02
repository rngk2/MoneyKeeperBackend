using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace DAL.Utils
{
	class SqlUtils
   	{
        public static string SqlSelectBuild(string tableName, Dictionary<string, object> conditions = null, string selectWhat = "*")
        {
            string sql = $"select {selectWhat} from " + tableName + " where 1=1 ";

            foreach (KeyValuePair<string, object> kvp in conditions ?? new())
            {
                string strType = kvp.Value.GetType().ToString().ToLower(); //Get the current type
                bool blNum = strType.Contains("int") || strType.Contains("float") ||
                                                                   strType.Contains("double"); //This field is a numeric type, without quotes true
                if (blNum) //numeric field
                {
                    sql += " and " + kvp.Key + "=" + kvp.Value;
                }
                else //non-numeric field
                {
                    sql += " and " + kvp.Key + "='" + kvp.Value + "'";
                }
            }
            return sql;
        }


        public static string SqlInsertBuild(object obj, string tableName, string[] remove = null)
        {
            string sqlBefor = "insert into " + tableName + "(";
            string sqlAfter = " values(";
            Type type = obj.GetType();
            PropertyInfo[] pis = type.GetProperties();
			foreach (PropertyInfo pi in pis)
            {
				bool addRemove = true;
				if (remove is not null)
                {
                    foreach (string re in remove) //Exclude some fields (self-increasing fields)
                    {
                        if (pi.Name.ToLower().Equals(re.ToLower()))
                        {
                            addRemove = false;
                        }
                    }
                }

                if (addRemove)
                {
                    string strType = type.GetProperty(pi.Name).PropertyType.ToString().ToLower(); //Get the current field type
                    bool blNum = strType.Contains("int") || strType.Contains("float") ||
                                                                   strType.Contains("double"); //This field is a numeric type, without quotes true
                    sqlBefor += pi.Name + ",";
                    if (blNum)
                    {
                        sqlAfter += type.GetProperty(pi.Name).GetValue(obj, null) + " ,";
                    }
                    else
                    {
                        sqlAfter += "'" + type.GetProperty(pi.Name).GetValue(obj, null) + "' ,";
                    }
                }
            }
            return sqlBefor.Substring(0, sqlBefor.Length - 1) + ") " + sqlAfter.Substring(0, sqlAfter.Length - 1) + ")";
        }

        public static string SqlUpdateBuild(object obj, string tableName, string[] whereFieldName)
        {
            string sqlSet = "update " + tableName + " set ";
            string sqlWhere = " where ";
            Type type = obj.GetType();
            PropertyInfo[] pis = type.GetProperties();
            
            foreach (PropertyInfo pi in pis)
            {
                bool blSetOrWher = true;
                if (whereFieldName != null)
                {
                    foreach (string whereFile in whereFieldName) //Whether it belongs to a condition field
                    {
                        if (pi.Name.ToLower().Equals(whereFile.ToLower()))
                        {
                            blSetOrWher = false;
                            break;
                        }
                    }
                }
                string strType = type.GetProperty(pi.Name).PropertyType.ToString().ToLower(); //Get the current field type
                bool blNum = strType.Contains("int") || strType.Contains("float") ||
                                                           strType.Contains("double"); //This field is a numeric type, without quotes true
                if (blSetOrWher)
                {
                    if (blNum)
                    {
                        sqlSet += pi.Name + "=" + type.GetProperty(pi.Name).GetValue(obj, null) + " ,";
                    }
                    else
                    {
                        sqlSet += pi.Name + "='" + type.GetProperty(pi.Name).GetValue(obj, null) + "' ,";
                    }
                }
                else
                {
                    if (blNum)
                    {
                        sqlWhere += pi.Name + "=" + type.GetProperty(pi.Name).GetValue(obj, null) + " and ";
                    }
                    else
                    {
                        sqlWhere += pi.Name + "='" + type.GetProperty(pi.Name).GetValue(obj, null) + "' and ";
                    }
                }
            }
            if (whereFieldName == null)
            {
                return null; //There is no condition for this statement
            }
            return sqlSet.Substring(0, sqlSet.Length - 1) + sqlWhere.Substring(0, sqlWhere.Length - 4); //Condition-4 means "and"
        }

        public static string SqlDeleteBuild(string tableName, string primaryKey, object primaryKeyvalue)
        {
            Type type = primaryKeyvalue.GetType();
            string strType = type.FullName.ToLower(); //Get parameter type
            bool blNum = strType.Contains("int") || strType.Contains("float") ||
                                                                   strType.Contains("double"); //Whether it is a numeric type
            if (blNum) //Number type
            {
                return "delete from " + tableName + " where " + primaryKey + " = " + primaryKeyvalue.ToString();
            }
            else //non-numeric type
            {
                return "delete from " + tableName + " where " + primaryKey + " = '" + primaryKeyvalue.ToString() + "'";
            }
        }


    }
}
