using SQLite;

public class HistoricoConta
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }
    public string Date { get; set; }
}

