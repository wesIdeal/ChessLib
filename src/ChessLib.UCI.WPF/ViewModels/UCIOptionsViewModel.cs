using ChessLib.UCI.Commands.FromEngine.Options;
using ChessLib.UCI.WPF.OptionsControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ChessLib.UCI.WPF.ViewModels
{

    public sealed class UCIOptionsViewModel : ViewModelBase
    {
        private IUCIOption[] _options;
        private List<Control> _optionControls;
        public List<Control> OptionControls
        {
            get { return _optionControls; }
            set { SetProperty(ref _optionControls, value); }
        }

        public UCIOptionsViewModel(IEnumerable<IUCIOption> options)
        {
            _options = options.Select(x => new { option = x, ordinal = GetOrdinal(x.GetType()) })
                .OrderBy(x => x.ordinal).ThenBy(x => x.option.Name)
                .Select(x => x.option)
                .ToArray();
            PutOptionsOnDialog();
        }

        private void PutOptionsOnDialog()
        {
            foreach (var option in _options)
            {

            }
        }

        private Control GetControlFromoption(IUCIOption option)
        {
            Type type = option.GetType();
            if (!type.GetInterfaces().Contains(typeof(IUCIOption)))
            {
                throw new ArgumentException("Options passesd to UCIOptionsViewModel must be of type IUCIOption.");
            }

            if (type == typeof(UCIStringOption))
            {
                return new StringOption() { DataContext = option };
            }
            else if (type == typeof(UCIComboOption))
                return new ComboOption() { DataContext = option };
            else if (type == typeof(UCICheckOption))
                return new CheckOption() { DataContext = option };
            else if (type == typeof(UCISpinOption))
                return new SpinOption() { DataContext = option };
            else if (type == typeof(UCIButtonOption))
                return new ButtonOption();
            else
            {
                throw new ArgumentException($"Type {type.Name} is not currently supported.");
            }
        }

        private int GetOrdinal(Type type)
        {
            if (!type.GetInterfaces().Contains(typeof(IUCIOption)))
            {
                throw new ArgumentException("Options passesd to UCIOptionsViewModel must be of type IUCIOption.");
            }

            if (type == typeof(UCIStringOption))
                return 1;
            else if (type == typeof(UCIComboOption))
                return 2;
            else if (type == typeof(UCICheckOption))
                return 3;
            else if (type == typeof(UCISpinOption))
                return 4;
            else if (type == typeof(UCIButtonOption))
                return 5;
            else
            {
                throw new ArgumentException($"Type {type.Name} is not currently supported.");
            }
        }
    }

    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName]string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
            return false;
        }
    }
}
