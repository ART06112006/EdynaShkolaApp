using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication_Students_Teachers_.Models
{
    public class GoogleAuthenticationSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string CallbackPath { get; set; }
    }
}