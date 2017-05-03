using System.Web.Http;
using FileHierarchy.Common.Abstract;

namespace FileHierarchy.Controllers
{
	public abstract class BaseController : ApiController
	{
		protected readonly IFileRepository FileRepository;

		protected BaseController(IFileRepository fileRepository)
		{
			FileRepository = fileRepository;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				FileRepository.Dispose();

			base.Dispose(disposing);
		}
	}
}