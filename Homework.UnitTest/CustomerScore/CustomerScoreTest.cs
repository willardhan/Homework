using System;
using System.Collections.Generic;
using System.Linq;
using Homework.Infrastructure.ComponentModels;
using Homework.Services.IServices;
using Xunit;
using Xunit.Abstractions;

namespace Homework.UnitTest.CustomerScore
{
    public class CustomerScoreTest: BaseService
    {
        private readonly ICustomerScoreService _customerScoreService;
        public CustomerScoreTest(
            ITestOutputHelper iTestOutputHelper,
            ICustomerScoreService customerScoreService
            ): base(iTestOutputHelper)
        {
            _customerScoreService = customerScoreService;
        }

        public static IEnumerable<object[]> CustomerScoreAddForEmpty
        {
            get
            {
                return new List<object[]>
                {
                    new object[]{100001, 0.9},
                    new object[]{100002, 100.99},
                    new object[]{5, 2},
                    new object[]{555666688, 999.999},
                    new object[]{new Random().Next(1,Int32.MaxValue), new Random().Next(0, 1000)},
                };
            }
        }
        
        [Theory(DisplayName = "Given Empty When Add Customer Score Then Success")]
        [MemberData(nameof(CustomerScoreAddForEmpty))]
        public void GivenEmptyWhenAddCustomerScoreThenSuccess(int customerId, decimal score)
        {
            // Arrange
            var leaderboards = _customerScoreService.GetClearLeaderboards();

            //Act
            _customerScoreService.AddOrUpdateScore(customerId, score);
            
            // Assert
            Assert.Equal(1, leaderboards.Count);
            Assert.Equal(customerId, leaderboards.First().CustomerId);
            Assert.Equal(score, leaderboards.First().Score);
        }
        
        public static IEnumerable<object[]> InvalidCustomerScoreAddForEmpty
        {
            get
            {
                return new List<object[]>
                {
                    new object[]{1000, -100},
                    new object[]{10123, 0},
                    new object[]{555666888, 2000},
                    new object[]{555666, -1800},
                    new object[]{new Random().Next(1,Int32.MaxValue), new Random().Next(1,Int32.MaxValue)},
                };
            }
        }
        
        [Theory(DisplayName = "Given Empty When Add Invalid Customer Score Then Fail")]
        [MemberData(nameof(InvalidCustomerScoreAddForEmpty))]
        public void GivenEmptyWhenAddInvalidCustomerScoreThenFail(int customerId, decimal score)
        {
            // Arrange
            var leaderboards = _customerScoreService.GetClearLeaderboards();

            try
            {
                //Act
                _customerScoreService.AddOrUpdateScore(customerId, score);
            }
            catch (Exception e)
            {
                // Assert
                Assert.Equal(GlobalParameter.AddOrUpdateScoreInputInvalidMsg, e.Message);
            }
            finally
            {
                // Assert
                Assert.Equal(0, leaderboards.Count);
            }
        }
        
        public static IEnumerable<object[]> CustomerScoreAddForNotEmpty
        {
            get
            {
                return new List<object[]>
                {
                    new object[]{100000, 1},
                    new object[]{100001, 0.9},
                    new object[]{100002, 100.99},
                    new object[]{5, 2},
                    new object[]{555666688, 999.999},
                };
            }
        }
        
        [Theory(DisplayName = "Given Not Empty When Add Customer Score Then Success")]
        [MemberData(nameof(CustomerScoreAddForNotEmpty))]
        public void GivenNotEmptyWhenAddCustomerScoreThenSuccess(int customerId, decimal score)
        {
            // Arrange
            var leaderboards = _customerScoreService.GetClearLeaderboards();
            _customerScoreService.AddOrUpdateScore(2, 1);

            //Act
            _customerScoreService.AddOrUpdateScore(customerId, score);
            
            // Assert
            Assert.Equal(2, leaderboards.Count);

            var item = leaderboards.FirstOrDefault(m => m.CustomerId == customerId);
            Assert.NotNull(item);
            Assert.Equal(customerId, item.CustomerId);
            Assert.Equal(score, item.Score);
        }
        
        public static IEnumerable<object[]> InvalidCustomerScoreAddForNotEmpty
        {
            get
            {
                return new List<object[]>
                {
                    new object[]{1000, -100},
                    new object[]{10123, 0},
                    new object[]{555666888, 2000},
                    new object[]{555666, -1800},
                };
            }
        }
        
