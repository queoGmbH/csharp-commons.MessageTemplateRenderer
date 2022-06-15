using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using Queo.Commons.Checks;
using Queo.Commons.MessageTemplateRenderer.Shared;
using Queo.Commons.MessageTemplateRenderer.Templates;

namespace Queo.Commons.MessageTemplateRenderer.Context
{
    public class PlaceholderRenderContext : IRenderContext
    {
        private const string PLACEHOLDER_CLOSING = "}";
        private const string PLACEHOLDER_FORMAT_SEPARATOR = ":";

        private const string PLACEHOLDER_OPENING = "{";
        private readonly ILogger _logger;

        /// <summary>
        ///     Erzeugt einen neuen Templater, der die <see cref="CultureInfo.CurrentCulture">CurrentCulture</see> verwendet, um
        ///     Zeichenfolgen zu formatieren.
        /// </summary>
        public PlaceholderRenderContext(ILogger<PlaceholderRenderContext> logger)
        {
            Require.NotNull(logger, nameof(logger));
            _logger = logger;
            Culture = CultureInfo.CurrentCulture;
        }

        /// <summary>
        ///     Erzeugt einen neuen Templater, mit abweichendem Default-Wert.
        /// </summary>
        /// <param name="defaultValue">
        ///     Der Wert durch den Platzhalter ersetzt werden sollen, deren Pfad im Model nicht gefunden
        ///     wird.
        /// </param>
        /// <param name="logger"></param>
        public PlaceholderRenderContext(string defaultValue, ILogger<PlaceholderRenderContext> logger)
            : this(logger)
        {
            DefaultValue = defaultValue;
        }

        /// <summary>
        ///     Erzeugt einen neuen Templater.
        /// </summary>
        /// <param name="culture">
        ///     Legt die Kultur fest, in welcher die Zeichenfolgen ersetzt werden, wenn ein format für den
        ///     Platzhalter definiert ist.
        /// </param>
        /// <param name="logger"></param>
        /// <param name="defaultValue">
        ///     Der Wert durch den Platzhalter ersetzt werden sollen, deren Pfad im Model nicht gefunden
        ///     wird.
        /// </param>
        public PlaceholderRenderContext(CultureInfo culture, ILogger<PlaceholderRenderContext> logger, string defaultValue = null)
        {
            Require.NotNull(culture, nameof(culture));
            Require.NotNull(logger, nameof(logger));
            Culture = culture;
            _logger = logger;
            DefaultValue = defaultValue;
        }

        /// <summary>
        ///     Ruft die Kultur ab, in welcher die Zeichenfolgen ersetzt werden, wenn ein format für den Platzhalter definiert ist.
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// ///
        /// <summary>
        ///     Ruft den Wert ab, der für einen Platzhalter eingefügt wird, wenn der Pfad im Model nicht gefunden wird.
        ///     Ist der Wert NULL werden die Platzhalter so gelassen wie sie im Template stehen.
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        ///     Bereitet eine Vorlage für das Rendering vor. <see cref="ITemplate.Render" />
        /// </summary>
        /// <param name="template"></param>
        /// <returns>Ein vorbereitetes Template, das gerendert werden kann.</returns>
        public ITemplate Parse(string template)
        {
            return new PlaceholderTemplate(template, this);
        }

        /// <summary>
        ///     Bereitet eine Vorlage vor und rendert sie dann anhand der übergebenen Modelldaten.
        /// </summary>
        /// <param name="template">Die Vorlage</param>
        /// <param name="modelMap">Die Daten zum Rendern</param>
        /// <returns>Die gerenderte Vorlage</returns>
        public string ParseAndRender(string template, ModelMap modelMap)
        {
            Require.NotNull(modelMap, nameof(modelMap));
            Require.NotNullOrEmpty(template, nameof(template));

            StringBuilder regexStringBuilder = new StringBuilder();

            /*Platzhalter beginnt mit ...*/
            regexStringBuilder.Append("\\" + PLACEHOLDER_OPENING);

            /* Gruppe beginnen */
            regexStringBuilder.Append("(");

            /* Alle Zeichen die nicht dem Beginnzeichen und nicht dem Endzeichen entsprechen*/
            regexStringBuilder.Append("[^" + PLACEHOLDER_CLOSING + "]+");

            /* Gruppe beenden */
            regexStringBuilder.Append(")");

            /*Platzhalter endet mit ...*/
            regexStringBuilder.Append("\\" + PLACEHOLDER_CLOSING);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(
                    $"In der Zeichenfolge [{template}] sollen alle Teilzeichenfolgen, die den RegEx [{regexStringBuilder}] erfüllen, ersetzt werden.");
            }

