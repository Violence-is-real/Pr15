using System.Collections.Generic;
using System.Linq;

namespace Pr15
{
    public partial class basepart_
    {
        public string Details
        {
            get {
                var info = new List<string>();
                return info.Count == 0 ? "—" : string.Join(" | ", info);
            }
            
        }
    }
}