        [Theory(DisplayName = "Given Not Empty When Add Invalid Customer Score Then Fail")]
        [MemberData(nameof(InvalidCustomerScoreAddForNotEmpty))]
        public void GivenNotEmptyWhenAddInvalidCustomerScoreThenFail(int customerId, decimal score)
        {
            // Arrange
            var leaderboards = _customerScoreService.GetClearLeaderboards();
            _customerScoreService.AddOrUpdateScore(2, 1);

            try
            {
                //Act
                _customerScoreService.AddOrUpdateScore(customerId, score);
            }
            catch (Exception e)
            {
                // Assert
                Assert.Equal(GlobalParameter.AddOrUpdateScoreInputInvalidMsg, e.Message);
            }
            finally
            {
                Assert.Equal(1, leaderboards.Count);
                Assert.Equal(2, leaderboards.First().CustomerId);
                Assert.Equal(1, leaderboards.First().Score);
            }
        }
        
        [Fact(DisplayName = "Given Customer Score When Update Score Then Success")]
        public void GivenCustomerScoreWhenUpdateScoreThenSuccess()
        {
            // Arrange
            var customerId = 10001;
            var score = 100;

            var updateScore1 = 10;
            var updateScore2 = 20;
            var updateScore3 = 30;
            var updateScore4 = -5;
            var updateScore5 = 200;
            var updateScore6 = -100;
            var updateScore7 = 0;
            var updateScore8 = -500;
            
            var leaderboards = _customerScoreService.GetClearLeaderboards();
            _customerScoreService.AddOrUpdateScore(customerId, score);
            _customerScoreService.AddOrUpdateScore(10002, 2);

            //Act
            _customerScoreService.AddOrUpdateScore(customerId, updateScore1);
            var item = leaderboards.FirstOrDefault(m => m.CustomerId == customerId);
            // Assert
            Assert.NotNull(item);
            Assert.Equal(2, leaderboards.Count);
            Assert.Equal(customerId, item.CustomerId);
            Assert.Equal(110, item.Score);

            
            //Act
            _customerScoreService.AddOrUpdateScore(customerId, updateScore2);
            item = leaderboards.FirstOrDefault(m => m.CustomerId == customerId);
            // Assert
            Assert.NotNull(item);
            Assert.Equal(2, leaderboards.Count);
            Assert.Equal(customerId, item.CustomerId);
            Assert.Equal(130, item.Score);

            
            //Act
            _customerScoreService.AddOrUpdateScore(customerId, updateScore3);
            item = leaderboards.FirstOrDefault(m => m.CustomerId == customerId);
            // Assert
            Assert.NotNull(item);
            Assert.Equal(2, leaderboards.Count);
            Assert.Equal(customerId, item.CustomerId);
            Assert.Equal(160, item.Score);
            
            
            //Act
            _customerScoreService.AddOrUpdateScore(customerId, updateScore4);
            item = leaderboards.FirstOrDefault(m => m.CustomerId == customerId);
            // Assert
            Assert.NotNull(item);
            Assert.Equal(2, leaderboards.Count);
            Assert.Equal(customerId, item.CustomerId);
            Assert.Equal(155, item.Score);
            
            
            //Act
            _customerScoreService.AddOrUpdateScore(customerId, updateScore5);
            item = leaderboards.FirstOrDefault(m => m.CustomerId == customerId);
            // Assert
            Assert.NotNull(item);
            Assert.Equal(2, leaderboards.Count);
            Assert.Equal(customerId, item.CustomerId);
            Assert.Equal(355, item.Score);
            
            
            //Act
            _customerScoreService.AddOrUpdateScore(customerId, updateScore6);
            item = leaderboards.FirstOrDefault(m => m.CustomerId == customerId);
            // Assert
            Assert.NotNull(item);
            Assert.Equal(2, leaderboards.Count);
            Assert.Equal(customerId, item.CustomerId);
            Assert.Equal(255, item.Score);
            
            
            //Act
            _customerScoreService.AddOrUpdateScore(customerId, updateScore7);
            item = leaderboards.FirstOrDefault(m => m.CustomerId == customerId);
            // Assert
            Assert.NotNull(item);
            Assert.Equal(2, leaderboards.Count);
            Assert.Equal(customerId, item.CustomerId);
            Assert.Equal(255, item.Score);
            
            //Act
            _customerScoreService.AddOrUpdateScore(customerId, updateScore8);
            item = leaderboards.FirstOrDefault(m => m.CustomerId == customerId);
            // Assert
            Assert.Null(item);
            Assert.Equal(1, leaderboards.Count);
        }
        
