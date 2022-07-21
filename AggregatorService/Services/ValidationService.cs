using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AggregatorService.Services
{
    public class ValidationService: IValidationService
    {
        private readonly ILogger<ValidationService> _logger;        

        public ValidationService(ILogger<ValidationService> logger)
        {
            _logger = logger;
        }

        public bool IsUserIdValid(string userId)
        {
            try
            {
                int _userId = Convert.ToInt32(userId);
                if (_userId > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
    }
}
