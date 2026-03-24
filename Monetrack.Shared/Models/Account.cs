using System;
using System.Collections.Generic;
using System.Text;

namespace Monetrack.Shared.Models
{
    public class Account
    {
        public int Id {  get; set; }
        public string UserId {  get; set; }
        public string Name { get; set; }
        public decimal Balance {  get; set; }
        public string Currency { get; set; } = "UAH";
    }
}
