using System;
using System.Data.Entity;
using System.Linq;
using FileHierarchy.Common.Abstract;
using FileHierarchy.Common.Models;

namespace FileHierarchy.DataAccess.Repositories
{
	public abstract class BaseRepository : IBaseRepository, IDisposable
	{
		protected Context Context = new Context();

		public virtual IQueryable<FileEntity> Entities() => Context.Entities.Include(f => f.Parent);

		public IQueryable<FileEntity> AllEntities() => Context.Entities.Include(f => f.Parent);

		public void Dispose()
		{
			Context.Dispose();
		}

		public void MarkModified(FileEntity fileEntity)
		{
			Context.Entry(fileEntity).State = EntityState.Modified;
		}

		public void Save() => Context.SaveChanges();

		public bool Exists(int id) => Entities().Count(e => e.Id == id) > 0;

		public void SaveInTransaction(Action action)
		{
			using (var dbContextTransaction = Context.Database.BeginTransaction())
			{
				action();
				dbContextTransaction.Commit();
			}
		}

		public void SaveInTransaction(Action<IBaseRepository> action)
		{
			using (var dbContextTransaction = Context.Database.BeginTransaction())
			{
				action(this);
				dbContextTransaction.Commit();
			}
		}

		#region CRUD

		public FileEntity Get(int id)
			=> Entities().Include(f => f.Descendants).SingleOrDefault(f => f.Id == id);

		public void Update(FileEntity fileEntity)
		{
			Context.Entry(fileEntity).State = EntityState.Modified;
			Save();
		}

		public void Create(FileEntity fileEntity)
		{
			Context.Entities.Add(fileEntity);
			Save();
		}

		public void Remove(FileEntity fileEntity)
		{
			Context.Entities.Remove(fileEntity);
			Save();
		}

		#endregion
	}
}