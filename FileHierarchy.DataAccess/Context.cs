using System.Data.Entity;
using FileHierarchy.Common.Models;
using FileHierarchy.DataAccess.Configurations;

namespace FileHierarchy.DataAccess
{
	public class Context : DbContext
	{
		public Context() : base("Azure")
		{
		}

		public virtual DbSet<FileEntity> Entities { get; set; }
		public virtual DbSet<Tree> Trees { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Configurations.Add(new FileEntityConfiguration());
		}
	}
}