using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication_Students_Teachers_.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(24, ErrorMessage = "Invalid password", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display (Name = "Password")]
        public string Password { get; set; }

        public string Code { get; set; }

        public ModalMessageViewModel ModalMessage { get; set; }
    }
}