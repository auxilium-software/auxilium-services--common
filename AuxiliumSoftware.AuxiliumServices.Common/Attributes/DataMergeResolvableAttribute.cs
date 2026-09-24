using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DataMergeResolvableAttribute : Attribute
    {
        public DataMergeFieldDefaultEnum Default { get; }
        public bool AllowCombine { get; }

        public DataMergeResolvableAttribute(
            DataMergeFieldDefaultEnum @default = DataMergeFieldDefaultEnum.PreferSurvivorUnlessEmpty,
            bool allowCombine = false
        )
        {
            Default = @default;
            AllowCombine = allowCombine;
        }
    }
}
