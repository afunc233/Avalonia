using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Avalonia.UnitTests
{
    public class NotifyingBase : INotifyPropertyChanged
    {
        private PropertyChangedEventHandler _propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _propertyChanged += value;
                ++PropertyChangedSubscriptionCount;
                ++PropertyChangedGeneration;
            }

            remove
            {
                if (_propertyChanged?.GetInvocationList().Contains(value) == true)
                {
                    _propertyChanged -= value;
                    --PropertyChangedSubscriptionCount;
                    ++PropertyChangedGeneration;
                }
            }
        }

        public int PropertyChangedGeneration { get; private set; }
        public int PropertyChangedSubscriptionCount { get; private set; }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
