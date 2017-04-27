using System.Data.Entity;
using System.IO;

namespace FileHierarchy.DataAccess
{
	public class Initializer : DropCreateDatabaseIfModelChanges<Context>
	{
		protected override void Seed(Context context)
		{
			base.Seed(context);

			//context.Database.ExecuteSqlCommand(File.ReadAllText(@"~/DBInit"));
		}
	}
}