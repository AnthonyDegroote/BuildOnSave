using EnvDTE;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace BuildOnSave.Extension
{
    internal class RunningDocTableEvents : IVsRunningDocTableEvents
    {
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
            // Handle the save event here
            System.Diagnostics.Debug.WriteLine("Document saved.");
            _dte.Solution.SolutionBuild.Build(false);

            // Execute tests after build
            _dte.Events.BuildEvents.OnBuildDone += BuildEvents_OnBuildDone;

            return VSConstants.S_OK;
        }

        private void BuildEvents_OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            _dte.Events.BuildEvents.OnBuildDone -= BuildEvents_OnBuildDone;
            _dte.ExecuteCommand("TestExplorer.RunAllTests");
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
