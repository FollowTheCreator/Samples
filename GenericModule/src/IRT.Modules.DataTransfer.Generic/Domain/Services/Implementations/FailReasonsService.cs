using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using IRT.Domain;
using IRT.Domain.Services.Interfaces;
using IRT.Domain.ValueObjects;
using Kernel.Utilities.Extensions;

namespace IRT.Modules.DataTransfer.Generic.Domain.Services.Implementations
{
    public class FailReasonsService : Generic.Domain.Services.Interfaces.IFailReasonsService
    {
        private const string RegexGuid = @"(?im)[{(]?[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?";

        private readonly IFailReasonLocalizationHelper failReasonLocalizationHelper;
        private readonly IRTDbContext dbContext;

        public FailReasonsService(
            IRTDbContext dbContext,
            IFailReasonLocalizationHelper failReasonLocalizationHelper)
        {
            this.dbContext = dbContext;
            this.failReasonLocalizationHelper = failReasonLocalizationHelper;
        }

        public List<string> GetFailReasons(
            string failReasonIdsString,
            bool? orderAsc = null)
        {
            if (failReasonIdsString.IsNullOrWhiteSpace())
            {
                return new List<string>();
            }

            var failReasonIds = Regex.Matches(failReasonIdsString, RegexGuid)
                .Select(x => new Guid(x.ToString()))
                .ToList();

            var failReasons = GetFailReasons(null, failReasonIds);

            foreach (var failReasonId in failReasonIds)
            {
                var failReason = failReasons.FirstOrDefault(x => x.FailReasonId == failReasonId);
                if (failReason != null)
                {
                    failReasonIdsString = failReasonIdsString.Replace(failReasonId.ToString(), failReason.Reason);
                }
            }

            var result = failReasonIdsString
                .SplitByComma()
                .Select(x => x.Replace(IrtConstants.SymbolsUTFCode.FreeTextReasonPrefixCode, " "))
                .Select(x => x.Replace(IrtConstants.Symbols.FreeTextReasonPrefix, " "));

            switch (orderAsc)
            {
                case true:
                    result = result.Order();

                    break;

                case false:
                    result = result.OrderDescending();

                    break;
            }

            return result.ToList();
        }

        public List<FailReason> GetFailReasons(
            IEnumerable<FailReasonType> failReasonTypes,
            IEnumerable<Guid> failReasonIds)
        {
            var failReasons = dbContext.FailReasons
                .OrderBy(r => r.DisplayIndex)
                .Select(r => new FailReason()
                {
                    FailReasonId = r.FailReasonId,
                    IsFreeTextEntry = r.FreeTextEntry,
                    Reason = r.Reason,
                    DisplayIndex = r.DisplayIndex,
                    FailReasonType = r.FailReasonType,
                    Status = r.Status
                });

            if (failReasonTypes != null)
            {
                failReasons = failReasons.Where(r => failReasonTypes.Contains(r.FailReasonType));
            }

            if (failReasonIds != null)
            {
                failReasons = failReasons.Where(x => failReasonIds.Contains(x.FailReasonId));
            }

            failReasons = failReasonLocalizationHelper.GetLocalizedFailReasons(failReasons);

            return failReasons.ToList();
        }
    }
}
