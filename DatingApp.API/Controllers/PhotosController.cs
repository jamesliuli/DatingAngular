using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Data;
using AutoMapper;
using Microsoft.Extensions.Options;
using DatingApp.API.Helps;
using CloudinaryDotNet;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController: ControllerBase
    {
        private Cloudinary _cloudinary;

        public IDatingRepository _repo { get; }
        public IMapper _mapper { get; }
        public IOptions<CloudinarySettings> _cloudinaryConfig { get; }

        public PhotosController(IDatingRepository repo, IMapper mapper, 
        IOptions<CloudinarySettings> CloudinaryConfig )
        {
            _repo = repo;
            _mapper = mapper;
            _cloudinaryConfig = CloudinaryConfig;

            Account acct = new Account(
                 _cloudinaryConfig.Value.CloudName,
                 _cloudinaryConfig.Value.ApiKey,
                 _cloudinaryConfig.Value.ApiSecret);

            _cloudinary = new Cloudinary(acct);
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> AddPhotoForUser()
        {
            //
            return StatusCode(201);
        }
        
    }
}