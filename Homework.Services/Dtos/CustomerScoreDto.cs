namespace Homework.Services.Dtos
{
    /// <summary>
    /// customer score dto
    /// </summary>
    public class CustomerScoreDto
    {
        public int CustomerId { get; set; }
        public decimal Score { get; set; }
        public int Index { get; set; }
    }
}