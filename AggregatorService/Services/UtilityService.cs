using AggregatorService.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AggregatorService.Services
{
    public class UtilityService : IUtilityService
    {

        private readonly ILogger<UtilityService> _logger;
        private readonly HttpClient client;
        
        public UtilityService(ILogger<UtilityService> logger)
        {
            _logger = logger;
            client = new HttpClient()
            {
                BaseAddress = new Uri("http://jsonplaceholder.typicode.com"),
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<List<PhotoModel>> GetAllPhotos()
        {
            try
            {
                string urlParameters = "photos/";
                
                List<PhotoModel> photos = new List<PhotoModel>();
                // List data response.
                HttpResponseMessage response = client.GetAsync(urlParameters).Result;
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body.
                    var raw = await response.Content.ReadAsStringAsync();
                    photos = JsonConvert.DeserializeObject<List<PhotoModel>>(raw);
                }
                else
                {
                    _logger.LogError("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                }
                return photos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new List<PhotoModel>();
            }
        }
        public async Task<List<AlbumModel>> GetAllAlbums()
        {
            try
            {
                List<AlbumModel> albums = new List<AlbumModel>();                
                string urlParameters = "albums/";                
                var response = client.GetAsync(urlParameters).Result;  
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body.
                    var raw = await response.Content.ReadAsStringAsync();
                    albums = JsonConvert.DeserializeObject<List<AlbumModel>>(raw);
                }
                else
                {
                    _logger.LogError("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                }
                return albums;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new List<AlbumModel>();
            }
        }
        public List<User> GetAlbumsByUserId(int UserId)
        {
            try
            {
                List<AlbumModel> albums = new List<AlbumModel>();
                List<User> response = new List<User>();                                
                string urlParameters = "users/" + UserId + "/albums";
                var apiResponse = client.GetAsync(urlParameters).Result; 
                if (apiResponse.IsSuccessStatusCode)
                {
                    // Parse the response body.
                    var rawResponse = apiResponse.Content.ReadAsStringAsync();
                    albums = JsonConvert.DeserializeObject<List<AlbumModel>>(rawResponse.Result);
                    if (albums.Count > 0)
                    {
                        List<PhotoModel> photos = new List<PhotoModel>();
                        foreach (var item in albums)
                        {
                            urlParameters = "albums/" + item.Id + "/photos";
                            var result = client.GetAsync(urlParameters).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                            if (result.IsSuccessStatusCode)
                            {
                                // Parse the response body.
                                var rawList = result.Content.ReadAsStringAsync();
                                photos.AddRange(JsonConvert.DeserializeObject<List<PhotoModel>>(rawList.Result));
                            }
                            else
                            {
                                _logger.LogError("{0} ({1})", (int)result.StatusCode, result.ReasonPhrase);
                                return new List<User>();
                            }
                        }
                        response = ListOfPhotosByUserId(albums, photos);
                        return response;
                    }
                    else
                    {
                        _logger.LogDebug("No records found for the user : " + UserId);
                        return new List<User>();
                    }
                }
                else
                {
                    //user doesn't exist or error occured in fetching 
                    _logger.LogError("{0} ({1})", (int)apiResponse.StatusCode, apiResponse.ReasonPhrase);
                    return new List<User>();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new List<User>();
            }
        }
        public async Task<List<User>> GetAlbumsForAllUsers()
        {
            Task<List<PhotoModel>> task1 = GetAllPhotos();            
            Task<List<AlbumModel>> task2 = GetAllAlbums();
            List<PhotoModel> photos = await task1;
            List<AlbumModel> albums = await task2;
            _logger.LogInformation("Number of Photos : "+photos.Count);
            return ListOfPhotosByUserId(albums, photos);
        }
        private List<User> ListOfPhotosByUserId(List<AlbumModel> albums, List<PhotoModel> photos)
        {

            List<User> response = new List<User>();
            //list of all photos and its album and the UserId to which they belong
            var query = from album in albums
                        let AlbumId = album.Id
                        let AlbumTitle = album.Title
                        join photo in photos
                             on album.Id equals photo.AlbumId
                        let PhotoId = photo.Id
                        let PhotoTitle = photo.Title
                        select new
                        {
                            album.UserId,
                            AlbumTitle,
                            AlbumId,
                            PhotoId,
                            PhotoTitle,
                            photo.ThumbnailURL,
                            photo.URL
                        };
            foreach (var item in query)
            {
                int userIndex = response.FindIndex(value => value.Id == item.UserId);

                if (userIndex >= 0)
                {
                    User selectedUser = response.ElementAt(userIndex);
                    int albumIndex = selectedUser.Albums.FindIndex(value => value.Id == item.AlbumId);
                    {
                        if (albumIndex >= 0)       //album id already present in populated list
                        {
                            AlbumDTO selectedAlbum = selectedUser.Albums.ElementAt(albumIndex);
                            selectedAlbum.Photos.Add(new PhotoDTO
                            {
                                Id = item.PhotoId,
                                Title = item.PhotoTitle,
                                ThumbnailURL = item.ThumbnailURL,
                                URL = item.URL
                            });
                        }
                        else //add new album Id in the response list
                        {
                            List<PhotoDTO> newPhoto = new List<PhotoDTO>();
                            newPhoto.Add(new PhotoDTO
                            {
                                Id = item.PhotoId,
                                Title = item.PhotoTitle,
                                ThumbnailURL = item.ThumbnailURL,
                                URL = item.URL
                            });
                            AlbumDTO newAlbum = new AlbumDTO()
                            {
                                Id = item.AlbumId,
                                Title = item.AlbumTitle,
                                Photos = newPhoto
                            };
                            selectedUser.Albums.Add(newAlbum);

                        }
                    }
                }
                else //Add the new user and its album & photos 
                {
                    List<PhotoDTO> photo = new List<PhotoDTO>();
                    photo.Add(new PhotoDTO
                    {
                        Id = item.PhotoId,
                        Title = item.PhotoTitle,
                        ThumbnailURL = item.ThumbnailURL,
                        URL = item.URL
                    });
                    List<AlbumDTO> albm = new List<AlbumDTO>();
                    albm.Add(new AlbumDTO
                    {
                        Id = item.AlbumId,
                        Title = item.AlbumTitle,
                        Photos = photo
                    });

                    response.Add(new User
                    {
                        Id = item.UserId,
                        Albums = albm
                    });
                }
            }
            return response;
        }
    }
}
