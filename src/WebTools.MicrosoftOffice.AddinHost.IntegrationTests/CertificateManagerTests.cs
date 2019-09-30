using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using WebTools.MicrosoftOffice.AddinHost.Certificates;

namespace WebTools.MicrosoftOffice.AddinHost.IntegrationTests
{
    [TestClass]
    public sealed class CertificateManagerTests
    {
        [TestMethod]
        public void CreateDevelopmentCertificate()
        {
            var manager = new CertificateManager();
            var notBefore = new DateTime(2015, 01, 01, 01, 01, 01, DateTimeKind.Utc);
            var certificate = manager.CreateDevelopmentCertificate(notBefore, notBefore.AddMonths(1) - notBefore);

            Assert.IsNotNull(certificate);
            Assert.AreEqual(notBefore.Date, certificate.NotBefore.ToUniversalTime());
            Assert.AreEqual(notBefore.Date.AddMonths(1), certificate.NotAfter.ToUniversalTime());
            Assert.IsNotNull(certificate.Subject);
            Assert.IsTrue(certificate.Subject.Contains("DO_NOT_TRUST"));
        }
    }
}
