using SQLite;

namespace Pagamentos
{
    public class Conta
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        public bool IsPaid { get; set; }
        public string Date { get; set; }
    }
}
