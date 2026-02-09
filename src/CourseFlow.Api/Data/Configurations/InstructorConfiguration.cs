using CourseFlow.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseFlow.Api.Data.Configurations;

public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.ToTable("instructors");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.FullName).IsRequired().HasMaxLength(250);
        builder.Property(i => i.Email).IsRequired().HasMaxLength(254);
        builder.Property(i => i.IdentityId).HasMaxLength(500);
        
        builder.HasIndex(i => i.Email).IsUnique();
        builder.HasIndex(i => i.IdentityId).IsUnique();

        builder.HasMany(i => i.Courses)
                .WithOne(c => c.Instructor)
                .OnDelete(DeleteBehavior.Restrict);   
    }
}
