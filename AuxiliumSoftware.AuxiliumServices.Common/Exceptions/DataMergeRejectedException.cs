using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Exceptions
{
    public class DataMergeRejectedException : Exception
    {
        public DataMergeRejectionReasonEnum Reason { get; }

        public DataMergeRejectedException(DataMergeRejectionReasonEnum reason, string message)
            : base(message)
        {
            Reason = reason;
        }
    }
}
