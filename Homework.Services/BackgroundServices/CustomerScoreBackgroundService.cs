using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Homework.Infrastructure.Extensions;
using Homework.Services.Dtos;
using Microsoft.Extensions.Hosting;

namespace Homework.Services.BackgroundServices
{
    public class CustomerScoreBackgroundService: BackgroundService  
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                UpdateCustomerScore();

                //wait to loop
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }  
            
            LeaderboardDataHelper.Leaderboards.Clear();
            LeaderboardDataHelper.ScoreDictionary.Clear();
            LeaderboardDataHelper.RankDictionary.Clear();
            LeaderboardDataHelper.UpdateQueue.Clear();
        }

        public static void UpdateCustomerScore()
        {
            //read update queue
            var count = LeaderboardDataHelper.UpdateQueue.Count;

            //batch handle input
            var inputDic = new Dictionary<int, decimal>();
            for (int i = 0; i < count; i++)
            {
                if (LeaderboardDataHelper.UpdateQueue.TryDequeue(out Leaderboard leaderboard))
                {
                    if (inputDic.ContainsKey(leaderboard.CustomerId))
                    {
                        inputDic[leaderboard.CustomerId] += leaderboard.Score;
                    }
                    else
                    {
                        inputDic.Add(leaderboard.CustomerId, leaderboard.Score);
                    }
                }
            }

            //batch handle update
            if (inputDic.HasElement())
            {
                LeaderboardDataHelper.RwLock.EnterWriteLock();
                try
                {
                    var needUpdateIndex = true;
                    foreach (var item in inputDic)
                    {
                        var customerId = item.Key;
                        var score = item.Value;
                        if (LeaderboardDataHelper.ScoreDictionary.ContainsKey(customerId)) // do update
                        {
                            // need to do whether update or remove in leader board 
                            LeaderboardDataHelper.Leaderboards.RemoveWhere(entry => entry.CustomerId == customerId);
                            var oldScore = LeaderboardDataHelper.ScoreDictionary[customerId];
                            var newScore = oldScore + score;
                            if (newScore > 0) // update when score is greater than zero
                            {
                                LeaderboardDataHelper.ScoreDictionary[customerId] = newScore;
                                LeaderboardDataHelper.Leaderboards.Add(new Leaderboard(customerId, newScore));
                            }
                            else // remove when score is not greater than zero
                            {
                                LeaderboardDataHelper.ScoreDictionary.Remove(customerId, out _);
                            }
                        }
                        else if (score > 0) // do add
                        {
                            LeaderboardDataHelper.ScoreDictionary.Add(customerId, score);
                            LeaderboardDataHelper.Leaderboards.Add(new Leaderboard(customerId, score));
                        }
                        else // do noting
                        {
                            needUpdateIndex = false;
                        }
                    }

                    if (needUpdateIndex) // rebuild customer and rank dictionary
                    {
                        LeaderboardDataHelper.RankDictionary = LeaderboardDataHelper.Leaderboards
                            .Select((m, i) => new {CustomerId = m.CustomerId, Index = i})
                            .ToDictionary(m => m.CustomerId, n => n.Index);
                    }
                }
                finally
                {
                    LeaderboardDataHelper.RwLock.ExitWriteLock();
                }
            }
        }
    }
}