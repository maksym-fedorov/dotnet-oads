using System;
using Anemonis.MicrosoftOffice.AddinHost.Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.MicrosoftOffice.AddinHost.IntegrationTests
{
    [TestClass]
    public sealed class CertificateManagerTests
    {
        [TestMethod]
        public void CreateDevelopmentCertificate()
        {
            var manager = new CertificateManager();
            var notBefore = new DateTime(2015, 01, 01, 01, 01, 01, DateTimeKind.Utc);
            var certificate = manager.CreateDevelopmentCertificate(notBefore, 1);

            Assert.IsNotNull(certificate);
            Assert.AreEqual(notBefore.Date, certificate.NotBefore.ToUniversalTime());
            Assert.AreEqual(notBefore.Date.AddYears(1), certificate.NotAfter.ToUniversalTime());
            Assert.IsNotNull(certificate.Subject);
            Assert.IsTrue(certificate.Subject.Contains("DO_NOT_TRUST"));
        }
    }
}