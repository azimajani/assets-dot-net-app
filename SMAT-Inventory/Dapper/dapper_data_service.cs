using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data.Common;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using Dapper;
using SMAT_Inventory.Exceptions;
using System.Threading.Tasks;


namespace SMAT_Inventory.Dapper
{
    public class DAPPER_DATA_SERVICE : IDisposable
    {
        protected SqlConnection sql_connection;
        protected string connection_string;
        private bool IsDisposed = false;
        public DynamicParameters dp;
        protected bool _run_in_text_mode = false;

        public DAPPER_DATA_SERVICE()
        {
            connection_string = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            dp = new DynamicParameters();
        }
        public DAPPER_DATA_SERVICE(string connection_string_key_name)
        {
            connection_string = ConfigurationManager.ConnectionStrings[connection_string_key_name].ConnectionString;
            dp = new DynamicParameters();
        }


        public void AddIntegerParameter(string name, int value)
        {
            dp.Add(name, value, dbType: DbType.Int32, direction: ParameterDirection.Input);
        }

        public void AddDecimalParameter(string name, decimal value)
        {
            dp.Add(name, value, dbType: DbType.Decimal, direction: ParameterDirection.Input);
        }

        public void SetIntegerParameter(string name, int value)
        {
            dp.Add(name, value, dbType: DbType.Int32, direction: ParameterDirection.Input);
        }

        public void AddOutputIntegerParameter(string name)
        {
            dp.Add(name, default(Int32), dbType: DbType.Int32, direction: ParameterDirection.Output);
        }

        public void AddOutputDecimalParameter(string name)
        {
            dp.Add(name, default(Decimal), dbType: DbType.Decimal, direction: ParameterDirection.Output);
        }

        public int GetOutputIntegerParameter(string name)
        {
            return dp.Get<Int32>(name);
        }

        public Decimal GetOutputDecimalParameter(string name)
        {
            return dp.Get<Decimal>(name);
        }

        public void AddShortParameter(string name, short value)
        {
            dp.Add(name, value, dbType: DbType.Int16, direction: ParameterDirection.Input);
        }

        public void SetShortParameter(string name, short value)
        {
            dp.Add(name, value, dbType: DbType.Int16, direction: ParameterDirection.Input);
        }

        public void AddOutputShortParameter(string name)
        {
            dp.Add(name, default(Int16), dbType: DbType.Int16, direction: ParameterDirection.Output);
        }

        public short GetOutputShortParameter(string name)
        {
            return dp.Get<Int16>(name);
        }

        public void AddByteParameter(string name, byte value)
        {
            dp.Add(name, value, dbType: DbType.Byte, direction: ParameterDirection.Input);
        }

        public void SetByteParameter(string name, object value)
        {
            dp.Add(name, value, dbType: DbType.Byte, direction: ParameterDirection.Input);
        }

        public void AddOutputByteParameter(string name)
        {
            dp.Add(name, default(byte), dbType: DbType.Byte, direction: ParameterDirection.Output);
        }

        public byte GetOutputByteParameter(string name)
        {
            return dp.Get<byte>(name);
        }

        public void AddStringParameter(string name, string value, int size)
        {
            value = value == null ? "" : value;

            if (String.Format("{0}", value).Length > size)
            {
                Exception ex = new System.Exception(String.Format("'{0}' should be max. of {1} characters. You entered {2} characters.", name, size, value.Length));
                //LOGGER.error(String.Format("Name: {0}; Value: {1}", name, value), ex);
                throw ex;
            }

            dp.Add(name, value, dbType: DbType.AnsiString, direction: ParameterDirection.Input, size: size);
        }

        public void AddGUIDParameter(string name, Guid value)
        {
            dp.Add(name, value, dbType: DbType.Guid, direction: ParameterDirection.Input);
        }

        public void AddUnicodeStringParameter(string name, string value, int size)
        {
            value = value == null ? "" : value;

            if (String.Format("{0}", value).Length > size)
            {
                Exception ex = new System.Exception(String.Format("You are exceeded the maximum allowed length of {0}.", size));
                //LOGGER.error(String.Format("Name: {0}; Value: {1}", name, value), ex);
                throw ex;
            }

            dp.Add(name, value, dbType: DbType.String, direction: ParameterDirection.Input, size: size);
        }

        private int GetStringSize<T>(string property_name)
        {
            foreach (PropertyDescriptor propertyInfo in TypeDescriptor.GetProperties(typeof(T)))
            {
                if (propertyInfo.Name == property_name)
                {
                    StringLengthAttribute StringLength = propertyInfo.Attributes.OfType<StringLengthAttribute>().FirstOrDefault();
                    return StringLength.MaximumLength;
                }
            }

            return 8000;
        }

        public void AddStringParameter<T>(string name, string value)
        {
            value = value == null ? "" : value;

            int StringSize = GetStringSize<T>(name);
            if (String.Format("{0}", value).Length > StringSize)
            {
                Exception ex = new System.Exception(String.Format("You are exceeding the maximum allowed length of {0}.", StringSize));
                //LOGGER.error(String.Format("Name: {0}; Value: {1}", name, value), ex);
                throw ex;
            }

            dp.Add(name, value, dbType: DbType.AnsiString, direction: ParameterDirection.Input, size: StringSize);
        }

        public void AddUnicodeStringParameter<T>(string name, string value)
        {
            value = value == null ? "" : value;

            int StringSize = GetStringSize<T>(name);
            if (String.Format("{0}", value).Length > StringSize)
            {
                Exception ex = new System.Exception(String.Format("You are exceeded the maximum allowed length of {0}.", StringSize));
                //LOGGER.error(String.Format("Name: {0}; Value: {1}", name, value), ex);
                throw ex;
            }

            dp.Add(name, value, dbType: DbType.String, direction: ParameterDirection.Input, size: StringSize);
        }

        public void SetStringParameter(string name, string value, int size)
        {
            value = value == null ? "" : value;

            if (String.Format("{0}", value).Length > size)
            {
                throw new System.Exception(String.Format("You are exceeded the maximum allowed length of {0}.", size));
            }

            dp.Add(name, value, dbType: DbType.AnsiString, direction: ParameterDirection.Input, size: size);
        }

        public void AddOutputStringParameter(string name, int size)
        {
            dp.Add(name, default(string), dbType: DbType.AnsiString, direction: ParameterDirection.Output, size: size);
        }

        public string GetOutputStringParameter(string name)
        {
            return dp.Get<string>(name);
        }

        public void AddBooleanParameter(string name, bool? value)
        {
            dp.Add(name, value, dbType: DbType.Boolean, direction: ParameterDirection.Input);
        }

        public void SetBooleanParameter(string name, bool value)
        {
            dp.Add(name, value, dbType: DbType.Boolean, direction: ParameterDirection.Input);
        }

        public void GetBooleanParameter(string name, bool value)
        {
            dp.Add(name, value, dbType: DbType.Boolean, direction: ParameterDirection.Input);
        }

        public void AddOutputBooleanParameter(string name)
        {
            dp.Add(name, default(bool), dbType: DbType.Boolean, direction: ParameterDirection.Output);
        }

        public bool GetOutputBooleanParameter(string name)
        {
            return dp.Get<bool>(name);
        }

