using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WorkoutTimer.Visual
{
    public static class PropertyChangedEventHandlerExtensions
    {
        public static void Invoke(
            this PropertyChangedEventHandler handler,
            object sender,
            [CallerMemberName] string? propertyName = default)
        {
            handler?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
        }
    }
}