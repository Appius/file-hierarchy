using System;
using System.Linq;
using FileHierarchy.Common.Models;

namespace FileHierarchy.BusinessFacade
{
	public static class IndexCalculator
	{
		public static string Calculate(FileEntity fileEntity)
		{
			if (fileEntity == null)
				throw new ArgumentException($"{nameof(fileEntity)} is null");

			if (fileEntity.Descendants.Count == 0)
				return fileEntity.SeqNum.ToString();

			var indexes = fileEntity.Descendants
				.OrderBy(d => d.Level)
				.Select(s => s.Parent.SeqNum)
				.ToList();

			indexes.Add(fileEntity.SeqNum);
			return string.Join(".", indexes);
		}
	}
}