        public void AddDateTimeParameter(string name, DateTime? value)
        {
            dp.Add(name, value == null ? (object)DBNull.Value : value.Value, dbType: DbType.DateTime, direction: ParameterDirection.Input);
        }

        public void SetDateTimeParameter(string name, DateTime value)
        {
            dp.Add(name, value, dbType: DbType.DateTime, direction: ParameterDirection.Input);
        }

        public void GetDateTimeParameter(string name, DateTime value)
        {
            dp.Add(name, value, dbType: DbType.DateTime, direction: ParameterDirection.Input);
        }

        public void AddOutputDateTimeParameter(string name)
        {
            dp.Add(name, default(DateTime), dbType: DbType.DateTime, direction: ParameterDirection.Output);
        }

        public DateTime GetOutputDateTimeParameter(string name)
        {
            return dp.Get<DateTime>(name);
        }

        public void AddSmallDateTimeParameter(string name, DateTime value)
        {
            dp.Add(name, value, dbType: DbType.DateTime, direction: ParameterDirection.Input);
        }

        public void SetSmallDateTimeParameter(string name, DateTime value)
        {
            dp.Add(name, value, dbType: DbType.DateTime, direction: ParameterDirection.Input);
        }

        public void GetSmallDateTimeParameter(string name, DateTime value)
        {
            dp.Add(name, value, dbType: DbType.DateTime, direction: ParameterDirection.Input);
        }

        public void AddOutputSmallDateTimeParameter(string name, DateTime value)
        {
            dp.Add(name, default(DateTime), dbType: DbType.DateTime, direction: ParameterDirection.Output);
        }

        public DateTime GetOutputSmallDateTimeParameter(string name)
        {
            return dp.Get<DateTime>(name);
        }

        public void AddGuidParameter(string name, Guid value)
        {
            dp.Add(name, value, dbType: DbType.Guid, direction: ParameterDirection.Input);
        }

        public void AddStructuredTypeParameter(string name, string type_name, object value)
        {
            //dp.Add(name, value, DbType.I ParameterDirection.Input);

            //var type_parameter = new SqlParameter(name, SqlDbType.Structured);
            //type_parameter.Direction = ParameterDirection.Input;
            //type_parameter.Value = value;


        }

        private DbType get_db_type(string type_name)
        {
            if (String.Compare(type_name, "String", true) == 0)
            {
                return DbType.AnsiString;
            }
            else if (String.Compare(type_name, "DateTime", true) == 0)
            {
                return DbType.DateTime;
            }
            else if (String.Compare(type_name, "Boolean", true) == 0)
            {
                return DbType.Boolean;
            }
            else
            {
                return DbType.Int32;
            }
        }

        private void initialize()
        {
            sql_connection = new SqlConnection(connection_string);
        }

        internal void open_connection()
        {
            if (sql_connection == null) { sql_connection = new SqlConnection(connection_string); }
            else { sql_connection = new SqlConnection(connection_string); }

            if (sql_connection.State != ConnectionState.Open)
            {
                try
                {
                    
                    sql_connection.Open();
                }
                catch (SqlException ex)
                {
                    switch (ex.Number)
                    {
                        case 2:
                            throw new SERVER_CONNECTION_EXCEPTION();
                        case 17:
                            throw new SERVER_NOT_FOUND_EXCEPTION();
                        case 18456:
                            throw new LOGIN_FAILED_EXCEPTION();
                        case 4060:
                            throw new DATABASE_NOT_FOUND_EXCEPTION();
                        default:
                            throw ex;

                    }
                }
            }
        }

        public void close_connection()
        {
            if ((sql_connection != null) && (sql_connection.State != ConnectionState.Closed))
            {
                sql_connection.Close();
            }

        }

        private string get_command_text(string stored_procedure_name, params object[] parameters)
        {
            StringBuilder command_text = new StringBuilder();
            try
            {
                command_text.Append("exec ");
                command_text.Append(stored_procedure_name);
                command_text.Append(" ");

                bool first_param = true;
                foreach (object p in parameters)
                {
                    if (first_param)
                        first_param = false;
                    else
                        command_text.Append(", ");

                    if (p.GetType().Name == "String")
                    {
                        command_text.Append(String.Format("'{0}'", p));
                    }
                    else if (p.GetType().Name == "DateTime")
                    {
                        command_text.Append(String.Format("'{0}'", Convert.ToDateTime(p).ToString("MM/dd/yyyy")));
                    }
                    else if (p.GetType().Name == "Bool")
                    {
                        if (Convert.ToBoolean(p))
                        {
                            command_text.Append("1");
                        }
                        else
                        {
                            command_text.Append("0");
                        }
                    }
                    else
                    {
                        command_text.Append(String.Format("{0}", p));
                    }
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("{0}-{1}", stored_procedure_name, command_text.ToString());
                //LOGGER.error(error_message, ex);
                throw ex;
            }

            return command_text.ToString();
        }

        /// <summary>
        /// Used when require to get multiple recordset
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <param name="parameters">Parameter as Comma Separated Values, sequence should be same as stored procedure parameter</param>
        /// <returns>SqlMapper.GridReader</returns>
        public SqlMapper.GridReader query_multiple(string stored_procedure_name, params object[] parameters)
        {
            return this.query_multiple(get_command_text(stored_procedure_name, parameters));
        }

        /// <summary>
        /// Used when require to get multiple recordset
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <param name="parameters">Parameter as DynamicParamters Object, support output parameter.</param>
        /// <returns>SqlMapper.GridReader</returns>
        public SqlMapper.GridReader query_multiple(string stored_procedure_name, ref DynamicParameters parameters)
        {
            SqlMapper.GridReader value = null;
            try
            {
                open_connection();
                value = sql_connection.QueryMultiple(stored_procedure_name, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}-paramteres:     {1}", stored_procedure_name, parameters.ToString());
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}-paramteres:     {1}", stored_procedure_name, parameters.ToString());
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }

            return value;
        }

