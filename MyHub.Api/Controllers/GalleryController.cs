using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyHub.Domain.DtoMappingProfiles.Gallery;
using MyHub.Domain.Gallery.GalleryDto;
using MyHub.Domain.Gallery.Interfaces;
using MyHub.Domain.Users.Interfaces;

namespace MyHub.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class GalleryController : BaseController
	{
		private readonly IMapper _mapper;
		private readonly IGalleryService _galleryService;
		private readonly IMarcoService _adminService;

		public GalleryController(IGalleryService galleryService, IMapper mapper, IMarcoService adminService)
		{
			_galleryService = galleryService;
			_mapper = mapper;
			_adminService = adminService;
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpPost]
		public async Task<IActionResult> UploadImage(UploadGalleryImageDto uploadGalleryImageDto)
		{
			var galleryImage = await _galleryService.UploadImage(_adminService.GetMarcoId(), uploadGalleryImageDto.Image, uploadGalleryImageDto.Caption);

			if (galleryImage is null)
				return BadRequest("Failed to upload image.");

			return Ok(_mapper.Map<UserGalleryDto>(galleryImage));
		}

		[HttpGet]
		public IActionResult GetImages()
		{
			var usersGallery = _galleryService.GetUserImages(_adminService.GetMarcoId());

			if (usersGallery is null)
				return BadRequest("Failed to get image ids.");

			return Ok(new { Images = _mapper.Map<List<UserGalleryDto>>(usersGallery, opt => opt.Items[GalleryContextOptions.UserId] = UserId) });
		}

		[HttpGet("ImageData/{imageId}")]
		public IActionResult GetImageData(string imageId)
		{
			var image = _galleryService.GetImageData(imageId);

			if (image is null)
				return BadRequest("Failed to get image.");

			return Ok(_mapper.Map<UserGalleryCommentsDto>(image, opt => opt.Items[GalleryContextOptions.UserId] = UserId));
		}

		[HttpGet("{imageId}")]
		public async Task<IActionResult> GetImage(string imageId)
		{
			var image = await _galleryService.GetUserImage(_adminService.GetMarcoId(), imageId);

			if (image is null)
				return BadRequest("Failed to upload image.");

			return File(image, "image/png");
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpDelete("{imageId}")]
		public async Task<IActionResult> DeleteImage(string imageId)
		{
			if (!IsAdmin)
				return BadRequest("This user cannot upload images.");

			var imageRemoved = await _galleryService.RemoveUserImage(imageId);

			if (!imageRemoved)
				return BadRequest("Failed to upload image.");

			return Ok();
		}

		[Authorize]
		[HttpPost("Like")]
		public IActionResult LikeImage(LikeImageDto likeImageDto)
		{
			var imageLiked = _galleryService.LikeImage(UserId, likeImageDto.ImageId);

			if (!imageLiked)
				return BadRequest("Image already liked.");

			return Ok();
		}

		[Authorize]
		[HttpPost("Unlike")]
		public IActionResult UnlikeImage(UnlikeImageDto unlikeImageDto)
		{
			var imageLiked = _galleryService.UnlikeImage(UserId, unlikeImageDto.ImageId);

			if (!imageLiked)
				return BadRequest("Image already unliked.");

			return Ok();
		}

		[Authorize]
		[HttpPost("Comment")]
		public IActionResult PostComment(CommentDto commentDto)
		{
			var comment = _galleryService.PostCommentToImage(UserId, commentDto.ImageId, commentDto.Comment);

			if (comment is null)
				return BadRequest("Unable to comment on image.");

			return Ok(_mapper.Map<GalleryImageCommentDto>(comment));
		}

		[HttpGet("Comments/{imageId}")]
		public IActionResult GetComment(string imageId)
		{
			var comments = _galleryService.GetImageComments(imageId);

			if (comments is null)
				return BadRequest("Unable to comment on image.");

			return Ok(_mapper.Map<List<GalleryImageCommentDto>>(comments));
		}
	}
}