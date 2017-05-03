using System;
using System.Collections.Generic;
using System.Linq;
using FileHierarchy.Common.Models;

namespace FileHierarchy.Common.Abstract
{
	public interface IFileRepository
	{
		void Create(FileEntity fileEntity);
		void Dispose();
		IQueryable<FileEntity> Entities();
		bool FileExists(int id);
		bool FolderExists(int id);
		FileEntity GetFile(int id);
		FileEntity GetFolder(int id);
		void MarkModified(FileEntity fileEntity);
		void Remove(FileEntity fileEntity);
		void Save();
		void Update(FileEntity fileEntity);
		void SaveInTransaction(Action<IFileRepository> action);
		IEnumerable<FileEntity> Search(string query = "", bool loadDescendants = false);
		IQueryable<FileEntity> Files();
		IQueryable<FileEntity> Folders();
		IQueryable<FileEntity> GetChildren(int id);
	}
}