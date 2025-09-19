using Asp.Versioning;
using EasyPay.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EasyPay.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;

        public FilesController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        /// <summary>
        /// Uploads a document file for identity verification.
        /// </summary>
        /// <param name="file">The file to upload, sent as multipart/form-data.</param>
        /// <returns>A JSON object with the URL of the uploaded file.</returns>
        [HttpPost("upload-document")]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadDocument(IFormFile file)
        {
            try
            {
                var fileUrl = await _fileStorageService.SaveFileAsync(file, "user-documents");
                return Created(fileUrl, new { url = fileUrl });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new Error(400, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new Error(400, ex.Message));
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new Error(500, "An unexpected error occurred during file upload."));
            }
        }

        /// <summary>
        /// Deletes a previously uploaded file.
        /// </summary>
        /// <param name="request">A JSON object containing the URL of the file to delete.</param>
        [HttpDelete("delete-document")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteDocument([FromBody] DeleteFileRequest request)
        {
            await _fileStorageService.DeleteFileAsync(request.FileUrl);
            return NoContent();
        }
    }

    public record DeleteFileRequest(string FileUrl);
}