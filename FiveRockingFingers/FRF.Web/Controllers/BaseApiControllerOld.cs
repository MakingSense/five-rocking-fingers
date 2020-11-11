using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRF.Web.Controllers
{
    /* TODO:ESTE CONTROLLER SERÁ ELIMINADO CUANDO SE TERMINE EL TICKET FIVE-115*/
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class BaseApiControllerOld<T> : ControllerBase where T : class
    {
        public abstract IActionResult Get(int id);
        public abstract IActionResult GetAll();
        public abstract IActionResult Save(T entity);
        public abstract IActionResult Update(T entity);
        public abstract IActionResult Delete(int id);
    }
}
