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
	public class FolderController : BaseController
	{
		public FolderController(IFileRepository fileRepository) : base(fileRepository)
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
			var folder = FileRepository.GetFolder(id);
			if (folder == null)
				return NotFound();

			var fileEntities = FileRepository.GetChildren(id).OrderBy(f => f.SeqNum).ToList();
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
			var folder = FileRepository.GetFolder(id);
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

			var folder = FileRepository.GetFolder(id);
			if (folder == null)
				return NotFound();

			FileRepository.MarkModified(folder);
			fileViewModel.UpdateFileEntity(FileRepository, folder);

			try
			{
				FileRepository.Update(folder);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!FileRepository.FolderExists(id))
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
		[ResponseType(typeof(FileViewModel))]
		public IHttpActionResult PostFolder(FileViewModel fileViewModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			// if ParentId is not a folder
			if (fileViewModel.ParentId != null && !FileRepository.FolderExists(fileViewModel.ParentId.Value))
				return BadRequest(ModelState);

			var fileEntity = fileViewModel.ToFileEntity();
			fileEntity.SeqNum = fileViewModel.GetNextSeqNumber(FileRepository, fileEntity);

			fileEntity.Type = EntityType.Folder;
			FileRepository.Create(fileEntity);

			return CreatedAtRoute("DefaultApi", new {id = fileViewModel.Id}, fileEntity.ToFileViewModel());
		}

		/// <summary>
		///     Deletes the folder
		/// </summary>
		/// <param name="id">Folder ID</param>
		[SwaggerOperation("Delete")]
		[SwaggerResponse(HttpStatusCode.BadRequest)]
		[SwaggerResponse(HttpStatusCode.Created)]
		[ResponseType(typeof(FileViewModel))]
		public IHttpActionResult DeleteFolder(int id)
		{
			var fileEntity = FileRepository.GetFolder(id);
			if (fileEntity == null)
				return NotFound();

			var children = FileRepository.GetChildren(id).ToList();
			if (children.Count == 0)
				return BadRequest(ModelState);

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