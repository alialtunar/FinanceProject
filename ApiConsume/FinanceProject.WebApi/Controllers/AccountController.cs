using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.EntityLayer.Concreate;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult> AccountList()
        {
            var response = await _accountService.TGetAllAsync();
            if (!response.isSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("Admin/paged")]
        public async Task<ActionResult> GetPagedAccounts(int page = 1, int pageSize = 6)
        {
            var response = await _accountService.TGetAdminPagedAccountsAsync(page, pageSize);
            if (!response.isSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> AddAccount([FromBody] Account account)
        {
            var response = await _accountService.TInsertAsync(account);
            if (!response.isSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAccount(int id)
        {
            var response = await _accountService.TDeleteAsync(id);
            if (!response.isSuccess)
            {
                return BadRequest(response);
            }
            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateAccount([FromBody] Account account)
        {
            var response = await _accountService.TUpdateAsync(account);
            if (!response.isSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetAccount(int id)
        {
            var response = await _accountService.TGetByIdAsync(id);
            if (!response.isSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("details/{userId}")]
public async Task<ActionResult> GetAccountDetails(int userId)
{
    var accountResponse = await _accountService.TGetAccountByUserId(userId);
    if (!accountResponse.isSuccess)
    {
        return BadRequest(accountResponse);
    }


    var account = accountResponse.Result as Account;


    var accountDetailsResponse = await _accountService.TGetAccountDetailsAsync(account.ID);
    if (!accountDetailsResponse.isSuccess)
    {
        return BadRequest(accountDetailsResponse);
    }

    return Ok(accountDetailsResponse);
}

    }
}
