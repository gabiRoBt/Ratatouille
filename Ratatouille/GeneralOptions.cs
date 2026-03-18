using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace Ratatouille // Asigura-te ca namespace-ul coincide cu cel din proiectul tau
{
    public class GeneralOptions : DialogPage
    {
        [Category("Autentificare API")]
        [DisplayName("Cheie API Gemini")]
        [Description("Introdu cheia ta secreta generata in Google AI Studio. Aceasta va fi salvata local, in siguranta.")]
        [PasswordPropertyText(true)] // Transforma textul in stelute (***) cand este tastat
        public string ApiKey { get; set; } = "";
    }
}