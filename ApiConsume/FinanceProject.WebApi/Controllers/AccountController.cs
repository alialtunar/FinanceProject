using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.EntityLayer.Concreate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult AccountList()
        {
            var values = _accountService.TGetAll();
            return Ok(values);
        }

        [HttpPost]

        public IActionResult AddAccount(Account account)
        {
            _accountService.TInsert(account);
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAccount(int id)
        {
            var value = _accountService.TGetById(id);
            _accountService.TDelete(value.ID);
            return Ok();
        }

        [HttpPut]
        public IActionResult UpdateAccount(Account account)
        {
            _accountService.TUpdate(account);
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetAccount(int id)
        {
            var value = _accountService.TGetById(id);
            return Ok(value);
        }

    }
}
