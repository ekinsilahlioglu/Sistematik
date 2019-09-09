using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace First_part.Models
{
    public class Permission
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
    }



   
}
