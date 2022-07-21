using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Security.Cryptography.Xml;
using Newtonsoft.Json;
using AggregatorService.Models;
using AggregatorService.Services;
using System.Net;

namespace AggregatorService.Controllers
{
    [ApiController]

    public class AlbumController : ControllerBase
    {

        private const string URL = "http://jsonplaceholder.typicode.com";

        private readonly ILogger<AlbumController> _logger;
        private readonly IUtilityService _utilityService;
        private readonly IValidationService _validationService;

        public AlbumController(ILogger<AlbumController> logger, IUtilityService utilityService, IValidationService validationService)
        {
            _logger = logger;
            _utilityService = utilityService;
            _validationService = validationService;
        }

        [HttpGet]
        [Route("photos")]
        public IActionResult GetAllPhotos()
        {
            try
            {
                _logger.LogDebug("GetAllPhotos started");
                List<User> response = _utilityService.GetAlbumsForAllUsers().Result;
                if (response.Count > 0)
                    return Ok(response);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        [HttpGet]
        [Route("photos/{userId:int:min(1)}")]
        public IActionResult GetAllPhotosByUserId(string userId)
        {
            try
            {
                _logger.LogDebug("GetAllPhotosByUserId started");
                _logger.LogDebug("Input UserId : " + userId);
                if (!String.IsNullOrEmpty(userId) && _validationService.IsUserIdValid(userId))
                {
                    List<User> response = _utilityService.GetAlbumsByUserId(Convert.ToInt32(userId));
                    if (response.Count == 0)
                    {
                        return NotFound();
                    }
                    else
                        return Ok(response);
                }
                else
                {
                    _logger.LogDebug("Invalid UserId input by user");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}