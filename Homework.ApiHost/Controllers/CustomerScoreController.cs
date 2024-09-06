using System.Collections.Generic;
using Homework.Services.Dtos;
using Homework.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Homework.ApiHost.Controllers
{
    public class CustomerScoreController : ApiController
    {
        private readonly ICustomerScoreService _customerScoreService;
        public CustomerScoreController(ICustomerScoreService customerScoreService)
        {
            _customerScoreService = customerScoreService;
        }

        /// <summary>
        /// add or update customer score
        /// </summary>
        /// <param name="customerId">customer id</param>
        /// <param name="score">score</param>
        /// <returns>if succeed</returns>
        [HttpPost]
        [Route("customer/{customerId}/score/{score}")]
        public bool AddOrUpdateScore([FromRoute] int customerId, decimal score)
        {
            var result = _customerScoreService.AddOrUpdateScore(customerId, score);
            return result;
        }

        /// <summary>
        /// get customer score list by range
        /// </summary>
        /// <param name="start">start index</param>
        /// <param name="end">end index</param>
        /// <returns>customer score json list</returns>
        [HttpGet]
        [Route("leaderboard")]
        public List<CustomerScoreDto> GetCustomerScoreDataByRange([FromQuery] int start, int end)
        {
            var result = _customerScoreService.GetCustomerScoreDataByRange(start, end);
            return result;
        }
        
        /// <summary>
        /// get customer score list by customer and range
        /// </summary>
        /// <param name="customerId">customer id</param>
        /// <param name="start">start index</param>
        /// <param name="end">end index</param>
        /// <returns>customer score json list</returns>
        [HttpGet]
        [Route("leaderboard/{customerId}")]
        public List<CustomerScoreDto> GetCustomerScoreDataByCustomerAndRange([FromRoute] int customerId, [FromQuery] int? start, int? end)
        {
            var result = _customerScoreService.GetCustomerScoreDataByCustomerAndRange(customerId, start, end);
            return result;
        }
    }
}