        [Fact(DisplayName = "Given Customer Scores When Get By All Rank Then Return Result")]
        public void GivenCustomerScoresWhenGetByAllRankThenReturnResult()
        {
            // Arrange
            var customerScores = new Dictionary<int, decimal>
            {
                {10001, 60},
                {10002, 65},
                {10003, 55},
                {10004, 100},
                {10005, 65},
                {10006, 60},
                {10007, 150},
                {10008, 10},
                {10009, 200},
                {10010, 1},
            };
            _customerScoreService.GetClearLeaderboards();
            foreach (var pair in customerScores)
            {
                _customerScoreService.AddOrUpdateScore(pair.Key, pair.Value);
            }

            //Act
            var items = _customerScoreService.GetCustomerScoreDataByRange(1, 10);

            // Assert
            Assert.Equal(10, items.Count);
            
            Assert.Equal(10009, items[0].CustomerId);
            Assert.Equal(200, items[0].Score);
            Assert.Equal(1, items[0].Index);

            Assert.Equal(10007, items[1].CustomerId);
            Assert.Equal(150, items[1].Score);
            Assert.Equal(2, items[1].Index);
                        
            Assert.Equal(10004, items[2].CustomerId);
            Assert.Equal(100, items[2].Score);
            Assert.Equal(3, items[2].Index);
                        
            Assert.Equal(10002, items[3].CustomerId);
            Assert.Equal(65, items[3].Score);
            Assert.Equal(4, items[3].Index);

            Assert.Equal(10005, items[4].CustomerId);
            Assert.Equal(65, items[4].Score);
            Assert.Equal(5, items[4].Index);
            
            Assert.Equal(10001, items[5].CustomerId);
            Assert.Equal(60, items[5].Score);
            Assert.Equal(6, items[5].Index);
            
            Assert.Equal(10006, items[6].CustomerId);
            Assert.Equal(60, items[6].Score);
            Assert.Equal(7, items[6].Index);
                        
            Assert.Equal(10003, items[7].CustomerId);
            Assert.Equal(55, items[7].Score);
            Assert.Equal(8, items[7].Index);
                        
            Assert.Equal(10008, items[8].CustomerId);
            Assert.Equal(10, items[8].Score);
            Assert.Equal(9, items[8].Index);
            
            Assert.Equal(10010, items[9].CustomerId);
            Assert.Equal(1, items[9].Score);
            Assert.Equal(10, items[9].Index);
        }
        
        [Fact(DisplayName = "Given Customer Scores When Get By Middle Rank Then Return Result")]
        public void GivenCustomerScoresWhenGetByMiddleRankThenReturnResult()
        {
            // Arrange
            var customerScores = new Dictionary<int, decimal>
            {
                {10001, 60},
                {10002, 65},
                {10003, 55},
                {10004, 100},
                {10005, 65},
                {10006, 60},
                {10007, 150},
                {10008, 10},
                {10009, 200},
                {10010, 1},
            };
            _customerScoreService.GetClearLeaderboards();
            foreach (var pair in customerScores)
            {
                _customerScoreService.AddOrUpdateScore(pair.Key, pair.Value);
            }

            //Act
            var items = _customerScoreService.GetCustomerScoreDataByRange(2, 6);

            // Assert
            Assert.Equal(5, items.Count);

            Assert.Equal(10007, items[0].CustomerId);
            Assert.Equal(150, items[0].Score);
            Assert.Equal(2, items[0].Index);
                        
            Assert.Equal(10004, items[1].CustomerId);
            Assert.Equal(100, items[1].Score);
            Assert.Equal(3, items[1].Index);
                        
            Assert.Equal(10002, items[2].CustomerId);
            Assert.Equal(65, items[2].Score);
            Assert.Equal(4, items[2].Index);

            Assert.Equal(10005, items[3].CustomerId);
            Assert.Equal(65, items[3].Score);
            Assert.Equal(5, items[3].Index);
            
            Assert.Equal(10001, items[4].CustomerId);
            Assert.Equal(60, items[4].Score);
            Assert.Equal(6, items[4].Index);
            
            
            //Act
            items = _customerScoreService.GetCustomerScoreDataByRange(3, 3);

            // Assert
            Assert.Equal(1, items.Count);
            Assert.Equal(10004, items[0].CustomerId);
            Assert.Equal(100, items[0].Score);
            Assert.Equal(3, items[0].Index);
        }
        
