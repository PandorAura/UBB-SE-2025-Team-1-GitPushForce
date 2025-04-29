using src.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace src.ViewModel
{
    public class ActivityViewModel : INotifyPropertyChanged
    {
        private ActivityService _activityService;

        public event PropertyChangedEventHandler PropertyChanged;

        public ActivityViewModel(ActivityService activityService)
        {
            _activityService = activityService ?? throw new ArgumentNullException(nameof(activityService));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
