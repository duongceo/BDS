using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace HappyRE.Core.Entities.Model
{
    [Serializable]
    public class RoleInGroup
    {
        public string RoleId { get; set; }
        public int RoleGroupId { get; set; }
    }
}
