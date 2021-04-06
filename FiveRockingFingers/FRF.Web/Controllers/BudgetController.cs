﻿using FRF.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FRF.Web.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        [HttpGet("~/api/projects/{projectId}/budget")]
        public async Task<IActionResult> GetBudget(int projectId)
        {
            var response = await _budgetService.GetBudget(projectId);

            if (!response.Success) return NotFound($"Error {response.Error.Code}: {response.Error.Message}");

            return Ok(response.Value);
        }
    }
}
