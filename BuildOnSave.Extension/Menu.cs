using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnvDTE;

using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace BuildOnSave.Extension
{
    internal class Menu
    {
        const int CommandId = 0x0100;
        const int TopMenuCommandId = 0x1021;
        const int BuildTypeSolutionCommandId = 0x101;
        const int BuildTypeStartupProjectCommandId = 0x102;
        private static readonly Guid CommandSet = new Guid("DA08C663-A54C-44FA-931D-D81A5F65D866");

        private readonly MenuCommand _topMenu;
        private readonly MenuCommand _menuItem;
        private readonly MenuCommand _buildTypeSolution;
        private readonly MenuCommand _buildTypeStartupProject;

        private readonly Window _window;
        private readonly OutputWindowPane _windowPane;

        public Menu(DTE dte, OleMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _topMenu = new MenuCommand(delegate { }, new CommandID(CommandSet, TopMenuCommandId));  
            _menuItem = new MenuCommand(EnableDisableBuildOnSave, new CommandID(CommandSet, CommandId));
            _buildTypeSolution = new MenuCommand(SetBuildTypeToSolution, new CommandID(CommandSet, BuildTypeSolutionCommandId));
            _buildTypeStartupProject = new MenuCommand(SetBuildTypeToStartupProject, new CommandID(CommandSet, BuildTypeStartupProjectCommandId));

            commandService.AddCommand(_topMenu);
            commandService.AddCommand(_menuItem);
            commandService.AddCommand(_buildTypeSolution);
            commandService.AddCommand(_buildTypeStartupProject);

            _window = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
            _windowPane = (_window.Object as OutputWindow).OutputWindowPanes.Add("BuildOnSave");

            _topMenu.Visible = true;
        }

        private void EnableDisableBuildOnSave(object sender, EventArgs e)
        {
            //_solutionOptions.Enabled = !_solutionOptions.Enabled;
            //syncOptions();
        }

        private void SetBuildTypeToSolution(object sender, EventArgs e)
        {
            //setBuildTypeTo(BuildType.Solution);
        }

        private void SetBuildTypeToStartupProject(object sender, EventArgs e)
        {
            //setBuildTypeTo(BuildType.StartupProject);
        }

        private void SyncOptions()
        {
            //if (_solutionOptions.Enabled)
            //{
            //    _menuItem.Text = "Disable BuildOnSave";
            //}
            //else
            //{
            //    _menuItem.Text = "Enable BuildOnSave";
            //}
        }
    }
}
