using AggregatorService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AggregatorService.Services
{
    public interface IUtilityService
    {
        Task<List<PhotoModel>> GetAllPhotos();
        Task<List<AlbumModel>> GetAllAlbums();
        List<User> GetAlbumsByUserId(int UserId);
        Task<List<User>> GetAlbumsForAllUsers();
    }
}
