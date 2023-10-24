using System;
using System.Collections.Generic;

namespace volcanes_api.Models
{
    public partial class Role
    {
        public Role()
        {
            Usuarios = new HashSet<Usuario>();
        }

        public int Id { get; set; }
        public string RoleName { get; set; } = null!;

        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
