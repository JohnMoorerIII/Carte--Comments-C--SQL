using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.Comments
{
    public class CommentAddRequest
    {
        [MinLength(1)]
        [MaxLength(50)]
        public string Subject { get; set; }
        
        [Required]
        [MinLength(1)]
        [MaxLength(3000)]
        public string Text { get; set; }
        
        public int ParentId { get; set; }

        public int EntityTypeId { get; set; }
    
        public int EntityId { get; set; }
        
        [Required]
        public bool IsDeleted { get; set; }
    }
}
