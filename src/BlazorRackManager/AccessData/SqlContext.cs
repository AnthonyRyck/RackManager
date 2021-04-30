using AccessData.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessData
{
	public class SqlContext
	{
		public string ConnectionString { get; set; }

		public SqlContext(string connectionString)
		{
			ConnectionString = connectionString;
		}

		#region Clients

        /// <summary>
        /// Charge tous les clients
        /// </summary>
        /// <returns></returns>
		public async Task<IEnumerable<Client>> LoadClients()
		{
            var commandText = @"SELECT IdClient, NomClient FROM Clients;";

            Func<MySqlCommand, Task<List<Client>>> funcCmd = async (cmd) =>
            {
                List<Client> clients = new List<Client>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var client = new Client()
                        {
                            IdClient = reader.GetInt32(0),
                            NomClient = reader.GetString(1)
                        };

                        clients.Add(client);
                    }
                }

                return clients;
            };

            List<Client> clients = new List<Client>();

            try
            {
                clients = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception ex)
            {
                throw;
            }

            return clients;
		}

        /// <summary>
		/// Ajout d'un nouveau client
		/// </summary>
		/// <param name="nomClient"></param>
		/// <returns></returns>
		public async Task<int> AddClient(string nomClient)
        {
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    string command = "INSERT INTO Clients (NomClient)"
                                    + " VALUES(@client);";

                    using (var cmd = new MySqlCommand(command, conn))
                    {
                        cmd.Parameters.AddWithValue("@client", nomClient);

                        conn.Open();
                        int result = await cmd.ExecuteNonQueryAsync();
                        conn.Close();
                    }
                }

                string commandId = " SELECT LAST_INSERT_ID();";
                int idClient = await GetIntCore(commandId);

                return idClient;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        /// <summary>
        /// Ajout une session en base de donné.
        /// </summary>
        /// <param name="idFormation"></param>
        /// <param name="idFormateur"></param>
        /// <param name="idSalle"></param>
        /// <param name="dateFormation"></param>
        /// <param name="nbrePlace"></param>
        /// <returns></returns>
        public async Task AddToRack(Rack nouveauRack)
        {
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    string command = "INSERT INTO Rack (Gisement, Position, NomClient, Commande, DateEntre)"
                                    + " VALUES(@gisement, @position, @client, @commande, @date);";


                    using (var cmd = new MySqlCommand(command, conn))
                    {
                        cmd.Parameters.AddWithValue("@gisement", nouveauRack.NomDuRack);
                        cmd.Parameters.AddWithValue("@position", nouveauRack.Position);
                        cmd.Parameters.AddWithValue("@client", nouveauRack.NomClient);
                        cmd.Parameters.AddWithValue("@commande", nouveauRack.NumeroCommande);
                        cmd.Parameters.AddWithValue("@date", nouveauRack.Date);

                        conn.Open();
                        int result = await cmd.ExecuteNonQueryAsync();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Récupère les formations par rapport à un titre de formation.
        /// </summary>
        /// <param name="nomFormation"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Rack>> GetRackByCommande(int numeroCommande)
        {
            var commandText = @"SELECT Gisement, Position, NomClient, Commande, DateEntre"
                            + $" WHERE Commande LIKE '%{numeroCommande}%';";

            Func<MySqlCommand, Task<List<Rack>>> funcCmd = async (cmd) =>
            {
                List<Rack> racks = new List<Rack>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var rack = new Rack()
                        {
                            NomDuRack = reader.GetString(0),
                            Position = reader.GetString(1),
                            NomClient = reader.GetString(2),
                            NumeroCommande = reader.GetInt32(3),
                            Date = reader.GetDateTime(4)
                        };

                        racks.Add(rack);
                    }
                }

                return racks;
            };

            List<Rack> racks = new List<Rack>();

            try
            {
                racks = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception ex)
            {
                var exs = ex.Message;
            }

            return racks;
        }

        /// <summary>
        /// Supprime la formation du catalogue.
        /// </summary>
        /// <param name="currentFormation"></param>
        /// <returns></returns>
        public async Task DeleteByCommande(int numeroCommande)
        {
            string commandDelete = $"DELETE FROM rack WHERE Commande={numeroCommande};";
            await ExecuteCoreAsync(commandDelete);
        }

        #region Private Methods

        private async Task<List<T>> GetCoreAsync<T>(string commandSql, Func<MySqlCommand, Task<List<T>>> func)
            where T : new()
        {
            List<T> result = new List<T>();

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    MySqlCommand cmd = new MySqlCommand(commandSql, conn);
                    conn.Open();
                    result = await func.Invoke(cmd);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandSql"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private async Task<T> GetCoreAsync<T>(string commandSql, Func<MySqlCommand, Task<T>> func)
            where T : new()
        {
            T result = new T();

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    MySqlCommand cmd = new MySqlCommand(commandSql, conn);
                    conn.Open();
                    result = await func.Invoke(cmd);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// Execute une commande qui n'attend pas de retour.
        /// </summary>
        /// <param name="commandSql"></param>
        /// <returns></returns>
        private async Task ExecuteCoreAsync(string commandSql)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(commandSql, conn);

                conn.Open();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Permet la récupération d'un BLOB uniquement !
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        private async Task<byte[]> GetBytesCore(string commandText)
        {
            byte[] file = null;

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(commandText, conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            file = (byte[])reader[0];

                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return file;
        }

        /// <summary>
        /// Permet la récupération d'un ID type int uniquement !
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        private async Task<int> GetIntCore(string commandText)
        {
            int id = 0;

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(commandText, conn);

                    UInt64 idTemp = 0;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            idTemp = (UInt64)reader[0];
                        }
                    }

                    id = Convert.ToInt32(idTemp);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return id;
        }

        /// <summary>
        /// Permet de gérer les retours de valeur null de la BDD
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static T ConvertFromDBVal<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default(T);
            }
            else
            {
                return (T)obj;
            }
        }

        #endregion
    }
}
