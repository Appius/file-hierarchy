using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using FileHierarchy.BusinessFacade;
using FileHierarchy.Common.Abstract;
using FileHierarchy.Common.Models;
using FileHierarchy.Mappers;
using FileHierarchy.ViewModel;
using Swashbuckle.Swagger.Annotations;

namespace FileHierarchy.Controllers
{
	public class FileController : BaseController
	{
		public FileController(IFileRepository fileRepository) : base(fileRepository)
		{
		}

		/// <summary>
		///     Gets all the files matching search criteria
		/// </summary>
		/// <param name="query">Substring to search</param>
		[SwaggerOperation("SearchByQuery")]
		public IEnumerable<FileViewModel> Get(string query)
		{
			return FileRepository.Search(query, true).Select(e => e.ToFileViewModel());
		}

		/// <summary>
		///     Gets specific file details
		/// </summary>
		/// <param name="id">File ID</param>
		[SwaggerOperation("GetById")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[ResponseType(typeof(FileViewModel))]
		public IHttpActionResult GetFile(int id)
		{
			var fileEntity = FileRepository.GetFile(id);
			if (fileEntity == null)
				return NotFound();

			return Ok(fileEntity.ToFileViewModel());
		}

		/// <summary>
		///     Updates the file
		/// </summary>
		/// <param name="id">File ID</param>
		/// <param name="fileViewModel">Model to update</param>
		[SwaggerOperation("Update")]
		[SwaggerResponse(HttpStatusCode.BadRequest)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[SwaggerResponse(HttpStatusCode.NoContent)]
		[ResponseType(typeof(void))]
		public IHttpActionResult PutFile(int id, FileViewModel fileViewModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var fileEntity = FileRepository.GetFile(id);
			if (fileEntity == null)
				return NotFound();

			FileRepository.MarkModified(fileEntity);
			fileViewModel.UpdateFileEntity(FileRepository, fileEntity);

			try
			{
				FileRepository.Save();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!FileRepository.FileExists(id))
					return NotFound();
				throw;
			}

			return StatusCode(HttpStatusCode.NoContent);
		}

		/// <summary>
		///     Creates a file
		/// </summary>
		/// <param name="fileViewModel">File model</param>
		[SwaggerOperation("Create")]
		[SwaggerResponse(HttpStatusCode.BadRequest)]
		[SwaggerResponse(HttpStatusCode.Created)]
		[ResponseType(typeof(FileViewModel))]
		public IHttpActionResult PostFile(FileViewModel fileViewModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			// if ParentId is not a folder
			if (fileViewModel.ParentId != null && !FileRepository.FolderExists(fileViewModel.ParentId.Value))
				return BadRequest(ModelState);

			var fileEntity = fileViewModel.ToFileEntity();
			fileEntity.SeqNum = fileViewModel.GetNextSeqNumber(FileRepository, fileEntity);

			fileEntity.Type = EntityType.File;
			FileRepository.Create(fileEntity);

			return CreatedAtRoute("DefaultApi", new {id = fileViewModel.Id}, fileEntity.ToFileViewModel());
		}

		/// <summary>
		///     Delete the file
		/// </summary>
		/// <param name="id">File ID</param>
		[SwaggerOperation("Delete")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[ResponseType(typeof(FileViewModel))]
		public IHttpActionResult DeleteFile(int id)
		{
			var fileEntity = FileRepository.GetFile(id);
			if (fileEntity == null)
				return NotFound();

			var fileUpdater = new FileEntityUpdater(fileEntity, FileRepository);
			fileUpdater.ChangeSeqNum(int.MaxValue);

			FileRepository.SaveInTransaction(r =>
			{
				r.Save();
				r.Remove(fileEntity);
			});

			return Ok(fileEntity.ToFileViewModel());
		}
	}
}