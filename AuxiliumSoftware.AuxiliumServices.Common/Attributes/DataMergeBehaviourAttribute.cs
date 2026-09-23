using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DataMergeBehaviourAttribute : Attribute
    {
        public DataMergeBehaviourEnum Behaviour { get; }

        public DataMergeBehaviourAttribute(DataMergeBehaviourEnum behaviour)
        {
            Behaviour = behaviour;
        }
    }
}
