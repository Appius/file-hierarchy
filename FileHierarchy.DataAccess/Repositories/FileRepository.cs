using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FileHierarchy.Common.Abstract;
using FileHierarchy.Common.Models;

namespace FileHierarchy.DataAccess.Repositories
{
	public class FileRepository : IFileRepository, IDisposable
	{
		public IQueryable<FileEntity> Entities() => _context.Entities.Include(f => f.Parent);

		public IQueryable<FileEntity> Files() => Entities().Where(e => e.Type == EntityType.File);

		public IQueryable<FileEntity> Folders() => Entities().Where(e => e.Type == EntityType.Folder);

		public IQueryable<FileEntity> GetChildren(int id)
			=> Entities().Include(e => e.Children).Where(f => f.Id == id).SelectMany(f => f.Children);

		public void Dispose() => _context.Dispose();

		public void MarkModified(FileEntity fileEntity) => _context.Entry(fileEntity).State = EntityState.Modified;

		public void Save() => _context.SaveChanges();

		public bool FileExists(int id) => Files().Count(e => e.Id == id) > 0;

		public bool FolderExists(int id) => Folders().Count(e => e.Id == id) > 0;

		public IEnumerable<FileEntity> Search(string query = "", bool loadDescendants = false)
		{
			var result = Entities();

			if (!string.IsNullOrWhiteSpace(query))
				result = result.Where(f => f.Name.Contains(query));

			if (loadDescendants)
				result = result.Include(e => e.Descendants);

			return result.ToList();
		}

		public void SaveInTransaction(Action<IFileRepository> action)
		{
			using (var dbContextTransaction = _context.Database.BeginTransaction())
			{
				action(this);
				dbContextTransaction.Commit();
			}
		}

		private readonly Context _context = new Context();

		#region CRUD

		public FileEntity GetFile(int id) => Files().Include(f => f.Descendants).SingleOrDefault(f => f.Id == id);

		public FileEntity GetFolder(int id) => Folders().Include(f => f.Descendants).SingleOrDefault(f => f.Id == id);

		public void Update(FileEntity fileEntity)
		{
			_context.Entry(fileEntity).State = EntityState.Modified;
			Save();
		}

		public void Create(FileEntity fileEntity)
		{
			_context.Entities.Add(fileEntity);
			Save();
		}

		public void Remove(FileEntity fileEntity)
		{
			_context.Entities.Remove(fileEntity);
			Save();
		}

		#endregion
	}
}