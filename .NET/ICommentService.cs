using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Requests.Comments;
using System.Collections.Generic;

namespace Sabio.Services.Interfaces
{
    public interface ICommentService
    {
        void DeleteById(int id);    
        List<Comment> GetByCreatedBy(int id);
        List<Comment> GetByEntityId(int entityTypeId, int entityId);
        public Paged<Comment> PaginateByEntityTypeId(int pageIndex, int pageSize, int entityTypeId);
        int Insert(CommentAddRequest model, int userId);
        void Update(CommentUpdateRequest model, int userId);
    }
}