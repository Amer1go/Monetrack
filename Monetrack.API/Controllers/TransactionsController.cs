using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;
using Monetrack.Shared.Models; 

namespace Monetrack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IDbConnection _db;

        public TransactionsController(IDbConnection db)
        {
            _db = db;
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetUserTransactions(string userId)
        {
            try
            {
                string sql = "SELECT * FROM Transactions WHERE UserId = @UserId ORDER BY TransactionDate DESC";

                var transactions = _db.Query<Transaction>(sql, new { UserId = userId }).ToList();

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddTransaction([FromBody] Transaction transaction)
        {
            try
            {
                if (string.IsNullOrEmpty(transaction.Id))
                {
                    transaction.Id = Guid.NewGuid().ToString();
                }

                string sql = @"INSERT INTO Transactions (Id, UserId, AccountId, CategoryId, Amount, TransactionDate, Note, IsSynced) 
                               VALUES (@Id, @UserId, @AccountId, @CategoryId, @Amount, @TransactionDate, @Note, @IsSynced)";

                _db.Execute(sql, transaction);

                return Ok(new { message = "Транзакция успешно сохранена!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}