using System;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HappyRE.Core.Entities.Model
{
    [Serializable]
    public class RoleGroup: BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Tên nhóm quyền")]
        [Required]
        public string Name { get; set; }
        [DisplayName("Quyền")]
        public int Roles { get; set; }
    }
}
