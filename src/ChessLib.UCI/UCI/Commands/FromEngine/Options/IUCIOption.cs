using System.ComponentModel;

namespace ChessLib.EngineInterface.UCI.Commands.FromEngine.Options
{
    public interface IUCIOption
    {
        string Name { get; set; }
    }

    public abstract class UCIOption<T> : IUCIOption, INotifyPropertyChanged
    {
        private T _value;
        public string Name { get; set; }
        public T Value
        {
            get { return _value; }
            set
            {

            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
