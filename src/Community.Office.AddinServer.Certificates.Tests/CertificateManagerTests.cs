using System;
using Community.Office.AddinServer.Certificates.Generation;
using Xunit;

namespace Community.Office.AddinServer.Certificates.Tests
{
    public sealed class CertificateManagerTests
    {
        [Fact]
        public void CreateDevelopmentCertificate()
        {
            var manager = new CertificateManager();
            var notBefore = new DateTime(2015, 01, 01, 01, 01, 01, DateTimeKind.Utc);
            var certificate = manager.CreateDevelopmentCertificate(notBefore);

            Assert.NotNull(certificate);
            Assert.Equal(notBefore.Date, certificate.NotBefore.ToUniversalTime());
            Assert.Equal(notBefore.Date.AddYears(1), certificate.NotAfter.ToUniversalTime());
            Assert.Contains("DO_NOT_TRUST", certificate.Subject);
        }
    }
}