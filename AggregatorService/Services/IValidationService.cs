using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AggregatorService.Services
{
    public interface IValidationService
    {
        bool IsUserIdValid(string userId);
    }
}
