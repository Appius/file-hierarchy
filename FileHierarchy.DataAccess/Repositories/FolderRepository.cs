using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FileHierarchy.Common.Abstract;
using FileHierarchy.Common.Models;

namespace FileHierarchy.DataAccess.Repositories
{
	public class FolderRepository : BaseRepository, IFolderRepository
	{
		public override IQueryable<FileEntity> Entities()
			=> base.Entities().Where(e => e.Type == EntityType.Folder);

		public IQueryable<FileEntity> GetChildren(int id)
			=> Entities().Include(e => e.Children).Where(f => f.Id == id).SelectMany(f=>f.Children);
	}
}