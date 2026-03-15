using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.DynamicSetting.Commands;
using Kernel.DDD.Dispatching;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.DynamicSetting
{
    public class DynamicSettingCommandHandler : ICommandHandler
    {
        private readonly AggregateRepository<DynamicSettingAggregate> aggregateRepository;

        public DynamicSettingCommandHandler(
            AggregateRepository<DynamicSettingAggregate> aggregateRepository)
        {
            this.aggregateRepository = aggregateRepository;
        }

        public void Handle(CreateDynamicSetting command)
        {
            aggregateRepository.Perform(
                    aggregateId: command.DynamicSettingId,
                    action: a => a.CreateDynamicSetting(command: command));
        }
    }
}
