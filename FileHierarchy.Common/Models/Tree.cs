using System.ComponentModel.DataAnnotations.Schema;

namespace FileHierarchy.Common.Models
{
	[Table("Tree")]
	public class Tree
	{
		public int Id { get; set; }

		public int Level { get; set; }

		public int ParentId { get; set; }
		public virtual FileEntity Parent { get; set; }

		public int ChildId { get; set; }
		public virtual FileEntity Child { get; set; }
	}
}