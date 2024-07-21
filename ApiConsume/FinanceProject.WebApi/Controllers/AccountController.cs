using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.EntityLayer.Concreate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FinanceProject.Core.Exceptions;

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
            try
            {
                var values = await _accountService.TGetAllAsync();
                return Ok(values);
            }
            catch (ErrorException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Bir hata oluştu. Lütfen tekrar deneyin." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAccount(Account account)
        {
            try
            {
                await _accountService.TInsertAsync(account);
                return Ok();
            }
            catch (ErrorException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Bir hata oluştu. Lütfen tekrar deneyin." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                await _accountService.TDeleteAsync(id);
                return Ok();
            }
            catch (ErrorException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Bir hata oluştu. Lütfen tekrar deneyin." });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAccount(Account account)
        {
            try
            {
                await _accountService.TUpdateAsync(account);
                return Ok();
            }
            catch (ErrorException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Bir hata oluştu. Lütfen tekrar deneyin." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(int id)
        {
            try
            {
                var value = await _accountService.TGetByIdAsync(id);
                return Ok(value);
            }
            catch (ErrorException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Bir hata oluştu. Lütfen tekrar deneyin." });
            }
        }
    }
}
