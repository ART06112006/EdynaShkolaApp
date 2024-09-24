using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication_Students_Teachers_.Models.ViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(24, ErrorMessage = "Invalid password", MinimumLength = 4)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        
        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        public ModalMessageViewModel ModalMessageViewModel { get; set; }
    }
}