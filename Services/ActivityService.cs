using src.Model;
using src.Repos;
using System;
using System.Collections.Generic;

namespace src.Services
{
    public class ActivityService : IActivityService
    {
        private readonly ActivityRepository _activityRepository;

        public ActivityService(ActivityRepository activityRepository)
        {
            _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        public List<ActivityLog> GetActivityForUser(string userCNP)
        {
            if (string.IsNullOrWhiteSpace(userCNP))
            {
                throw new ArgumentException("user cannot be found");
            }
            return _activityRepository.GetActivityForUser(userCNP);
        }   

    }
}
