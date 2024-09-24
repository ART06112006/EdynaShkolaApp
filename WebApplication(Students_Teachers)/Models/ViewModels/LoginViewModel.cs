using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication_Students_Teachers_.Models.ViewModels
{
    public class LoginViewModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public bool? RememberMe { get; set; }
        public ModalMessageViewModel ModalMessageViewModel { get; set; }
    }
}