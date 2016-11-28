using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JumpStreetMobile.Utils
{
    public interface IPushNotifications
    {
        Task InitializeNotificationsAsync();
    }
}
