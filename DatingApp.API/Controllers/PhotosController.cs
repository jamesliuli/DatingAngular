using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Data;
using AutoMapper;
using Microsoft.Extensions.Options;
using DatingApp.API.Helps;
using CloudinaryDotNet;
using DatingApp.API.Dtos;
using System.Security.Claims;
using CloudinaryDotNet.Actions;
using DatingApp.API.Models;
using System.Linq;

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

        [HttpGet("{id}", Name="GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            //often used mthod to check the current logged in user
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

                var userFromRepo = await _repo.GetUser(userId);

                var file = photoForCreationDto.File;

                var uploadResult = new ImageUploadResult();
            
                if (file.Length > 0)
                {
                    using (var stream = file.OpenReadStream()){
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.Name, stream),
                            Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                        };
                        uploadResult = _cloudinary.Upload(uploadParams);
                    }
                }

                photoForCreationDto.Url = uploadResult.Uri.ToString();
                photoForCreationDto.PublicId = uploadResult.PublicId;
                var photo = _mapper.Map<Photo>(photoForCreationDto);
                if (!userFromRepo.Photos.Any( u => u.IsMain))
                    photo.IsMain = true;
                
                userFromRepo.Photos.Add(photo);

                if (await _repo.SaveAll())
                {
                    var photoForRetun = _mapper.Map<Photo>(photo);
                    return CreatedAtRoute("GetPhoto", new {userId = userId, id = photo.Id }, photoForRetun);
                }

                return BadRequest("Could not add the photo");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            //often used mthod to check the current logged in user
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var user = await _repo.GetUser(userId);
            if (!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id);
            if (photoFromRepo.IsMain)
                return BadRequest("Can not delete the main photo");     
            
            if (!string.IsNullOrEmpty( photoFromRepo.PublicId))
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deleteParams);
                if (result.Result == "ok")
                    _repo.Delete(photoFromRepo);
            }
            else
                _repo.Delete(photoFromRepo);

            if (await _repo.SaveAll())
                return Ok();

            return BadRequest("Faild to delete the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var user = await _repo.GetUser(userId);
            if (!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id);
            if (photoFromRepo.IsMain)
                return BadRequest("Already the main photo");     

            foreach ( var p in user.Photos)
            {  p.IsMain = false; }
            
            photoFromRepo.IsMain = true;

            if (await _repo.SaveAll())
               return Ok();

            return BadRequest("Faild to set the main photo");
        }

    }
}