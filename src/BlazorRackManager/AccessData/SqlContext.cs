using AccessData.Models;
using AccessData.Views;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
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

		#region Commandes

		/// <summary>
		/// Ajoute une commande
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public async Task AddCommande(SuiviCommande commande)
        {
			try
			{
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    string command = "INSERT IGNORE INTO SuiviCommande (IdCommande, ClientId, DescriptionCmd)"
                                    + " VALUES(@idcmd, @idclient, @descriptionCmd);";

                    using (var cmd = new MySqlCommand(command, conn))
                    {
                        cmd.Parameters.AddWithValue("@idcmd", commande.IdCommande);
                        cmd.Parameters.AddWithValue("@idclient", commande.ClientId);
                        cmd.Parameters.AddWithValue("@descriptionCmd", commande.DescriptionCmd);

                        conn.Open();
                        int result = await cmd.ExecuteNonQueryAsync();
                        conn.Close();
                    }
                }
            }
			catch (Exception)
			{
				throw;
			}
        }

        #endregion

        #region Rack

        /// <summary>
        /// Charge tous les racks
        /// </summary>
        /// <returns></returns>
        public async Task<List<Rack>> LoadRacks()
        {
            var commandText = @"SELECT IdRack, Gisement, PosRack FROM Rack;";

            Func<MySqlCommand, Task<List<Rack>>> funcCmd = async (cmd) =>
            {
                List<Rack> racks = new List<Rack>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var rack = new Rack()
                        {
                            IdRack = reader.GetInt32(0),
                            Gisement = reader.GetString(1),
                            PosRack = reader.GetString(2)
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
                throw;
            }

            return racks;
        }

        /// <summary>
        /// Ajoute un nouveau Rack
        /// </summary>
        /// <param name="gisement"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public async Task<int> AddRack(string gisement, string position)
        {
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    string command = "INSERT INTO Rack (Gisement, PosRack)"
                                    + " VALUES(@gisement, @position);";

                    using (var cmd = new MySqlCommand(command, conn))
                    {
                        cmd.Parameters.AddWithValue("@gisement", gisement);
                        cmd.Parameters.AddWithValue("@position", position);

                        conn.Open();
                        int result = await cmd.ExecuteNonQueryAsync();
                        conn.Close();
                    }
                }

                string commandId = " SELECT LAST_INSERT_ID();";
                int idRack = await GetIntCore(commandId);

                return idRack;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Rack>> GetRackEmpty()
        {
            var commandText = @"SELECT rac.IdRack, rac.Gisement, rac.PosRack"
                                + " FROM rack rac"
                                + " LEFT OUTER JOIN geocommande geo"
	                            + " ON rac.IdRack = geo.RackId"
                                + " WHERE geo.RackId IS NULL;";

            Func<MySqlCommand, Task<List<Rack>>> funcCmd = async (cmd) =>
            {
                List<Rack> racks = new List<Rack>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var rack = new Rack()
                        {
                            IdRack = reader.GetInt32(0),
                            Gisement = reader.GetString(1),
                            PosRack = reader.GetString(2)
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
                throw;
            }

            return racks;
        }

        #endregion

        #region Hangar

        /// <summary>
        /// Récupère une entrée du hangar
        /// </summary>
        /// <param name="commandeId"></param>
        /// <param name="rackId"></param>
        /// <returns></returns>
        public async Task<HangarView> GetHangar(int commandeId, int rackId)
        {
            var commandText = @"SELECT cli.IdClient, cli.NomClient, rac.IdRack, rac.Gisement, rac.PosRack, cmd.IdCommande, cmd.DescriptionCmd, geo.DateEntre"
                                + " FROM geocommande geo"
                                + " INNER JOIN suivicommande cmd ON cmd.IdCommande = geo.CommandeId"
                                + " INNER JOIN clients cli ON cli.IdClient = cmd.ClientId"
                                + " INNER JOIN rack rac ON rac.IdRack = geo.RackId"
                                + $" WHERE geo.CommandeId = {commandeId} AND geo.RackId = {rackId};";

            Func<MySqlCommand, Task<HangarView>> funcCmd = async (cmd) =>
            {
                HangarView hangar = new HangarView();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        hangar = new HangarView()
                        {
                            IdClient = reader.GetInt32(0),
                            NomClient = reader.GetString(1),
                            IdRack = reader.GetInt32(2),
                            Gisement = reader.GetString(3),
                            PosRack = reader.GetString(4),
                            IdCommande = reader.GetInt32(5),
                            DescriptionCmd = ConvertFromDBVal<string?>(reader.GetValue(6)),
                            DateEntree = reader.GetDateTime(7)
                        };
                    }
                }

                return hangar;
            };

            HangarView hangar = new HangarView();

            try
            {
                hangar = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception ex)
            {
                var exs = ex.Message;
            }

            return hangar;
        }

        /// <summary>
        /// Récupère les formations par rapport à un titre de formation.
        /// </summary>
        /// <param name="nomFormation"></param>
        /// <returns></returns>
        public async Task<List<HangarView>> GetHangar()
        {
            var commandText = @"SELECT cli.IdClient, cli.NomClient, rac.IdRack, rac.Gisement, rac.PosRack, cmd.IdCommande, cmd.DescriptionCmd, geo.DateEntre"
                                + " FROM geocommande geo"
                                + " INNER JOIN suivicommande cmd ON cmd.IdCommande = geo.CommandeId"
                                + " INNER JOIN clients cli ON cli.IdClient = cmd.ClientId"
                                + " INNER JOIN rack rac ON rac.IdRack = geo.RackId;";

            Func<MySqlCommand, Task<List<HangarView>>> funcCmd = async (cmd) =>
            {
                List<HangarView> hangar = new List<HangarView>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var infoHangar = new HangarView()
                        {
                            IdClient = reader.GetInt32(0),
                            NomClient = reader.GetString(1),
                            IdRack = reader.GetInt32(2),
                            Gisement = reader.GetString(3),
                            PosRack = reader.GetString(4),
                            IdCommande = reader.GetInt32(5),
                            DescriptionCmd = ConvertFromDBVal<string?>(reader.GetValue(6)),
                            DateEntree = reader.GetDateTime(7)
                        };

                        hangar.Add(infoHangar);
                    }
                }

                return hangar;
            };

            List<HangarView> hangar = new List<HangarView>();

            try
            {
                hangar = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception ex)
            {
                var exs = ex.Message;
            }

            return hangar;
        }

        /// <summary>
        /// Ajoute une entrée dans le Hangar
        /// </summary>
        /// <param name="nouvelleEntreHangar"></param>
        /// <returns></returns>
        public async Task AddToHangar(GeoCommande nouvelleEntreHangar)
        {
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    string command = "INSERT INTO GeoCommande (RackId, CommandeId, DateEntre)"
                                    + " VALUES(@rackid, @cmdId, @dateEntree);";

                    using (var cmd = new MySqlCommand(command, conn))
                    {
                        cmd.Parameters.AddWithValue("@rackid", nouvelleEntreHangar.RackId);
                        cmd.Parameters.AddWithValue("@cmdId", nouvelleEntreHangar.CommandeId);
                        cmd.Parameters.AddWithValue("@dateEntree", nouvelleEntreHangar.DateEntree);

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

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task CreateTables(string pathSql)
        {
            try
            {
                string cmd = await File.ReadAllTextAsync(pathSql);
                await ExecuteCoreAsync(cmd);
            }
            catch (Exception)
            {
                throw;
            }
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
