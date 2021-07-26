using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace WorkoutTimer.Visual
{
    public sealed class SwitchedViewModel : INotifyPropertyChanged
    {
        private object? _viewModel;

        public SwitchedViewModel(object? viewModel, IAsyncEnumerable<object?> viewModels)
        {
            ViewModel = viewModel;
            _ = SwitchViewModels(viewModels);
        }

        private async Task SwitchViewModels(IAsyncEnumerable<object?> viewModels)
        {
            try
            {
                await foreach (var viewModel in viewModels)
                {
                    ViewModel = viewModel;
                }
            }
            finally
            {
                ViewModel = null;
            }
        }

        public object? ViewModel
        {
            get => _viewModel;
            private set
            {
                _viewModel = value;
                PropertyChanged?.Invoke(this);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}