using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Ingredients;
using Sabio.Models.Requests.Comments;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/comments")]
    [ApiController]
    public class CommentApiController : BaseApiController
    {
        private ICommentService _service = null;
        private IAuthenticationService<int> _authService = null;
        public CommentApiController(ICommentService service
            , ILogger<CommentApiController>logger
            , IAuthenticationService<int> authService) : base(logger)
        {
            _service = service;
            _authService = authService;
        }

        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(CommentUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;
            var userId = _authService.GetCurrentUserId();

            try
            {
                _service.Update(model, userId);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }
        [HttpGet("entitytype")]
        public ActionResult<ItemResponse<Paged<Comment>>> PaginateByEntityTypeId(int pageIndex, int pageSize, int entityTypeId)
        {
            ActionResult result = null;

            try
            {
                Paged<Comment> page = _service.PaginateByEntityTypeId(pageIndex, pageSize, entityTypeId);

                if (page == null)
                {
                    result = NotFound404(new ErrorResponse("Records not found"));
                }
                else
                {
                    ItemResponse<Paged<Comment>> response = new ItemResponse<Paged<Comment>>();
                    response.Item = page;
                    result = Ok200(response);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.Message.ToString()));
            }
            return result;
        }

        [HttpGet("entitytype/{entityTypeId:int}/entity/{entityId:int}")]
        public ActionResult<ItemsResponse<Comment>> GetByEntityId(int entityTypeId, int entityId)
        {
            int iCode = 200;
            BaseResponse response = null;

            try
            {
                List<Comment> commentList = _service.GetByEntityId(entityTypeId, entityId);

                if (commentList == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Could not find comment.");
                }
                else
                {
                    response = new ItemsResponse<Comment> { Items = commentList };
                }
            }
            catch (Exception ex)
            {

                iCode = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }

            return StatusCode(iCode, response);
        }


        [HttpGet("user/{id}")]
        public ActionResult<ItemsResponse<Comment>> GetByCreatedBy(int id)
        {
            int iCode = 200;
            BaseResponse response = null;

            try
            {
                List<Comment> commentList = _service.GetByCreatedBy(id);

                if ( commentList == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Could not find comment.");
                }
                else
                {
                    response = new ItemsResponse<Comment> { Items = commentList };
                }
            }
            catch (Exception ex)
            {

                iCode = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }

            return StatusCode(iCode, response);

        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Create(CommentAddRequest model)
        {

            ObjectResult result = null;

            try
            {
                int userId = _authService.GetCurrentUserId();

                int id = _service.Insert(model, userId);
                ItemResponse<int> response = new ItemResponse<int>() { Item = id };
                
                result = Created201(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }

            return result;
        }

        [HttpPut("delete/{id:int}")]
        public ActionResult<SuccessResponse> DeleteById(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.DeleteById( id);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }
    }
}
