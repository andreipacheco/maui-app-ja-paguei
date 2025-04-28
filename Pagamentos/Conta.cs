using SQLite;
using System.ComponentModel;

public class Conta : INotifyPropertyChanged
{
    private string name;
    private bool isPaid;
    private string date;

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name
    {
        get => name;
        set
        {
            if (name != value)
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    public bool IsPaid
    {
        get => isPaid;
        set
        {
            if (isPaid != value)
            {
                isPaid = value;
                OnPropertyChanged(nameof(IsPaid));
            }
        }
    }

    public string Date
    {
        get => date;
        set
        {
            if (date != value)
            {
                date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}