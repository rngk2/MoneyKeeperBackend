using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Auth
{
	internal class SqlUtils
	{
        internal static string SqlInsertBuild(object obj, string tableName, string[] remove = null)
        {
            string sqlBefor = "insert into " + tableName + "(";
            string sqlAfter = " values(";
            Type type = obj.GetType();
            PropertyInfo[] pis = type.GetProperties();
            bool addRemove = true; //Whether the current field is not used as a modified field
            foreach (PropertyInfo pi in pis)
            {
                addRemove = true;
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

                if (pi.Name.ToLower().Contains("Is".ToLower()) || pi.Name.ToLower().Equals("Id".ToLower()))
                {
                    addRemove = false;
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
    }
}
