using System.Collections.Generic;

namespace Homework.Services.Dtos
{
    /// <summary>
    /// leader board
    /// </summary>
    public class Leaderboard
    {
        public int CustomerId { get; set; }
        public decimal Score { get; set; }
    
        public Leaderboard(int customerId, decimal score)
        {
            CustomerId = customerId;
            Score = score;
        }
    }
    
    public class CustomerScoreComparer : IComparer<Leaderboard>
    {
        public int Compare(Leaderboard x, Leaderboard y)
        {
            int result = y.Score.CompareTo(x.Score); 
            if (result == 0)
            {
                return x.CustomerId.CompareTo(y.CustomerId);
            }
            return result;
        }
    }
}