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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Microsoft.Practices.ProjectLinker.LinksEditor;
using Microsoft.Practices.ProjectLinker.Services;
using Microsoft.Practices.ProjectLinker.VisualStudio.Helper;

namespace Microsoft.Practices.ProjectLinker.LinksEditor
{
    public partial class LinksEditorView : Form, ILinksEditorView
    {
        private readonly IProjectLinkTracker projectLinkTracker;
        private readonly IServiceProvider provider;

        public event EventHandler ProjectsUnlinking;

        public LinksEditorView(IProjectLinkTracker projectLinkTracker, IServiceProvider provider)
        {
            this.projectLinkTracker = projectLinkTracker;
            this.provider = provider;
            InitializeComponent();
        }

        // FxCop: This sets the data context for the view from the presenter
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public Collection<ProjectLinkItem> ProjectLinks
        {
            set { this.gridProjectLinks.DataSource = value; }
        }

        //FxCop: Allows selected content to be set from outside.  Consider moving this to a method instead of property
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public Collection<ProjectLinkItem> SelectedProjectLinkItems
        {
            get
            {
                Collection<ProjectLinkItem> selectedProjectLinks = new Collection<ProjectLinkItem>();
                foreach (DataGridViewRow row in gridProjectLinks.SelectedRows)
                {
                    selectedProjectLinks.Add(row.DataBoundItem as ProjectLinkItem);
                }

                return selectedProjectLinks;
            }

            set
            {
                foreach (DataGridViewRow row in gridProjectLinks.Rows)
                {
                    row.Selected = value.Contains((ProjectLinkItem)row.DataBoundItem);
                }
            }
        }
        
        private void buttonOk_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        // Presenter is passed to view which and then monitors view activity based on events.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.Practices.ProjectLinker.LinksEditor.LinksEditorPresenter")]
        private void LinksEditorView_Load(object sender, EventArgs e)
        {
            new LinksEditorPresenter(this, projectLinkTracker, new HierarchyNodeFactory(provider));
        }

        private void gridProjectLinks_SelectionChanged(object sender, EventArgs e)
        {
            buttonUnbind.Enabled = gridProjectLinks.SelectedCells.Count > 0;
        }

        private void buttonUnbind_Click(object sender, EventArgs e)
        {
            ProjectsUnlinking(this, EventArgs.Empty);
        }
    }

    public interface ILinksEditorView
    {
        event EventHandler ProjectsUnlinking;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly"), 
        System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        Collection<ProjectLinkItem> ProjectLinks { set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        Collection<ProjectLinkItem> SelectedProjectLinkItems { get; set; }
    }
}