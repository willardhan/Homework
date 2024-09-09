using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        /// <summary>
        /// sort leader board
        /// </summary>
        private static SortedSet<Leaderboard> Leaderboards = new(new CustomerScoreComparer());
        /// <summary>
        /// customer and score dictionary
        /// fast query: score by customer id
        /// </summary>
        private static Dictionary<int, decimal> ScoreDictionary = new();
        /// <summary>
        /// customer and rank dictionary
        /// fast query: rank in leader board 
        /// </summary>
        private static Dictionary<int, int> RankDictionary = new();
        private static ReaderWriterLockSlim _rwLock = new();

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
            var result = false;
            var needUpdateIndex = true;

            if (score < -1000 || score > 1000)
                throw new ValidationLevelException(GlobalParameter.AddOrUpdateScoreInputInvalidMsg);
            
            _rwLock.EnterWriteLock();  
            try  
            {  
                if (ScoreDictionary.ContainsKey(customerId)) // do update
                {
                    // need to do whether update or remove in leader board 
                    Leaderboards.RemoveWhere(entry => entry.CustomerId == customerId);
                    var oldScore = ScoreDictionary[customerId];
                    var newScore = oldScore + score;
                    if (newScore > 0) // update when score is greater than zero
                    {
                        ScoreDictionary[customerId] = newScore;
                        result = Leaderboards.Add(new Leaderboard(customerId, newScore));
                    }
                    else // remove when score is not greater than zero
                    {
                        ScoreDictionary.Remove(customerId, out _);
                    }
                }
                else if(score > 0) // do add
                {
                    ScoreDictionary.Add(customerId, score);
                    result = Leaderboards.Add(new Leaderboard(customerId, score));
                }
                else // do noting
                {
                    needUpdateIndex = false;
                }

                if (needUpdateIndex) // rebuild customer and rank dictionary
                {
                    RankDictionary = Leaderboards.Select((m, i) => new {CustomerId = m.CustomerId, Index = i})
                        .ToDictionary(m => m.CustomerId, n => n.Index);
                }
            }  
            finally  
            {  
                _rwLock.ExitWriteLock();  
            }  

            return result;
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

            _rwLock.EnterReadLock();  
            try  
            {  
                var skipCount = start - 1;
                var takeCount = end - start + 1;
                var items = Leaderboards.Skip(skipCount).Take(takeCount).ToList();
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
                _rwLock.ExitReadLock();  
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

            _rwLock.EnterReadLock();  
            try  
            {  
                if (!RankDictionary.ContainsKey(customerId)) return result;

                var customerIndex = RankDictionary[customerId]; 
                var skipCount = Math.Max(customerIndex - startIndex , 0);
                var takeCount = endIndex + customerIndex + 1 - skipCount;
                var items = Leaderboards.Skip(skipCount).Take(takeCount).ToList();
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
                _rwLock.ExitReadLock();  
            }  

            return result;
        }

        /// <summary>
        /// for unit test
        /// </summary>
        public SortedSet<Leaderboard> GetClearLeaderboards()
        {
            Leaderboards.Clear();
            ScoreDictionary.Clear();
            RankDictionary.Clear();
            return Leaderboards;
        }
    }
}