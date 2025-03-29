﻿using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace BuildOnSave.Extension.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class BuildCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("22933c3f-e455-40e2-bb6e-c61e133eabe5");

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static BuildCommand Instance { get; private set; }

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        private bool isChecked = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private BuildCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(Execute, menuCommandID);

            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in Command1's constructor requires
            // the UI thread.
            //await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new BuildCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            string message = !isChecked ? "Feature enabled" : "Feature disabled";
            string title = "Hello World Command";

            var result = VsShellUtilities.ShowMessageBox(
                package,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_YESNO,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

            var dialogResult = (DialogResult)result;

            if (dialogResult == DialogResult.Yes)
            {
                isChecked = !isChecked;
                if (sender is MenuCommand menuCommand)
                {
                    menuCommand.Checked = isChecked;
                }
            }
        }
    }
}
