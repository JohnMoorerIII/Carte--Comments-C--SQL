using Sabio.Models.Domain.User;
using System;

namespace Sabio.Models.Domain
{
    public class Comment
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public int ParentId { get; set; }
        public LookUp EntityTypeId { get; set; }
        public int EntityId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public UserProfileBase CreatedBy { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
    }
}
