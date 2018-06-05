using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Community.MicrosoftOffice.AddinHost.Certificates
{
    /// <summary>Represents office add-in development certificate manager.</summary>
    public sealed class CertificateManager
    {
        /// <summary>Creates a development certificate.</summary>
        /// <param name="notBefore">The date on which a certificate becomes valid.</param>
        /// <param name="years">The number of years for a certificate to be valid.</param>
        /// <returns>The X.509 certificate.</returns>
        public X509Certificate2 CreateDevelopmentCertificate(DateTime notBefore, byte years)
        {
            using (var key = RSA.Create(2048))
            {
                var request = new CertificateRequest("CN=DO_NOT_TRUST_OFFICE_ADDIN_TESTING", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                var sanBuilder = new SubjectAlternativeNameBuilder();

                sanBuilder.AddDnsName("localhost");

                request.CertificateExtensions.Add(sanBuilder.Build(true));
                request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, true));
                request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment, true));
                request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, true));

                return request.CreateSelfSigned(notBefore.ToUniversalTime().Date, notBefore.ToUniversalTime().Date.AddYears(years));
            }
        }
    }
}