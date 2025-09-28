using System.Collections.Generic;

namespace SO.Web.Authorization
{
    public class PermissionOptions
    {
        public Dictionary<string, string[]> PermissionRoles { get; set; } = new Dictionary<string, string[]>();
    }
}


