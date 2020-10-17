using Queo.Commons.Checks;
using Queo.Commons.MessageTemplateRenderer.Context;
using Queo.Commons.MessageTemplateRenderer.Shared;

namespace Queo.Commons.MessageTemplateRenderer.Templates {
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
            Require.NotNullOrEmpty(template, nameof(template));
            Require.NotNull(placeholderRenderContext, nameof(placeholderRenderContext));

            _template = template;
            _placeholderRenderContext = placeholderRenderContext;
        }

        /// <summary>
        ///     Rendert das Template mit den Angegebenen Daten.
        /// </summary>
        /// <param name="modelMap">Die Daten die im Template verwendet werden. Siehe: <see cref="ModelMap" /></param>
        /// <returns>Den gerenderten Text</returns>
        public string Render(ModelMap modelMap) {
            Require.NotNull(modelMap, nameof(modelMap));

            return _placeholderRenderContext.ParseAndRender(_template, modelMap);
        }
    }
}