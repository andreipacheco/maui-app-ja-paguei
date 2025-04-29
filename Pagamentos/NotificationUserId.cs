using SQLite;

namespace Pagamentos.Models
{
    public class NotificationUserId
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string PlayerId { get; set; }
    }
}