        [Fact(DisplayName = "Given Customer Scores When Get By Boundary Rank Then Return Result")]
        public void GivenCustomerScoresWhenGetByBoundaryRankThenReturnResult()
        {
            // Arrange
            var customerScores = new Dictionary<int, decimal>
            {
                {10001, 60},
                {10002, 65},
                {10003, 55},
                {10004, 100},
                {10005, 65},
                {10006, 60},
                {10007, 150},
                {10008, 10},
                {10009, 200},
                {10010, 1},
            };
            _customerScoreService.GetClearLeaderboards();
            foreach (var pair in customerScores)
            {
                _customerScoreService.AddOrUpdateScore(pair.Key, pair.Value);
            }

            //Act
            var items = _customerScoreService.GetCustomerScoreDataByRange(-10, 3);

            // Assert
            Assert.Equal(3, items.Count);

            Assert.Equal(10009, items[0].CustomerId);
            Assert.Equal(200, items[0].Score);
            Assert.Equal(1, items[0].Index);
            
            Assert.Equal(10007, items[1].CustomerId);
            Assert.Equal(150, items[1].Score);
            Assert.Equal(2, items[1].Index);
                        
            Assert.Equal(10004, items[2].CustomerId);
            Assert.Equal(100, items[2].Score);
            Assert.Equal(3, items[2].Index);
            
            
            //Act
            items = _customerScoreService.GetCustomerScoreDataByRange(7, 15);

            // Assert
            Assert.Equal(4, items.Count);

            Assert.Equal(10006, items[0].CustomerId);
            Assert.Equal(60, items[0].Score);
            Assert.Equal(7, items[0].Index);
                        
            Assert.Equal(10003, items[1].CustomerId);
            Assert.Equal(55, items[1].Score);
            Assert.Equal(8, items[1].Index);
                        
            Assert.Equal(10008, items[2].CustomerId);
            Assert.Equal(10, items[2].Score);
            Assert.Equal(9, items[2].Index);
            
            Assert.Equal(10010, items[3].CustomerId);
            Assert.Equal(1, items[3].Score);
            Assert.Equal(10, items[3].Index);
        }
        
        [Fact(DisplayName = "Given Customer Scores When Get By Invalid Rank Then Return Empty")]
        public void GivenCustomerScoresWhenGetByInvalidRankThenReturnEmpty()
        {
            // Arrange
            var customerScores = new Dictionary<int, decimal>
            {
                {10001, 60},
                {10002, 65},
                {10003, 55},
                {10004, 100},
                {10005, 65},
                {10006, 60},
                {10007, 150},
                {10008, 10},
                {10009, 200},
                {10010, 1},
            };
            _customerScoreService.GetClearLeaderboards();
            foreach (var pair in customerScores)
            {
                _customerScoreService.AddOrUpdateScore(pair.Key, pair.Value);
            }

            //Act
            var items = _customerScoreService.GetCustomerScoreDataByRange(5, 3);

            // Assert
            Assert.Empty(items);
            
            
            //Act
            items = _customerScoreService.GetCustomerScoreDataByRange(11, 20);

            // Assert
            Assert.Empty(items);
        }
        
