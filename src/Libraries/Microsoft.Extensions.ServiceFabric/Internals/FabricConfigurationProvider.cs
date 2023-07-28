using System;
using System.Collections.Generic;
using System.Fabric.Description;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.ServiceFabric.Internals
{
    internal sealed class FabricConfigurationProvider : ConfigurationProvider
    {
        readonly ConfigurationSettings _source;

        public FabricConfigurationProvider(ConfigurationSettings source)
        {
            _source = source;
        }

        public override void Load()
        {
            foreach (var section in _source.Sections)
            {
                string sectionPrefix = section.Name + ConfigurationPath.KeyDelimiter;
                foreach (var param in section.Parameters)
                {
                    string key = sectionPrefix + param.Name;
                    string value = param.IsEncrypted ? DecryptValue(param) : param.Value;

                    Data.TryAdd(key, value);
                }
            }

            static string DecryptValue(ConfigurationProperty prop)
            {
                using (SecureString secured = prop.DecryptValue())
                {
                    IntPtr binaryString = Marshal.SecureStringToBSTR(secured);
                    try
                    {
                        return Marshal.PtrToStringBSTR(binaryString);
                    }
                    finally
                    {
                        Marshal.FreeBSTR(binaryString);
                    }
                }
            }
        }
    }
}
