using System.Text.RegularExpressions;

using NUnit.Framework;

using Queo.Commons.MessageTemplateRenderer.Context;
using Queo.Commons.MessageTemplateRenderer.Shared;
using Queo.Commons.MessageTemplateRenderer.Templates;

namespace Queo.Commons.MessageTemplateRenderer.Tests {
    [TestFixture]
    public class DotLiquidRenderContextTest {
        [SetUp]
        public void Setup() {
        }

        [Test]
        public void TestParseAndRender() {
            /* Given: Ein einfacher String mit einem Platzhalter */
            const string TEMPLATE = "Das ist ein {{ value }} Template";
            const string PLACE_HOLDER_VALUE = "einfaches";
            string expectedString = TEMPLATE.Replace("{{ value }}", PLACE_HOLDER_VALUE);

            ModelMap model = new ModelMap();
            model.Add("value", PLACE_HOLDER_VALUE);

            /* When: Das Template mit dem Platzhaklter gefüllt werden soll */
            string result = new DotLiquidRenderContext().ParseAndRender(TEMPLATE, model);

            /* Then: Muss der Platzhalter korrekt ersetzt werden. */
            Assert.AreEqual(expectedString, result);
        }

        [Test]
        public void TestParseAndRenderTemplate() {
            /* Given: Ein einfacher String mit einem Platzhalter */
            const string TEMPLATE = "Das ist ein {{ foo.Description }} Template";
            const string PLACE_HOLDER_VALUE = "einfaches";
            string expectedString = TEMPLATE.Replace("{{ foo.Description }}", PLACE_HOLDER_VALUE);

            ModelMap model = new ModelMap();
            ParseTestObject foo = new ParseTestObject(PLACE_HOLDER_VALUE);

            model.Add("foo", foo);

            /* When: Das Template mit dem Platzhalter gefüllt werden soll */
            ITemplate template = new DotLiquidRenderContext().Parse(TEMPLATE);
            string result = template.Render(model);

            /* Then: Muss der Platzhalter korrekt ersetzt werden. */
            Assert.AreEqual(expectedString, result);
        }

        [Test]
        public void TestParseAndRenderTemplatePascalcasePropertyDoNotWork() {
            /* Given: Ein einfacher String mit einem Platzhalter */
            const string TEMPLATE = "Das ist ein {{ foo.Description }} {{ foo.PascalCaseProperty }} Template";
            const string PLACE_HOLDER_VALUE = "einfaches";
            const string PASCALCASE_PLACE_HOLDER_VALUE = "spass";
            string expectedString = TEMPLATE.Replace("{{ foo.Description }}", PLACE_HOLDER_VALUE);
            expectedString = expectedString.Replace("{{ foo.PascalCaseProperty }}", PASCALCASE_PLACE_HOLDER_VALUE);

            ModelMap model = new ModelMap();
            ParseTestObject foo = new ParseTestObject(PLACE_HOLDER_VALUE, PASCALCASE_PLACE_HOLDER_VALUE);

            model.Add("foo", foo);

            /* When: Das Template mit dem Platzhalter gefüllt werden soll */
            ITemplate template = new DotLiquidRenderContext().Parse(TEMPLATE);
            string result = template.Render(model);

            /* Then: Muss der Platzhalter korrekt ersetzt werden. */
            Assert.IsTrue(Regex.IsMatch(result, ".*Missing property.*"));
        }
    }

    public class ParseTestObject {
        public ParseTestObject(string placeHolderValue) {
            Description = placeHolderValue;
        }

        public ParseTestObject(string placeHolderValue, string pascalCaseProperty) {
            Description = placeHolderValue;
            PascalCaseProperty = pascalCaseProperty;
        }

        public string Description { get; }

        public string PascalCaseProperty { get; }
    }
}