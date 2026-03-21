using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace Ratatouille
{
    public class GeneralOptions : DialogPage
    {
        [Category("1. Gemini API")]
        [DisplayName("API Key")]
        [Description("Your secret key from Google AI Studio. Stored locally, never sent anywhere outside the Gemini API.")]
        [PasswordPropertyText(true)]
        public string ApiKey { get; set; } = "";

        [Category("1. Gemini API")]
        [DisplayName("Language Model")]
        [Description(
            "Choose the model that will generate code:\n" +
            "  flash-lite  — fastest, 1000 req/day free (recommended)\n" +
            "  flash       — smarter, 250 req/day free\n" +
            "  pro         — most powerful, 50 req/day free")]
        [TypeConverter(typeof(GeminiModelConverter))]
        public string Model { get; set; } = "gemini-2.5-flash-lite";

        [Category("2. Generation")]
        [DisplayName("Temperature (0.0 - 1.0)")]
        [Description(
            "Controls how creative the AI response is:\n" +
            "  0.0-0.2  — precise, deterministic code (recommended)\n" +
            "  0.4-0.6  — balanced\n" +
            "  0.8-1.0  — more creative, less predictable")]
        public double Temperature { get; set; } = 0.1;

        [Category("2. Generation")]
        [DisplayName("Custom System Prompt")]
        [Description("Additional instructions sent to the AI before every request. Leave empty for default behavior (raw code, no explanations).")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor),
                typeof(System.Drawing.Design.UITypeEditor))]
        public string SystemPrompt { get; set; } =
            "You are a coding assistant integrated directly into Visual Studio. " +
            "Reply ONLY with the exact code requested, no explanations, no Markdown fences.";

        [Category("3. Customisation")]
        [DisplayName("Keyboard Shortcut")]
        [Description("To change the shortcut, go to Tools -> Options -> Environment -> Keyboard and search for 'Ratatouille.FakeTypingCommand'.")]
        [ReadOnly(true)]
        public string ShortcutInfo { get; set; } = "Ctrl+Alt+Shift+R (change via Tools -> Keyboard)";

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (Temperature < 0.0) Temperature = 0.0;
            if (Temperature > 1.0) Temperature = 1.0;
            base.OnApply(e);
        }
    }

    public class GeminiModelConverter : StringConverter
    {
        private static readonly string[] _models = new[]
        {
            "gemini-2.5-flash-lite",
            "gemini-2.5-flash",
            "gemini-2.5-pro"
        };

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            => new StandardValuesCollection(_models);
    }
}