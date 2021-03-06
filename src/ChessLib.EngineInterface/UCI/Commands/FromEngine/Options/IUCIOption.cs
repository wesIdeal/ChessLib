﻿using System.ComponentModel;
using System.Threading;

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
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            Volatile.Read(ref handler)?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
