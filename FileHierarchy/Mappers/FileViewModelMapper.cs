using FileHierarchy.BusinessFacade;
using FileHierarchy.Common.Models;
using FileHierarchy.ViewModel;

namespace FileHierarchy.Mappers
{
	public static class FileViewModelMapper
	{
		public static FileEntity ToFileEntity(this FileViewModel fileViewModel)
		{
			return new FileEntity
			{
				Id = fileViewModel.Id,
				Name = fileViewModel.Name,
				ParentId = fileViewModel.ParentId,
				Type = fileViewModel.Type
			};
		}

		public static FileViewModel ToFileViewModel(this FileEntity fileEntity)
		{
			return new FileViewModel
			{
				Id = fileEntity.Id,
				Name = fileEntity.Name,
				SeqNum = fileEntity.SeqNum,
				ParentId = fileEntity.ParentId,
				Index = IndexCalculator.Calculate(fileEntity),
				Type = fileEntity.Type
			};
		}
	}
}