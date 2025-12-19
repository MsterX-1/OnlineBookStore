using Application.Dtos.PublisherDto;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OnlineBookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly PublisherService _publisherService;

        public PublisherController(PublisherService publisherService)
        {
            _publisherService = publisherService;
        }
		[HttpGet("GetAllPublishers")]
		public async Task<IActionResult> GetAllPublishers()
		{
			try
			{
				var publishers = await _publisherService.Getallpublishersasync();
				return Ok(publishers);
			}
			catch (Exception ex)
			{
				return NotFound(ex.Message);
			}
		}
		[HttpGet("GetPublisher/{id}")]
		public async Task<IActionResult> GetPublisherbyid(int id)
		{
			try
			{
				var publisher = await _publisherService.GetpublisherByIdAsync(id);
				return Ok(publisher);
			}
			catch (Exception ex)
			{
				return NotFound(ex.Message);
			}
		}
		[HttpPost("CreatePublisher")]
		public async Task<IActionResult> CreatePublisher([FromBody] CreatePublisherDto dto)
		{
			try
			{
				var publisherId = await _publisherService.CreatePublisherAsync(dto);
				return Ok(new { PublisherId = publisherId, Message = "Publisher created successfully" });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpPut("UpdatePublisher")]
		public async Task<IActionResult> UpdatePublisher([FromBody] UpdatePublisherDto dto)
		{
			try
			{
				await _publisherService.UpdatePublisherAsync(dto);
				return Ok(new { Message = "Publisher updated successfully" });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("DeletePublisher/{id}")]
		public async Task<IActionResult> DeletePublisher(int id)
		{
			try
			{
				await _publisherService.DeletePublisherAsync(id);
				return Ok(new { Message = "Publisher deleted successfully" });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

	}
}