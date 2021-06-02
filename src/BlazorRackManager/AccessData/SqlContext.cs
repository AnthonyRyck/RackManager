using RackCore;
using AccessData.Views;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RackCore.EntityView;

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
            catch (Exception)
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
            catch (Exception)
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

        public async Task UpdateSortieCommande(int idcommande, DateTime dateSortie)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                var commandUpdateCompetence = @$"UPDATE SuiviCommande SET DateSortie=@date"
                                      + $" WHERE IdCommande={idcommande};";

                using (var cmd = new MySqlCommand(commandUpdateCompetence, conn))
                {
                    cmd.Parameters.AddWithValue("@date", dateSortie);

                    conn.Open();
                    await cmd.ExecuteNonQueryAsync();
                    conn.Close();
                }
            }
        }


        public async Task<CommandeView> GetCommandeByIdRack(int idRack)
		{
            string cmd = "SELECT sc.IdCommande, sc.ClientId, cli.NomClient, sc.DescriptionCmd"
                            + " FROM geocommande geo"
                            + " INNER JOIN suivicommande sc"
                            + " ON geo.CommandeId = sc.IdCommande"
                            + " INNER JOIN clients cli"
                            + " ON sc.ClientId = cli.IdClient"
                            + $" WHERE geo.RackId = {idRack};";

            Func<MySqlCommand, Task<CommandeView>> funcCmd = async (cmd) =>
            {
                CommandeView commande = new CommandeView();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        commande.IdCommande = reader.GetInt32(0);
                        commande.IdClient = reader.GetInt32(1);
                        commande.NomClient = reader.GetString(2);
                        commande.DescriptionCmd = ConvertFromDBVal<string?>(reader.GetValue(3));
                    }
                }

                return commande;
            };

            CommandeView commandeView = new CommandeView();

            try
            {
                commandeView = await GetCoreAsync(cmd, funcCmd);
            }
            catch (Exception)
            {
                throw;
            }

            return commandeView;
        }

		/// <summary>
		/// Récupère la liste des commandes qui sont "terminées"/sorties. 
		/// </summary>
		/// <returns></returns>
		public async Task<IEnumerable<CommandeSortieView>> GetSorties()
        {
            string cmd = "SELECT sc.IdCommande, sc.ClientId, cli.NomClient, sc.DescriptionCmd, sc.DateSortie"
                            + " FROM suivicommande sc"
                            + " INNER JOIN clients cli"
                            + " ON sc.ClientId = cli.IdClient"
                            + " WHERE sc.DateSortie IS NOT NULL; ";

            Func<MySqlCommand, Task<List<CommandeSortieView>>> funcCmd = async (cmd) =>
            {
                List<CommandeSortieView> commandes = new List<CommandeSortieView>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        CommandeSortieView sortieView = new CommandeSortieView();

                        sortieView.IdCommande = reader.GetInt32(0);
                        sortieView.IdClient = reader.GetInt32(1);
                        sortieView.NomClient = reader.GetString(2);
                        sortieView.DescriptionCmd = ConvertFromDBVal<string?>(reader.GetValue(3));
                        sortieView.DateSortie = reader.GetDateTime(4);

                        commandes.Add(sortieView);
                    }
                }

                return commandes;
            };

            List<CommandeSortieView> sorties = new List<CommandeSortieView>();

            try
            {
                sorties = await GetCoreAsync(cmd, funcCmd);
            }
            catch (Exception)
            {
                throw;
            }

            return sorties;
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
            return await GetRacks(commandText);
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
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Récupère les racks qui sont vides.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Rack>> GetRackEmpty()
        {
            var commandText = @"SELECT rac.IdRack, rac.Gisement, rac.PosRack"
                                + " FROM rack rac"
                                + " LEFT OUTER JOIN geocommande geo"
	                            + " ON rac.IdRack = geo.RackId"
                                + " LEFT OUTER JOIN stock stoc"
                                + " ON stoc.RackId = rac.IdRack"
                                + " WHERE geo.RackId IS NULL"
                                + " AND stoc.ProduitId IS NULL;";
            return await GetRacks(commandText);
        }

        /// <summary>
        /// Récupère les racks qui sont pleins.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Rack>> GetRackFull()
		{
            var commandText = @"SELECT rac.IdRack, rac.Gisement, rac.PosRack"
                                + " FROM rack rac"
                                + " LEFT OUTER JOIN geocommande geo"
                                + " ON rac.IdRack = geo.RackId"
                                + " LEFT OUTER JOIN stock stoc"
                                + " ON stoc.RackId = rac.IdRack"
                                + " WHERE geo.RackId IS NOT NULL"
                                + " OR stoc.ProduitId IS NOT NULL;";

            return await GetRacks(commandText);
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
            catch (Exception)
            {
                throw;
            }

            return hangar;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRack"></param>
        /// <returns></returns>
        public async Task<HangarView> GetHangar(int idRack)
        {
            var commandText = @"SELECT cli.IdClient, cli.NomClient, rac.IdRack, rac.Gisement, rac.PosRack, cmd.IdCommande, cmd.DescriptionCmd, geo.DateEntre"
                    + " FROM geocommande geo"
                    + " INNER JOIN suivicommande cmd ON cmd.IdCommande = geo.CommandeId"
                    + " INNER JOIN clients cli ON cli.IdClient = cmd.ClientId"
                    + " INNER JOIN rack rac ON rac.IdRack = geo.RackId"
                    + $" WHERE geo.RackId = {idRack};";

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
            catch (Exception)
            {
                throw;
            }

            return hangar;
        }

        /// <summary>
        /// Récupère toutes les entrées du hangar
        /// </summary>
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Supprime de ce rack une commande.
        /// </summary>
        /// <param name="idRack"></param>
        /// <param name="idCommande"></param>
        /// <returns></returns>
        public async Task DeleteToHangar(int idRack, int idCommande)
        {
			try
			{
                string commandDelete = $"DELETE FROM GeoCommande"
                                   + $" WHERE RackId={idRack} AND CommandeId={idCommande};";

                await ExecuteCoreAsync(commandDelete);
            }
			catch (Exception)
			{
				throw;
			};
        }

        /// <summary>
        /// Change de place une palette d'un rack à un autre.
        /// </summary>
        /// <param name="idrackPartant"></param>
        /// <param name="idrackArrivant"></param>
        /// <returns></returns>
        public async Task TransfertRackTo(int idrackPartant, int idrackArrivant)
        {
			try
			{
                string cmdUpdate = $"UPDATE GeoCommande SET RackId={idrackArrivant}"
                                   + $" WHERE RackId={idrackPartant};";

                await ExecuteCoreAsync(cmdUpdate);
            }
			catch (Exception)
			{
				throw;
			}
        }

        /// <summary>
        /// Change de place une palette d'un rack à un autre.
        /// </summary>
        /// <param name="idrackPartant"></param>
        /// <param name="idrackArrivant"></param>
        /// <param name="idCommande"></param>
        /// <returns></returns>
        public async Task IntervertirRackTo(int idrackPartant, int idrackArrivant, int idCommande)
        {
            try
            {
                string cmdUpdate = $"UPDATE GeoCommande SET RackId={idrackArrivant}"
                                   + $" WHERE RackId={idrackPartant}" 
                                   + $" AND CommandeId={idCommande};";

                await ExecuteCoreAsync(cmdUpdate);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Stock
        
        /// <summary>
        /// Charge tous les stocks.
        /// </summary>
        /// <returns></returns>
        public async Task<List<StockView>> GetStocks()
        {
            var commandText = @"SELECT stoc.RackId, rac.Gisement, rac.PosRack, stoc.ProduitId, prod.Nom, stoc.Quantite, mes.Unite"
                            + " FROM Stock stoc"
                            + " INNER JOIN rack rac ON rac.IdRack = stoc.RackId"
                            + " INNER JOIN produit prod ON prod.IdProduit = stoc.ProduitId"
                            + " INNER JOIN mesure mes ON prod.MesureId = mes.IdMesure;";

            Func<MySqlCommand, Task<List<StockView>>> funcCmd = async (cmd) =>
            {
                List<StockView> stocks = new List<StockView>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var stock = new StockView()
                        {
                            IdRack = reader.GetInt32(0),
                            Gisement = reader.GetString(1),
                            PosRack = reader.GetString(2),
                            ReferenceProduit = reader.GetString(3),
                            NomDuProduit = reader.GetString(4),
                            Quantite = reader.GetDouble(5),
                            Unite = reader.GetString(6)
                        };

                        stocks.Add(stock);
                    }
                }

                return stocks;
            };

            List<StockView> stocks = new List<StockView>();

            try
            {
                stocks = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception)
            {
                throw;
            }

            return stocks;
        }

        /// <summary>
        /// Charge tous les stocks.
        /// </summary>
        /// <returns></returns>
        public async Task<List<StockView>> GetStocks(string refProduit)
        {
            var commandText = @"SELECT stoc.RackId, rac.Gisement, rac.PosRack, stoc.ProduitId, prod.Nom, stoc.Quantite, mes.Unite"
                            + " FROM Stock stoc"
                            + " INNER JOIN rack rac ON rac.IdRack = stoc.RackId"
                            + " INNER JOIN produit prod ON prod.IdProduit = stoc.ProduitId"
                            + " INNER JOIN mesure mes ON prod.MesureId = mes.IdMesure"
                            + $" WHERE stoc.ProduitId='{refProduit}';";

            Func<MySqlCommand, Task<List<StockView>>> funcCmd = async (cmd) =>
            {
                List<StockView> stocks = new List<StockView>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var stock = new StockView()
                        {
                            IdRack = reader.GetInt32(0),
                            Gisement = reader.GetString(1),
                            PosRack = reader.GetString(2),
                            ReferenceProduit = reader.GetString(3),
                            NomDuProduit = reader.GetString(4),
                            Quantite = reader.GetDouble(5),
                            Unite = reader.GetString(6)
                        };

                        stocks.Add(stock);
                    }
                }

                return stocks;
            };

            List<StockView> stocks = new List<StockView>();

            try
            {
                stocks = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception)
            {
                throw;
            }

            return stocks;
        }

        /// <summary>
        /// Charge tous les stocks.
        /// </summary>
        /// <returns></returns>
        public async Task<StockView> GetStocks(int rackId, string produitId)
        {
            var commandText = @"SELECT stoc.RackId, rac.Gisement, rac.PosRack, stoc.ProduitId, prod.Nom, stoc.Quantite, mes.Unite"
                            + " FROM Stock stoc"
                            + " INNER JOIN rack rac ON rac.IdRack = stoc.RackId"
                            + " INNER JOIN produit prod ON prod.IdProduit = stoc.ProduitId"
                            + " INNER JOIN mesure mes ON prod.MesureId = mes.IdMesure"
                            + $" WHERE stoc.RackId = {rackId} AND stoc.ProduitId='{produitId}';";

            Func<MySqlCommand, Task<StockView>> funcCmd = async (cmd) =>
            {
                StockView stock = new StockView();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        stock = new StockView()
                        {
                            IdRack = reader.GetInt32(0),
                            Gisement = reader.GetString(1),
                            PosRack = reader.GetString(2),
                            ReferenceProduit = reader.GetString(3),
                            NomDuProduit = reader.GetString(4),
                            Quantite = reader.GetDouble(5),
                            Unite = reader.GetString(6)
                        };
                    }
                }

                return stock;
            };

            StockView stock = new StockView();

            try
            {
                stock = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception)
            {
                throw;
            }

            return stock;
        }

        /// <summary>
        /// AJout une nouvelle entrée de stock
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        public async Task AddNewStock(Stock stock)
        {
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    string command = "INSERT INTO Stock (RackId, ProduitId, Quantite)"
                                    + " VALUES(@rack, @produit, @quantite);";

                    using (var cmd = new MySqlCommand(command, conn))
                    {
                        cmd.Parameters.AddWithValue("@rack", stock.RackId);
                        cmd.Parameters.AddWithValue("@produit", stock.ProduitId);
                        cmd.Parameters.AddWithValue("@quantite", stock.Quantite);

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

        /// <summary>
        /// Met à jour le stock d'un rack
        /// </summary>
        /// <param name="idRack"></param>
        /// <param name="referenceProduit"></param>
        /// <param name="quantite"></param>
        /// <returns></returns>
        public async Task UpdateStock(int idRack, string referenceProduit, double quantite)
        {
			try
			{
                string cmdUpdate = @"UPDATE stock SET Quantite=@quantite"
                                + $" WHERE RackId={idRack} AND ProduitId='{referenceProduit}'";

                using (var conn = new MySqlConnection(ConnectionString))
                {
                    using (var cmd = new MySqlCommand(cmdUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@quantite", quantite);

                        conn.Open();
                        await cmd.ExecuteNonQueryAsync();
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

        #region SortieStock

        /// <summary>
        /// AJout une nouvelle entrée de stock
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        public async Task AddNewSortieStock(string produitId, double quantite, DateTime dateSortie)
        {
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    string command = "INSERT INTO SortieStock (ProduitId, Quantite, DateSortie)"
                                    + " VALUES(@produit, @quantite, @date);";

                    using (var cmd = new MySqlCommand(command, conn))
                    {
                        cmd.Parameters.AddWithValue("@produit", produitId);
                        cmd.Parameters.AddWithValue("@quantite", quantite);
                        cmd.Parameters.AddWithValue("@date", dateSortie);

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

        /// <summary>
        /// Retourne tous les stocks sorties
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SortieStockView>> GetStockSortie()
        {
            var commandText = @"SELECT prod.IdProduit, prod.Nom, mes.Unite, stoc.Quantite, stoc.DateSortie"
                                + " FROM sortiestock stoc"
                                + " INNER JOIN produit prod ON prod.IdProduit = stoc.ProduitId"
                                + " INNER JOIN mesure mes ON mes.IdMesure = prod.MesureId;";

            Func<MySqlCommand, Task<List<SortieStockView>>> funcCmd = async (cmd) =>
            {
                List<SortieStockView> stocks = new List<SortieStockView>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var stockView = new SortieStockView()
                        {
                            Produit = new ProduitView()
                            {
                                IdReference = reader.GetString(0),
                                Nom = reader.GetString(1),
                                UniteMesure = reader.GetString(2)
							},
                            Quantite = reader.GetDouble(3),
                            DateDeSortie = reader.GetDateTime(4)
                        };

                        stocks.Add(stockView);
                    }
                }

                return stocks;
            };

            List<SortieStockView> stocks = new List<SortieStockView>();

            try
            {
                stocks = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception)
            {
                throw;
            }

            return stocks;
        }

        #endregion

        #region Logs

        /// <summary>
        /// Récupère les logs du niveau choisi et avec une date de début.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="dateDebut"></param>
        /// <returns></returns>
        public async Task<List<LogEntity>> GetLogs(string level, DateTime dateDebut)
        {
            var commandText = @"SELECT lg.Timestamp, lg.Level, lg.Message, lg.Exception"
                                + " FROM logs lg"
                                + $" WHERE lg._ts > '{dateDebut.ToString("yyyy-MM-dd")}'" 
                                + $" AND lg.Level='{level}'" 
                                + " ORDER BY _ts DESC;";

            Func<MySqlCommand, Task<List<LogEntity>>> funcCmd = async (cmd) =>
            {
                List<LogEntity> logs = new List<LogEntity>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var log = new LogEntity()
                        {
                            DateLog = DateTime.Parse(reader.GetString(0)),
                            Level = reader.GetString(1),
                            Message = reader.GetString(2),
                            ExceptionMsg = ConvertFromDBVal<string?>(reader.GetValue(3))
                        };

                        logs.Add(log);
                    }
                }

                return logs;
            };

            List<LogEntity> logs = new List<LogEntity>();

            try
            {
                logs = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception)
            {
                throw;
            }

            return logs;
        }

        /// <summary>
        /// Supprime les logs qui sont supérieurs à la date donnée.
        /// </summary>
        /// <param name="nombreJoursToDeleteLogs"></param>
        public async Task<int> DeleteLogs(uint nombreJoursToDeleteLogs)
        {
            DateTime dateToDelete = DateTime.Now.AddDays(-nombreJoursToDeleteLogs);

            var commandText = @"DELETE FROM logs lg"
                               + $" WHERE '{dateToDelete.ToString("yyyy-MM-dd")}' > lg._ts;";

			try
			{
                return await ExecuteCoreAsync(commandText);
            }
			catch (Exception)
			{
                throw;
			}
        }

        #endregion

        #region Unite de mesure

        /// <summary>
        /// Charge tous les unités de mesure
        /// </summary>
        /// <returns></returns>
        public async Task<List<UniteMesure>> GetUniteMesure()
        {
            var commandText = @"SELECT IdMesure, Unite FROM Mesure;";

            Func<MySqlCommand, Task<List<UniteMesure>>> funcCmd = async (cmd) =>
            {
                List<UniteMesure> mesures = new List<UniteMesure>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var produit = new UniteMesure()
                        {
                            IdMesure = reader.GetInt32(0),
                            Unite = reader.GetString(1)
                        };

                        mesures.Add(produit);
                    }
                }

                return mesures;
            };

            List<UniteMesure> mesures = new List<UniteMesure>();

            try
            {
                mesures = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception)
            {
                throw;
            }

            return mesures;
        }

        /// <summary>
		/// Ajout d'une nouvelle unité de mesure
		/// </summary>
		/// <param name="uniteMesure"></param>
		/// <returns></returns>
		public async Task<int> AddUniteMesure(string uniteMesure)
        {
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    string command = "INSERT INTO Mesure (Unite)"
                                    + " VALUES(@unite);";

                    using (var cmd = new MySqlCommand(command, conn))
                    {
                        cmd.Parameters.AddWithValue("@unite", uniteMesure);

                        conn.Open();
                        await cmd.ExecuteNonQueryAsync();
                        conn.Close();
                    }
                }

                string cmdLastId = " SELECT LAST_INSERT_ID();";
                int lastId = await GetIntCore(cmdLastId);

                return lastId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Produits

        /// <summary>
        /// Charge tous les produits
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProduitView>> GetProduits()
        {
            var commandText = @"SELECT prod.IdProduit, prod.Nom, mes.Unite, mes.IdMesure, prod.ImgProduit"
                                + " FROM Produit prod"
                                + " INNER JOIN mesure mes ON mes.IdMesure = prod.MesureId;";

            Func<MySqlCommand, Task<List<ProduitView>>> funcCmd = async (cmd) =>
            {
                List<ProduitView> produits = new List<ProduitView>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var produit = new ProduitView()
                        {
                            IdReference = reader.GetString(0),
                            Nom = reader.GetString(1),
                            UniteMesure = reader.GetString(2),
                            IdMesure = reader.GetInt32(3),
                            ImageProduit = ConvertFromDBVal<byte[]>(reader[4])
                        };

                        produits.Add(produit);
                    }
                }

                return produits;
            };

            List<ProduitView> produits = new List<ProduitView>();

            try
            {
                produits = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception)
            {
                throw;
            }

            return produits;
        }


        /// <summary>
        /// Charge le produit par sa référence.
        /// </summary>
        /// <returns></returns>
        public async Task<ProduitView> GetProduits(string reference)
        {
            var commandText = @"SELECT prod.IdProduit, prod.Nom, mes.Unite, mes.IdMesure, prod.ImgProduit"
                                + " FROM Produit prod"
                                + " INNER JOIN mesure mes ON mes.IdMesure = prod.MesureId"
                                + $" WHERE prod.IdProduit = '{reference}';";

            Func<MySqlCommand, Task<ProduitView>> funcCmd = async (cmd) =>
            {
                ProduitView produit = new ProduitView();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        produit = new ProduitView()
                        {
                            IdReference = reader.GetString(0),
                            Nom = reader.GetString(1),
                            UniteMesure = reader.GetString(2),
                            IdMesure = reader.GetInt32(3),
                            ImageProduit = ConvertFromDBVal<byte[]>(reader[4])
                        };
                    }
                }

                return produit;
            };

            ProduitView produit = new ProduitView();

            try
            {
                produit = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception)
            {
                throw;
            }

            return produit;
        }

        /// <summary>
		/// Ajout d'un nouveau produit
		/// </summary>
		/// <param name="nouveauProduit"></param>
		/// <returns></returns>
		public async Task AddProduit(Produit nouveauProduit)
        {
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    string command = "INSERT INTO Produit (IdProduit, Nom, MesureId, ImgProduit)"
                                    + " VALUES(@id, @nom, @unite, @imgContent);";

                    using (var cmd = new MySqlCommand(command, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", nouveauProduit.IdReference);
                        cmd.Parameters.AddWithValue("@nom", nouveauProduit.Nom);
                        cmd.Parameters.AddWithValue("@unite", nouveauProduit.UniteId);
                        cmd.Parameters.AddWithValue("@imgContent", nouveauProduit.ImageContent);

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

        /// <summary>
        /// Met à jour un produit.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="produit"></param>
        /// <returns></returns>
        public async Task UpdateProduit(string reference, Produit produit)
        {
            try
            {
                string cmdUpdate = @"UPDATE produit SET Nom=@nomproduit, MesureId=@mesure, ImgProduit=@imgContent "
                                + $" WHERE IdProduit='{reference}';";

                using (var conn = new MySqlConnection(ConnectionString))
                {
                    using (var cmd = new MySqlCommand(cmdUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@nomproduit", produit.Nom);
                        cmd.Parameters.AddWithValue("@mesure", produit.UniteId);
                        cmd.Parameters.AddWithValue("@imgContent", produit.ImageContent);

                        conn.Open();
                        await cmd.ExecuteNonQueryAsync();
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

        #region Inventaire

        /// <summary>
        /// Fait l'inventaire des stocks présents.
        /// </summary>
        /// <returns></returns>
        public async Task<List<InventaireView>> GetInventaire()
		{
            string commandText = "SELECT sc.ProduitId, SUM(sc.Quantite), prod.Nom, mes.Unite"
                            + " FROM stock sc"
                            + " INNER JOIN produit prod ON prod.IdProduit = sc.ProduitId"
                            + " INNER JOIN mesure mes ON mes.IdMesure = prod.MesureId"
                            + " GROUP BY sc.ProduitId;";

            Func<MySqlCommand, Task<List<InventaireView>>> funcCmd = async (cmd) =>
            {
                List<InventaireView> stocks = new List<InventaireView>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var inventaire = new InventaireView()
                        {
                            Produit = new ProduitView()
                            {
                                IdReference = reader.GetString(0),
                                Nom = reader.GetString(2),
                                UniteMesure = reader.GetString(3)
                            },
                            Quantite = reader.GetDouble(1)
                        };

                        stocks.Add(inventaire);
                    }
                }

                return stocks;
            };

            List<InventaireView> stocks = new List<InventaireView>();

            try
            {
                stocks = await GetCoreAsync(commandText, funcCmd);
            }
            catch (Exception)
            {
                throw;
            }

            return stocks;
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

        private async Task<List<Rack>> GetRacks(string commande)
        {
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
                racks = await GetCoreAsync(commande, funcCmd);
            }
            catch (Exception)
            {
                throw;
            }

            return racks;
        }


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
            catch (Exception)
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
            catch (Exception)
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
        private async Task<int> ExecuteCoreAsync(string commandSql)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(commandSql, conn);

                conn.Open();
                return await cmd.ExecuteNonQueryAsync();
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
