using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using BuildOnSave.Extension.Commands;

using EnvDTE;

using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace BuildOnSave.Extension
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(Constants.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class BuildOnSaveExtensionPackage : AsyncPackage
    {
        private uint _rdtEventCookie;
        private RunningDocTableEvents _rdtEvents;
        private uint _solutionEventsCookie;
        private SolutionEventsHandler _solutionEventsHandler;

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // Get the DTE service
            DTE dte = await GetServiceAsync(typeof(DTE)) as DTE;
            Assumes.Present(dte);

            // Get the Running Document Table (RDT) service
            IVsRunningDocumentTable rdt = await GetServiceAsync(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
            Assumes.Present(rdt);

            // Create an instance of the event handler
            _rdtEvents = new RunningDocTableEvents(dte);

            // Advise the RDT of our event handler
            rdt.AdviseRunningDocTableEvents(_rdtEvents, out _rdtEventCookie);

            // Get the Solution service
            IVsSolution solution = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
            Assumes.Present(solution);

            // Create an instance of the solution events handler
            _solutionEventsHandler = new SolutionEventsHandler(this);

            // Advise the solution of our event handler
            solution.AdviseSolutionEvents(_solutionEventsHandler, out _solutionEventsCookie);

            await BuildCommand.InitializeAsync(this);
            await base.InitializeAsync(cancellationToken, progress);

            /**
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            await BuildCommand.InitializeAsync(this);
            await base.InitializeAsync(cancellationToken, progress);
            **/
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (disposing)
            {
                // Unadvise the RDT of our event handler
                if (_rdtEventCookie != 0)
                {
                    if (GetService(typeof(SVsRunningDocumentTable)) is IVsRunningDocumentTable rdt)
                    {
                        rdt.UnadviseRunningDocTableEvents(_rdtEventCookie);
                        _rdtEventCookie = 0;
                    }
                }



                // Unadvise the solution of our event handler
                if (_solutionEventsCookie != 0)
                {
                    if (GetService(typeof(SVsSolution)) is IVsSolution solution)
                    {
                        solution.UnadviseSolutionEvents(_solutionEventsCookie);
                        _solutionEventsCookie = 0;
                    }
                }
            }

            base.Dispose(disposing);
        }
    }
}
