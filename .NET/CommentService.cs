using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Ingredients;
using Sabio.Models.Requests.Comments;
using Sabio.Services.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Sabio.Services
{
    public class CommentService : ICommentService
    {
        IDataProvider _data = null;
        public CommentService(IDataProvider data)
        {
            _data = data;
        }
        public List<Comment> GetByEntityId(int entityTypeId, int entityId)
        {
            string procName = "[dbo].[Comments_Select_ByEntityIdV2]";
            List<Comment> commentList = null;
            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@EntityTypeId", entityTypeId);
                parameterCollection.AddWithValue("@EntityId", entityId);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                Comment comment = MapSingleComment(reader, ref startingIndex);
                if (commentList == null)
                {
                    commentList = new List<Comment>();
                }

                commentList.Add(comment);
            }
            );
            return commentList;
        }
        public Paged<Comment> PaginateByEntityTypeId(int pageIndex, int pageSize, int entityTypeId)
        {
            Paged<Comment> pagedList = null;
            List<Comment> list = null;
            int totalCount = 0;
            string procName = "[dbo].[Comments_Select_ByEntityTypeId_Paginated]";

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection param)
            {
                param.AddWithValue("@PageIndex", pageIndex);
                param.AddWithValue("@PageSize", pageSize);
                param.AddWithValue("@EntityTypeId", entityTypeId);

            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                Comment comment = MapSingleComment(reader, ref startingIndex );
                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(startingIndex++);
                }
                if (list == null)
                {
                    list = new List<Comment>();
                }
                list.Add(comment);
            }
            );
            if (list != null)
            {
                pagedList = new Paged<Comment>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

        public int Insert(CommentAddRequest model, int userId)
        {
            int id = 0;
            string procName = "[dbo].[Comments_Insert]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonParams(model, col);
                    col.AddWithValue("@CreatedBy", userId);
                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;
                    col.Add(idOut);
                }, returnParameters: delegate (SqlParameterCollection returnCollection)
                {
                    object oId = returnCollection["@Id"].Value;
                    int.TryParse(oId.ToString(), out id);
                }
                );
            return id;
        }
        public List<Comment> GetByCreatedBy(int id)
        {
            string procName = "[dbo].[Comments_Select_ByCreatedByV2]";
            List<Comment> commentList = null;
            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@CreatedBy", id);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                Comment comment = MapSingleComment(reader, ref startingIndex);
                if (commentList == null)
                {
                    commentList = new List<Comment>();
                }

                commentList.Add(comment);
            }
            );
            return commentList;
        }

        public void DeleteById(int id)
        {
            string procName = "[dbo].[Comments_Delete_ById]";
            _data.ExecuteNonQuery(procName,
                delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Id", id);
                },
                returnParameters: null);
        }
        public void Update(CommentUpdateRequest model, int userId)
        {
            string procName = "[dbo].[Comments_Update]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Id", model.Id);
                    col.AddWithValue("@Subject", model.Subject);
                    col.AddWithValue("@Text", model.Text);
                    col.AddWithValue("@IsDeleted", model.IsDeleted);
                },
                returnParameters: null);
        }
        private static void AddCommonParams(CommentAddRequest model, SqlParameterCollection col)
        {
            col.AddWithValue("@Subject", model.Subject);
            col.AddWithValue("@Text", model.Text);
            col.AddWithValue("@ParentId", model.ParentId);
            col.AddWithValue("@EntityTypeId", model.EntityTypeId);
            col.AddWithValue("@EntityId", model.EntityId);
            col.AddWithValue("@IsDeleted", model.IsDeleted);
        }
        private static Comment MapSingleComment(IDataReader reader, ref int startingIndex )
        {
            Comment aComment = new Comment();
            aComment.CreatedBy = new UserProfile();
            aComment.EntityTypeId = new LookUp();

            aComment.Id = reader.GetSafeInt32(startingIndex++);
            aComment.Subject = reader.GetSafeString(startingIndex++);
            aComment.Text = reader.GetSafeString(startingIndex++);
            aComment.ParentId = reader.GetSafeInt32(startingIndex++);
            aComment.EntityTypeId.Id = reader.GetSafeInt32(startingIndex++);
            aComment.EntityTypeId.Name = reader.GetSafeString(startingIndex++);
            aComment.EntityId = reader.GetSafeInt32(startingIndex++);
            aComment.DateCreated = reader.GetDateTime(startingIndex++);
            aComment.DateModified = reader.GetDateTime(startingIndex++);
            aComment.CreatedBy.FirstName = reader.GetSafeString(startingIndex++);
            aComment.CreatedBy.LastName = reader.GetSafeString(startingIndex++);
            aComment.CreatedBy.Mi = reader.GetSafeString(startingIndex++);
            aComment.CreatedBy.AvatarUrl = reader.GetSafeString(startingIndex++);
            aComment.Email = reader.GetSafeString(startingIndex++);
            aComment.IsDeleted = reader.GetSafeBool(startingIndex++);
            return aComment;
        }
    }
}
