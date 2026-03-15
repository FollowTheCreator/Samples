using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IRT.Domain.ViewsSql;
using Kernel.EntityFramework.Interfaces;

namespace IRT.Modules.DataTransfer.Generic.Domain.SqlViews.DynamicSetting
{
    [Table("DynamicSettings")]
    public class DynamicSettingSqlView : IDbModuleEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid DynamicSettingId { get; set; }

        [Required]
        [MaxLength(DatabaseConstants.EntityNameMaxLength)]
        public string Name { get; set; }

        [Required]
        [MaxLength(DatabaseConstants.NvarcharMaxLength)]
        public string Description { get; set; }

        [Required]
        [MaxLength(DatabaseConstants.NvarcharMaxLength)]
        public string DefaultValue { get; set; }

        [Required]
        public string EntityTypeName { get; set; }

        [Required]
        public string ExtendedPropertyName { get; set; }
    }

    public class DynamicSettingSqlViewConfiguration : IEntityTypeConfiguration<DynamicSettingSqlView>
    {
        public void Configure(EntityTypeBuilder<DynamicSettingSqlView> builder)
        {
        }
    }
}
