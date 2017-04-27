using System.Linq;
using FileHierarchy.Common.Models;

namespace FileHierarchy.Common.Abstract
{
	public interface IBaseRepository
	{
		void Create(FileEntity fileEntity);
		void Dispose();
		IQueryable<FileEntity> Entities();
		IQueryable<FileEntity> AllEntities();
		bool Exists(int id);
		FileEntity Get(int id);
		void MarkModified(FileEntity fileEntity);
		void Remove(FileEntity fileEntity);
		void Save();
		void Update(FileEntity fileEntity);
	}
}