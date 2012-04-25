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
using System;
using System.Windows.Forms;
using Microsoft.Practices.ProjectLinker.SolutionPicker;
using Microsoft.Practices.ProjectLinker.VisualStudio.Helper;
using Microsoft.Practices.ProjectLinker.VisualStudio.Helper.Design;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.Practices.ProjectLinker.SolutionPicker
{
    public partial class SolutionPickerView : Form, ISolutionPickerView
    {
        private SolutionPickerControl _pickerControl;
        private bool _canExit;

        //FxCop: Presenter attaches to ISolutionPickerView and events once created.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.Practices.ProjectLinker.SolutionPicker.SolutionPickerPresenter")]
        public SolutionPickerView(IServiceProvider provider, IHierarchyNode targetProject)
        {
            InitializeComponent();
            IVsSolution solution = provider.GetService(typeof(SVsSolution)) as IVsSolution;
            new SolutionPickerPresenter(new HierarchyNode(solution), this, targetProject);
            this.CenterToParent();
        }

        public void SetRootHierarchyNode(IHierarchyNode value)
        {
            UpdatePickerControl(value);
        }

        private void UpdatePickerControl(IHierarchyNode rootNode)
        {
            if (_pickerControl != null)
            {
                this.contentPanel.Controls.Remove(_pickerControl);
            }

            _pickerControl = new SolutionPickerControl(rootNode, new OnlyProjectsFilter());
            _pickerControl.Left = 0;
            _pickerControl.Top = 0;
            _pickerControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _pickerControl.Size = contentPanel.ClientSize;
            _pickerControl.SelectionChanged += new EventHandler(_pickerControl_SelectionChanged);

            this.contentPanel.Controls.Add(_pickerControl);
        }

        void _pickerControl_SelectionChanged(object sender, EventArgs e)
        {
            EventHandler selectedNodeChangedHandler = SelectedNodeChanged;
            if (selectedNodeChangedHandler != null) selectedNodeChangedHandler(this, EventArgs.Empty);
        }

        public bool CanExit
        {
            get { return _canExit; }
            set
            {
                _canExit = value;
                buttonOK.Enabled = _canExit;
            }
        }

        public IHierarchyNode SelectedNode
        {
            get { return _pickerControl.SelectedTarget; }
        }

        public event EventHandler SelectedNodeChanged;
    }
}