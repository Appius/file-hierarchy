using System.Web.Http;
using FileHierarchy.Common.Abstract;
using FileHierarchy.DataAccess.Repositories;

namespace FileHierarchy.Controllers
{
	public abstract class BaseController : ApiController
	{
		protected readonly IFileRepository FileRepository;
		protected readonly IFolderRepository FolderRepository;

		protected BaseController(IFileRepository fileRepository, IFolderRepository folderRepository)
		{
			FileRepository = fileRepository;
			FolderRepository = folderRepository;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				FileRepository.Dispose();

			base.Dispose(disposing);
		}
	}
}