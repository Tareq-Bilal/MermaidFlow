using MermaidFlow.Domain.Mermaid;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MermaidFlow.Infrastructure.Persistence.Configurations;

public class DiagramCacheConfiguration : IEntityTypeConfiguration<DiagramCache>
{
    public void Configure(EntityTypeBuilder<DiagramCache> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.MermaidHash)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasIndex(c => c.MermaidHash);

        builder.Property(c => c.RenderedSvg)
            .IsRequired();

        builder.Property(c => c.Theme)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.ExpiresAt)
            .IsRequired();

        builder.HasIndex(c => c.ExpiresAt);
    }
}
