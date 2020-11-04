using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FRF.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    // [Authorize] TODO:Pending AWS Credentials. Login is bypassed!!! [FIVE-6]
    public abstract class BaseApiController<T> : ControllerBase where T : class
    {
        public abstract Task<IActionResult> GetAsync(int id);
        public abstract Task<IActionResult> GetAllAsync();
        public abstract Task<IActionResult> SaveAsync(T entity);
        public abstract Task<IActionResult> UpdateAsync(int id, T entity);
        public abstract Task<IActionResult> DeleteAsync(int id);
    }
}
