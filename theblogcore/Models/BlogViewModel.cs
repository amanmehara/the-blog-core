using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace theblogcore.Models
{
    public class BlogViewModel
    {
        public virtual Blog Blog { get; set; }

        public virtual Comment Comment { get; set; }
    }
}
