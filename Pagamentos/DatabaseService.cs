using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pagamentos
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
        }

        // Métodos para Conta
        public Task<List<Conta>> GetContasAsync()
        {
            return _database.Table<Conta>().ToListAsync();
        }

        public Task<int> SaveContaAsync(Conta conta)
        {
            if (conta.Id != 0)
            {
                return _database.UpdateAsync(conta);
            }
            else
            {
                return _database.InsertAsync(conta);
            }
        }

        public Task<int> DeleteContaAsync(Conta conta)
        {
            return _database.DeleteAsync(conta);
        }

        public Task DeleteAllContasAsync()
        {
            return _database.DeleteAllAsync<Conta>();
        }

        // Métodos para HistoricoConta
        public Task<List<HistoricoConta>> GetHistoricosAsync()
        {
            return _database.Table<HistoricoConta>().ToListAsync();
        }

        public Task<int> SaveHistoricoAsync(HistoricoConta historico)
        {
            if (historico.Id != 0)
            {
                return _database.UpdateAsync(historico);
            }
            else
            {
                return _database.InsertAsync(historico);
            }
        }

        public Task<int> DeleteHistoricoAsync(HistoricoConta historico)
        {
            return _database.DeleteAsync(historico);
        }

        public Task DeleteAllHistoricosAsync()
        {
            return _database.DeleteAllAsync<HistoricoConta>();
        }

        // Métodos para MesReferencia
        public Task<List<MesReferencia>> GetMesesReferenciaAsync()
        {
            return _database.Table<MesReferencia>().ToListAsync();
        }

        public async Task<MesReferencia> GetUltimoMesReferenciaAsync()
        {
            return await _database.Table<MesReferencia>().OrderByDescending(m => m.Id).FirstOrDefaultAsync();
        }

        public Task<int> SaveMesReferenciaAsync(MesReferencia mesReferencia)
        {
            if (mesReferencia.Id != 0)
            {
                return _database.UpdateAsync(mesReferencia);
            }
            else
            {
                return _database.InsertAsync(mesReferencia);
            }
        }

        public Task<int> DeleteMesReferenciaAsync(MesReferencia mesReferencia)
        {
            return _database.DeleteAsync(mesReferencia);
        }

    }
}
