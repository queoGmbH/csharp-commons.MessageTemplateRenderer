using System.Globalization;
using System.IO;
using System.Threading;

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using NUnit.Framework;
using NUnit.Framework.Legacy;

using Queo.Commons.MessageTemplateRenderer.Context;
using Queo.Commons.MessageTemplateRenderer.Provider;
using Queo.Commons.MessageTemplateRenderer.Shared;

namespace Queo.Commons.MessageTemplateRenderer.Tests
{
    public class FileMessageProviderTest
    {
        private FileMessageProvider _mailMessageProvider;

        [SetUp]
        public void Setup()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            IRenderContext renderContext = new DotLiquidRenderContext();
            string resourceRelativePath = Path.Combine("Resources", "MailTemplates");
            _mailMessageProvider = new FileMessageProvider(renderContext, resourceRelativePath, new NullLogger<FileMessageProvider>());
        }

        [Test]
        public void TestLoadResource()
        {
            string renderedMessage = _mailMessageProvider.RenderMessage("Test", new ModelMap());

            ClassicAssert.IsNotNull(renderedMessage);
            StringAssert.StartsWith("Subject: Testbetreff", renderedMessage);
        }

        [Test]
        public void TestRenderMessageWithoutCulture()
        {
            string renderedMessage = _mailMessageProvider.RenderMessage("TestWithoutCulture", new ModelMap());

            ClassicAssert.IsNotNull(renderedMessage);
            StringAssert.StartsWith("Subject: TestWithoutCulture", renderedMessage);
        }

        [Test]
        public void TestRenderNotExistingResource()
        {
            Assert.Throws<FileNotFoundException>(() => _mailMessageProvider.RenderMessage("TestNotExistingResource", new ModelMap()));
        }

        [Test]
        public void TestLoadResourceWithOptionsConstructor()
        {
            IRenderContext renderContext = new DotLiquidRenderContext();
            FileMessageProviderOptions providerOptions = new FileMessageProviderOptions
            {
                ResourceRelativePath = Path.Combine("Resources", "MailTemplates")
            };

            IMessageProvider messageProvider = new FileMessageProvider(
                renderContext,
                Options.Create(providerOptions),
                new NullLogger<FileMessageProvider>());

            string renderedMessage = messageProvider.RenderMessage("Test", new ModelMap());

            ClassicAssert.IsNotNull(renderedMessage);
            StringAssert.StartsWith("Subject: Testbetreff", renderedMessage);
        }
    }
}
