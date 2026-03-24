using System;
using System.Collections.Generic;
using System.Text;

namespace Monetrack.Shared.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string ColorHex {  get; set; }
        public string IconCode {  get; set; }
    }
}
