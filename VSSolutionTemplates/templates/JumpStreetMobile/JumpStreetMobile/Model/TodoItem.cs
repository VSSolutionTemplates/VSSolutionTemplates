using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace JumpStreetMobile.Model
{
	public class TodoItem
	{
        #region Azure Mobile Service Properties
        public string Id { get; set; }

        #endregion

        public string Name { get; set; }
        public bool Done { get; set; }

        [Version]
        public string Version { get; set; }
    }
}

