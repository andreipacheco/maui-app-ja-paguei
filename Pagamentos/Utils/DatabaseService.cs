using Pagamentos.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pagamentos.Utils
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Conta>().Wait();
            _database.CreateTableAsync<HistoricoConta>().Wait();
            _database.CreateTableAsync<MesReferencia>().Wait();
            _database.CreateTableAsync<NotificationUserId>().Wait();
        }

        // Métodos para Conta
        public Task<List<Conta>> GetContasAsync() => _database.Table<Conta>().ToListAsync();

        public async Task<int> SaveContaAsync(Conta conta)
        {
            if (conta.Id != 0)
            {
                return await _database.UpdateAsync(conta);
            }
            else
            {
                return await _database.InsertAsync(conta);
            }
        }


        public Task<int> DeleteContaAsync(Conta conta) => _database.DeleteAsync(conta);
        public Task DeleteAllContasAsync() => _database.DeleteAllAsync<Conta>();
        public async Task<Conta?> GetContaByIdAsync(int id)
        {
            return await _database.Table<Conta>().FirstOrDefaultAsync(c => c.Id == id);
        }

        // Métodos para HistoricoConta
        public Task<List<HistoricoConta>> GetHistoricosAsync() => _database.Table<HistoricoConta>().ToListAsync();

        public Task<int> SaveHistoricoAsync(HistoricoConta historico) =>
            historico.Id != 0 ? _database.UpdateAsync(historico) : _database.InsertAsync(historico);

        public Task<int> DeleteHistoricoAsync(HistoricoConta historico) => _database.DeleteAsync(historico);
        public Task DeleteAllHistoricosAsync() => _database.DeleteAllAsync<HistoricoConta>();

        // Métodos para MesReferencia
        public Task<List<MesReferencia>> GetMesesReferenciaAsync() => _database.Table<MesReferencia>().ToListAsync();

        public Task<MesReferencia> GetUltimoMesReferenciaAsync() =>
            _database.Table<MesReferencia>().OrderByDescending(m => m.Id).FirstOrDefaultAsync();

        public Task<int> SaveMesReferenciaAsync(MesReferencia mesReferencia) =>
            mesReferencia.Id != 0 ? _database.UpdateAsync(mesReferencia) : _database.InsertAsync(mesReferencia);

        public Task<int> DeleteMesReferenciaAsync(MesReferencia mesReferencia) => _database.DeleteAsync(mesReferencia);

        //                                                                                                                                                                  q1 Métodos para NotificationPlayer (OneSignal)
        public async Task<int> SavePlayerIdAsync(string playerId)
        {
            // Limpa anterior e salva novo ID
            await _database.DeleteAllAsync<NotificationUserId>();
            return await _database.InsertAsync(new NotificationUserId { PlayerId = playerId });
        }

        public async Task<string?> GetPlayerIdAsync()
        {
            var player = await _database.Table<NotificationUserId>().FirstOrDefaultAsync();
            return player?.PlayerId;
        }
    }
}
