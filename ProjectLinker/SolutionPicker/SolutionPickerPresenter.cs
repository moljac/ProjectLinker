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
using Microsoft.Practices.ProjectLinker.SolutionPicker;
using Microsoft.Practices.ProjectLinker.VisualStudio.Helper;

namespace Microsoft.Practices.ProjectLinker.SolutionPicker
{
    public class SolutionPickerPresenter
    {
        private ISolutionPickerView view;
        private readonly IHierarchyNode targetProject;

        public SolutionPickerPresenter(IHierarchyNode solutionNode, ISolutionPickerView view, IHierarchyNode targetProject)
        {
            this.view = view;
            this.targetProject = targetProject;
            view.CanExit = false;
            view.SetRootHierarchyNode(solutionNode);
            view.SelectedNodeChanged += new EventHandler(view_SelectedNodeChanged);
        }

        void view_SelectedNodeChanged(object sender, EventArgs e)
        {
            IHierarchyNode node = view.SelectedNode;
            bool isValidSelection = (node.TypeGuid == targetProject.TypeGuid);

            isValidSelection &= node.ProjectGuid != targetProject.ProjectGuid;

            view.CanExit = isValidSelection;
        }
    }
}