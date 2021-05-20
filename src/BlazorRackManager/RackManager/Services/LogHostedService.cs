using AccessData;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RackManager.Services
{
	public class LogHostedService : IHostedService, IDisposable
	{
        private Timer _timerReset;
        private readonly SqlContext SqlContext;
        
        #region Constructeur
        public LogHostedService(SqlContext sqlContext)
        {
            SqlContext = sqlContext;
        }
        #endregion

        #region Implement Interfaces

        /// <summary>
        /// Lancement du service
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Défault 30 jours.
            _timerReset = new Timer(DeleteOldLogs, null, TimeSpan.FromDays(30), TimeSpan.FromDays(30));
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Arrêt du service
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timerReset?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timerReset?.Dispose();
        }

        #endregion

        #region Private methods

        private void DeleteOldLogs(object state)
        {
			try
			{
                int nombreLigneDeleted = SqlContext.DeleteLogs(30).GetAwaiter().GetResult();
                Log.Information($"LOGS - Suppression de {nombreLigneDeleted} lignes de logs.");
            }
            catch (Exception ex)
			{
                Log.Error(ex, "Erreur dans DeleteOldLogs");
			}
        }

        #endregion
    }
}
