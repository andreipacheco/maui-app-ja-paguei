using SQLite;

public class MesReferencia
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string NomeMes { get; set; }
}

