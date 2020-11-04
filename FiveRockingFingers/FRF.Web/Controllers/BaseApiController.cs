using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FRF.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    // [Authorize] TODO:Pending AWS Credentials. Login is bypassed!!! [FIVE-6]
    public abstract class BaseApiController<TPost, TPut> : ControllerBase where TPost : class where TPut : class
    {
        public abstract Task<IActionResult> GetAsync(int id);
        public abstract Task<IActionResult> GetAllAsync();
        public abstract Task<IActionResult> SaveAsync(TPost entity);
        public abstract Task<IActionResult> UpdateAsync(int id, TPut entity);
        public abstract Task<IActionResult> DeleteAsync(int id);
    }
}
