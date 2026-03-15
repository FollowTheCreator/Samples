using System;
using System.Collections.Generic;
using IRT.Domain.ValueObjects;

namespace IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces
{
    public interface IFailReasonsService
    {
        /// <summary>
        /// Translates the failReasonIdsString to the ScreenFail reason display names list
        /// </summary>
        /// <param name="failReasonIdsString">ScreenFail reason ids string</param>
        /// <param name="orderAsc">Null - no order, true - order ascending, false - order descending</param>
        /// <returns></returns>
        List<string> GetFailReasons(
            string failReasonIdsString,
            bool? orderAsc = null);

        List<FailReason> GetFailReasons(
            IEnumerable<FailReasonType> failReasonTypes,
            IEnumerable<Guid> failReasonIds);
    }
}
