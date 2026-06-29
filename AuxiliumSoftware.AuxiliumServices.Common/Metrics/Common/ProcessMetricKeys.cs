using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Common
{
    public sealed record ProcessMetricKeys(
        SystemMetricKeyEnum Cpu,
        SystemMetricKeyEnum Memory,
        SystemMetricKeyEnum Uptime
    );
}
