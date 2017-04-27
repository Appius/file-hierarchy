using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace FileHierarchy.Common.Models
{
	[Table("Entities")]
	public sealed class FileEntity
	{
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public FileEntity()
		{
			Children = new HashSet<FileEntity>();

			Ancestors = new HashSet<Tree>();
			Descendants = new HashSet<Tree>();
		}

		public int Id { get; set; }

		[Required]
		[StringLength(50)]
		public string Name { get; set; }

		public EntityType Type { get; set; }

		public int SeqNum { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public ICollection<FileEntity> Children { get; set; }
		public FileEntity Parent { get; set; }
		public int? ParentId { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public ICollection<Tree> Ancestors { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public ICollection<Tree> Descendants { get; set; }
	}
}