using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.EntityLayer.Concreate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FinanceProject.ApplicationLayer.Exceptions;
using FinanceProject.ApplicationLayer.Dtos.TransactionHistoryDto;
using FinanceProject.Application.Models;

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
        public async Task<ActionResult> TransactionHistoryList()
        {
            var response = await _transactionHistoryService.TGetAllAsync();
            if (!response.isSuccess)
            {
                return BadRequest( response);
            }
            return Ok(response);
        }

        [HttpGet("admin/paged")]
        public async Task<ActionResult> GetPagedTransactionHistory(int page = 1, int pageSize = 6)
        {
            var response = await _transactionHistoryService.TGetAdminPagedTransactionHistoryAsync(page, pageSize);
            if (!response.isSuccess)
            {
                return BadRequest( response);
            }
            return Ok(response);
        }

        [HttpGet("last5")]
        public async Task<ActionResult> GetLastFiveTransactions()
        {
            var response = await _transactionHistoryService.TGetLastFiveTransactionsAsync();
            if (!response.isSuccess)
            {
                return BadRequest( response);
            }
            return Ok(response);
        }

        [HttpGet("lastUsers")]
        public async Task<ActionResult> GetLastTransferUsers()
        {
            var response = await _transactionHistoryService.TGetLast5TransfersAllUsersAsync();
            if (!response.isSuccess)
            {
                return BadRequest( response);
            }
            return Ok(response);
        }

        [HttpGet("last5/{userId}")]
        public async Task<ActionResult> GetLastFiveTransactions(int userId)
        {
            var accountResponse = await _accountService.TGetAccountByUserId(userId);
            if (!accountResponse.isSuccess)
            {
                return BadRequest( accountResponse);
            }

            var account = accountResponse.Result as Account;
            var transactions = await _transactionHistoryService.TGetLastFiveTransactionsAsync(account.ID);
            if (!transactions.isSuccess)
            {
                return BadRequest( transactions);
            }
            return Ok(transactions);
        }

        [HttpGet("lastUsers/{userId}")]
        public async Task<ActionResult> GetLastTransferUsers(int userId)
        {
            var accountResponse = await _accountService.TGetAccountByUserId(userId);
            if (!accountResponse.isSuccess)
            {
                return BadRequest( accountResponse);
            }

            var account = accountResponse.Result as Account;
            var transactions = await _transactionHistoryService.TGetLast5TransfersUsersAsync(account.ID);
            if (!transactions.isSuccess)
            {
                return BadRequest( transactions);
            }
            return Ok(transactions);
        }

        [HttpGet("totalamountlast24hours/{userId}")]
        public async Task<ActionResult> GetTotalAmountLast24Hours(int userId)
        {
            var accountResponse = await _accountService.TGetAccountByUserId(userId);
            if (!accountResponse.isSuccess)
            {
                return BadRequest( accountResponse);
            }

            var account = accountResponse.Result as Account;
            var totalAmount = await _transactionHistoryService.TGetTotalAmountLast24HoursAsync(account.ID);
            if (!totalAmount.isSuccess)
            {
                return BadRequest( totalAmount);
            }
            return Ok(totalAmount);
        }

        [HttpGet("paged/{userId}")]
        public async Task<ActionResult> GetPagedTransactionHistory(int userId, int page = 1, int pageSize = 6)
        {
            var accountResponse = await _accountService.TGetAccountByUserId(userId);
            if (!accountResponse.isSuccess)
            {
                return BadRequest( accountResponse);
            }

            var account = accountResponse.Result as Account;
            var transactions = await _transactionHistoryService.TGetPagedTransactionHistoryAsync(account.ID, page, pageSize);
            if (!transactions.isSuccess)
            {
                return BadRequest( transactions);
            }
            return Ok(transactions);
        }

        [HttpPost]
        public async Task<ActionResult> AddTransactionHistory(TransactionHistory transactionHistory)
        {
            var response = await _transactionHistoryService.TInsertAsync(transactionHistory);
            if (!response.isSuccess)
            {
                return BadRequest( response);
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTransactionHistory(int id)
        {
            var value = await _transactionHistoryService.TGetByIdAsync(id);
            if (!value.isSuccess)
            {
                return BadRequest( value);
            }

            var transaction = value.Result as TransactionHistory;
            var deleteResponse = await _transactionHistoryService.TDeleteAsync(transaction.ID);
            if (!deleteResponse.isSuccess)
            {
                return BadRequest(deleteResponse);
            }
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateTransactionHistory(TransactionHistory transactionHistory)
        {
            var response = await _transactionHistoryService.TUpdateAsync(transactionHistory);
            if (!response.isSuccess)
            {
                return BadRequest(response);
            }
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetTransactionHistory(int id)
        {
            var value = await _transactionHistoryService.TGetByIdAsync(id);
            if (!value.isSuccess)
            {
                return BadRequest(value);
            }
            return Ok(value);
        }

        [HttpPost("InitiateDeposit")]
        public async Task<ActionResult> InitiateDeposit([FromBody] DepositeRquestDto request)
        {
            var accountResponse = await _accountService.TGetAccountByUserId(request.userId);
            if (!accountResponse.isSuccess)
            {
                return BadRequest(accountResponse);
            }

            var account = accountResponse.Result as Account;
            var result = await _transactionHistoryService.InitiateDeposit(account.ID, request.amount);
            var code = result.Result as string;
            var response = await _userService.TGetByIdAsync(request.userId);
            if (!response.isSuccess)
            {
                return BadRequest(response);
            }

            var user = response.Result as User;
            var userEmailAddress = user.Email;

            var subject = "Deposit Verification Code";
            var body = $"Your deposit verification code is: {code}";

            await _emailService.SendEmailAsync(userEmailAddress, subject, body);

            return Ok(new { Message = "Mailinize gelen kodu giriniz" });
        }

        [HttpPost("InitiateWithdraw")]
        public async Task<ActionResult> InitiateWithdraw([FromBody] WithdrawRequestDto request)
        {
            var accountResponse = await _accountService.TGetAccountByUserId(request.userId);
            if (!accountResponse.isSuccess)
            {
                return BadRequest(accountResponse);
            }

            var account = accountResponse.Result as Account;
            var result = await _transactionHistoryService.InitiateWithdraw(account.ID, request.amount);
            var code = result.Result as string;
            if (!result.isSuccess)
            {
                return BadRequest(result);
            }

            var response = await _userService.TGetByIdAsync(request.userId);
            if (!response.isSuccess)
            {
                return BadRequest(response);
            }

            var user = response.Result as User;
            var userEmailAddress = user.Email;

            var subject = "Withdrawal Verification Code";
            var body = $"Your withdrawal verification code is: {code}"; // Ensure this is the code

            await _emailService.SendEmailAsync(userEmailAddress, subject, body);

            return Ok(new { Message = "Mailinize gelen kodu giriniz" });
        }


        [HttpPost("Deposit")]
        public async Task<ActionResult> Deposit([FromBody] DepositeDto request)
        {
            var accountResponse = await _accountService.TGetAccountByUserId(request.userId);
            if (!accountResponse.isSuccess)
            {
                return BadRequest(accountResponse);
            }

            var account = accountResponse.Result as Account;
            var response = await _transactionHistoryService.Deposit(account.ID, request.amount, request.code);
            if (!response.isSuccess)
            {
                return BadRequest(response);
            }
            return Ok(new { Message = "Para Yatırma Başarılı" });
        }

        [HttpPost("Withdraw")]
        public async Task<ActionResult> Withdraw([FromBody] WithdrawDto request)
        {
            var accountResponse = await _accountService.TGetAccountByUserId(request.userId);
            if (!accountResponse.isSuccess)
            {
                return BadRequest(accountResponse);
            }

            var account = accountResponse.Result as Account;
            var response = await _transactionHistoryService.Withdraw(account.ID, request.amount, request.code);
            if (!response.isSuccess)
            {
                return BadRequest(response);
            }
            return Ok(new { Message = "Para Çekme Başarılı" });
        }

        [HttpPost("initiate-transfer")]
        public async Task<ActionResult> InitiateTransfer([FromBody] TransferRequestDto request)
        {
            var accountResponse = await _accountService.TGetAccountByUserId(request.userId);
            if (!accountResponse.isSuccess)
            {
                return BadRequest(accountResponse);
            }

            var account = accountResponse.Result as Account;
            var response = await _transactionHistoryService.InitiateTransfer(account.ID, request.recipientAccountNumber, request.amount);
            if (!response.isSuccess)
            {
                return BadRequest(response);
            }
            return Ok(new { Message = "Alıcının Ad ve Soyadını Girin" });
        }

        [HttpPost("transfer")]
        public async Task<ActionResult> Transfer([FromBody] TransferDto request)
        {
            var accountResponse = await _accountService.TGetAccountByUserId(request.userId);
            if (!accountResponse.isSuccess)
            {
                return BadRequest(accountResponse);
            }

            var account = accountResponse.Result as Account;
            var response = await _transactionHistoryService.Transfer(account.ID, request.recipientAccountNumber, request.amount, request.recipientName, request.description);
            if (!response.isSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


        [HttpGet("transaction-volume")]
        public async Task<IActionResult> GetTransactionVolumeLast24Hours()
        {
            var volume = await _transactionHistoryService.TGetTransactionVolumeLast24Hours();
            if (!volume.isSuccess)
            {
                return BadRequest(volume);
            }
            return Ok(volume);
        }
    }
}
