using System;
using System.ComponentModel.DataAnnotations;

namespace First_part.Models
{
    public class RegisterModel
    {
        
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

       

    }
}
