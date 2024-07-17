using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.BusinessLayer.Concreate;
using FinanceProject.EntityLayer.Concreate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        public TransactionHistoryController(ITransactionHistoryService transactionHistoryService,IEmailService emailService, IUserService userService, IAccountService accountService)
        {
            _transactionHistoryService = transactionHistoryService;
            _emailService = emailService;
            _userService = userService;
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult TransactionHistoryList()
        {
            var values = _transactionHistoryService.TGetAllAsync();
            return Ok(values);
        }

        [HttpPost]

        public IActionResult AddTransactionHistory(TransactionHistory transactionHistory)
        {
            _transactionHistoryService.TInsertAsync(transactionHistory);
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTransactionHistory(int id)
        {
            var value = _transactionHistoryService.TGetByIdAsync(id);
            _transactionHistoryService.TDeleteAsync(value.Id);
            return Ok();
        }

        [HttpPut]
        public IActionResult UpdateTransactionHistory(TransactionHistory transactionHistory)
        {
            _transactionHistoryService.TUpdateAsync(transactionHistory);
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetTransactionHistory(int id)
        {
            var value = _transactionHistoryService.TGetByIdAsync(id);
            return Ok(value);
        }
        [HttpPost("InitiateDeposit")]
        public async Task<IActionResult> InitiateDeposit(int accountId, decimal amount, string description = null)
        {
            var code = await _transactionHistoryService.InitiateDeposit(accountId, amount, description);

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

        [HttpPost("InitiateWithdraw")]
        public async Task<IActionResult> InitiateWithdraw(int accountId, decimal amount, string description = null)
        {
            var code = await _transactionHistoryService.InitiateWithdraw(accountId, amount, description);

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



        [HttpPost("Deposit")]
        public async Task<IActionResult> Deposit(int accountId, decimal amount, string code, string description = null)
        {
            await _transactionHistoryService.Deposit(accountId, amount, code, description);
            return Ok();
        }

        [HttpPost("Withdraw")]
        public async Task<IActionResult> Withdraw(int accountId, decimal amount, string code, string description = null)
        {
            await _transactionHistoryService.Withdraw(accountId, amount, code, description);
            return Ok();
        }
    }

}

