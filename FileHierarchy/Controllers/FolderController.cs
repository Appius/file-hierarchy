using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using FileHierarchy.Common.Abstract;
using FileHierarchy.Common.Models;
using FileHierarchy.Mappers;
using FileHierarchy.ViewModel;
using Swashbuckle.Swagger.Annotations;

namespace FileHierarchy.Controllers
{
	public class FolderController : BaseController
	{
		public FolderController(IFileRepository fileRepository, IFolderRepository folderRepository) : base(fileRepository,
			folderRepository)
		{
		}

		/// <summary>
		///     Get the sub-items of specified folder
		/// </summary>
		/// <param name="id">Folder ID</param>
		[SwaggerOperation("GetChildren")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[Route("api/folder/{id}/children")]
		[ResponseType(typeof(IQueryable<FileViewModel>))]
		public IHttpActionResult GetChildren(int id)
		{
			var folder = FolderRepository.Get(id);
			if (folder == null)
				return NotFound();

			var fileEntities = FolderRepository.GetChildren(id);
			var fileViewModels = fileEntities.Select(e => e.ToFileViewModel());
			return Ok(fileViewModels);
		}

		/// <summary>
		///     Get the folder with specified identifier
		/// </summary>
		/// <param name="id">Folder ID</param>
		[SwaggerOperation("GetById")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[ResponseType(typeof(FileViewModel))]
		public IHttpActionResult GetFolder(int id)
		{
			var folder = FolderRepository.Get(id);
			if (folder == null)
				return NotFound();

			return Ok(folder.ToFileViewModel());
		}

		/// <summary>
		///     Updates the folder
		/// </summary>
		/// <param name="id">Folder ID</param>
		/// <param name="fileViewModel">Folder model</param>
		[SwaggerOperation("Update")]
		[SwaggerResponse(HttpStatusCode.BadRequest)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[SwaggerResponse(HttpStatusCode.NoContent)]
		[ResponseType(typeof(void))]
		public IHttpActionResult PutFolder(int id, FileViewModel fileViewModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var folder = fileViewModel.ToFileEntity();
			if (id != folder.Id)
				return BadRequest();

			fileViewModel.UpdateFileEntity(FolderRepository, folder);

			try
			{
				FolderRepository.Update(folder);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!FolderRepository.Exists(id))
					return NotFound();

				throw;
			}

			return StatusCode(HttpStatusCode.NoContent);
		}

		/// <summary>
		///     Creates new folder
		/// </summary>
		/// <param name="fileViewModel">Folder model</param>
		[SwaggerOperation("Create")]
		[SwaggerResponse(HttpStatusCode.BadRequest)]
		[SwaggerResponse(HttpStatusCode.Created)]
		[ResponseType(typeof(FileEntity))]
		public IHttpActionResult PostFolder(FileViewModel fileViewModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			// if ParentId is not a folder
			if (fileViewModel.ParentId != null && !FolderRepository.Exists(fileViewModel.ParentId.Value))
				return BadRequest(ModelState);

			var fileEntity = fileViewModel.ToFileEntity();
			fileEntity.SeqNum = fileViewModel.GetNextSeqNumber(FolderRepository, fileEntity);

			fileEntity.Type = EntityType.Folder;
			FolderRepository.Create(fileEntity);

			return CreatedAtRoute("DefaultApi", new {id = fileViewModel.Id}, fileViewModel);
		}

		/// <summary>
		///     Deletes the folder
		/// </summary>
		/// <param name="id">Folder ID</param>
		[SwaggerOperation("Delete")]
		[SwaggerResponse(HttpStatusCode.BadRequest)]
		[SwaggerResponse(HttpStatusCode.Created)]
		[ResponseType(typeof(FileEntity))]
		public IHttpActionResult DeleteFolder(int id)
		{
			var fileEntity = FolderRepository.Get(id);
			if (fileEntity == null)
				return NotFound();

			FileRepository.Remove(fileEntity);
			return Ok(fileEntity);
		}
	}
}