//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
// VsPkg.cs : Implementation of ProjectLinker
//

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.Practices.ProjectLinker;
using Microsoft.Practices.ProjectLinker.Services;
using Microsoft.Practices.ProjectLinker.VisualStudio.Helper;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using AddProjectLinkCommandConstants = Microsoft.Practices.ProjectLinker.Commands.AddProjectLinkCommandConstants;
using EditLinksCommandConstants = Microsoft.Practices.ProjectLinker.Commands.EditLinksCommandConstants;
using LinksEditorView = Microsoft.Practices.ProjectLinker.LinksEditor.LinksEditorView;
using SolutionPickerView = Microsoft.Practices.ProjectLinker.SolutionPicker.SolutionPickerView;
using EnvDTE100;

namespace Microsoft.Practices.ProjectLinker
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the registration utility (regpkg.exe) that this class needs
    // to be registered as package.
    [ProvideAutoLoad("{ADFC4E64-0397-11D1-9F4E-00A0C911004F}")]
    [DefaultRegistryRoot(@"Software\Microsoft\VisualStudio\9.0")]
    [ProvideService(typeof(SProjectLinkTracker), ServiceName = "ProjectLinkTracker")]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // A Visual Studio component can be registered under different regitry roots; for instance
    // when you debug your package you want to register it in the experimental hive. This
    // attribute specifies the registry root to use if no one is provided to regpkg.exe with
    // the /root switch.
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
#pragma warning disable 612,618
    [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
#pragma warning restore 612,618
    // In order be loaded inside Visual Studio in a machine that has not the VS SDK installed, 
    // package needs to have a valid load key (it can be requested at 
    // http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this 
    // package has a load key embedded in its resources.
    [ProvideLoadKey("Standard", "1.0", "Custom Project Linker Package", "Company Name", 1)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource(1000, 1)]
    [Guid(GuidList.guidProjectLinkerPkgString)]
    public sealed class ProjectLinkerPackage : Package, IVsShellPropertyEvents
    {

        uint cookie;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public ProjectLinkerPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // set an eventlistener for shell property changes 
            IVsShell shellService = GetService<IVsShell, SVsShell>();
            if (shellService != null)
                ErrorHandler.ThrowOnFailure(shellService.AdviseShellPropertyChanges(this, out cookie));

            AddAddProjectLinkCommandHandler();
            AddEditLinksCommandHandler();

            //CreateOutputWindow(); //This should be done after Zombie mode is over.
        }

        private void CreateOutputWindow()
        {
            const int initiallyVisible = 1;
            const int clearWhenSolutionUnloads = 1;

            var outputPaneGuid = ProjectLinkerGuids.GuidProjectLinkerOutputPane;
            IVsOutputWindow outputWindow = GetService<IVsOutputWindow, SVsOutputWindow>();
            if (outputWindow == null)
                throw new ProjectLinkerException(Resources.FailedToCreateOutputWindow);

            IVsOutputWindowPane existingPane;
            if (ErrorHandler.Failed(outputWindow.GetPane(ref outputPaneGuid, out existingPane)) || existingPane == null)
            {
                ErrorHandler.ThrowOnFailure(outputWindow.CreatePane(ref outputPaneGuid,
                                                                    Resources.LoggerOutputPaneTitle,
                                                                    initiallyVisible,
                                                                    clearWhenSolutionUnloads)
                    );
            }

        }
        #endregion

        private void AddAddProjectLinkCommandHandler()
        {
            OleMenuCommandService menuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (menuCommandService != null)
            {
                //Register command with OleMenuCommandService
                CommandID menuCommandID = new CommandID(AddProjectLinkCommandConstants.guidAddProjectLinkCommand, (int)AddProjectLinkCommandConstants.cmdidAddProjectLinkCommand);
                OleMenuCommand menuItem = new OleMenuCommand(new EventHandler(AddProjectLinkCommandCallback), menuCommandID);
                menuCommandService.AddCommand(menuItem);
            }
        }

        private void AddProjectLinkCommandCallback(object sender, EventArgs e)
        {
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            IntPtr parentHwnd = IntPtr.Zero;

            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.GetDialogOwnerHwnd(out parentHwnd));

            HierarchyNodeFactory hierarchyNodeFactory = new HierarchyNodeFactory(this);
            IHierarchyNode targetProject = hierarchyNodeFactory.GetSelectedProject();
            var solutionPicker = new SolutionPickerView(this, targetProject);

            DialogResult result = solutionPicker.ShowDialog(new WindowHandleAdapter(parentHwnd));

            if (result == DialogResult.OK)
            {
                IHierarchyNode sourceProject = solutionPicker.SelectedNode;


                var linker = GetService<IProjectLinkTracker, SProjectLinkTracker>();

                try
                {
                    linker.AddProjectLink(sourceProject.ProjectGuid, targetProject.ProjectGuid);

                    ShowInformationalMessageBox(
                                Resources.ProjectLinkerCaption,
                                Resources.ProjectsSuccessfullyLinked,
                                false);

                }
                catch (ProjectLinkerException ex)
                {
                    ShowInformationalMessageBox(Resources.ProjectLinkerCaption, ex.Message, true);
                }
            }
        }

        private void ShowInformationalMessageBox(string title, string text, bool modal)
        {
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            int makeModal = (modal ? 1 : 0);


            ErrorHandler.ThrowOnFailure(
                uiShell.ShowMessageBox(0, // Not used but required by api
                                       ref clsid, // Not used but required by api
                                       title,
                                       text,
                                       String.Empty,
                                       0,
                                       OLEMSGBUTTON.OLEMSGBUTTON_OK,
                                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                                       OLEMSGICON.OLEMSGICON_INFO,
                                       makeModal,
                                       out result
                        )
                    );

        }

        private class WindowHandleAdapter : IWin32Window
        {
            private IntPtr handle;
            public WindowHandleAdapter(IntPtr hwnd)
            {
                handle = hwnd;
            }

            public IntPtr Handle
            {
                get { return handle; }
            }
        }

        private object CreateProjectLinkTracker(IServiceContainer container, Type serviceType)
        {
            if (container != this)
            {
                return null;
            }

            if (typeof(SProjectLinkTracker) == serviceType)
            {
                var trackProjectDocuments = GetService<IVsTrackProjectDocuments2, SVsTrackProjectDocuments>();
                var solution = GetService<IVsSolution, SVsSolution>();
                var outputWindow = GetService<IVsOutputWindow, SVsOutputWindow>();
                var dte = GetService<DTE, SDTE>();
                Solution4 dteSolution = null;
                if (dte != null)
                {
                    dteSolution = (Solution4)dte.Solution;
                }

                return new ProjectLinkTracker(trackProjectDocuments, solution, new OutputWindowLogger(outputWindow), dteSolution);
            }

            return null;
        }

        private TInterface GetService<TInterface, TService>()
            where TInterface : class
            where TService : class
        {
            return this.GetService(typeof(TService)) as TInterface;
        }

        private void AddEditLinksCommandHandler()
        {
            OleMenuCommandService menuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (menuCommandService != null)
            {
                //Register command with OleMenuCommandService
                CommandID menuCommandID = new CommandID(EditLinksCommandConstants.guidEditLinksCommand, (int)EditLinksCommandConstants.cmdidEditLinksCommand);
                OleMenuCommand menuItem = new OleMenuCommand(EditLinksCommandCallback, menuCommandID);
                menuCommandService.AddCommand(menuItem);
            }
        }

        private void EditLinksCommandCallback(object sender, EventArgs e)
        {
            var solution = GetService<IVsSolution, SVsSolution>();

            // As the "Edit Links" command is added on the Project menu even when no solution is opened, it must
            // be validated that a solution exists (in case it doesn't, GetSolutionInfo() returns 3 nulled strings).
            string s1, s2, s3;
            ErrorHandler.ThrowOnFailure(solution.GetSolutionInfo(out s1, out s2, out s3));
            if (s1 != null && s2 != null && s3 != null)
            {
                IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
                IntPtr parentHwnd = IntPtr.Zero;

                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.GetDialogOwnerHwnd(out parentHwnd));
                var linker = GetService<IProjectLinkTracker, SProjectLinkTracker>();
                var linksEditor = new LinksEditorView(linker, this);
                linksEditor.ShowDialog(new WindowHandleAdapter(parentHwnd));
            }
        }

        #region IVsShellPropertyEvents Members

        public int OnShellPropertyChange(int propid, object var)
        {
            // when zombie state changes to false, finish package initialization
            if ((int)__VSSPROPID.VSSPROPID_Zombie == propid)
            {
                if ((bool)var == false)
                {
                    //zombie state dependent code/initialization
                    CreateOutputWindow();
                    
                    var tracker = CreateProjectLinkTracker(this, typeof(SProjectLinkTracker));
                    Debug.Assert(tracker != null, "ProjectLinkTracker creation failed");

                    ((IServiceContainer)this).AddService(typeof(SProjectLinkTracker), tracker, true);
                    ((IServiceContainer)this).AddService(typeof(IHierarchyNodeFactory), new HierarchyNodeFactory(this));

                    // eventlistener no longer needed
                    IVsShell shellService = GetService<IVsShell, SVsShell>();
                    if (shellService != null)
                        ErrorHandler.ThrowOnFailure(shellService.UnadviseShellPropertyChanges(this.cookie));

                    this.cookie = 0;
                }
            }
            return VSConstants.S_OK;
        }

        #endregion
    }
}