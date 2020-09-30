﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRF.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class BaseApiController<T> : ControllerBase where T : class
    {
        public abstract IActionResult Get(int id);
        public abstract IActionResult GetAll();
        public abstract IActionResult Save(T entity);
        public abstract IActionResult Update(T entity);
        public abstract IActionResult Delete(int id);
    }
}
