using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseFlow.Api;

public sealed class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("enrollments");

        builder.HasKey(e => new {e.CourseId, e.StudentId});

        builder.Property(e => e.Status).HasDefaultValue(EnrollmentStatus.Active);
        builder.Property(e => e.EnrollmentDateUtc).HasDefaultValue(DateTime.UtcNow);

        builder.HasIndex(e => e.Status);

        builder.HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId);
        
        builder.HasOne(e => e.Student)
            .WithMany(e => e.Enrollments)
            .HasForeignKey (e => e.StudentId);
    }
}
