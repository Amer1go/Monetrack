using System;
using System.Collections.Generic;
using System.Text;

namespace Monetrack.Shared.Models
{
    public class Transaction
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public int AccountId { get; set; }
        public int CategoryId {  get; set; }
        public decimal Amount {  get; set; }
        public DateTime TransactionDate { set; get; }
        public string Note {  get; set; }
        public bool IsSynced { get; set; } = true;
    }
}
