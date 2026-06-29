using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Configuration.Sections.FileSystem
{
    public class RootStorageDirectoriesConfigurationSection
    {
        public string FormData { get; set; } = null!;
        public string AuxLFS { get; set; } = null!;
        public string Metrics { get; set; } = null!;



        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(FormData))    throw new InvalidOperationException("Configuration value 'RootStorageDirectories->FormData' is missing.");
            if (string.IsNullOrWhiteSpace(AuxLFS))      throw new InvalidOperationException("Configuration value 'RootStorageDirectories->AuxLFS' is missing.");
            if (string.IsNullOrWhiteSpace(Metrics))     throw new InvalidOperationException("Configuration value 'RootStorageDirectories->Metrics' is missing.");
        }
    }
}
