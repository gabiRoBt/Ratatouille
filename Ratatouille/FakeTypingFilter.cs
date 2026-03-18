using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Ratatouille
{
    internal class FakeTypingFilter : IOleCommandTarget
    {
        private IOleCommandTarget m_nextCommandHandler;
        private IWpfTextView m_textView;

        public bool IsFakeTypingActive { get; set; } = false;

        private string _textToType = "";
        private int _currentPosition = 0;

        // BUG FIX #5: Folosim o proprietate cu setter explicit astfel încât
        // _currentPosition să se reseteze automat când se setează un text nou.
        // Înainte, dacă utilizatorul genera cod a doua oară fără să tasteze tot
        // primul răspuns, _currentPosition rămânea la o valoare din mijloc și
        // fake typing-ul sărea direct în text sau nu funcționa deloc.
        public string TextToType
        {
            get => _textToType;
            set
            {
                _textToType = value ?? "";
                _currentPosition = 0;
            }
        }

        public FakeTypingFilter(IVsTextView textViewAdapter, IWpfTextView textView)
        {
            this.m_textView = textView;
            textViewAdapter.AddCommandFilter(this, out m_nextCommandHandler);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return m_nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (IsFakeTypingActive && pguidCmdGroup == VSConstants.VSStd2K)
            {
                if (nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
                {
                    if (_currentPosition < _textToType.Length)
                    {
                        char charToInsert = _textToType[_currentPosition];
                        m_textView.TextBuffer.Insert(m_textView.Caret.Position.BufferPosition, charToInsert.ToString());
                        _currentPosition++;

                        if (_currentPosition >= _textToType.Length)
                        {
                            StopFakeTyping();
                        }

                        // Înghite tasta reală apăsată de utilizator
                        return VSConstants.S_OK;
                    }
                }
                else if (nCmdID == (uint)VSConstants.VSStd2KCmdID.CANCEL)
                {
                    StopFakeTyping();
                }
            }

            return m_nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        public void StopFakeTyping()
        {
            IsFakeTypingActive = false;
            _currentPosition = 0;
        }
    }
}