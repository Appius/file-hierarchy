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
		public FileController(IFileRepository fileRepository, IFolderRepository folderRepository) : base(fileRepository,
			folderRepository)
		{
		}

		/// <summary>
		///     Gets all the files matching search criteria
		/// </summary>
		/// <param name="query">Substring to search</param>
		[SwaggerOperation("SearchByQuery")]
		public IEnumerable<FileViewModel> Get(string query)
		{
			return FileRepository.Get(query, true).Select(e => e.ToFileViewModel());
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
			var fileEntity = FileRepository.Get(id);
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

			var fileEntity = FileRepository.Get(id);
			if (fileEntity == null)
				return NotFound();

			FileRepository.MarkModified(fileEntity);
			fileViewModel.UpdateFileEntity(FolderRepository, fileEntity);

			try
			{
				FolderRepository.SaveInTransaction(() =>
				{
					FolderRepository.Save();
					FileRepository.Save();
				});
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!FileRepository.Exists(id))
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
		[ResponseType(typeof(FileEntity))]
		public IHttpActionResult PostFile(FileViewModel fileViewModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			// if ParentId is not a folder
			if (fileViewModel.ParentId != null && !FolderRepository.Exists(fileViewModel.ParentId.Value))
				return BadRequest(ModelState);

			var fileEntity = fileViewModel.ToFileEntity();
			fileEntity.SeqNum = fileViewModel.GetNextSeqNumber(FolderRepository, fileEntity);

			fileEntity.Type = EntityType.File;
			FileRepository.Create(fileEntity);

			return CreatedAtRoute("DefaultApi", new {id = fileViewModel.Id}, fileViewModel);
		}

		/// <summary>
		///     Delete the file
		/// </summary>
		/// <param name="id">File ID</param>
		[SwaggerOperation("Delete")]
		[SwaggerResponse(HttpStatusCode.BadRequest)]
		[SwaggerResponse(HttpStatusCode.OK)]
		[ResponseType(typeof(FileEntity))]
		public IHttpActionResult DeleteFile(int id)
		{
			var fileEntity = FileRepository.Get(id);
			if (fileEntity == null)
				return NotFound();

			var fileUpdater = new FileEntityUpdater(fileEntity, FolderRepository);
			fileUpdater.ChangeSeqNum(int.MaxValue);

			FolderRepository.SaveInTransaction(() =>
			{
				FolderRepository.Save();
				FileRepository.Remove(fileEntity);
			});

			return Ok(fileEntity);
		}
	}
}