using System.Collections.Generic;

namespace Divar.Models
{
    public class Role
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public List<AccessLevel> Permissions { get; set; } = new List<AccessLevel>();
    }
}
