using CourseFlow.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseFlow.Api.Data.Configurations;

public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("students");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.FullName).IsRequired().HasMaxLength(250);
        builder.Property(s => s.Email).IsRequired().HasMaxLength(254);
        builder.Property(s => s.IdentityId).HasMaxLength(500);
        
        builder.HasIndex(s => s.Email).IsUnique();
        builder.HasIndex(s => s.IdentityId).IsUnique();

        builder.HasMany(s => s.Enrollments)
                .WithOne(e => e.Student)
                .OnDelete(DeleteBehavior.Cascade);         
    }
}
