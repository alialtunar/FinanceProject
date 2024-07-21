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
    public class TransactionHistoryController : ControllerBase
    {
        private readonly ITransactionHistoryService _transactionHistoryService;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;

        public TransactionHistoryController(ITransactionHistoryService transactionHistoryService, IEmailService emailService, IUserService userService, IAccountService accountService)
        {
            _transactionHistoryService = transactionHistoryService;
            _emailService = emailService;
            _userService = userService;
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> TransactionHistoryList()
        {
            try
            {
                var values = await _transactionHistoryService.TGetAllAsync();
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
        public async Task<IActionResult> AddTransactionHistory(TransactionHistory transactionHistory)
        {
            try
            {
                await _transactionHistoryService.TInsertAsync(transactionHistory);
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
        public async Task<IActionResult> DeleteTransactionHistory(int id)
        {
            try
            {
                var value = await _transactionHistoryService.TGetByIdAsync(id);
                await _transactionHistoryService.TDeleteAsync(value.ID);
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
        public async Task<IActionResult> UpdateTransactionHistory(TransactionHistory transactionHistory)
        {
            try
            {
                await _transactionHistoryService.TUpdateAsync(transactionHistory);
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
        public async Task<IActionResult> GetTransactionHistory(int id)
        {
            try
            {
                var value = await _transactionHistoryService.TGetByIdAsync(id);
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

        [HttpPost("InitiateDeposit")]
        public async Task<IActionResult> InitiateDeposit(int accountId, decimal amount)
        {
            try
            {
                var code = await _transactionHistoryService.InitiateDeposit(accountId, amount);

                // Get user's email address from your database using accountId
                var account = await _accountService.TGetByIdAsync(accountId);
                var user = await _userService.TGetByIdAsync(account.UserID); // Use UserID to get User
                var userEmailAddress = user.Email; // Assuming you have an Email property in your user object

                var subject = "Deposit Verification Code";
                var body = $"Your deposit verification code is: {code}";

                // Send email
                await _emailService.SendEmailAsync(userEmailAddress, subject, body);

                return Ok(new { Code = code });
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

        [HttpPost("InitiateWithdraw")]
        public async Task<IActionResult> InitiateWithdraw(int accountId, decimal amount)
        {
            try
            {
                var code = await _transactionHistoryService.InitiateWithdraw(accountId, amount);

                // Get user's email address from your database using accountId
                var account = await _accountService.TGetByIdAsync(accountId);
                var user = await _userService.TGetByIdAsync(account.UserID); // Use UserID to get User
                var userEmailAddress = user.Email; // Assuming you have an Email property in your user object

                var subject = "Withdrawal Verification Code";
                var body = $"Your withdrawal verification code is: {code}";

                // Send email
                await _emailService.SendEmailAsync(userEmailAddress, subject, body);

                return Ok(new { Code = code });
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

        [HttpPost("Deposit")]
        public async Task<IActionResult> Deposit(int accountId, decimal amount, string code)
        {
            try
            {
                await _transactionHistoryService.Deposit(accountId, amount, code);
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

        [HttpPost("Withdraw")]
        public async Task<IActionResult> Withdraw(int accountId, decimal amount, string code)
        {
            try
            {
                await _transactionHistoryService.Withdraw(accountId, amount, code);
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

        [HttpPost("initiate-transfer")]
        public async Task<IActionResult> InitiateTransfer(int senderAccountId, string recipientAccountNumber, decimal amount)
        {
            try
            {
                await _transactionHistoryService.InitiateTransfer(senderAccountId, recipientAccountNumber, amount);
                return Ok("Transfer initiated. Please verify the recipient's name.");
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

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(int senderAccountId, string recipientAccountNumber, decimal amount, string recipientName, string description = null)
        {
            try
            {
                await _transactionHistoryService.Transfer(senderAccountId, recipientAccountNumber, amount, recipientName, description);
                return Ok("Transfer successful");
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
