using System;
using System.Collections.Generic;
using System.Text;

namespace JumpStreetMobile.Messages
{
    public class ShowMessageDialog
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string OkLabel { get; set; }
        public string CancelLabel { get; set; }
    }
}
