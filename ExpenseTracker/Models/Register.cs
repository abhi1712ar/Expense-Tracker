using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    [Index(nameof(UserName), IsUnique = true)]
    [Index(nameof(EmailId), IsUnique = true)]
    public class Register
    {
        [Key]
        public int Id { get; set; }
        public string? UserName { get; set; } 
        public string? EmailId { get; set; }
        public string? Password { get; set; }
    }
}
