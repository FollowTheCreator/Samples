using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Frameworks.ExtendedProperties.Services.Interfaces;
using IRT.Domain.ViewsSql;

namespace IRT.Modules.DataTransfer.Generic.Domain.SqlViews.DynamicDataTransferSettings
{
    [Table("DynamicDataTransferSettings")]
    public class DynamicDataTransferSettingSqlView : IExtendedPropertySqlView
    {
        public string EntityTypeName { get; set; }

        public string ExtendedPropertiesTypeName { get; set; }

        public string EntityId { get; set; }

        public string ExtendedPropertyName { get; set; }

        [MaxLength(DatabaseConstants.NvarcharMaxLength)]
        public string Value { get; set; }
    }

    internal class DynamicDataTransferSettingSqlViewConfiguration : IEntityTypeConfiguration<DynamicDataTransferSettingSqlView>
    {
        public void Configure(EntityTypeBuilder<DynamicDataTransferSettingSqlView> builder)
        {
            builder.HasKey(x => new { x.EntityTypeName, x.ExtendedPropertiesTypeName, x.EntityId, x.ExtendedPropertyName });
        }
    }
}
