using Catalog.API.Products.GetProductsByFilter;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API
{
	public class ReadWriteDbContext : DbContext
	{
		public ReadWriteDbContext(DbContextOptions<ReadWriteDbContext> options)
			: base(options)
		{
		}
	}

	public class ReadOnlyDbContext : DbContext
	{
		public DbSet<Product> Products { get; set; } = null!;
		public DbSet<ProductDataResult> ProductDataResults { get; set; } = null!; // For function results

		// Configure the table and function mappings
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Function mapping for ProductDataResult
			modelBuilder.Entity<ProductDataResult>(entity =>
			{
				entity.HasNoKey(); // Since it's a function result, no primary key
				entity.ToView(null); // Prevent EF from mapping it to a table or view

				entity.Property(e => e.Data)
					  .HasColumnName("data")
					  .HasColumnType("jsonb");
			});
		}

			public ReadOnlyDbContext(DbContextOptions<ReadOnlyDbContext> options)
			: base(options)
		{
		}
	}
}
