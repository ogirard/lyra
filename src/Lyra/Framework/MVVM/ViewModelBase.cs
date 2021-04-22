using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Lyra.Framework.MVVM
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private bool isInitialized;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected virtual bool SetPropertyValue<T>(T value, ref T field, [CallerMemberName] string propertyName = null)
        {
            if (Equals(value, field))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public void Initialize()
        {
            if (isInitialized)
            {
                return;
            }

            OnInitialize();
            isInitialized = true;
        }

        protected virtual void OnInitialize()
        {
        }
    }
}