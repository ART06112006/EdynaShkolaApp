using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication_Students_Teachers_.Models
{
    public class FacebookAuthenticationSettings
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string CallbackPath { get; set; }
    }
}