        /// <summary>
        /// Used when require to get multiple recordset without parameter
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <returns>SqlMapper.GridReader</returns>
        public SqlMapper.GridReader query_multiple(string stored_procedure_name)
        {
            SqlMapper.GridReader value = null;
            try
            {
                open_connection();
                if (dp.Length() > 0)
                {
                    value = sql_connection.QueryMultiple(stored_procedure_name, dp, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    value = sql_connection.QueryMultiple(stored_procedure_name);
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                throw ex;
            }

            return value;
        }

        /// <summary>
        /// Used when require to get multiple recordset without parameter
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <returns>SqlMapper.GridReader</returns>
        public async Task<SqlMapper.GridReader> query_multipleAsync(string stored_procedure_name)
        {
            SqlMapper.GridReader value = null;
            try
            {
                open_connection();
                if (dp.Length() > 0)
                {
                    value = await sql_connection.QueryMultipleAsync(stored_procedure_name, dp, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    value = await sql_connection.QueryMultipleAsync(stored_procedure_name);
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                throw ex;
            }

            return value;
        }

        /// <summary>
        /// Used when require to get nested recordset like profile with his address
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <param name="parameters">Parameter as Comma Separated Values, sequence should be same as stored procedure parameter</param>
        /// <returns>IEnumerable<Tuple<A, B>></returns>
        public IEnumerable<Tuple<A, B>> query_nested_map<A, B>(string stored_procedure_name, params object[] parameters)
        {
            if (parameters.Length > 0)
            {
                return this.query_nested_map<A, B>(get_command_text(stored_procedure_name, parameters));
            }
            else
            {
                return this.query_nested_map<A, B>(stored_procedure_name);
            }
        }

        /// <summary>
        /// Used when require to get nested recordset like profile with his address
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <param name="parameters">Parameter as DynamicParamters Object, support output parameter.</param>
        /// <returns>IEnumerable<Tuple<A, B>></returns>
        public IEnumerable<Tuple<A, B>> query_nested_map<A, B>(string stored_procedure_name, ref DynamicParameters parameters, string split_on = "id")
        {
            IEnumerable<Tuple<A, B>> value = null;
            try
            {
                open_connection();
                value = sql_connection.Query<A, B, Tuple<A, B>>(stored_procedure_name, (a, b) => Tuple.Create(a, b), parameters, commandType: CommandType.StoredProcedure, splitOn: split_on);
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}-paramteres:     {1}", stored_procedure_name, parameters.ToString());
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}-paramteres:     {1}", stored_procedure_name, parameters.ToString());
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }


            return value;
        }

        /// <summary>
        /// Used when require to get nested recordset without parameter like profile with his address
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <returns>IEnumerable<Tuple<A, B>></returns>
        public IEnumerable<Tuple<A, B>> query_nested_map<A, B>(string stored_procedure_name, string split_on = "id")
        {
            IEnumerable<Tuple<A, B>> value = null;
            try
            {
                open_connection();
                if (dp.Length() > 0)
                {
                    value = sql_connection.Query<A, B, Tuple<A, B>>(stored_procedure_name, (a, b) => Tuple.Create(a, b), dp, commandType: CommandType.StoredProcedure, splitOn: split_on);
                }
                else
                {
                    value = sql_connection.Query<A, B, Tuple<A, B>>(stored_procedure_name, (a, b) => Tuple.Create(a, b), splitOn: split_on);
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }


            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="stored_procedure_name"></param>
        /// <param name="split_on"></param>
        /// <returns></returns>
        public IEnumerable<Tuple<A, B, C>> query_nested_map<A, B, C>(string stored_procedure_name, string split_on = "id")
        {
            IEnumerable<Tuple<A, B, C>> value = null;
            try
            {
                open_connection();
                if (dp.Length() > 0)
                {
                    value = sql_connection.Query<A, B, C, Tuple<A, B, C>>(stored_procedure_name, (a, b, c) => Tuple.Create(a, b, c), dp, commandType: CommandType.StoredProcedure, splitOn: split_on);
                }
                else
                {
                    value = sql_connection.Query<A, B, C, Tuple<A, B, C>>(stored_procedure_name, (a, b, c) => Tuple.Create(a, b, c), splitOn: split_on);
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }


            return value;
        }

        /// <summary>
        /// Used when require to get single recordset
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <param name="parameters">Parameter as Comma Separated Values, sequence should be same as stored procedure parameter</param>
        /// <returns>List<T></returns>
        public List<T> execute_stored_procedure<T>(string stored_procedure_name, params object[] parameters)
        {
            return this.execute_stored_procedure<T>(get_command_text(stored_procedure_name, parameters));
        }


        public IEnumerable<T> ExecuteQuery<T>(string sql, object parameters = null)
        {
            try
            {
                open_connection();
                using (var connection = sql_connection)
                {

                    //connection.Open();
                    return connection.Query<T>(sql, parameters).ToList();
                }
            }
            finally
            {
                close_connection();
            }
        }
        public int ExecuteQueryint(string sql, object parameters = null)
        {
            try
            {
                open_connection();
                using (var connection = sql_connection)
                {

                    //connection.Open();
                    return connection.Query<int>(sql, parameters).FirstOrDefault();
                }
            }
            finally
            {
                close_connection();
            }
        }
        /// <summary>
        /// Used when require to get single recordset
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <param name="parameters">Parameter as DynamicParamters Object, support output parameter.</param>
        /// <returns>List<T></returns>
        public List<T> execute_stored_procedure<T>(string stored_procedure_name, ref DynamicParameters parameters)
        {
            List<T> list_object = new List<T>();
            try
            {
                open_connection();
                list_object = sql_connection.Query<T>(stored_procedure_name, parameters, commandType: CommandType.StoredProcedure).ToList();
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}-paramteres:     {1}", stored_procedure_name, parameters.ToString());
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}-paramteres:     {1}", stored_procedure_name, parameters.ToString());
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }

            return list_object;
        }

        /// <summary>
        /// Used when require to get single recordset
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <returns>List<T></returns>
        public List<T> execute_stored_procedure<T>(string stored_procedure_name)
        {
            List<T> list_object = new List<T>();
            try
            {
                open_connection();
                if (dp.Length() > 0)
                {
                    list_object = sql_connection.Query<T>(stored_procedure_name, dp, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();
                }
                else
                {
                    list_object = sql_connection.Query<T>(stored_procedure_name).ToList();
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }

            return list_object;
        }

        /// <summary>
        /// Used when require to execute stored procedure
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <param name="parameters">Parameter as Comma Separated Values, sequence should be same as stored procedure parameter</param>
        /// <returns></returns>
        public void execute_stored_procedure(string stored_procedure_name, params object[] parameters)
        {
            try
            {
                _run_in_text_mode = true;
                this.execute_stored_procedure(get_command_text(stored_procedure_name, parameters));
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}-paramteres:     {1}", stored_procedure_name, parameters.ToString());
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}-paramteres:     {1}", stored_procedure_name, parameters.ToString());
                //LOGGER.error(error_message, ex);
                throw ex;
            }
        }

        /// <summary>
        /// Used when require to execute stored procedure
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <param name="parameters">Parameter as DynamicParamters Object, support output parameter.</param>
        /// <returns></returns>
        public void execute_stored_procedure(string stored_procedure_name, ref DynamicParameters parameters)
        {
            try
            {
                open_connection();
                sql_connection.Execute(stored_procedure_name, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}-paramteres:     {1}", stored_procedure_name, parameters.ToString());
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}-paramteres:     {1}", stored_procedure_name, parameters.ToString());
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }

        }

        /// <summary>
        /// Used when require to execute stored procedure
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <returns></returns>
        public void execute_stored_procedure(string stored_procedure_name)
        {
            try
            {
                open_connection();
                if (dp.Length() > 0)
                {
                    sql_connection.Execute(stored_procedure_name, dp, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    if (_run_in_text_mode)
                        sql_connection.Execute(stored_procedure_name, null, commandType: CommandType.Text);
                    else
                        sql_connection.Execute(stored_procedure_name, null, commandType: CommandType.StoredProcedure);
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }
        }

        /// <summary>
        /// Used when require to execute stored procedure
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <returns></returns>
        public async Task execute_stored_procedureAsync(string stored_procedure_name)
        {
            try
            {
                open_connection();
                if (dp.Length() > 0)
                {
                    await sql_connection.ExecuteAsync(stored_procedure_name, dp, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    if (_run_in_text_mode)
                        await sql_connection.ExecuteAsync(stored_procedure_name, null, commandType: CommandType.Text);
                    else
                        await sql_connection.ExecuteAsync(stored_procedure_name, null, commandType: CommandType.StoredProcedure);
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }
        }
        /// <summary>
        /// Used when require to execute SQL
        /// </summary>
        /// <param name="sql">SQL query</param>
        /// <returns></returns>
        public void execute(string sql)
        {
            try
            {
                open_connection();
                sql_connection.Execute(sql);
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sql:   {0}", sql);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sql:   {0}", sql);
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }
        }

        public DataTable Execute_SP(string storedProc, int RuleId, int EngagementId)
        {
            DataTable table = new DataTable();
            try
            {
                open_connection();
                //using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString))
                var cmd = new SqlCommand(storedProc, sql_connection);
                cmd.Parameters.Add("@RuleId", SqlDbType.Int).Value = RuleId;
                cmd.Parameters.Add("@EngagementId", SqlDbType.Int).Value = EngagementId;

                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    da.Fill(table);
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("SP:   {0}", storedProc);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("SP:   {0}", storedProc);
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }
            return table;

        }

        public DataTable Execute_SP(string storedProc, int EngagementId)
        {
            DataTable table = new DataTable();
            try
            {
                open_connection();
                //using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString))
                var cmd = new SqlCommand(storedProc, sql_connection);
                cmd.Parameters.Add("@EngagementId", SqlDbType.Int).Value = EngagementId;

                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    da.Fill(table);
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("SP:   {0}", storedProc);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("SP:   {0}", storedProc);
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }
            return table;

        }
        public DataTable Execute_SPRuleViolations(string storedProc, int RuleId, int ObservationId)
        {
            DataTable table = new DataTable();
            try
            {
                open_connection();
                //using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString))
                var cmd = new SqlCommand(storedProc, sql_connection);
                cmd.Parameters.Add("@RuleViolationMasterId", SqlDbType.Int).Value = RuleId;
                //cmd.Parameters.Add("@ObservationId", SqlDbType.Int).Value = ObservationId;

                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    da.Fill(table);
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("SP:   {0}", storedProc);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("SP:   {0}", storedProc);
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }
            return table;

        }

        public DataTable Execute_SPRuleViolationsByRuleViolationMasterId(string storedProc, int RuleViolationMasterId)
        {
            DataTable table = new DataTable();
            try
            {
                open_connection();
                //using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString))
                var cmd = new SqlCommand(storedProc, sql_connection);
                cmd.Parameters.Add("@RuleViolationMasterId", SqlDbType.Int).Value = RuleViolationMasterId;

                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    da.Fill(table);
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("SP:   {0}", storedProc);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("SP:   {0}", storedProc);
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }
            return table;

        }

        #region IDisposable Members

        // Allow your Dispose method to be called multiple times,
        // but throw an exception if the object has been disposed.
        // Whenever you do something with this class, 
        // check to see if it has been disposed.
        /// <summary>
        /// Verify the object has been disposed
        /// </summary>
        protected void CheckDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("SQLDataMapper");
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                close_connection();
                IsDisposed = true;
            }
        }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);

            // Suppress finalization of this disposed instance.
            GC.SuppressFinalize(this);
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method 
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~DAPPER_DATA_SERVICE()
        {
            Dispose(false);
        }

        #endregion

        //void IDisposable.Dispose()
        //{
        //    throw new NotImplementedException();
        //}/

        /*
         * How to use
         * 
         * var imports = new List<Product>();
           //Load up the imports
 
           //Pass in cnx, tablename, and list of imports
           SQL_BULK_COPY.BulkInsert(context.Database.Connection.ConnectionString, "Products", imports);
         * 
         */

        public void BulkInsert<T>(string ConnectionString, string tableName, IList<T> list)
        {
            using (var bulkCopy = new SqlBulkCopy(ConnectionString, SqlBulkCopyOptions.TableLock))
            {
                bulkCopy.BatchSize = list.Count;
                bulkCopy.DestinationTableName = tableName;

                var table = new DataTable();
                var props = TypeDescriptor.GetProperties(typeof(T))
                                           //Dirty hack to make sure we only have system data types 
                                           //i.e. filter out the relationships/collections
                                           .Cast<PropertyDescriptor>()
                                           .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                           .ToArray();

                foreach (var propertyInfo in props)
                {
                    bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                    table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                }

                var values = new object[props.Length];
                foreach (var item in list)
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);
                    }

                    table.Rows.Add(values);
                }

                bulkCopy.WriteToServer(table);
            }
        }

