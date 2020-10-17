using System.Globalization;
using System.IO;
using System.Threading;

using Microsoft.Extensions.Logging.Abstractions;

using NUnit.Framework;

using Queo.Commons.MessageTemplateRenderer.src.Context;
using Queo.Commons.MessageTemplateRenderer.src.Provider;
using Queo.Commons.MessageTemplateRenderer.src.Shared;

namespace Queo.Commons.MessageTemplateRenderer.Tests.src {
    public class FileMessageProviderTest {
        private FileMessageProvider _mailMessageProvider;

        [SetUp]
        public void Setup() {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            IRenderContext renderContext = new DotLiquidRenderContext();
            string resourceRelativePath = Path.Combine("Resources", "MailTemplates");
            _mailMessageProvider = new FileMessageProvider(renderContext, resourceRelativePath, new NullLogger<FileMessageProvider>());
        }

        [Test]
        public void TestLoadResource() {
            string renderedMessage = _mailMessageProvider.RenderMessage("Test", new ModelMap());

            Assert.IsNotNull(renderedMessage);
            StringAssert.StartsWith("Subject: Testbetreff", renderedMessage);
        }

        [Test]
        public void TestRenderMessageWithoutCulture() {
            string renderedMessage = _mailMessageProvider.RenderMessage("TestWithoutCulture", new ModelMap());

            Assert.IsNotNull(renderedMessage);
            StringAssert.StartsWith("Subject: TestWithoutCulture", renderedMessage);
        }

        [Test]
        public void TestRenderNotExistingResource() {
            Assert.Throws<FileNotFoundException>(() => _mailMessageProvider.RenderMessage("TestNotExistingResource", new ModelMap()));
        }
    }
}