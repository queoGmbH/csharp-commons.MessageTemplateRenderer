using DotLiquid;

using Queo.Commons.MessageTemplateRenderer.src.Shared;
using Queo.Commons.MessageTemplateRenderer.src.Templates;

namespace Queo.Commons.MessageTemplateRenderer.src.Context {
    /// <summary>
    ///     RenderContext der DotLiquid als Rendering Engine verwendet.
    /// </summary>
    public class DotLiquidRenderContext : IRenderContext {
        /// <summary>
        ///     Bereitet eine Vorlage für das Rendering vor. <see cref="ITemplate.Render" />
        /// </summary>
        /// <param name="template"></param>
        /// <returns>Ein vorbereitetes Template, das gerendert werden kann.</returns>
        public ITemplate Parse(string template) {
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
        public string ParseAndRender(string template, ModelMap modelMap) {
            ITemplate dotLiquidTemplate = Parse(template);
            string render = dotLiquidTemplate.Render(modelMap);
            return render;
        }
    }
}