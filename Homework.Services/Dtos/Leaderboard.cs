using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

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

    public static class LeaderboardDataHelper
    {
        /// <summary>
        /// sort leader board
        /// </summary>
        public static SortedSet<Leaderboard> Leaderboards = new(new CustomerScoreComparer());
        /// <summary>
        /// customer and score dictionary
        /// fast query: score by customer id
        /// </summary>
        public static Dictionary<int, decimal> ScoreDictionary = new();
        /// <summary>
        /// customer and rank dictionary
        /// fast query: rank in leader board 
        /// </summary>
        public static Dictionary<int, int> RankDictionary = new();
        /// <summary>
        /// update queue
        /// </summary>
        public static ConcurrentQueue<Leaderboard> UpdateQueue = new();

        public static ReaderWriterLockSlim RwLock = new();
    }
}