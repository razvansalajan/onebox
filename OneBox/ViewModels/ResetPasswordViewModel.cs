using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OneBox_WebServices.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords does not match")]
        public string CheckNewPassword { get; set; }

        public string Code { get; set; }
        public string Email { get; set; }
    }
}