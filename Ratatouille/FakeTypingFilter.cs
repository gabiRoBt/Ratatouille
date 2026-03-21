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
        public bool IsPaused { get; set; } = false;

        private string _textToType = "";
        private int _currentPosition = 0;

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
            if (IsFakeTypingActive && !IsPaused && pguidCmdGroup == VSConstants.VSStd2K)
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
                            ScheduleFinishAsync();
                        }
                    }

                    // Swallow the key during both typing and cooldown
                    return VSConstants.S_OK;
                }
                else if (nCmdID == (uint)VSConstants.VSStd2KCmdID.CANCEL)
                {
                    StopFakeTyping();
                }
            }

            return m_nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        public void FinishInstantly()
        {
            if (!IsFakeTypingActive) return;
            if (_currentPosition < _textToType.Length)
            {
                string remaining = _textToType.Substring(_currentPosition);
                m_textView.TextBuffer.Insert(m_textView.Caret.Position.BufferPosition, remaining);
            }
            StopFakeTyping();
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }

        public void StopFakeTyping()
        {
            IsFakeTypingActive = false;
            IsPaused = false;
            _currentPosition = 0;
        }

        private async void ScheduleFinishAsync()
        {
            await System.Threading.Tasks.Task.Delay(700);
            StopFakeTyping();
        }
    }
}