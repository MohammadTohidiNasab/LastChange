﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Divar.Models
{
    public class AssignRoleViewModel
    {
        public string UserId { get; set; }
        public List<SelectListItem> Users { get; set; }
        public List<string> AvailableRoles { get; set; }
        public List<string> SelectedRoles { get; set; } = new List<string>();
    }
}
