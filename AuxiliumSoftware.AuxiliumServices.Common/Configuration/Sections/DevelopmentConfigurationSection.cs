using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Configuration.Sections
{
    public class DevelopmentConfigurationSection
    {
        public bool PHPAcceptSelfSignedCertificatesForAPI { get; set; } = false;
        


        public void Validate()
        {
            // DisableReCAPTCHA
            // PHPAcceptSelfSignedCertificatesForAPI
        }
    }
}
