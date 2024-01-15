using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace AlpineRed.DB
{
    public static class Util
    {
        public enum ParamType
        {
            Int,
            Decimal,
            Text
        }

        public static string ProtectSQL(string param, ParamType type)
        {
            switch (type)
            {
                case ParamType.Text:
                    return param.Replace("'", "''");

                case ParamType.Int:
                    try
                    {
                        return Convert.ToInt32(param).ToString();
                    }
                    catch //(Exception ex)
                    {
                        throw new ArgumentException($"'{ param }' is not a valid integer");
                    }

                case ParamType.Decimal:
                    try
                    {
                        return Convert.ToDecimal(param).ToString();
                    }
                    catch //(Exception ex)
                    {
                        throw new ArgumentException($"'{ param }' is not a valid decimal");
                    }

                default:
                    throw new ArgumentException($"Unrecognised parameter type '{ type }'");

            }
        }


        public static SqlParameter CreateFromNameTypeDefAndVal(string name, string typeDefinition, object value)
        {
            var parameterType = SqlDbType.Variant;

            #region "Sort the parameter data type"
            var length = 0;
            if (typeDefinition.Contains("("))
            {
                if (typeDefinition.Contains("(MAX)"))
                    length = -1;
                else
                {
                    var open = typeDefinition.IndexOf("(");
                    length = Convert.ToInt32(typeDefinition.Substring(open + 1, typeDefinition.IndexOf(")") - open - 1));
                }
                typeDefinition = typeDefinition.Substring(0, typeDefinition.IndexOf("("));
            }

            switch (typeDefinition)
            {
                case "bit":
                    parameterType = SqlDbType.Bit;
                    break;
                case "date":
                    parameterType = SqlDbType.Date;
                    break;
                case "datetime":
                    parameterType = SqlDbType.DateTime;
                    break;
                case "smalldatetime":
                    parameterType = SqlDbType.SmallDateTime;
                    break;
                case "decimal":
                    parameterType = SqlDbType.Decimal;
                    break;
                case "money":
                    parameterType = SqlDbType.Money;
                    break;
                case "smallmoney":
                    parameterType = SqlDbType.SmallMoney;
                    break;
                case "int":
                    parameterType = SqlDbType.Int;
                    break;
                case "nvarchar":
                    parameterType = SqlDbType.NVarChar;
                    break;
                case "nchar":
                    parameterType = SqlDbType.NChar;
                    break;
                case "uniqueidentifier":
                    parameterType = SqlDbType.UniqueIdentifier;
                    break;
                case "varbinary":
                    parameterType = SqlDbType.VarBinary;
                    break;
                case "varchar":
                    parameterType = SqlDbType.VarChar;
                    break;
                case "char":
                    parameterType = SqlDbType.Char;
                    break;

                default:
                    throw new ArgumentException($"Type '{ typeDefinition }' not handled");
            }
            #endregion
            try
            {
                #region "Create the parameter"
                //Now actually create and add the parameter to the command. Different data types need different options.
                switch (parameterType)
                {
                    case SqlDbType.Char:
                    case SqlDbType.VarChar:
                    case SqlDbType.NChar:
                    case SqlDbType.NVarChar:
                        return CreateParameter(name, parameterType, length, value);

                    case SqlDbType.Decimal:
                    case SqlDbType.Int:
                    case SqlDbType.UniqueIdentifier:
                    case SqlDbType.VarBinary:
                        return CreateParameter(name, parameterType, (value.ToString() == string.Empty ? DBNull.Value : value));

                    case SqlDbType.Bit:
                        bool bVal;
                        if (value.ToString() == String.Empty)
                        {
                            return CreateParameter(name, parameterType, DBNull.Value);
                        }
                        else
                        {
                            switch (value.ToString().ToUpper().Trim())
                            {
                                case "YES":
                                case "TRUE":
                                case "1":
                                    bVal = true;
                                    break;

                                case "NO":
                                case "FALSE":
                                case "0":
                                    bVal = false;
                                    break;

                                default:
                                    throw new Exception($"Unhandled bit input '{ value }'");
                            }
                            return CreateParameter(name, parameterType, bVal);
                        }

                    case SqlDbType.Date:
                    case SqlDbType.DateTime:
                        return CreateParameter(name, parameterType, (value.ToString() == string.Empty ? DBNull.Value : value));

                    default:
                        return CreateParameter(name, parameterType, value);
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to create parameter '{ name }'", ex);
            }
        }

        public static T IsDBNullRep<T>(DataRow profileData, string param, T replacement)
        {
            var data = profileData[param];
            if (data == DBNull.Value) return replacement;
            return (T)data;
        }

        public static string SQLDateFromDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public static string SQLDateAndTimeFromDate(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd hh:mm:ss");
        }

        #region "CreateParameter"
        public static SqlParameter CreateParameter(string name, SqlDbType type, object value)
        {
            return new SqlParameter
            {
                ParameterName = name,
                Value = value ?? DBNull.Value,
                SqlDbType = type,
                Direction = ParameterDirection.Input,
            };
        }
        public static SqlParameter CreateParameter(string name, SqlDbType type, int size, object value)
        {
            SqlParameter ret = CreateParameter(name, type, value);
            ret.Size = size;

            return ret;
        }
        public static SqlParameter CreateParameter(string name, SqlDbType type, ParameterDirection direction, object value)
        {
            SqlParameter ret = CreateParameter(name, type, value);
            ret.Direction = direction;

            return ret;
        }
        public static SqlParameter CreateParameter(string name, SqlDbType type, int size, ParameterDirection direction, object value)
        {
            SqlParameter ret = CreateParameter(name, type, size, value);
            ret.Direction = direction;

            return ret;
        }
        #endregion
        public static SqlParameter CreateParameter_Dt(string name, SqlDbType type, DateTime value)
        {
            return new SqlParameter()
            {
                ParameterName = name,
                Value = value,
                SqlDbType = type
            };
        }
        public static SqlParameter CreateParameter_Dt(string name, SqlDbType type, ParameterDirection direction, DateTime value)
        {
            SqlParameter ret = CreateParameter_Dt(name, type, value);
            ret.Direction = direction;

            return ret;
        }
    }
}
