using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Frameworks.Notifications.Entities;
using Kernel.EntityFramework.Interfaces;

namespace IRT.Modules.DataTransfer.Generic.Domain.SqlViews.GenericNotificationDependency
{
    [Table("NotificationDependency")]
    public class NotificationDependencySqlView : IDbModuleEntity
    {
        [Key]
        public Guid NotificationId { get; set; }

        public Guid? DependsOnId { get; set; }

        [Required]
        public string GroupKey { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public NotificationSqlView Notification { get; set; } = null!;
    }

    public class NotificationDependencySqlViewConfiguration : IEntityTypeConfiguration<NotificationDependencySqlView>
    {
        public void Configure(EntityTypeBuilder<NotificationDependencySqlView> builder)
        {
            builder.HasOne(n => n.Notification)
                .WithOne();
        }
    }
}
