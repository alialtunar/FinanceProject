using FinanceProject.BusinessLayer.Abstract;
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

        public TransactionHistoryController(ITransactionHistoryService transactionHistoryService)
        {
            _transactionHistoryService = transactionHistoryService;
        }

        [HttpGet]
        public IActionResult TransactionHistoryList()
        {
            var values = _transactionHistoryService.TGetAll();
            return Ok(values);
        }

        [HttpPost]

        public IActionResult AddTransactionHistory(TransactionHistory transactionHistory)
        {
            _transactionHistoryService.TInsert(transactionHistory);
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTransactionHistory(int id)
        {
            var value = _transactionHistoryService.TGetById(id);
            _transactionHistoryService.TDelete(value.ID);
            return Ok();
        }

        [HttpPut]
        public IActionResult UpdateTransactionHistory(TransactionHistory transactionHistory)
        {
            _transactionHistoryService.TUpdate(transactionHistory);
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetTransactionHistory(int id)
        {
            var value = _transactionHistoryService.TGetById(id);
            return Ok(value);
        }

    }
}
