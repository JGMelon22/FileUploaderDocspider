using FileUploaderDocspider.Core.Domains.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileUploaderDocspider.Infrastructure.Configuration
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            {
                builder.ToTable("documents");

                builder.HasKey(e => e.Id);

                builder.Property(e => e.Id)
                    .HasColumnName("id");

                builder.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("title");

                builder.Property(e => e.Description)
                    .HasMaxLength(2000)
                    .HasColumnName("description");

                builder.Property(e => e.FileName)
                    .IsRequired()
                    .HasColumnName("file_name");

                builder.Property(e => e.FilePath)
                    .IsRequired()
                    .HasColumnName("file_path");

                builder.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasColumnName("created_at");

                builder.Property(e => e.FileSize)
                    .IsRequired()
                    .HasColumnName("file_size");

                builder.Property(e => e.ContentType)
                    .HasColumnName("content_type");

                builder.HasIndex(e => e.Title)
                    .HasDatabaseName("ux_documents_title");
                //    builder.HasIndex(e => e.Title)
                //        .IsUnique()
                //        .HasName("ux_documents_title");
            }
        }
    }
}