        [Fact(DisplayName = "Given Customer Scores When Get By Customer Then Return Result")]
        public void GivenCustomerScoresWhenGetByCustomerThenReturnResult()
        {
            // Arrange
            var customerScores = new Dictionary<int, decimal>
            {
                {10001, 60},
                {10002, 65},
                {10003, 55},
                {10004, 100},
                {10005, 65},
                {10006, 60},
                {10007, 150},
                {10008, 10},
                {10009, 200},
                {10010, 1},
            };
            _customerScoreService.GetClearLeaderboards();
            foreach (var pair in customerScores)
            {
                _customerScoreService.AddOrUpdateScore(pair.Key, pair.Value);
            }

            //Act
            var items = _customerScoreService.GetCustomerScoreDataByCustomerAndRange(10009);

            // Assert
            Assert.Equal(1, items.Count);
            Assert.Equal(10009, items[0].CustomerId);
            Assert.Equal(200, items[0].Score);
            Assert.Equal(1, items[0].Index);
            
            
            //Act
            items = _customerScoreService.GetCustomerScoreDataByCustomerAndRange(10001);

            // Assert
            Assert.Equal(1, items.Count);
            Assert.Equal(10001, items[0].CustomerId);
            Assert.Equal(60, items[0].Score);
            Assert.Equal(6, items[0].Index);
        }
        
        [Fact(DisplayName = "Given Customer Scores When Get By Customer And Start Rank Then Return Result")]
        public void GivenCustomerScoresWhenGetByCustomerAndStartRankThenReturnResult()
        {
            // Arrange
            var customerScores = new Dictionary<int, decimal>
            {
                {10001, 60},
                {10002, 65},
                {10003, 55},
                {10004, 100},
                {10005, 65},
                {10006, 60},
                {10007, 150},
                {10008, 10},
                {10009, 200},
                {10010, 1},
            };
            _customerScoreService.GetClearLeaderboards();
            foreach (var pair in customerScores)
            {
                _customerScoreService.AddOrUpdateScore(pair.Key, pair.Value);
            }

            //Act
            var items = _customerScoreService.GetCustomerScoreDataByCustomerAndRange(10001, 2);

            // Assert
            Assert.Equal(3, items.Count);
            
            Assert.Equal(10002, items[0].CustomerId);
            Assert.Equal(65, items[0].Score);
            Assert.Equal(4, items[0].Index);

            Assert.Equal(10005, items[1].CustomerId);
            Assert.Equal(65, items[1].Score);
            Assert.Equal(5, items[1].Index);
            
            Assert.Equal(10001, items[2].CustomerId);
            Assert.Equal(60, items[2].Score);
            Assert.Equal(6, items[2].Index);
        }
        
        [Fact(DisplayName = "Given Customer Scores When Get By Customer And Boundary Start Rank Then Return Result")]
        public void GivenCustomerScoresWhenGetByCustomerAndBoundaryStartRankThenReturnResult()
        {
            // Arrange
            var customerScores = new Dictionary<int, decimal>
            {
                {10001, 60},
                {10002, 65},
                {10003, 55},
                {10004, 100},
                {10005, 65},
                {10006, 60},
                {10007, 150},
                {10008, 10},
                {10009, 200},
                {10010, 1},
            };
            _customerScoreService.GetClearLeaderboards();
            foreach (var pair in customerScores)
            {
                _customerScoreService.AddOrUpdateScore(pair.Key, pair.Value);
            }

            //Act
            var items = _customerScoreService.GetCustomerScoreDataByCustomerAndRange(10001, 10);

            // Assert
            Assert.Equal(6, items.Count);
            
            Assert.Equal(10009, items[0].CustomerId);
            Assert.Equal(200, items[0].Score);
            Assert.Equal(1, items[0].Index);

            Assert.Equal(10007, items[1].CustomerId);
            Assert.Equal(150, items[1].Score);
            Assert.Equal(2, items[1].Index);
                        
            Assert.Equal(10004, items[2].CustomerId);
            Assert.Equal(100, items[2].Score);
            Assert.Equal(3, items[2].Index);
                        
            Assert.Equal(10002, items[3].CustomerId);
            Assert.Equal(65, items[3].Score);
            Assert.Equal(4, items[3].Index);

            Assert.Equal(10005, items[4].CustomerId);
            Assert.Equal(65, items[4].Score);
            Assert.Equal(5, items[4].Index);
            
            Assert.Equal(10001, items[5].CustomerId);
            Assert.Equal(60, items[5].Score);
            Assert.Equal(6, items[5].Index);
            
            
            //Act
            items = _customerScoreService.GetCustomerScoreDataByCustomerAndRange(10001, -10);

            // Assert
            Assert.Equal(1, items.Count);

            Assert.Equal(10001, items[0].CustomerId);
            Assert.Equal(60, items[0].Score);
            Assert.Equal(6, items[0].Index);
        }
        
