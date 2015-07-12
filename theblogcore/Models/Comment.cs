using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace theblogcore.Models
{
    public class Comment
    {
        public Guid CommentId { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public Guid BlogId { get; set; }
        
        [ForeignKey("BlogId")]
        public virtual Blog Blog { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser Commentator { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Posted At")]
        [Editable(false)]
        public DateTime DateTime { get; set; }

        public Boolean IsEdited { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Edited At")]
        public DateTime EditedTime { get; set; }
    }
}
