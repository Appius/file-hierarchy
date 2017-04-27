using System.Linq;
using FileHierarchy.Common.Models;

namespace FileHierarchy.Common.Abstract
{
	public interface IFolderRepository : IBaseRepository
	{
		IQueryable<FileEntity> GetChildren(int id);
	}
}