using DotLiquid;

using Queo.Commons.Checks;
using Queo.Commons.MessageTemplateRenderer.Shared;
using Queo.Commons.MessageTemplateRenderer.Templates;

namespace Queo.Commons.MessageTemplateRenderer.Context
{
    /// <summary>
    ///     RenderContext der DotLiquid als Rendering Engine verwendet.
    /// </summary>
    public class DotLiquidRenderContext : IRenderContext
    {
        /// <summary>
        ///     Bereitet eine Vorlage für das Rendering vor. <see cref="ITemplate.Render" />
        /// </summary>
        /// <param name="template"></param>
        /// <returns>Ein vorbereitetes Template, das gerendert werden kann.</returns>
        public ITemplate Parse(string template)
        {
            Require.NotNullOrEmpty(template, nameof(template));
            Template internalTemplate = Template.Parse(template);
            return new DotLiquidTemplate(internalTemplate);
        }

        /// <summary>
        ///     Bereitet eine Vorlage vor und rendert sie dann anhand der übergebenen Modelldaten.
        ///     Hinweis: Camel/ Pascal-Case Properties werfen Fehler.
        /// </summary>
        /// <param name="template">Die Vorlage</param>
        /// <param name="modelMap">Die Daten zum Rendern</param>
        /// <returns>Die gerenderte Vorlage</returns>
        public string ParseAndRender(string template, ModelMap modelMap)
        {
            Require.NotNullOrEmpty(template, nameof(template));
            Require.NotNull(modelMap, nameof(modelMap));
            ITemplate dotLiquidTemplate = Parse(template);
            string render = dotLiquidTemplate.Render(modelMap);
            return render;
        }
    }
}
