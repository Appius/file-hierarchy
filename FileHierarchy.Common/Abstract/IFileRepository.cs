using System.Collections.Generic;
using FileHierarchy.Common.Models;

namespace FileHierarchy.Common.Abstract
{
	public interface IFileRepository : IBaseRepository
	{
		IEnumerable<FileEntity> Get(string query = "", bool loadDescendants = false);
	}
}