        /*
         * How to use
         * 
         * var imports = new List<Product>();
           //Load up the imports
 
           //Pass in cnx, tablename, and list of imports
           BulkInsert("Products", imports);
         * 
         */

        public void BulkInsert<T>(string tableName, IList<T> list)
        {
            try
            {
                using (SqlConnection srcConnection = new SqlConnection(this.connection_string))
                {
                    srcConnection.Open();
                    using (var bulkCopy = new SqlBulkCopy(srcConnection))
                    {
                        bulkCopy.BatchSize = list.Count;
                        bulkCopy.DestinationTableName = tableName;

                        var table = new DataTable();
                        var props = TypeDescriptor.GetProperties(typeof(T))
                                                   //Dirty hack to make sure we only have system data types 
                                                   //i.e. filter out the relationships/collections
                                                   .Cast<PropertyDescriptor>()
                                                   .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                                   .ToArray();

                        try
                        {
                            props = props.Where(p => p.Name != "Number" && p.Name != "ROWS").ToArray();
                            foreach (var propertyInfo in props)
                            {
                                bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                                table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                            }
                        }
                        catch (Exception ex)
                        {
                            //LOGGER.error(ex);
                            throw ex;
                        }

                        try
                        {
                            var values = new object[props.Length];
                            foreach (var item in list)
                            {
                                for (var i = 0; i < values.Length; i++)
                                {
                                    values[i] = props[i].GetValue(item);
                                }

                                table.Rows.Add(values);
                            }
                        }
                        catch (Exception ex)
                        {
                            //LOGGER.error(ex);
                            throw ex;
                        }

                        try
                        {
                            bulkCopy.WriteToServer(table);
                        }
                        catch (Exception ex)
                        {
                            //LOGGER.error(ex);
                            throw ex;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //LOGGER.error(ex);
                throw ex;
            }
        }

    }

    public class DAPPER_DATA_SERVICESMA : IDisposable
    {
        protected SqlConnection sql_connection;
        protected string connection_string;
        private bool IsDisposed = false;
        public DynamicParameters dp;
        protected bool _run_in_text_mode = false;

        public DAPPER_DATA_SERVICESMA()
        {
            connection_string = ConfigurationManager.ConnectionStrings["DefaultConnectionSMA"].ConnectionString;
            dp = new DynamicParameters();
        }
        public DAPPER_DATA_SERVICESMA(string connection_string_key_name)
        {
            connection_string = ConfigurationManager.ConnectionStrings[connection_string_key_name].ConnectionString;
            dp = new DynamicParameters();
        }


        public void AddIntegerParameter(string name, int value)
        {
            dp.Add(name, value, dbType: DbType.Int32, direction: ParameterDirection.Input);
        }

        public void AddDecimalParameter(string name, decimal value)
        {
            dp.Add(name, value, dbType: DbType.Decimal, direction: ParameterDirection.Input);
        }

        public void SetIntegerParameter(string name, int value)
        {
            dp.Add(name, value, dbType: DbType.Int32, direction: ParameterDirection.Input);
        }

        public void AddOutputIntegerParameter(string name)
        {
            dp.Add(name, default(Int32), dbType: DbType.Int32, direction: ParameterDirection.Output);
        }

        public void AddOutputDecimalParameter(string name)
        {
            dp.Add(name, default(Decimal), dbType: DbType.Decimal, direction: ParameterDirection.Output);
        }

        public int GetOutputIntegerParameter(string name)
        {
            return dp.Get<Int32>(name);
        }

        public Decimal GetOutputDecimalParameter(string name)
        {
            return dp.Get<Decimal>(name);
        }

        public void AddShortParameter(string name, short value)
        {
            dp.Add(name, value, dbType: DbType.Int16, direction: ParameterDirection.Input);
        }

        public void SetShortParameter(string name, short value)
        {
            dp.Add(name, value, dbType: DbType.Int16, direction: ParameterDirection.Input);
        }

        public void AddOutputShortParameter(string name)
        {
            dp.Add(name, default(Int16), dbType: DbType.Int16, direction: ParameterDirection.Output);
        }

        public short GetOutputShortParameter(string name)
        {
            return dp.Get<Int16>(name);
        }

        public void AddByteParameter(string name, byte value)
        {
            dp.Add(name, value, dbType: DbType.Byte, direction: ParameterDirection.Input);
        }

        public void SetByteParameter(string name, object value)
        {
            dp.Add(name, value, dbType: DbType.Byte, direction: ParameterDirection.Input);
        }

        public void AddOutputByteParameter(string name)
        {
            dp.Add(name, default(byte), dbType: DbType.Byte, direction: ParameterDirection.Output);
        }

        public byte GetOutputByteParameter(string name)
        {
            return dp.Get<byte>(name);
        }

        public void AddStringParameter(string name, string value, int size)
        {
            value = value == null ? "" : value;

            if (String.Format("{0}", value).Length > size)
            {
                Exception ex = new System.Exception(String.Format("'{0}' should be max. of {1} characters. You entered {2} characters.", name, size, value.Length));
                //LOGGER.error(String.Format("Name: {0}; Value: {1}", name, value), ex);
                throw ex;
            }

            dp.Add(name, value, dbType: DbType.AnsiString, direction: ParameterDirection.Input, size: size);
        }

        public void AddGUIDParameter(string name, Guid value)
        {
            dp.Add(name, value, dbType: DbType.Guid, direction: ParameterDirection.Input);
        }

        public void AddUnicodeStringParameter(string name, string value, int size)
        {
            value = value == null ? "" : value;

            if (String.Format("{0}", value).Length > size)
            {
                Exception ex = new System.Exception(String.Format("You are exceeded the maximum allowed length of {0}.", size));
                //LOGGER.error(String.Format("Name: {0}; Value: {1}", name, value), ex);
                throw ex;
            }

            dp.Add(name, value, dbType: DbType.String, direction: ParameterDirection.Input, size: size);
        }

        private int GetStringSize<T>(string property_name)
        {
            foreach (PropertyDescriptor propertyInfo in TypeDescriptor.GetProperties(typeof(T)))
            {
                if (propertyInfo.Name == property_name)
                {
                    StringLengthAttribute StringLength = propertyInfo.Attributes.OfType<StringLengthAttribute>().FirstOrDefault();
                    return StringLength.MaximumLength;
                }
            }

            return 8000;
        }

        public void AddStringParameter<T>(string name, string value)
        {
            value = value == null ? "" : value;

            int StringSize = GetStringSize<T>(name);
            if (String.Format("{0}", value).Length > StringSize)
            {
                Exception ex = new System.Exception(String.Format("You are exceeding the maximum allowed length of {0}.", StringSize));
                //LOGGER.error(String.Format("Name: {0}; Value: {1}", name, value), ex);
                throw ex;
            }

            dp.Add(name, value, dbType: DbType.AnsiString, direction: ParameterDirection.Input, size: StringSize);
        }

        public void AddUnicodeStringParameter<T>(string name, string value)
        {
            value = value == null ? "" : value;

            int StringSize = GetStringSize<T>(name);
            if (String.Format("{0}", value).Length > StringSize)
            {
                Exception ex = new System.Exception(String.Format("You are exceeded the maximum allowed length of {0}.", StringSize));
                //LOGGER.error(String.Format("Name: {0}; Value: {1}", name, value), ex);
                throw ex;
            }

            dp.Add(name, value, dbType: DbType.String, direction: ParameterDirection.Input, size: StringSize);
        }

        public void SetStringParameter(string name, string value, int size)
        {
            value = value == null ? "" : value;

            if (String.Format("{0}", value).Length > size)
            {
                throw new System.Exception(String.Format("You are exceeded the maximum allowed length of {0}.", size));
            }

            dp.Add(name, value, dbType: DbType.AnsiString, direction: ParameterDirection.Input, size: size);
        }

        public void AddOutputStringParameter(string name, int size)
        {
            dp.Add(name, default(string), dbType: DbType.AnsiString, direction: ParameterDirection.Output, size: size);
        }

        public string GetOutputStringParameter(string name)
        {
            return dp.Get<string>(name);
        }

        public void AddBooleanParameter(string name, bool? value)
        {
            dp.Add(name, value, dbType: DbType.Boolean, direction: ParameterDirection.Input);
        }

        public void SetBooleanParameter(string name, bool value)
        {
            dp.Add(name, value, dbType: DbType.Boolean, direction: ParameterDirection.Input);
        }

        public void GetBooleanParameter(string name, bool value)
        {
            dp.Add(name, value, dbType: DbType.Boolean, direction: ParameterDirection.Input);
        }

        public void AddOutputBooleanParameter(string name)
        {
            dp.Add(name, default(bool), dbType: DbType.Boolean, direction: ParameterDirection.Output);
        }

        public bool GetOutputBooleanParameter(string name)
        {
            return dp.Get<bool>(name);
        }

        public void AddDateTimeParameter(string name, DateTime? value)
        {
            dp.Add(name, value == null ? (object)DBNull.Value : value.Value, dbType: DbType.DateTime, direction: ParameterDirection.Input);
        }

        public void SetDateTimeParameter(string name, DateTime value)
        {
            dp.Add(name, value, dbType: DbType.DateTime, direction: ParameterDirection.Input);
        }

        public void GetDateTimeParameter(string name, DateTime value)
        {
            dp.Add(name, value, dbType: DbType.DateTime, direction: ParameterDirection.Input);
        }

        public void AddOutputDateTimeParameter(string name)
        {
            dp.Add(name, default(DateTime), dbType: DbType.DateTime, direction: ParameterDirection.Output);
        }

        public DateTime GetOutputDateTimeParameter(string name)
        {
            return dp.Get<DateTime>(name);
        }

        public void AddSmallDateTimeParameter(string name, DateTime value)
        {
            dp.Add(name, value, dbType: DbType.DateTime, direction: ParameterDirection.Input);
        }

        public void SetSmallDateTimeParameter(string name, DateTime value)
        {
            dp.Add(name, value, dbType: DbType.DateTime, direction: ParameterDirection.Input);
        }

        public void GetSmallDateTimeParameter(string name, DateTime value)
        {
            dp.Add(name, value, dbType: DbType.DateTime, direction: ParameterDirection.Input);
        }

        public void AddOutputSmallDateTimeParameter(string name, DateTime value)
        {
            dp.Add(name, default(DateTime), dbType: DbType.DateTime, direction: ParameterDirection.Output);
        }

        public DateTime GetOutputSmallDateTimeParameter(string name)
        {
            return dp.Get<DateTime>(name);
        }

        public void AddGuidParameter(string name, Guid value)
        {
            dp.Add(name, value, dbType: DbType.Guid, direction: ParameterDirection.Input);
        }

        public void AddStructuredTypeParameter(string name, string type_name, object value)
        {
            //dp.Add(name, value, DbType.I ParameterDirection.Input);

            //var type_parameter = new SqlParameter(name, SqlDbType.Structured);
            //type_parameter.Direction = ParameterDirection.Input;
            //type_parameter.Value = value;


        }

        private DbType get_db_type(string type_name)
        {
            if (String.Compare(type_name, "String", true) == 0)
            {
                return DbType.AnsiString;
            }
            else if (String.Compare(type_name, "DateTime", true) == 0)
            {
                return DbType.DateTime;
            }
            else if (String.Compare(type_name, "Boolean", true) == 0)
            {
                return DbType.Boolean;
            }
            else
            {
                return DbType.Int32;
            }
        }

        private void initialize()
        {
            sql_connection = new SqlConnection(connection_string);
        }

        internal void open_connection()
        {
            if (sql_connection == null) { sql_connection = new SqlConnection(connection_string); }
            else { sql_connection = new SqlConnection(connection_string); }

            if (sql_connection.State != ConnectionState.Open)
            {
                try
                {

                    sql_connection.Open();
                }
                catch (SqlException ex)
                {
                    switch (ex.Number)
                    {
                        case 2:
                            throw new SERVER_CONNECTION_EXCEPTION();
                        case 17:
                            throw new SERVER_NOT_FOUND_EXCEPTION();
                        case 18456:
                            throw new LOGIN_FAILED_EXCEPTION();
                        case 4060:
                            throw new DATABASE_NOT_FOUND_EXCEPTION();
                        default:
                            throw ex;

                    }
                }
            }
        }

        public void close_connection()
        {
            if ((sql_connection != null) && (sql_connection.State != ConnectionState.Closed))
            {
                sql_connection.Close();
            }

        }

        private string get_command_text(string stored_procedure_name, params object[] parameters)
        {
            StringBuilder command_text = new StringBuilder();
            try
            {
                command_text.Append("exec ");
                command_text.Append(stored_procedure_name);
                command_text.Append(" ");

                bool first_param = true;
                foreach (object p in parameters)
                {
                    if (first_param)
                        first_param = false;
                    else
                        command_text.Append(", ");

                    if (p.GetType().Name == "String")
                    {
                        command_text.Append(String.Format("'{0}'", p));
                    }
                    else if (p.GetType().Name == "DateTime")
                    {
                        command_text.Append(String.Format("'{0}'", Convert.ToDateTime(p).ToString("MM/dd/yyyy")));
                    }
                    else if (p.GetType().Name == "Bool")
                    {
                        if (Convert.ToBoolean(p))
                        {
                            command_text.Append("1");
                        }
                        else
                        {
                            command_text.Append("0");
                        }
                    }
                    else
                    {
                        command_text.Append(String.Format("{0}", p));
                    }
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("{0}-{1}", stored_procedure_name, command_text.ToString());
                //LOGGER.error(error_message, ex);
                throw ex;
            }

            return command_text.ToString();
        }

        /// <summary>
        /// Used when require to get multiple recordset
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <param name="parameters">Parameter as Comma Separated Values, sequence should be same as stored procedure parameter</param>
        /// <returns>SqlMapper.GridReader</returns>
        public SqlMapper.GridReader query_multiple(string stored_procedure_name, params object[] parameters)
        {
            return this.query_multiple(get_command_text(stored_procedure_name, parameters));
        }

        /// <summary>
        /// Used when require to get multiple recordset
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <param name="parameters">Parameter as DynamicParamters Object, support output parameter.</param>
        /// <returns>SqlMapper.GridReader</returns>
        public SqlMapper.GridReader query_multiple(string stored_procedure_name, ref DynamicParameters parameters)
        {
            SqlMapper.GridReader value = null;
            try
            {
                open_connection();
                value = sql_connection.QueryMultiple(stored_procedure_name, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}-paramteres:     {1}", stored_procedure_name, parameters.ToString());
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}-paramteres:     {1}", stored_procedure_name, parameters.ToString());
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }

            return value;
        }

        /// <summary>
        /// Used when require to get multiple recordset without parameter
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <returns>SqlMapper.GridReader</returns>
        public SqlMapper.GridReader query_multiple(string stored_procedure_name)
        {
            SqlMapper.GridReader value = null;
            try
            {
                open_connection();
                if (dp.Length() > 0)
                {
                    value = sql_connection.QueryMultiple(stored_procedure_name, dp, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    value = sql_connection.QueryMultiple(stored_procedure_name);
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                throw ex;
            }

            return value;
        }

        /// <summary>
        /// Used when require to get multiple recordset without parameter
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <returns>SqlMapper.GridReader</returns>
        public async Task<SqlMapper.GridReader> query_multipleAsync(string stored_procedure_name)
        {
            SqlMapper.GridReader value = null;
            try
            {
                open_connection();
                if (dp.Length() > 0)
                {
                    value = await sql_connection.QueryMultipleAsync(stored_procedure_name, dp, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    value = await sql_connection.QueryMultipleAsync(stored_procedure_name);
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                throw ex;
            }

            return value;
        }

        /// <summary>
        /// Used when require to get nested recordset like profile with his address
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <param name="parameters">Parameter as Comma Separated Values, sequence should be same as stored procedure parameter</param>
        /// <returns>IEnumerable<Tuple<A, B>></returns>
        public IEnumerable<Tuple<A, B>> query_nested_map<A, B>(string stored_procedure_name, params object[] parameters)
        {
            if (parameters.Length > 0)
            {
                return this.query_nested_map<A, B>(get_command_text(stored_procedure_name, parameters));
            }
            else
            {
                return this.query_nested_map<A, B>(stored_procedure_name);
            }
        }

        /// <summary>
        /// Used when require to get nested recordset like profile with his address
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <param name="parameters">Parameter as DynamicParamters Object, support output parameter.</param>
        /// <returns>IEnumerable<Tuple<A, B>></returns>
        public IEnumerable<Tuple<A, B>> query_nested_map<A, B>(string stored_procedure_name, ref DynamicParameters parameters, string split_on = "id")
        {
            IEnumerable<Tuple<A, B>> value = null;
            try
            {
                open_connection();
                value = sql_connection.Query<A, B, Tuple<A, B>>(stored_procedure_name, (a, b) => Tuple.Create(a, b), parameters, commandType: CommandType.StoredProcedure, splitOn: split_on);
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}-paramteres:     {1}", stored_procedure_name, parameters.ToString());
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}-paramteres:     {1}", stored_procedure_name, parameters.ToString());
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }


            return value;
        }

        /// <summary>
        /// Used when require to get nested recordset without parameter like profile with his address
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <returns>IEnumerable<Tuple<A, B>></returns>
        public IEnumerable<Tuple<A, B>> query_nested_map<A, B>(string stored_procedure_name, string split_on = "id")
        {
            IEnumerable<Tuple<A, B>> value = null;
            try
            {
                open_connection();
                if (dp.Length() > 0)
                {
                    value = sql_connection.Query<A, B, Tuple<A, B>>(stored_procedure_name, (a, b) => Tuple.Create(a, b), dp, commandType: CommandType.StoredProcedure, splitOn: split_on);
                }
                else
                {
                    value = sql_connection.Query<A, B, Tuple<A, B>>(stored_procedure_name, (a, b) => Tuple.Create(a, b), splitOn: split_on);
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }


            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="stored_procedure_name"></param>
        /// <param name="split_on"></param>
        /// <returns></returns>
        public IEnumerable<Tuple<A, B, C>> query_nested_map<A, B, C>(string stored_procedure_name, string split_on = "id")
        {
            IEnumerable<Tuple<A, B, C>> value = null;
            try
            {
                open_connection();
                if (dp.Length() > 0)
                {
                    value = sql_connection.Query<A, B, C, Tuple<A, B, C>>(stored_procedure_name, (a, b, c) => Tuple.Create(a, b, c), dp, commandType: CommandType.StoredProcedure, splitOn: split_on);
                }
                else
                {
                    value = sql_connection.Query<A, B, C, Tuple<A, B, C>>(stored_procedure_name, (a, b, c) => Tuple.Create(a, b, c), splitOn: split_on);
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }


            return value;
        }

        /// <summary>
        /// Used when require to get single recordset
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <param name="parameters">Parameter as Comma Separated Values, sequence should be same as stored procedure parameter</param>
        /// <returns>List<T></returns>
        public List<T> execute_stored_procedure<T>(string stored_procedure_name, params object[] parameters)
        {
            return this.execute_stored_procedure<T>(get_command_text(stored_procedure_name, parameters));
        }


        public IEnumerable<T> ExecuteQuery<T>(string sql, object parameters = null)
        {
            try
            {
                open_connection();
                using (var connection = sql_connection)
                {

                    //connection.Open();
                    return connection.Query<T>(sql, parameters).ToList();
                }
            }
            finally
            {
                close_connection();
            }
        }
        public int ExecuteQueryint(string sql, object parameters = null)
        {
            try
            {
                open_connection();
                using (var connection = sql_connection)
                {

                    //connection.Open();
                    return connection.Query<int>(sql, parameters).FirstOrDefault();
                }
            }
            finally
            {
                close_connection();
            }
        }
        /// <summary>
        /// Used when require to get single recordset
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <param name="parameters">Parameter as DynamicParamters Object, support output parameter.</param>
        /// <returns>List<T></returns>
        public List<T> execute_stored_procedure<T>(string stored_procedure_name, ref DynamicParameters parameters)
        {
            List<T> list_object = new List<T>();
            try
            {
                open_connection();
                list_object = sql_connection.Query<T>(stored_procedure_name, parameters, commandType: CommandType.StoredProcedure).ToList();
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}-paramteres:     {1}", stored_procedure_name, parameters.ToString());
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}-paramteres:     {1}", stored_procedure_name, parameters.ToString());
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }

            return list_object;
        }

        /// <summary>
        /// Used when require to get single recordset
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <returns>List<T></returns>
        public List<T> execute_stored_procedure<T>(string stored_procedure_name)
        {
            List<T> list_object = new List<T>();
            try
            {
                open_connection();
                if (dp.Length() > 0)
                {
                    list_object = sql_connection.Query<T>(stored_procedure_name, dp, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();
                }
                else
                {
                    list_object = sql_connection.Query<T>(stored_procedure_name).ToList();
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }

            return list_object;
        }

        /// <summary>
        /// Used when require to execute stored procedure
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <param name="parameters">Parameter as Comma Separated Values, sequence should be same as stored procedure parameter</param>
        /// <returns></returns>
        public void execute_stored_procedure(string stored_procedure_name, params object[] parameters)
        {
            try
            {
                _run_in_text_mode = true;
                this.execute_stored_procedure(get_command_text(stored_procedure_name, parameters));
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}-paramteres:     {1}", stored_procedure_name, parameters.ToString());
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}-paramteres:     {1}", stored_procedure_name, parameters.ToString());
                //LOGGER.error(error_message, ex);
                throw ex;
            }
        }

        /// <summary>
        /// Used when require to execute stored procedure
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <param name="parameters">Parameter as DynamicParamters Object, support output parameter.</param>
        /// <returns></returns>
        public void execute_stored_procedure(string stored_procedure_name, ref DynamicParameters parameters)
        {
            try
            {
                open_connection();
                sql_connection.Execute(stored_procedure_name, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}-paramteres:     {1}", stored_procedure_name, parameters.ToString());
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}-paramteres:     {1}", stored_procedure_name, parameters.ToString());
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }

        }

        /// <summary>
        /// Used when require to execute stored procedure
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <returns></returns>
        public void execute_stored_procedure(string stored_procedure_name)
        {
            try
            {
                open_connection();
                if (dp.Length() > 0)
                {
                    sql_connection.Execute(stored_procedure_name, dp, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    if (_run_in_text_mode)
                        sql_connection.Execute(stored_procedure_name, null, commandType: CommandType.Text);
                    else
                        sql_connection.Execute(stored_procedure_name, null, commandType: CommandType.StoredProcedure);
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }
        }

        /// <summary>
        /// Used when require to execute stored procedure
        /// </summary>
        /// <param name="stored_procedure_name">Stored Procedure Name</param>
        /// <returns></returns>
        public async Task execute_stored_procedureAsync(string stored_procedure_name)
        {
            try
            {
                open_connection();
                if (dp.Length() > 0)
                {
                    await sql_connection.ExecuteAsync(stored_procedure_name, dp, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    if (_run_in_text_mode)
                        await sql_connection.ExecuteAsync(stored_procedure_name, null, commandType: CommandType.Text);
                    else
                        await sql_connection.ExecuteAsync(stored_procedure_name, null, commandType: CommandType.StoredProcedure);
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sp:   {0}", stored_procedure_name);
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }
        }
        /// <summary>
        /// Used when require to execute SQL
        /// </summary>
        /// <param name="sql">SQL query</param>
        /// <returns></returns>
        public void execute(string sql)
        {
            try
            {
                open_connection();
                sql_connection.Execute(sql);
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("sql:   {0}", sql);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("sql:   {0}", sql);
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }
        }

        public DataTable Execute_SP(string storedProc, int RuleId, int EngagementId)
        {
            DataTable table = new DataTable();
            try
            {
                open_connection();
                //using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString))
                var cmd = new SqlCommand(storedProc, sql_connection);
                cmd.Parameters.Add("@RuleId", SqlDbType.Int).Value = RuleId;
                cmd.Parameters.Add("@EngagementId", SqlDbType.Int).Value = EngagementId;

                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    da.Fill(table);
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("SP:   {0}", storedProc);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("SP:   {0}", storedProc);
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }
            return table;

        }

        public DataTable Execute_SP(string storedProc, int EngagementId)
        {
            DataTable table = new DataTable();
            try
            {
                open_connection();
                //using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString))
                var cmd = new SqlCommand(storedProc, sql_connection);
                cmd.Parameters.Add("@EngagementId", SqlDbType.Int).Value = EngagementId;

                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    da.Fill(table);
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("SP:   {0}", storedProc);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("SP:   {0}", storedProc);
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }
            return table;

        }
        public DataTable Execute_SPRuleViolations(string storedProc, int RuleId, int ObservationId)
        {
            DataTable table = new DataTable();
            try
            {
                open_connection();
                //using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString))
                var cmd = new SqlCommand(storedProc, sql_connection);
                cmd.Parameters.Add("@RuleViolationMasterId", SqlDbType.Int).Value = RuleId;
                //cmd.Parameters.Add("@ObservationId", SqlDbType.Int).Value = ObservationId;

                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    da.Fill(table);
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("SP:   {0}", storedProc);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("SP:   {0}", storedProc);
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }
            return table;

        }

        public DataTable Execute_SPRuleViolationsByRuleViolationMasterId(string storedProc, int RuleViolationMasterId)
        {
            DataTable table = new DataTable();
            try
            {
                open_connection();
                //using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString))
                var cmd = new SqlCommand(storedProc, sql_connection);
                cmd.Parameters.Add("@RuleViolationMasterId", SqlDbType.Int).Value = RuleViolationMasterId;

                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    da.Fill(table);
                }
            }
            catch (SqlException ex)
            {
                string error_message = String.Format("SP:   {0}", storedProc);
                //LOGGER.error(error_message, ex);
                switch (ex.Number)
                {
                    case 2627:
                        throw new UNIQUE_KEY_CONSTRAINT_EXCEPTION(ex.Message);
                    default:
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                string error_message = String.Format("SP:   {0}", storedProc);
                //LOGGER.error(error_message, ex);
                throw ex;
            }
            finally
            {
                close_connection();
            }
            return table;

        }

        #region IDisposable Members

        // Allow your Dispose method to be called multiple times,
        // but throw an exception if the object has been disposed.
        // Whenever you do something with this class, 
        // check to see if it has been disposed.
        /// <summary>
        /// Verify the object has been disposed
        /// </summary>
        protected void CheckDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("SQLDataMapper");
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                close_connection();
                IsDisposed = true;
            }
        }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);

            // Suppress finalization of this disposed instance.
            GC.SuppressFinalize(this);
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method 
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~DAPPER_DATA_SERVICESMA()
        {
            Dispose(false);
        }

        #endregion

        //void IDisposable.Dispose()
        //{
        //    throw new NotImplementedException();
        //}/

        /*
         * How to use
         * 
         * var imports = new List<Product>();
           //Load up the imports
 
           //Pass in cnx, tablename, and list of imports
           SQL_BULK_COPY.BulkInsert(context.Database.Connection.ConnectionString, "Products", imports);
         * 
         */

        public void BulkInsert<T>(string ConnectionString, string tableName, IList<T> list)
        {
            using (var bulkCopy = new SqlBulkCopy(ConnectionString, SqlBulkCopyOptions.TableLock))
            {
                bulkCopy.BatchSize = list.Count;
                bulkCopy.DestinationTableName = tableName;

                var table = new DataTable();
                var props = TypeDescriptor.GetProperties(typeof(T))
                                           //Dirty hack to make sure we only have system data types 
                                           //i.e. filter out the relationships/collections
                                           .Cast<PropertyDescriptor>()
                                           .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                           .ToArray();

                foreach (var propertyInfo in props)
                {
                    bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                    table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                }

                var values = new object[props.Length];
                foreach (var item in list)
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);
                    }

                    table.Rows.Add(values);
                }

                bulkCopy.WriteToServer(table);
            }
        }

        /*
         * How to use
         * 
         * var imports = new List<Product>();
           //Load up the imports
 
           //Pass in cnx, tablename, and list of imports
           BulkInsert("Products", imports);
         * 
         */

        public void BulkInsert<T>(string tableName, IList<T> list)
        {
            try
            {
                using (SqlConnection srcConnection = new SqlConnection(this.connection_string))
                {
                    srcConnection.Open();
                    using (var bulkCopy = new SqlBulkCopy(srcConnection))
                    {
                        bulkCopy.BatchSize = list.Count;
                        bulkCopy.DestinationTableName = tableName;

                        var table = new DataTable();
                        var props = TypeDescriptor.GetProperties(typeof(T))
                                                   //Dirty hack to make sure we only have system data types 
                                                   //i.e. filter out the relationships/collections
                                                   .Cast<PropertyDescriptor>()
                                                   .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                                   .ToArray();

                        try
                        {
                            props = props.Where(p => p.Name != "Number" && p.Name != "ROWS").ToArray();
                            foreach (var propertyInfo in props)
                            {
                                bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                                table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                            }
                        }
                        catch (Exception ex)
                        {
                            //LOGGER.error(ex);
                            throw ex;
                        }

                        try
                        {
                            var values = new object[props.Length];
                            foreach (var item in list)
                            {
                                for (var i = 0; i < values.Length; i++)
                                {
                                    values[i] = props[i].GetValue(item);
                                }

                                table.Rows.Add(values);
                            }
                        }
                        catch (Exception ex)
                        {
                            //LOGGER.error(ex);
                            throw ex;
                        }

                        try
                        {
                            bulkCopy.WriteToServer(table);
                        }
                        catch (Exception ex)
                        {
                            //LOGGER.error(ex);
                            throw ex;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //LOGGER.error(ex);
                throw ex;
            }
        }

    }



    public static class EXTENSION_METHODS
    {
        public static bool hasRows(this SqlMapper.GridReader grid_reader)
        {
            return !grid_reader.IsConsumed;
        }

        public static int Length(this DynamicParameters dp)
        {
            return dp.ParameterNames.Count();
        }

        public static DataTable ToDataTable<T>(this List<T> iList)
        {
            DataTable dataTable = new DataTable();
            PropertyDescriptorCollection propertyDescriptorCollection =
                TypeDescriptor.GetProperties(typeof(T));
            for (int i = 0; i < propertyDescriptorCollection.Count; i++)
            {
                PropertyDescriptor propertyDescriptor = propertyDescriptorCollection[i];
                Type type = propertyDescriptor.PropertyType;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    type = Nullable.GetUnderlyingType(type);


                dataTable.Columns.Add(propertyDescriptor.Name, type);
            }
            object[] values = new object[propertyDescriptorCollection.Count];
            foreach (T iListItem in iList)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = propertyDescriptorCollection[i].GetValue(iListItem);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
    }
}
