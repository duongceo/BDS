using System.Collections.Generic;

namespace HappyRE.Web1
{
    internal class UserHubModels
    {
        public string UserName { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }
}