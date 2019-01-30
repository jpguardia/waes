using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WaesAssignment.Dto;

namespace WaesAssignment.Controllers
{
    public class BaseController : ApiController
    {
        
        protected virtual void Response<T>(ServiceResultWrapper<T> result, out IHttpActionResult response)
        {
            response = result.Success ? Ok(result.Data) : ReturnProblemResponse(result);
        }

        protected virtual void Response(ServiceResultWrapper result, out IHttpActionResult response)
        {
            response = result.Success ? Ok(new { success = true, result = result.Message }) : ReturnProblemResponse(result);
        }

        protected IHttpActionResult ReturnProblemResponse(ServiceResultWrapper result)
        {            
            switch (result.Code)
            {
                case ServiceResultCode.BadRequest:
                    return BadRequest(result.Message ?? (result.Exception?.ToString() ?? "Bad request"));                
                case ServiceResultCode.NotFound:
                    return NotFound();
                default:
                    return InternalServerError(result.Exception ?? new Exception("Unhandled result code"));
            }
        }

    }
}
