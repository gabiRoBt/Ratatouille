using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace Ratatouille
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")] // Se aplica pe orice fisier text (C#, JS, HTML etc.)
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal class FakeTypingListener : IWpfTextViewCreationListener
    {
        [Import]
        internal IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

        public void TextViewCreated(IWpfTextView textView)
        {
            IVsTextView textViewAdapter = EditorAdaptersFactoryService.GetViewAdapter(textView);
            if (textViewAdapter != null)
            {
                // Cream filtrul
                var filter = new FakeTypingFilter(textViewAdapter, textView);

                // Salvam instanta filtrului în proprietatile editorului ca sa o putem activa mai tarziu de pe buton
                textView.Properties.GetOrCreateSingletonProperty(() => filter);
            }
        }
    }
}