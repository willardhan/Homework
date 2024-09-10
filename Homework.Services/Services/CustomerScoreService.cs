using System;
using System.Collections.Generic;
using System.Linq;
using Homework.Infrastructure.Attributes;
using Homework.Infrastructure.ComponentModels;
using Homework.Infrastructure.Exceptions;
using Homework.Infrastructure.Extensions;
using Homework.Services.Dtos;
using Homework.Services.IServices;
using Microsoft.Extensions.DependencyInjection;

namespace Homework.Services.Services
{
    [Service(ServiceLifetime.Scoped)]
    public class CustomerScoreService: ICustomerScoreService
    {
        public CustomerScoreService()
        {
        }

        /// <summary>
        /// add or update customer score
        /// </summary>
        /// <param name="customerId">customer id</param>
        /// <param name="score">score</param>
        /// <returns>if succeed</returns>
        public bool AddOrUpdateScore(int customerId, decimal score)
        {
            if (score < -1000 || score > 1000)
                throw new ValidationLevelException(GlobalParameter.AddOrUpdateScoreInputInvalidMsg);
            
            LeaderboardDataHelper.UpdateQueue.Enqueue(new Leaderboard(customerId, score));

            return true;
        }

        /// <summary>
        /// get customer score list by range
        /// </summary>
        /// <param name="start">start index</param>
        /// <param name="end">end index</param>
        /// <returns>customer score json list</returns>
        public List<CustomerScoreDto> GetCustomerScoreDataByRange(int start, int end)
        {
            var result = new List<CustomerScoreDto>();
            if (start <= 0) start = 1;
            if (start > end) return result;

            LeaderboardDataHelper.RwLock.EnterReadLock();  
            try  
            {  
                var skipCount = start - 1;
                var takeCount = end - start + 1;
                
                var items = LeaderboardDataHelper.Leaderboards.Skip(skipCount).Take(takeCount).ToList();
                if (items.HasElement())
                {
                    result = items.Select((m, i) => new CustomerScoreDto
                    {
                        CustomerId = m.CustomerId,
                        Score = m.Score,
                        Index = skipCount + i + 1
                    }).ToList();
                }
            }  
            finally  
            {  
                LeaderboardDataHelper.RwLock.ExitReadLock();  
            }

            return result;
        }
        

        /// <summary>
        /// get customer score list by customer and range
        /// </summary>
        /// <param name="customerId">customer id</param>
        /// <param name="start">start index</param>
        /// <param name="end">end index</param>
        /// <returns>customer score json list</returns>
        public List<CustomerScoreDto> GetCustomerScoreDataByCustomerAndRange(int customerId, int? start = null, int? end = null)
        {
            var result = new List<CustomerScoreDto>();

            var startIndex = start ?? 0;
            var endIndex = end ?? 0;
            if (startIndex < 0) startIndex = 0;
            if (endIndex < 0) endIndex = 0;

            LeaderboardDataHelper.RwLock.EnterReadLock();  
            try  
            {  
                if (!LeaderboardDataHelper.RankDictionary.ContainsKey(customerId)) return result;

                var customerIndex = LeaderboardDataHelper.RankDictionary[customerId]; 
                var skipCount = Math.Max(customerIndex - startIndex , 0);
                var takeCount = endIndex + customerIndex + 1 - skipCount;
                var items = LeaderboardDataHelper.Leaderboards.Skip(skipCount).Take(takeCount).ToList();
                if (items.HasElement())
                {
                    result = items.Select((m, i) => new CustomerScoreDto
                    {
                        CustomerId = m.CustomerId,
                        Score = m.Score,
                        Index = skipCount + i + 1
                    }).ToList();
                }
            }  
            finally  
            {  
                LeaderboardDataHelper.RwLock.ExitReadLock();  
            }  

            return result;
        }

        /// <summary>
        /// for unit test
        /// </summary>
        public SortedSet<Leaderboard> GetClearLeaderboards()
        {
            LeaderboardDataHelper.Leaderboards.Clear();
            LeaderboardDataHelper.ScoreDictionary.Clear();
            LeaderboardDataHelper.RankDictionary.Clear();
            LeaderboardDataHelper.UpdateQueue.Clear();
            return LeaderboardDataHelper.Leaderboards;
        }
    }
}