using System;
using System.Linq;
using System.Reflection;

using DotLiquid;

using Queo.Commons.Checks;
using Queo.Commons.MessageTemplateRenderer.Shared;

namespace Queo.Commons.MessageTemplateRenderer.Templates
{
    /// <summary>
    ///     Kaspelt das Template für den DotLiquidRenderContext.
    /// </summary>
    /// <remarks>
    ///     Die Klasse ist für interne Zwecke bestimmt und sollte nicht direkt verwendet werden.
    /// </remarks>
    public class DotLiquidTemplate : ITemplate
    {
        private readonly Template _internalTemplate;

        public DotLiquidTemplate(Template internalTemplate)
        {
            Require.NotNull(internalTemplate, nameof(internalTemplate));
            _internalTemplate = internalTemplate;
        }

        /// <summary>
        ///     Rendert das Template mit den Angegebenen Daten.
        /// </summary>
        /// <param name="modelMap">Die Daten die im Template verwendet werden. Siehe: <see cref="ModelMap" /></param>
        /// <returns>Den gerenderten Text</returns>
        public string Render(ModelMap modelMap)
        {
            Require.NotNull(modelMap, nameof(modelMap));

            RegisterTypes(modelMap);
            string render = _internalTemplate.Render(Hash.FromDictionary(modelMap));
            return render;
        }

        private void RegisterTypes(ModelMap modelMap)
        {
            foreach (object modelMapValue in modelMap.Values)
            {
                Type modelMapValueType = modelMapValue.GetType();
                PropertyInfo[] propertyInfos = modelMapValueType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                Template.RegisterSafeType(modelMapValueType, propertyInfos.Select(pi => pi.Name).ToArray());
            }
        }
    }
}
