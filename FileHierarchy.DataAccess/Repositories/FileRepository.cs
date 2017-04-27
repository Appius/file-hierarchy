using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FileHierarchy.Common.Abstract;
using FileHierarchy.Common.Models;

namespace FileHierarchy.DataAccess.Repositories
{
	public class FileRepository : BaseRepository, IFileRepository
	{
		public override IQueryable<FileEntity> Entities()
			=> base.Entities().Where(e => e.Type == EntityType.File);

		public IEnumerable<FileEntity> Get(string query = "", bool loadDescendants = false)
		{
			var result = Entities();

			if (!string.IsNullOrWhiteSpace(query))
				result = result.Where(f => f.Name.Contains(query));

			if (loadDescendants)
				result = result.Include(e => e.Descendants);

			return result.ToList();
		}
	}
}