using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.EntityLayer.Concreate;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> AccountList()
        {
            var values = await _accountService.TGetAllAsync();
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> AddAccount(Account account)
        {
            await _accountService.TInsertAsync(account);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            await _accountService.TDeleteAsync(id);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAccount(Account account)
        {
            await _accountService.TUpdateAsync(account);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(int id)
        {
            var value = await _accountService.TGetByIdAsync(id);
            return Ok(value);
        }
    }
}