            /*Wie soll ersetzt werden.*/
            MatchEvaluator matchEvaluator = delegate (Match match)
            {
                try
                {
                    /*Pfad von Platzhalter-Öffner und -Schließer befreien.*/
                    string placeholder = match.Value.Replace(PLACEHOLDER_OPENING, "").Replace(PLACEHOLDER_CLOSING, "");
                    string[] strings = placeholder.Split(new[] { PLACEHOLDER_FORMAT_SEPARATOR }, 2, StringSplitOptions.None);
                    string format = "";

                    if (strings.Length >= 2)
                    {
                        /*Wenn ein Format definiert ist, nutze dieses.*/
                        format = strings[1];
                    }

                    /*Platzhalter auslesen*/
                    string placeholderPath = strings[0];

                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogDebug($"Der Platzhalter {placeholderPath} soll im Format {format} ersetzt werden.");
                    }

                    /*Wert anhand des Pfads aus dem Model auslesen.*/
                    PlaceholderModel placeholderModel = new PlaceholderModel(placeholderPath, modelMap);
                    object valueByPath = placeholderModel.Value;

                    /*Wert zu string formatierenm, wenn nicht null */
                    if (valueByPath != null)
                    {
                        if (!string.IsNullOrEmpty(format))
                        {
                            /*Wert formatieren, wenn Format definiert ist*/
                            // ReSharper disable FormatStringProblem
                            return string.Format(Culture, "{0:" + format + "}", valueByPath);
                            // ReSharper restore FormatStringProblem
                        }

                        return valueByPath.ToString();
                    }
                }
                catch (Exception ex)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogDebug($"Der Platzhalter {match.Value} konnte nicht ersetzt werden und wird durch den Default-Value ersetzt. {ex}");
                    }
                }

                /*Wenn das ersetzen nicht funktiont, wird der Platzhalter so gelassen wie er ist (DefaultValue == null) oder der DefaultValue eingefügt (DefaultValue != null).*/
                if (DefaultValue == null)
                {
                    return match.Value;
                }

                return DefaultValue;
            };

            /*Nach Platzhaltern suchen.*/
            return Regex.Replace(template, regexStringBuilder.ToString(), matchEvaluator);
        }
    }

    // ToDo: In eigene Datei legen.
    internal class PlaceholderModel
    {
        private readonly string _placeholderName;
        private readonly string _placeholderPath;
        private readonly object _value;

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.
        /// </summary>
        public PlaceholderModel(string placeholder, ModelMap modelMap)
        {
            Require.NotNullOrEmpty(placeholder, nameof(placeholder));
            Require.NotNull(modelMap, nameof(modelMap));

            _placeholderName = GetPlaceholderName(placeholder);
            _placeholderPath = GetPlaceholderPath(placeholder);
            object placeholderObject = GetPlaceholderValue(modelMap, PlaceholderName);
            _value = GetValueFromModelByPath(PlaceholderPath, placeholderObject);
        }

        /// <summary>
        ///     Namensteil des Platzhalters.
        /// </summary>
        public string PlaceholderName
        {
            get { return _placeholderName; }
        }

        /// <summary>
        ///     Pfadanteil des Platzhalters.
        /// </summary>
        public string PlaceholderPath
        {
            get { return _placeholderPath; }
        }

        /// <summary>
        ///     Liefert den durch den Pfad definierten Wert.
        /// </summary>
        public object Value
        {
            get { return _value; }
        }

        private string GetPlaceholderName(string placeholder)
        {
            int indexOf = placeholder.IndexOf(".", StringComparison.Ordinal);
            if (indexOf <= 0)
            {
                return placeholder;
            }

            return placeholder.Substring(0, indexOf);
        }

        private string GetPlaceholderPath(string placeholder)
        {
            int indexOf = placeholder.IndexOf(".", StringComparison.Ordinal);
            if (indexOf <= 0)
            {
                return "";
            }

            return placeholder.Substring(indexOf + 1);
        }

        private object GetPlaceholderValue(ModelMap modelMap, string placeholderName)
        {
            if (modelMap.ContainsKey(placeholderName))
            {
                return modelMap[PlaceholderName];
            }

            return null;
        }

        private object GetValueFromModelByPath(string placeholderPath, object model)
        {
            /*Wenn Model null, dann null liefern.*/
            if (model == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(placeholderPath))
            {
                return model;
            }

            /*Platzhalter Pfad den Properties zuordnen*/
            string[] pathParts = placeholderPath.Split(new[] { '.' }, StringSplitOptions.None);

            object value = model.GetType().GetProperty(pathParts[0]).GetValue(model, null);

            if (pathParts.Length > 1)
            {
                /*Der Pfad hat noch mehr als 1 Stufe => im Model weiter abwärts gehen.*/
                return GetValueFromModelByPath(string.Join(".", pathParts.Skip(1)), value);
            }

            return value;
        }
    }
}
