using System;
using System.Collections.Generic;

namespace volcanes_api.Models
{
    public partial class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public byte[] Password { get; set; } = null!;
        public byte[] SaltKey { get; set; } = null!;
        public int RoleId { get; set; }

        public virtual Role Role { get; set; } = null!;
    }
}
