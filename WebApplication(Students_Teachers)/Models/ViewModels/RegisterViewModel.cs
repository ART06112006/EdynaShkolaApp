using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication_Students_Teachers_.Models.ViewModels
{
    public class RegisterViewModel
    {
        public string Login { get; set; }

        [StringLength(24, ErrorMessage = "Invalid password", MinimumLength = 4)]
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public ModalMessageViewModel ModalMessageViewModel { get; set; }
    }
}