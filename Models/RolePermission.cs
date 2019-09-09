using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace First_part.Models
{
    public class RolePermission
    {
        [Key]
        public string id { get; set; }

        public string PermissionId { get; set; }
        public string RoleId { get; set; }


    }
}
