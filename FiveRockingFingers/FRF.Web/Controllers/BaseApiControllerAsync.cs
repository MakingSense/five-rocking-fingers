using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRF.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class BaseApiControllerAsync<T> : ControllerBase where T : class
    {
        public abstract Task<IActionResult> Get(int id);
        public abstract Task<IActionResult> GetAll();
        public abstract Task<IActionResult> Save(T entity);
        public abstract Task<IActionResult> Update(int id, T entity);
        public abstract Task<IActionResult> Delete(int id);
    }
}