        [Fact(DisplayName = "Given Customer Scores When Get By Customer And End Rank Then Return Result")]
        public void GivenCustomerScoresWhenGetByCustomerAndEndRankThenReturnResult()
        {
            // Arrange
            var customerScores = new Dictionary<int, decimal>
            {
                {10001, 60},
                {10002, 65},
                {10003, 55},
                {10004, 100},
                {10005, 65},
                {10006, 60},
                {10007, 150},
                {10008, 10},
                {10009, 200},
                {10010, 1},
            };
            _customerScoreService.GetClearLeaderboards();
            foreach (var pair in customerScores)
            {
                _customerScoreService.AddOrUpdateScore(pair.Key, pair.Value);
            }

            //Act
            var items = _customerScoreService.GetCustomerScoreDataByCustomerAndRange(10001, end: 2);

            // Assert
            Assert.Equal(3, items.Count);

            Assert.Equal(10001, items[0].CustomerId);
            Assert.Equal(60, items[0].Score);
            Assert.Equal(6, items[0].Index);
            
            Assert.Equal(10006, items[1].CustomerId);
            Assert.Equal(60, items[1].Score);
            Assert.Equal(7, items[1].Index);
                        
            Assert.Equal(10003, items[2].CustomerId);
            Assert.Equal(55, items[2].Score);
            Assert.Equal(8, items[2].Index);
        }
        
        [Fact(DisplayName = "Given Customer Scores When Get By Customer And Boundary End Rank Then Return Result")]
        public void GivenCustomerScoresWhenGetByCustomerAndBoundaryEndRankThenReturnResult()
        {
            // Arrange
            var customerScores = new Dictionary<int, decimal>
            {
                {10001, 60},
                {10002, 65},
                {10003, 55},
                {10004, 100},
                {10005, 65},
                {10006, 60},
                {10007, 150},
                {10008, 10},
                {10009, 200},
                {10010, 1},
            };
            _customerScoreService.GetClearLeaderboards();
            foreach (var pair in customerScores)
            {
                _customerScoreService.AddOrUpdateScore(pair.Key, pair.Value);
            }

            //Act
            var items = _customerScoreService.GetCustomerScoreDataByCustomerAndRange(10001, end: 10);

            // Assert
            Assert.Equal(5, items.Count);
            
            Assert.Equal(10001, items[0].CustomerId);
            Assert.Equal(60, items[0].Score);
            Assert.Equal(6, items[0].Index);
            
            Assert.Equal(10006, items[1].CustomerId);
            Assert.Equal(60, items[1].Score);
            Assert.Equal(7, items[1].Index);
                        
            Assert.Equal(10003, items[2].CustomerId);
            Assert.Equal(55, items[2].Score);
            Assert.Equal(8, items[2].Index);
                        
            Assert.Equal(10008, items[3].CustomerId);
            Assert.Equal(10, items[3].Score);
            Assert.Equal(9, items[3].Index);
            
            Assert.Equal(10010, items[4].CustomerId);
            Assert.Equal(1, items[4].Score);
            Assert.Equal(10, items[4].Index);
            
            
            //Act
            items = _customerScoreService.GetCustomerScoreDataByCustomerAndRange(10001, end: -10);

            // Assert
            Assert.Equal(1, items.Count);

            Assert.Equal(10001, items[0].CustomerId);
            Assert.Equal(60, items[0].Score);
            Assert.Equal(6, items[0].Index);
        }
        
        [Fact(DisplayName = "Given Customer Scores When Get By Customer And Rank Then Return Result")]
        public void GivenCustomerScoresWhenGetByCustomerAndRankThenReturnResult()
        {
            // Arrange
            var customerScores = new Dictionary<int, decimal>
            {
                {10001, 60},
                {10002, 65},
                {10003, 55},
                {10004, 100},
                {10005, 65},
                {10006, 60},
                {10007, 150},
                {10008, 10},
                {10009, 200},
                {10010, 1},
            };
            _customerScoreService.GetClearLeaderboards();
            foreach (var pair in customerScores)
            {
                _customerScoreService.AddOrUpdateScore(pair.Key, pair.Value);
            }

            //Act
            var items = _customerScoreService.GetCustomerScoreDataByCustomerAndRange(10001, 2, 2);

            // Assert
            Assert.Equal(5, items.Count);
            
            Assert.Equal(10002, items[0].CustomerId);
            Assert.Equal(65, items[0].Score);
            Assert.Equal(4, items[0].Index);

            Assert.Equal(10005, items[1].CustomerId);
            Assert.Equal(65, items[1].Score);
            Assert.Equal(5, items[1].Index);
            
            Assert.Equal(10001, items[2].CustomerId);
            Assert.Equal(60, items[2].Score);
            Assert.Equal(6, items[2].Index);
            
            Assert.Equal(10006, items[3].CustomerId);
            Assert.Equal(60, items[3].Score);
            Assert.Equal(7, items[3].Index);
                        
            Assert.Equal(10003, items[4].CustomerId);
            Assert.Equal(55, items[4].Score);
            Assert.Equal(8, items[4].Index);
        }
        
