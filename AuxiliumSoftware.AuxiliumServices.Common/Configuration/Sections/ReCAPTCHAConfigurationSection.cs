using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Configuration.Sections
{
    public class ReCAPTCHAConfigurationSection
    {
        public bool UseReCAPTCHA { get; set; } = true;
        public string SiteKey { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public float ScoreThreshold { get; set; } = 0.0f;
        


        public void Validate()
        {
            // UseReCAPTCHA
            if (string.IsNullOrWhiteSpace(SiteKey))     throw new InvalidOperationException("Configuration value 'ReCAPTCHA->SiteKey' is missing.");
            if (string.IsNullOrWhiteSpace(SecretKey))   throw new InvalidOperationException("Configuration value 'ReCAPTCHA->SecretKey' is missing.");
            if (ScoreThreshold <= 0)                    throw new InvalidOperationException("Configuration value 'ReCAPTCHA->ScoreThreshold' is missing or invalid.");
        }
    }
}
