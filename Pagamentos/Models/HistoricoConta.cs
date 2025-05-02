using SQLite;
using System.Text.Json.Serialization;

public class HistoricoConta
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }
    [JsonConverter(typeof(JsonDateConverter))]
    public DateTime Date { get; set; }

    public string Valor { get; set; }
}

