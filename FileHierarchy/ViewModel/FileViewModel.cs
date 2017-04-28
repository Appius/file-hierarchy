using FileHierarchy.BusinessFacade;
using FileHierarchy.Common.Abstract;
using FileHierarchy.Common.Models;

namespace FileHierarchy.ViewModel
{
	public class FileViewModel
	{
		public int Id { get; set; }

		public int? ParentId { get; set; }

		public string Name { get; set; }

		public int SeqNum { get; set; }

		public string Index { get; set; }

		public EntityType Type { get; set; }

		public void UpdateFileEntity(IFolderRepository folderRepository, FileEntity fileEntity)
		{
			var fileEntityUpdater = new FileEntityUpdater(fileEntity, folderRepository);

			if (!string.IsNullOrWhiteSpace(Name) && fileEntity.Name != Name)
				fileEntityUpdater.ChangeName(Name);

			if (SeqNum > 0 && SeqNum != fileEntity.SeqNum)
			{
				fileEntityUpdater.ChangeSeqNum(SeqNum);
				fileEntity.SeqNum = SeqNum;
			}
		}

		public int GetNextSeqNumber(IFolderRepository folderRepository, FileEntity fileEntity)
		{
			var fileEntityUpdater = new FileEntityUpdater(fileEntity, folderRepository);

			return fileEntityUpdater.GetMaxSeqNum() + 1;
		}
	}
}