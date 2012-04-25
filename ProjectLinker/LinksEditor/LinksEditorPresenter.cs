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
using Microsoft.Practices.ProjectLinker.Services;
using Microsoft.Practices.ProjectLinker.VisualStudio.Helper;

namespace Microsoft.Practices.ProjectLinker.LinksEditor
{
    public class LinksEditorPresenter
    {
        private readonly ILinksEditorView view;
        private readonly IProjectLinkTracker projectLinkTracker;
        private readonly IHierarchyNodeFactory hierarchyNodeFactory;

        public LinksEditorPresenter(ILinksEditorView view, IProjectLinkTracker projectLinkTracker, IHierarchyNodeFactory hierarchyNodeFactory)
        {
            this.view = view;
            this.view.ProjectsUnlinking += (s, e) => UnlinkProjects(this.view.SelectedProjectLinkItems);
            this.projectLinkTracker = projectLinkTracker;
            this.hierarchyNodeFactory = hierarchyNodeFactory;

            SetUpProjectLinks();
        }

        private void UnlinkProjects(IEnumerable<ProjectLinkItem> projectLinks)
        {
            foreach (ProjectLinkItem link in projectLinks)
            {
                projectLinkTracker.UnlinkProjects(link.SourceProjectGuid, link.TargetProjectGuid);
            }

            SetUpProjectLinks();
        }
        
        private void SetUpProjectLinks()
        {
            Collection<ProjectLinkItem> projectLinks = new Collection<ProjectLinkItem>();
            ProjectLinkItem item;

            foreach (ProjectLink projectLink in projectLinkTracker.GetProjectLinks())
            {
                item = new ProjectLinkItem
                           {
                               SourceProjectName = GetProjectNameFromGuid(projectLink.SourceProjectId),
                               SourceProjectGuid = projectLink.SourceProjectId,
                               TargetProjectName = GetProjectNameFromGuid(projectLink.TargetProjectId),
                               TargetProjectGuid = projectLink.TargetProjectId
                           };
                projectLinks.Add(item);
            }

            view.ProjectLinks = projectLinks;

            IHierarchyNode selectedHierarchyNode = hierarchyNodeFactory.GetSelectedProject();
            if (selectedHierarchyNode != null)
            {
                string selectedProjectRelativeName = selectedHierarchyNode.SolutionRelativeName;
                foreach (ProjectLinkItem linkItem in projectLinks)
                {
                    if (linkItem.TargetProjectName == selectedProjectRelativeName)
                    {
                        this.view.SelectedProjectLinkItems = new Collection<ProjectLinkItem> { linkItem };
                        break;
                    }
                }
            }
        }

        private string GetProjectNameFromGuid(Guid projectGuid)
        {
            IHierarchyNode hierarchyNode = hierarchyNodeFactory.CreateFromProjectGuid(projectGuid);
            return hierarchyNode.SolutionRelativeName;
        }
    }
}