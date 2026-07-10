using SQLite;
using System.ComponentModel;

namespace Pagamentos.Models
{
    public class Contato : INotifyPropertyChanged
    {
        private string nome;
        private string chavePix;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Nome
        {
            get => nome;
            set
            {
                if (nome != value)
                {
                    nome = value;
                    OnPropertyChanged(nameof(Nome));
                }
            }
        }

        public string ChavePix
        {
            get => chavePix;
            set
            {
                if (chavePix != value)
                {
                    chavePix = value;
                    OnPropertyChanged(nameof(ChavePix));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
