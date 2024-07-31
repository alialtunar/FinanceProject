using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.EntityLayer.Concreate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FinanceProject.Core.Exceptions;
using FinanceProject.DtoLayer.Dtos.TransactionHistoryDto;

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

        [HttpGet("last5/{userId}")]
        public async Task<IActionResult> GetLastFiveTransactions(int userId)
        {
            try
            {
                var account = await _accountService.TGetAccountByUserId(userId);
                var transactions = await _transactionHistoryService.TGetLastFiveTransactionsAsync(account.ID);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Hata: {ex.Message}");
            }
        }


        [HttpGet("lastUsers/{userId}")]
        public async Task<IActionResult> GetLastTransferUsers(int userId)
        {
            try
            {
                var account = await _accountService.TGetAccountByUserId(userId);
                var transactions = await _transactionHistoryService.TGetLast5TransfersUsersAsync(account.ID);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Hata: {ex.Message}");
            }
        }

        [HttpGet("totalamountlast24hours/{userId}")]
        public async Task<IActionResult> GetTotalAmountLast24Hours(int userId)
        {
            try
            {
                var account = await _accountService.TGetAccountByUserId(userId);
                var totalAmount = await _transactionHistoryService.TGetTotalAmountLast24HoursAsync(account.ID);
                return Ok(totalAmount);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Hata: {ex.Message}");
            }
        }


        [HttpGet("paged/{userId}")]
        public async Task<IActionResult> GetPagedTransactionHistory(int userId, int page = 1, int pageSize = 6)
        {
            try
            {
                var account = await _accountService.TGetAccountByUserId(userId);
                var transactions = await _transactionHistoryService.TGetPagedTransactionHistoryAsync(account.ID, page, pageSize);
            return Ok(transactions);
            }
            catch(Exception)
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
        public async Task<IActionResult> InitiateDeposit([FromBody] DepositeRquestDto request)
        {
            try
            {
                var account = await _accountService.TGetAccountByUserId(request.userId);
                var code = await _transactionHistoryService.InitiateDeposit(account.ID, request.amount);

                // Get user's email address from your database using accountId
                var user = await _userService.TGetByIdAsync(request.userId); // Use UserID to get User
                var userEmailAddress = user.Email; // Assuming you have an Email property in your user object

                var subject = "Deposit Verification Code";
                var body = $"Your deposit verification code is: {code}";

                // Send email
                await _emailService.SendEmailAsync(userEmailAddress, subject, body);

                return Ok(new { Message = "Mailinize gelen kodu giriniz" });
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
        public async Task<IActionResult> InitiateWithdraw([FromBody] WithdrawRequestDto request)
        {
            try
            {
                var account = await _accountService.TGetAccountByUserId(request.userId);
                var code = await _transactionHistoryService.InitiateWithdraw(account.ID, request.amount);
                var user = await _userService.TGetByIdAsync(request.userId); // Use UserID to get User
                var userEmailAddress = user.Email; // Assuming you have an Email property in your user object

                var subject = "Withdrawal Verification Code";
                var body = $"Your withdrawal verification code is: {code}";

                // Send email
                await _emailService.SendEmailAsync(userEmailAddress, subject, body);

                return Ok(new { Message = "Mailinize gelen kodu giriniz" });
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
        public async Task<IActionResult> Deposit([FromBody] DepositeDto request)
        {
            try
            {
                var account = await _accountService.TGetAccountByUserId(request.userId);
                await _transactionHistoryService.Deposit(account.ID, request.amount, request.code);
                return Ok(new { Message = "Para Yatırma Başarılı" });
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
        public async Task<IActionResult> Withdraw([FromBody] WithdrawDto request)
        {
            try
            {
                var account = await _accountService.TGetAccountByUserId(request.userId);
                await _transactionHistoryService.Withdraw(account.ID, request.amount, request.code);
                return Ok(new { Message = "Para Çekme Başarılı" });
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
        public async Task<IActionResult> InitiateTransfer([FromBody] TransferRequestDto request)
        {
            try
            {
                var account = await _accountService.TGetAccountByUserId(request.userId);
                await _transactionHistoryService.InitiateTransfer(account.ID, request.recipientAccountNumber, request.amount);
                return Ok(new { Message = "Alıcınnın Ad Ve Soyadını Girin" });
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
        public async Task<IActionResult> Transfer([FromBody] TransferDto request)
        {
            try
            {
                var account = await _accountService.TGetAccountByUserId(request.userId);
                await _transactionHistoryService.Transfer(account.ID, request.recipientAccountNumber, request.amount, request.recipientName, request.description);
                return Ok(new { Message = "Transfer Başarıyla Tamamlandı" });
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

        [HttpGet("transaction-volume")]
        public async Task<IActionResult> GetTransactionVolumeLast24Hours()
        {
            var volume = await _transactionHistoryService.TGetTransactionVolumeLast24Hours();
            return Ok(volume);
        }
    }
}
