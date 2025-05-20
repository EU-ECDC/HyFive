using System;
using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Constants
{
    [TsClass(IncludeNamespace = false)]
    public class TransferStatusTypeConstants
    {
        [TsProperty(Constant = true)]
        public const string TransferredToFhi = "TRANSFERRED_TO_FHI";
        [TsProperty(Constant = true)]
        public const string TransferredToCoordinator = "TRANSFERRED_TO_COORDINATOR";

        [TsIgnore]
        public static IEnumerable<string> GetTransferStatusTypes(string transferStatusType)
        {
            if (transferStatusType == null)
                throw new ArgumentException("TransferStatusType must have a value");
            if (transferStatusType == TransferStatusTypeConstants.TransferredToCoordinator)
            {
                yield return TransferredToCoordinator;
                yield return TransferredToFhi;
                yield break;
            }
            yield return transferStatusType;
        }
    }
}