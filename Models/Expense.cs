namespace ExpenseTracker.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
