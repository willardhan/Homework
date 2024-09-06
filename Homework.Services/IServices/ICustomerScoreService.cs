using System.Collections.Generic;
using Homework.Services.Dtos;

namespace Homework.Services.IServices
{
    public interface ICustomerScoreService
    {
        /// <summary>
        /// add or update customer score
        /// </summary>
        /// <param name="customerId">customer id</param>
        /// <param name="score">score</param>
        /// <returns>if succeed</returns>
        bool AddOrUpdateScore(int customerId, decimal score);
        
        /// <summary>
        /// get customer score list by range
        /// </summary>
        /// <param name="start">start index</param>
        /// <param name="end">end index</param>
        /// <returns>customer score json list</returns>
        List<CustomerScoreDto> GetCustomerScoreDataByRange(int start, int end);

        /// <summary>
        /// get customer score list by customer and range
        /// </summary>
        /// <param name="customerId">customer id</param>
        /// <param name="start">start index</param>
        /// <param name="end">end index</param>
        /// <returns>customer score json list</returns>
        List<CustomerScoreDto> GetCustomerScoreDataByCustomerAndRange(int customerId, int? start = null, int? end = null);

        /// <summary>
        /// for unit test
        /// </summary>
        SortedSet<Leaderboard> GetClearLeaderboards();
    }
}