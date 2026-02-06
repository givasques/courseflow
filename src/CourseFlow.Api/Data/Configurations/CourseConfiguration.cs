using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseFlow.Api;

public sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("courses");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title).HasMaxLength(150).IsRequired();
        builder.Property(c => c.Description).HasMaxLength(1000);

        builder.Property(c => c.WorkloadHours).IsRequired();

        builder.HasIndex(c => c.Title).IsUnique();
        builder.HasIndex(c => c.Category);

        builder.HasMany(c => c.Enrollments)
        .WithOne(e => e.Course)
        .OnDelete(DeleteBehavior.Cascade);      
    }
}
