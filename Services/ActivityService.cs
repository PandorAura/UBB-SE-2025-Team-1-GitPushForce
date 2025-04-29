using src.Model;
using src.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Services
{
    public class ActivityService
    {
        private readonly IActivityRepository _activityRepository;

        public ActivityService(IActivityRepository activityRepository)
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
