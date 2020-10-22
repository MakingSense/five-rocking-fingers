using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FRF.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public abstract class BaseApiController<T> : ControllerBase where T : class
    {
        public abstract IActionResult Get(int id);
        public abstract IActionResult GetAll();
        public abstract IActionResult Save(T entity);
        public abstract IActionResult Update(T entity);
        public abstract IActionResult Delete(int id);
    }
}
