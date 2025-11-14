using System;
using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Constants
{
    [TsClass(IncludeNamespace = false)]
    public static class TransferStatusTypeConstants
    {
        [TsProperty(Constant = true)]
        public const string TransferredToAdmin = "TRANSFERRED_TO_ADMIN";
        [TsProperty(Constant = true)]
        public const string TransferredToCoordinator = "TRANSFERRED_TO_COORDINATOR";

        [TsIgnore]
        public static IEnumerable<string> GetTransferStatusTypes(string transferStatusType)
        {
            ValidateTransferStatusType(transferStatusType);

            return GetTransferStatusTypesInternal(transferStatusType);
        }

        private static IEnumerable<string> GetTransferStatusTypesInternal(string transferStatusType)
        {
            if (transferStatusType == TransferredToCoordinator)
            {
                yield return TransferredToCoordinator;
                yield return TransferredToAdmin;
                yield break;
            }

            yield return transferStatusType;
        }

        private static void ValidateTransferStatusType(string transferStatusType)
        {
            if (transferStatusType == null)
                throw new ArgumentException("TransferStatusType must have a value");
        }
    }
}