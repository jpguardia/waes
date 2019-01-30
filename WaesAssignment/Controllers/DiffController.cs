using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WaesAssignment.Dto;
using WaesAssignment.Services;

namespace WaesAssignment.Controllers
{
    [RoutePrefix("api/v1/diff")]
    public class DiffController : BaseController
    {
        private IDiffService _diffService;
        


        public DiffController(IDiffService diffService)
        {
            _diffService = diffService;            
        }

        [HttpPost]
        [Route("left/{id}")]
        public async Task<IHttpActionResult> Left(int id, [FromBody] string content)
        {
            try
            {
                IHttpActionResult response;
                Response(_diffService.Add(id, DiffTypes.Left, content), out response);
                return response;
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("right/{id}")]
        public async Task<IHttpActionResult> Right(int id, [FromBody] string content)
        {
            try
            {
                IHttpActionResult response;
                Response(_diffService.Add(id, DiffTypes.Right, content), out response);
                return response;                
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(DiffResultDto))]
        public async Task<IHttpActionResult> Get(int id)
        {
            try
            {
                IHttpActionResult response;
                Response(_diffService.ComputeDifference(id), out response);
                return response;
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
    
}
