using EnvDTE;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace BuildOnSave.Extension
{
    internal class RunningDocTableEvents : IVsRunningDocTableEvents
    {
        private static readonly object locker = new object();

        private readonly DTE _dte;

        public RunningDocTableEvents(DTE dte)
        {
            _dte = dte;
        }

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            if (_dte.Solution.SolutionBuild.BuildState == vsBuildState.vsBuildStateInProgress)
            {
                return VSConstants.S_OK;
            }

            lock (locker)
            {
                if (_dte.Solution.SolutionBuild.BuildState == vsBuildState.vsBuildStateInProgress)
                {
                    return VSConstants.S_OK;
                }

                // Handle the save event here
                _dte.Solution.SolutionBuild.Build(true);

                if (_dte.Solution.SolutionBuild.BuildState == vsBuildState.vsBuildStateDone)
                {
                    // Execute tests after build
                    _dte.ExecuteCommand("TestExplorer.RunAllTests");
                }
            }

            return VSConstants.S_OK;
        }

        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }
    }
}
