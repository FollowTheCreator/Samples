using System.Linq;
using Microsoft.EntityFrameworkCore;
using IRT.Modules.DataTransfer.Generic.Domain.Database;
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews.DynamicDataTransferSettings;
using Kernel.DDD.Dispatching;
using Kernel.DDD.Domain.Events;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.DtExtendedProperty
{
    public class DtExtendedPropertySqlViewHandler : IEventHandler
    {
        private readonly ModuleDbContext dbContext;

        public DtExtendedPropertySqlViewHandler(ModuleDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public virtual void Handle(UpdatedEvent<DtExtendedPropertyDto> e)
        {
            var dtExtendedPropertyDto = e.Item;

            var dynamicDataTransferSettings = dbContext.DynamicDataTransferSettings
                .Where(x => x.EntityTypeName == dtExtendedPropertyDto.EntityTypeName
                    && x.EntityId == dtExtendedPropertyDto.EntityId
                    && x.ExtendedPropertiesTypeName == dtExtendedPropertyDto.ExtendedPropertiesTypeName
                    && x.ExtendedPropertyName == dtExtendedPropertyDto.ExtendedPropertyName)
                .SingleOrDefault();

            if (dynamicDataTransferSettings != null)
            {
                if (dtExtendedPropertyDto.Value != null)
                {
                    dynamicDataTransferSettings.Value = dtExtendedPropertyDto.Value;
                    dbContext.DynamicDataTransferSettings.Update(dynamicDataTransferSettings);
                    dbContext.SaveChanges();
                }
                else
                {
                    dbContext.DynamicDataTransferSettings
                        .Where(x => x.EntityTypeName == dtExtendedPropertyDto.EntityTypeName
                            && x.EntityId == dtExtendedPropertyDto.EntityId
                            && x.ExtendedPropertiesTypeName == dtExtendedPropertyDto.ExtendedPropertiesTypeName
                            && x.ExtendedPropertyName == dtExtendedPropertyDto.ExtendedPropertyName)
                        .ExecuteDelete();
                }
            }
            else
            {
                if (dtExtendedPropertyDto.Value != null)
                {
                    var itemToAdd = new DynamicDataTransferSettingSqlView
                    {
                        EntityTypeName = dtExtendedPropertyDto.EntityTypeName,
                        EntityId = dtExtendedPropertyDto.EntityId,
                        ExtendedPropertiesTypeName = dtExtendedPropertyDto.ExtendedPropertiesTypeName,
                        ExtendedPropertyName = dtExtendedPropertyDto.ExtendedPropertyName,
                        Value = dtExtendedPropertyDto.Value
                    };

                    dbContext.DynamicDataTransferSettings.Add(itemToAdd);
                    dbContext.SaveChanges();
                }
            }
        }
    }
}
