using Queo.Commons.MessageTemplateRenderer.src.Context;
using Queo.Commons.MessageTemplateRenderer.src.Shared;

namespace Queo.Commons.MessageTemplateRenderer.src.Templates {
    /// <summary>
    ///     Implementierung des <see cref="ITemplate" /> für den <see cref="PlaceholderRenderContext" />.
    /// </summary>
    /// <remarks>
    ///     Die Klasse wird vom <see cref="PlaceholderRenderContext" /> intern genutzt und darf nicht direkt verwendet werden.
    /// </remarks>
    public class PlaceholderTemplate : ITemplate {
        private readonly PlaceholderRenderContext _placeholderRenderContext;
        private readonly string _template;

        public PlaceholderTemplate(string template, PlaceholderRenderContext placeholderRenderContext) {
            _template = template;
            _placeholderRenderContext = placeholderRenderContext;
        }

        /// <summary>
        ///     Rendert das Template mit den Angegebenen Daten.
        /// </summary>
        /// <param name="modelMap">Die Daten die im Template verwendet werden. Siehe: <see cref="ModelMap" /></param>
        /// <returns>Den gerenderten Text</returns>
        public string Render(ModelMap modelMap) {
            return _placeholderRenderContext.ParseAndRender(_template, modelMap);
        }
    }
}