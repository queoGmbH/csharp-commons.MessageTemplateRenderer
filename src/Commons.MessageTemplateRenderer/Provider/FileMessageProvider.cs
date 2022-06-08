using System;
using System.Globalization;
using System.IO;
using System.Threading;

using Microsoft.Extensions.Logging;

using Queo.Commons.Checks;
using Queo.Commons.MessageTemplateRenderer.Context;
using Queo.Commons.MessageTemplateRenderer.Shared;

namespace Queo.Commons.MessageTemplateRenderer.Provider
{
    public class FileMessageProvider : IMessageProvider
    {
        private readonly ILogger _log;

        public FileMessageProvider(IRenderContext renderContext, string resourceRelativePath, ILogger<FileMessageProvider> logger)
        {
            Require.NotNull(renderContext, nameof(renderContext));
            Require.NotNullOrEmpty(resourceRelativePath, nameof(resourceRelativePath));
            Require.NotNull(logger, nameof(logger));

            RenderContext = renderContext;
            ResourceRelativePath = resourceRelativePath;

            _log = logger;
        }

        public IRenderContext RenderContext { get; set; }

        public string ResourceRelativePath { get; set; }

        /// <summary>
        ///     Rendert eine Mailmessage aus dem angegebenen Template und verwendet dabei die Daten aus dem Model.
        /// </summary>
        /// <param name="templateName">Name des Templates</param>
        /// <param name="model">Daten für das Template</param>
        /// <returns></returns>
        public string RenderMessage(string templateName, ModelMap model)
        {
            Require.NotNullOrEmpty(templateName, nameof(templateName));
            Require.NotNull(model, nameof(model));

            _log.LogDebug($"Render message für das Template {templateName}.");
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            string template = LoadMailMessageTemplate(templateName, cultureInfo);
            string evaluatedTemplate = RenderContext.ParseAndRender(template, model);

            return evaluatedTemplate;
        }

        private FileStream FindResource(string templateName, CultureInfo cultureInfo)
        {
            string cultureName = cultureInfo.Name;
            string cultureInfix = string.IsNullOrEmpty(cultureName) ? "" : $".{cultureName}";

            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory; // Directory.GetCurrentDirectory();
            string resourcesPath = Path.Combine(currentDirectory, ResourceRelativePath);
            string resourceName = $"{templateName}{cultureInfix}.template";

            string resourcePath = Path.Combine(resourcesPath, resourceName);

            FileStream resource;
            _log.LogDebug($"Versuche die Resource {resourcePath} zu laden.");

            if (File.Exists(resourcePath))
            {
                resource = new FileStream(resourcePath, FileMode.Open, FileAccess.Read);
            }
            else
            {
                if (string.IsNullOrEmpty(cultureInfo.Name))
                {
                    _log.LogError($"Die Ressource {resourceName} wurde unter {resourcePath} nicht gefunden.");
                    throw new FileNotFoundException($"Die Ressource {resourceName} wurde unter {resourcePath} nicht gefunden.");
                }

                resource = FindResource(templateName, cultureInfo.Parent);
            }

            return resource;
        }

        private string LoadMailMessageTemplate(string templateName, CultureInfo cultureInfo)
        {
            FileStream resource = FindResource(templateName, cultureInfo);
            string template;
            using (StreamReader reader = new StreamReader(resource))
            {
                template = reader.ReadToEnd();
            }

            return template;
        }
    }
}
