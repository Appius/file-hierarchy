using System;
using System.Linq;
using FileHierarchy.Common.Abstract;
using FileHierarchy.Common.Models;

namespace FileHierarchy.BusinessFacade
{
	public class FileEntityUpdater
	{
		private readonly FileEntity _fileEntity;
		private readonly IFolderRepository _folderRepository;

		public FileEntityUpdater(FileEntity fileEntity, IFolderRepository folderRepository)
		{
			_fileEntity = fileEntity;
			_folderRepository = folderRepository;
		}

		public void ChangeSeqNum(int newSeqNum)
		{
			var seqNum = _fileEntity.SeqNum;
			var affectedEntities = GetSameLevelEntities()
				.Where(SelectRange(seqNum, newSeqNum))
				.OrderBy(f => f.SeqNum)
				.ToList();

			foreach (var affectedEntity in affectedEntities)
			{
				if (seqNum < newSeqNum)
					affectedEntity.SeqNum--;
				else
					affectedEntity.SeqNum++;

				_folderRepository.MarkModified(affectedEntity);
			}
		}

		public void ChangeName(string name)
		{
			_fileEntity.Name = name;
		}

		public int GetMaxSeqNum()
		{
			var entities = GetSameLevelEntities().ToList();
			return entities.Count == 0 ? 0 : entities.Max(e => e.SeqNum);
		}

		private IQueryable<FileEntity> GetSameLevelEntities()
		{
			if (_fileEntity.ParentId == null)
			{
				return _folderRepository.AllEntities()
					.Where(f => f.ParentId == null);
			}

			return _folderRepository.GetChildren(_fileEntity.ParentId.Value);
		}

		private static Func<FileEntity, bool> SelectRange(int seqNum, int newSeqNum)
		{
			if (seqNum < newSeqNum)
				return c => c.SeqNum > seqNum && c.SeqNum <= newSeqNum;

			return c => c.SeqNum >= newSeqNum && c.SeqNum < seqNum;
		}
	}
}