        [Fact(DisplayName = "Given Customer Scores When Get By Customer And Boundary Rank Then Return Result")]
        public void GivenCustomerScoresWhenGetByCustomerAndBoundaryRankThenReturnResult()
        {
            // Arrange
            var customerScores = new Dictionary<int, decimal>
            {
                {10001, 60},
                {10002, 65},
                {10003, 55},
                {10004, 100},
                {10005, 65},
                {10006, 60},
                {10007, 150},
                {10008, 10},
                {10009, 200},
                {10010, 1},
            };
            _customerScoreService.GetClearLeaderboards();
            foreach (var pair in customerScores)
            {
                _customerScoreService.AddOrUpdateScore(pair.Key, pair.Value);
            }

            //Act
            var items = _customerScoreService.GetCustomerScoreDataByCustomerAndRange(10001, 10, 2);

            // Assert
            Assert.Equal(8, items.Count);
            
            Assert.Equal(10009, items[0].CustomerId);
            Assert.Equal(200, items[0].Score);
            Assert.Equal(1, items[0].Index);

            Assert.Equal(10007, items[1].CustomerId);
            Assert.Equal(150, items[1].Score);
            Assert.Equal(2, items[1].Index);
                        
            Assert.Equal(10004, items[2].CustomerId);
            Assert.Equal(100, items[2].Score);
            Assert.Equal(3, items[2].Index);
                        
            Assert.Equal(10002, items[3].CustomerId);
            Assert.Equal(65, items[3].Score);
            Assert.Equal(4, items[3].Index);

            Assert.Equal(10005, items[4].CustomerId);
            Assert.Equal(65, items[4].Score);
            Assert.Equal(5, items[4].Index);
            
            Assert.Equal(10001, items[5].CustomerId);
            Assert.Equal(60, items[5].Score);
            Assert.Equal(6, items[5].Index);
            
            Assert.Equal(10006, items[6].CustomerId);
            Assert.Equal(60, items[6].Score);
            Assert.Equal(7, items[6].Index);
                        
            Assert.Equal(10003, items[7].CustomerId);
            Assert.Equal(55, items[7].Score);
            Assert.Equal(8, items[7].Index);
            
            
            //Act
            items = _customerScoreService.GetCustomerScoreDataByCustomerAndRange(10001, 2, 10);

            // Assert
            Assert.Equal(7, items.Count);
            
            Assert.Equal(10002, items[0].CustomerId);
            Assert.Equal(65, items[0].Score);
            Assert.Equal(4, items[0].Index);

            Assert.Equal(10005, items[1].CustomerId);
            Assert.Equal(65, items[1].Score);
            Assert.Equal(5, items[1].Index);
            
            Assert.Equal(10001, items[2].CustomerId);
            Assert.Equal(60, items[2].Score);
            Assert.Equal(6, items[2].Index);
            
            Assert.Equal(10006, items[3].CustomerId);
            Assert.Equal(60, items[3].Score);
            Assert.Equal(7, items[3].Index);
                        
            Assert.Equal(10003, items[4].CustomerId);
            Assert.Equal(55, items[4].Score);
            Assert.Equal(8, items[4].Index);
                        
            Assert.Equal(10008, items[5].CustomerId);
            Assert.Equal(10, items[5].Score);
            Assert.Equal(9, items[5].Index);
            
            Assert.Equal(10010, items[6].CustomerId);
            Assert.Equal(1, items[6].Score);
            Assert.Equal(10, items[6].Index);
        }
    }
}

