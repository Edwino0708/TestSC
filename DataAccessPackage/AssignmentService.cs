using Domain;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace DataAccessPackage
{
    public class AssignmentService : IAssignmentService
    {
        private readonly string _connectionString;

        public AssignmentService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int CreateAssignment(Assignment assignment)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    using (OracleCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "AssignmentPkg.CreateAssignment";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 120; // Aumentar el tiempo de espera del comando

                        cmd.Parameters.Add("p_Title", OracleDbType.Varchar2).Value = assignment.Title;
                        cmd.Parameters.Add("p_Description", OracleDbType.Varchar2).Value = assignment.Description;
                        cmd.Parameters.Add("p_DueDate", OracleDbType.Date).Value = assignment.DueDate;
                        cmd.Parameters.Add("p_Status", OracleDbType.Varchar2).Value = assignment.Status;

                        // Agregar el parámetro de salida para capturar el ID generado
                        OracleParameter outputIdParam = new OracleParameter("p_AssignmentID", OracleDbType.Decimal)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputIdParam);

                        cmd.ExecuteNonQuery();

                        // Obtener el valor del ID generado y convertirlo a int
                        decimal assignmentIdDecimal = ((Oracle.ManagedDataAccess.Types.OracleDecimal)outputIdParam.Value).Value;
                        int assignmentId = Convert.ToInt32(assignmentIdDecimal);
                        return assignmentId;
                    }
                }
            }
            catch (OracleException ex)
            {
                // Manejar la excepción adecuadamente
                Console.WriteLine("Oracle error code: " + ex.Number);
                Console.WriteLine("Oracle error message: " + ex.Message);
                // Aquí puedes manejar errores específicos según el código de error
                throw;
            }
            catch (Exception ex)
            {
                // Manejar otras excepciones genéricas
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }


        public List<Assignment> ReadAllAssignments()
        {
            var assignments = new List<Assignment>();

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    using (OracleCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "AssignmentPkg.ReadAllAssignments";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 120; // Aumentar el tiempo de espera del comando

                        cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                assignments.Add(new Assignment
                                {
                                    Id = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    CreationDate = reader.GetDateTime(3),
                                    DueDate = reader.GetDateTime(4),
                                    Status = reader.GetString(5)
                                });
                            }
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                // Manejar la excepción adecuadamente
                Console.WriteLine("Oracle error code: " + ex.Number);
                Console.WriteLine("Oracle error message: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                // Manejar otras excepciones genéricas
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }

            return assignments;
        }

        public Assignment ReadAssignment(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    using (OracleCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "AssignmentPkg.ReadAssignment";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 120; // Aumentar el tiempo de espera del comando

                        cmd.Parameters.Add("p_Id", OracleDbType.Int32).Value = id;
                        cmd.Parameters.Add("p_Title", OracleDbType.Varchar2, 255).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("p_Description", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("p_CreationDate", OracleDbType.Date).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("p_DueDate", OracleDbType.Date).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("p_Status", OracleDbType.Varchar2, 50).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var creationDate = (Oracle.ManagedDataAccess.Types.OracleDate)cmd.Parameters["p_CreationDate"].Value;
                        var dueDate = (Oracle.ManagedDataAccess.Types.OracleDate)cmd.Parameters["p_DueDate"].Value;

                        return new Assignment
                        {
                            Id = id,
                            Title = cmd.Parameters["p_Title"].Value.ToString(),
                            Description = cmd.Parameters["p_Description"].Value.ToString(),
                            CreationDate = creationDate.Value,
                            DueDate = dueDate.Value,
                            Status = cmd.Parameters["p_Status"].Value.ToString()
                        };
                    }
                }
            }
            catch (OracleException ex)
            {
                // Manejar la excepción adecuadamente
                Console.WriteLine("Oracle error code: " + ex.Number);
                Console.WriteLine("Oracle error message: " + ex.Message);
                throw;
            }
        }


        public void UpdateAssignment(int id, Assignment assignment)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    using (OracleCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "AssignmentPkg.UpdateAssignment";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 120; // Aumentar el tiempo de espera del comando

                        cmd.Parameters.Add("p_Id", OracleDbType.Int32).Value = id;
                        cmd.Parameters.Add("p_Title", OracleDbType.Varchar2).Value = assignment.Title;
                        cmd.Parameters.Add("p_Description", OracleDbType.Varchar2).Value = assignment.Description;
                        cmd.Parameters.Add("p_DueDate", OracleDbType.Date).Value = assignment.DueDate;
                        cmd.Parameters.Add("p_Status", OracleDbType.Varchar2).Value = assignment.Status;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (OracleException ex)
            {
                // Manejar la excepción adecuadamente
                Console.WriteLine("Oracle error code: " + ex.Number);
                Console.WriteLine("Oracle error message: " + ex.Message);
                // Aquí puedes manejar errores específicos según el código de error
                throw;
            }
            catch (Exception ex)
            {
                // Manejar otras excepciones genéricas
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }


        public void DeleteAssignment(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    using (OracleCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "AssignmentPkg.DeleteAssignment";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 120; // Aumentar el tiempo de espera del comando

                        cmd.Parameters.Add("p_Id", OracleDbType.Int32).Value = id;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (OracleException ex)
            {
                // Manejar la excepción adecuadamente
                Console.WriteLine("Oracle error code: " + ex.Number);
                Console.WriteLine("Oracle error message: " + ex.Message);
                // Aquí puedes manejar errores específicos según el código de error
                throw;
            }
            catch (Exception ex)
            {
                // Manejar otras excepciones genéricas
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }

    }
}