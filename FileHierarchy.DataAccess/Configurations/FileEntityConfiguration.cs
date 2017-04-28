using System.Data.Entity.ModelConfiguration;
using FileHierarchy.Common.Models;

namespace FileHierarchy.DataAccess.Configurations
{
	public class FileEntityConfiguration : EntityTypeConfiguration<FileEntity>
	{
		public FileEntityConfiguration()
		{
			HasMany(e => e.Children)
				.WithOptional(e => e.Parent)
				.HasForeignKey(e => e.ParentId);

			HasMany(e => e.Ancestors)
				.WithRequired(e => e.Parent)
				.HasForeignKey(e => e.ParentId)
				.WillCascadeOnDelete(false);

			HasMany(e => e.Descendants)
				.WithRequired(e => e.Child)
				.HasForeignKey(e => e.ChildId)
				.WillCascadeOnDelete(true);
		}
	}
}