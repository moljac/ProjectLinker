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
using System.Linq;
using Microsoft.Practices.ProjectLinker.LinksEditor;
using Microsoft.Practices.ProjectLinker.Services;
using Microsoft.Practices.ProjectLinker.VisualStudio.Helper;
using Microsoft.Practices.VisualStudio.UnitTestLibrary.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectLinker.Tests.LinksEditor
{
    [TestClass]
    public class LinksEditorPresenterFixture
    {
        [TestMethod]
        public void ShouldSetProjectLinkItemsInView()
        {
            var mockView = new MockLinksEditorView();
            var mockProjectLinkTracker = new MockProjectLinkTracker();
            var sourceHierarchyNode = new MockHierarchyNode {ProjectGuid = Guid.NewGuid(), SolutionRelativeName = "SourceProject"};
            var targetHierarchyNode = new MockHierarchyNode { ProjectGuid = Guid.NewGuid(), SolutionRelativeName = "TargetProject" };
            mockProjectLinkTracker.ProjectLinks = new List<ProjectLink>
                                                      {
                                                          new ProjectLink
                                                              {
                                                                  SourceProjectId = sourceHierarchyNode.ProjectGuid,
                                                                  TargetProjectId = targetHierarchyNode.ProjectGuid
                                                              }
                                                      };
            var mockHierarchyNodeFactory = new MockHierarchyNodeFactory();
            mockHierarchyNodeFactory.HierarchyNodesList.Add(sourceHierarchyNode);
            mockHierarchyNodeFactory.HierarchyNodesList.Add(targetHierarchyNode);

            LinksEditorPresenter presenter = new LinksEditorPresenter(mockView, mockProjectLinkTracker, mockHierarchyNodeFactory);

            Assert.IsTrue(mockProjectLinkTracker.GetProjectLinksCalled);
            Assert.IsTrue(mockView.ProjectLinksCalled);
            Assert.IsNotNull(mockView.ProjectLinks);
            Assert.AreEqual(1, mockView.ProjectLinks.Count);
            Assert.AreEqual("SourceProject", mockView.ProjectLinks[0].SourceProjectName);
            Assert.AreEqual(sourceHierarchyNode.ProjectGuid, mockView.ProjectLinks[0].SourceProjectGuid);
            Assert.AreEqual("TargetProject", mockView.ProjectLinks[0].TargetProjectName);
            Assert.AreEqual(targetHierarchyNode.ProjectGuid, mockView.ProjectLinks[0].TargetProjectGuid);
        }

        [TestMethod]
        public void ShouldSetSelectedProjectOnView()
        {
            var mockProjectLinkTracker = new MockProjectLinkTracker();
            var sourceHierarchyNode = new MockHierarchyNode() { ProjectGuid = Guid.NewGuid(), SolutionRelativeName = "SourceProject" };
            var targetHierarchyNode = new MockHierarchyNode() { ProjectGuid = Guid.NewGuid(), SolutionRelativeName = "TargetProject" };
            mockProjectLinkTracker.ProjectLinks = new List<ProjectLink>
                                                      {
                                                          new ProjectLink
                                                              {
                                                                  SourceProjectId = sourceHierarchyNode.ProjectGuid,
                                                                  TargetProjectId = targetHierarchyNode.ProjectGuid
                                                              }
                                                      };
            var mockHierarchyNodeFactory = new MockHierarchyNodeFactory();
            mockHierarchyNodeFactory.GetSelectedProjectReturnValue = new MockHierarchyNode()
                                                                         {
                                                                             SolutionRelativeName =
                                                                                 targetHierarchyNode.
                                                                                 SolutionRelativeName
                                                                         };
            mockHierarchyNodeFactory.HierarchyNodesList.Add(sourceHierarchyNode);
            mockHierarchyNodeFactory.HierarchyNodesList.Add(targetHierarchyNode);
            var mockView = new MockLinksEditorView();

            LinksEditorPresenter presenter = new LinksEditorPresenter(mockView, mockProjectLinkTracker, mockHierarchyNodeFactory);

            Assert.IsTrue(mockView.SelectedProjectLinkItemCalled);
            Assert.AreEqual("TargetProject", mockView.SelectedProjectLinkItems[0].TargetProjectName);
        }

        [TestMethod]
        public void ShouldUnlinkProjects()
        {
            var mockProjectLinkTracker = new MockProjectLinkTracker();
            var sourceHierarchyNode = new MockHierarchyNode() { ProjectGuid = Guid.NewGuid(), SolutionRelativeName = "SourceProject" };
            var targetHierarchyNode = new MockHierarchyNode() { ProjectGuid = Guid.NewGuid(), SolutionRelativeName = "TargetProject" };
            mockProjectLinkTracker.ProjectLinks = new List<ProjectLink>
                                                      {
                                                          new ProjectLink
                                                              {
                                                                  SourceProjectId = sourceHierarchyNode.ProjectGuid,
                                                                  TargetProjectId = targetHierarchyNode.ProjectGuid
                                                              }
                                                      };
            var mockHierarchyNodeFactory = new MockHierarchyNodeFactory();
            mockHierarchyNodeFactory.GetSelectedProjectReturnValue = new MockHierarchyNode()
            {
                SolutionRelativeName =
                    targetHierarchyNode.
                    SolutionRelativeName
            };
            mockHierarchyNodeFactory.HierarchyNodesList.Add(sourceHierarchyNode);
            mockHierarchyNodeFactory.HierarchyNodesList.Add(targetHierarchyNode);
            var mockView = new MockLinksEditorView();
            LinksEditorPresenter presenter = new LinksEditorPresenter(mockView, mockProjectLinkTracker, mockHierarchyNodeFactory);
            mockView.SelectedProjectLinkItems = new Collection<ProjectLinkItem>
                                         {
                                             new ProjectLinkItem
                                                 {
                                                     SourceProjectGuid = sourceHierarchyNode.ProjectGuid,
                                                     TargetProjectGuid = targetHierarchyNode.ProjectGuid
                                                 }
                                         };
            mockView.ProjectLinksCalled = false;
            mockProjectLinkTracker.GetProjectLinksCalled = false;

            mockView.FireProjectsUnlinking();

            Assert.IsTrue(mockProjectLinkTracker.UnlinkProjectsCalled);
            Assert.AreEqual(sourceHierarchyNode.ProjectGuid, mockProjectLinkTracker.UnlinkProjectsSourceArgument);
            Assert.AreEqual(targetHierarchyNode.ProjectGuid, mockProjectLinkTracker.UnlinkProjectsTargetArgument);
            Assert.IsTrue(mockProjectLinkTracker.GetProjectLinksCalled);
            Assert.IsTrue(mockView.ProjectLinksCalled);
        }

    }

    internal class MockProjectLinkTracker : IProjectLinkTracker
    {
        public List<ProjectLink> ProjectLinks;
        public bool GetProjectLinksCalled;
        public bool UnlinkProjectsCalled;
        public Guid UnlinkProjectsSourceArgument;
        public Guid UnlinkProjectsTargetArgument;

        public void AddProjectLink(Guid sourceProjectId, Guid targetProjectId)
        {
            throw new System.NotImplementedException();
        }

        public void UnlinkProjects(Guid sourceProject, Guid targetProject)
        {
            UnlinkProjectsCalled = true;
            UnlinkProjectsSourceArgument = sourceProject;
            UnlinkProjectsTargetArgument = targetProject;
        }

        public IEnumerable<ProjectLink> GetProjectLinks()
        {
            GetProjectLinksCalled = true;
            return ProjectLinks;
        }
    }

    internal class MockLinksEditorView : ILinksEditorView
    {
        public bool ProjectLinksCalled;
        public bool SelectedProjectLinkItemCalled = false;

        private Collection<ProjectLinkItem> selectedProjectLinkItems;

        public event EventHandler ProjectsUnlinking;


        private Collection<ProjectLinkItem> projectLinks;
        public Collection<ProjectLinkItem> ProjectLinks
        {
            get { return projectLinks; }
            set
            {
                projectLinks = value;
                ProjectLinksCalled = true;
            }
        }

        public Collection<ProjectLinkItem> SelectedProjectLinkItems
        {
            get
            {
                return selectedProjectLinkItems;
            }
            set
            {
                SelectedProjectLinkItemCalled = true;
                selectedProjectLinkItems = value;
            }
        }

        public void FireProjectsUnlinking()
        {
            ProjectsUnlinking.Invoke(this, EventArgs.Empty);
        }
    }

    internal class MockHierarchyNodeFactory : IHierarchyNodeFactory
    {
        public List<MockHierarchyNode> HierarchyNodesList = new List<MockHierarchyNode>();
        public MockHierarchyNode GetSelectedProjectReturnValue;

        public IHierarchyNode CreateFromProjectGuid(Guid projectGuid)
        {
            return HierarchyNodesList.FirstOrDefault(h => h.ProjectGuid == projectGuid);
        }

        public IHierarchyNode GetSelectedProject()
        {
            return GetSelectedProjectReturnValue;
        }

        public IHierarchyNode CreateFromSolutionRelativeName(string projectRelativeName)
        {
            return HierarchyNodesList.FirstOrDefault(h => h.SolutionRelativeName == projectRelativeName);
        }
    }

}
