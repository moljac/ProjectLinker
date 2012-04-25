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
using System.Linq;
using Microsoft.Practices.ProjectLinker;
using Microsoft.Practices.ProjectLinker.Services;
using Microsoft.Practices.VisualStudio.UnitTestLibrary.Mocks;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectLinker.Tests.Services
{
    using System.Collections.Generic;
    using EnvDTE;
    using Mocks;
    using EnvDTE100;

    [TestClass]
    public class ProjectLinkTrackerFixture
    {
        [TestMethod]
        public void ShouldRegisterInDocumentTracker()
        {
            var documentTracker = new MockDocumentTracker();

            ProjectLinkTracker tracker = new ProjectLinkTracker(documentTracker, new MockIVsSolution(), null);

            Assert.IsTrue(documentTracker.AdviseTrackProjectDocumentsEventsCalled);
            Assert.AreSame(tracker, documentTracker.AdviseTrackProjectDocumentsEventsArgumentEventSink);
        }

        [TestMethod]
        public void ShouldRegisterInSolutionEvents()
        {
            var vsSolution = new MockIVsSolution();

            ProjectLinkTracker tracker = new ProjectLinkTracker(new MockDocumentTracker(), vsSolution, null);

            Assert.IsTrue(vsSolution.AdviseSolutionEventsCalled);
        }

        [TestMethod]
        public void ShouldAddNewItemsToSync()
        {
            var solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            var project1VsHierarchy = new MockVsHierarchy();
            var project2VsHierarchy = new MockVsHierarchy();
            solution.Hierarchies.Add(project1VsHierarchy);
            solution.Hierarchies.Add(project2VsHierarchy);

            tracker.AddProjectLink(project1VsHierarchy.GetPropertyProjectIdGuidValue, project2VsHierarchy.GetPropertyProjectIdGuidValue);

            Assert.IsTrue(tracker.ProjectsAreLinked(project1VsHierarchy, project2VsHierarchy));
            Assert.IsFalse(tracker.ProjectsAreLinked(project2VsHierarchy, project1VsHierarchy));
        }

        [TestMethod]
        public void ShouldTrackMultipleProjects()
        {
            var solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            var project1VsHierarchy = new MockVsHierarchy();
            var project2VsHierarchy = new MockVsHierarchy();
            var project3VsHierarchy = new MockVsHierarchy();
            solution.Hierarchies.Add(project1VsHierarchy);
            solution.Hierarchies.Add(project2VsHierarchy);
            solution.Hierarchies.Add(project3VsHierarchy);

            tracker.AddProjectLink(project1VsHierarchy, project2VsHierarchy);
            tracker.AddProjectLink(project1VsHierarchy, project3VsHierarchy);

            Assert.IsTrue(tracker.ProjectsAreLinked(project1VsHierarchy, project2VsHierarchy));
            Assert.IsTrue(tracker.ProjectsAreLinked(project1VsHierarchy, project3VsHierarchy));
        }

        [TestMethod]
        [ExpectedException(typeof(ProjectLinkerException))]
        public void ShouldThrowIfAddingSameLink()
        {
            var solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            var project1VsHierarchy = new MockVsHierarchy();
            var project2VsHierarchy = new MockVsHierarchy();
            solution.Hierarchies.Add(project1VsHierarchy);
            solution.Hierarchies.Add(project2VsHierarchy);

            tracker.AddProjectLink(project1VsHierarchy, project2VsHierarchy);
            tracker.AddProjectLink(project1VsHierarchy, project2VsHierarchy);
        }

        [TestMethod]
        public void ShouldDispatchAddToSyncher()
        {
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), new MockIVsSolution());
            var project1 = new MockVsHierarchy();
            var project2 = new MockVsHierarchy();

            MockProjectItemsSyncer syncher = new MockProjectItemsSyncer(project1.GetPropertyProjectValue, project2.GetPropertyProjectValue);

            tracker.AddProjectSyncer(syncher);

            tracker.OnAfterAddFilesEx(1, 1, new[] { project1 }, new[] { 0 }, new[] { "File1.txt" },
                                      new[] { VSADDFILEFLAGS.VSADDFILEFLAGS_NoFlags });

            Assert.IsTrue(syncher.FilesAddedToSourceCalled);
        }

        [TestMethod]
        public void ShouldDispatchOnlyToMatchingSynchers()
        {
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), new MockIVsSolution());
            var project1 = new MockVsHierarchy();
            var project2 = new MockVsHierarchy();
            var project3 = new MockVsHierarchy();

            MockProjectItemsSyncer syncherMatching = new MockProjectItemsSyncer(project1.GetPropertyProjectValue, project2.GetPropertyProjectValue);
            MockProjectItemsSyncer syncherNonMatching = new MockProjectItemsSyncer(project3.GetPropertyProjectValue, project2.GetPropertyProjectValue);

            tracker.AddProjectSyncer(syncherMatching);
            tracker.AddProjectSyncer(syncherNonMatching);

            tracker.OnAfterAddFilesEx(1, 1, new[] { project1 }, new[] { 0 }, new[] { "File1.txt" },
                                      new[] { VSADDFILEFLAGS.VSADDFILEFLAGS_NoFlags });

            Assert.IsTrue(syncherMatching.FilesAddedToSourceCalled);
            Assert.IsFalse(syncherNonMatching.FilesAddedToSourceCalled);
        }

        [TestMethod]
        public void ShouldDispatchRemoveToSyncher()
        {
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), new MockIVsSolution());
            var project1 = new MockVsHierarchy();
            var project2 = new MockVsHierarchy();

            MockProjectItemsSyncer syncher = new MockProjectItemsSyncer(project1.GetPropertyProjectValue, project2.GetPropertyProjectValue);

            tracker.AddProjectSyncer(syncher);

            tracker.OnAfterRemoveFiles(1, 1, new[] { project1 }, new[] { 0 }, new[] { "File1.txt" },
                                      new[] { VSREMOVEFILEFLAGS.VSREMOVEFILEFLAGS_NoFlags });

            Assert.IsTrue(syncher.FilesRemovedFromSourceCalled);
        }

        [TestMethod]
        public void ShouldDispatchRenameFilesToSyncher()
        {
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), new MockIVsSolution());
            var project1 = new MockVsHierarchy();
            var project2 = new MockVsHierarchy();

            MockProjectItemsSyncer syncher = new MockProjectItemsSyncer(project1.GetPropertyProjectValue, project2.GetPropertyProjectValue);

            tracker.AddProjectSyncer(syncher);

            tracker.OnAfterRenameFiles(1, 2, new[] { project1 }, new[] { 0 }, new[] { "oldFileName", "oldDirectoryName" }, new[] { "newFileName", "newDirectoryName" },
                                      new[] { VSRENAMEFILEFLAGS.VSRENAMEFILEFLAGS_NoFlags, VSRENAMEFILEFLAGS.VSRENAMEFILEFLAGS_Directory });

            Assert.IsTrue(syncher.FilesRenamedInSourceCalled);
        }


        [TestMethod]
        public void ShouldDispatchAddDirectoriesToSyncher()
        {
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), new MockIVsSolution());
            var project1 = new MockVsHierarchy();
            var project2 = new MockVsHierarchy();

            MockProjectItemsSyncer syncher = new MockProjectItemsSyncer(project1.GetPropertyProjectValue, project2.GetPropertyProjectValue);

            tracker.AddProjectSyncer(syncher);

            tracker.OnAfterAddDirectoriesEx(1, 1, new[] { project1 }, new[] { 0 }, new[] { "Myfolder" },
                                      new[] { VSADDDIRECTORYFLAGS.VSADDDIRECTORYFLAGS_NoFlags });

            Assert.IsTrue(syncher.DirectoriesAddedToSourceCalled);
        }

        [TestMethod]
        public void ShouldDispatchRemoveDirectoriesToSyncher()
        {
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), new MockIVsSolution());
            var project1 = new MockVsHierarchy();
            var project2 = new MockVsHierarchy();

            MockProjectItemsSyncer syncher = new MockProjectItemsSyncer(project1.GetPropertyProjectValue, project2.GetPropertyProjectValue);

            tracker.AddProjectSyncer(syncher);

            tracker.OnAfterRemoveDirectories(1, 1, new[] { project1 }, new[] { 0 }, new[] { "Myfolder" },
                                      new[] { VSREMOVEDIRECTORYFLAGS.VSREMOVEDIRECTORYFLAGS_NoFlags });

            Assert.IsTrue(syncher.DirectoriesRemovedFromSourceCalled);
        }

        [TestMethod]
        public void ShouldCreateSyncherWhenLoadingLinkedTargetProjectAfterSourceProject()
        {
            var solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            var sourceHierarchy = new MockVsHierarchy();
            var targetHierarchy = new MockVsHierarchy();
            targetHierarchy.GetPropertyProjectValue.Globals.Dictionary["ProjectLinkReference"] = sourceHierarchy.GetPropertyProjectIdGuidValue.ToString();

            solution.Hierarchies.Add(sourceHierarchy);
            tracker.OnAfterOpenProject(sourceHierarchy, 0);

            Assert.IsFalse(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));

            tracker.OnAfterOpenProject(targetHierarchy, 0);

            Assert.IsTrue(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));
        }

        [TestMethod]
        public void ShouldCreateSyncherWhenLoadingLinkedSourceProjectAfterTargetProject()
        {
            var solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            var sourceHierarchy = new MockVsHierarchy();
            var targetHierarchy = new MockVsHierarchy();
            targetHierarchy.GetPropertyProjectValue.Globals.Dictionary["ProjectLinkReference"] = sourceHierarchy.GetPropertyProjectIdGuidValue.ToString();

            tracker.OnAfterOpenProject(targetHierarchy, 0);

            Assert.IsFalse(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));

            solution.Hierarchies.Add(sourceHierarchy);
            tracker.OnAfterOpenProject(sourceHierarchy, 0);

            Assert.IsTrue(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));
        }

        [TestMethod]
        public void AddProjectLinkAddsPersistInfoToTargetProject()
        {
            var solution = new MockIVsSolution();
            ProjectLinkTracker tracker = new ProjectLinkTracker(new MockDocumentTracker(), solution, null);
            var sourceHierarchy = new MockVsHierarchy();
            var targetHierarchy = new MockVsHierarchy();

            tracker.AddProjectLink(sourceHierarchy, targetHierarchy);

            Assert.IsTrue(targetHierarchy.GetPropertyProjectValue.Globals.Dictionary.ContainsKey("ProjectLinkReference"));
            Assert.AreEqual(sourceHierarchy.GetPropertyProjectIdGuidValue.ToString(), targetHierarchy.GetPropertyProjectValue.Globals.Dictionary["ProjectLinkReference"]);
            Assert.IsTrue(targetHierarchy.GetPropertyProjectValue.Globals.set_VariablePersistsCalled);
        }

        [TestMethod]
        public void ShouldStopTrackingIfTargetProjectIsClosed()
        {
            var solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            var sourceHierarchy = new MockVsHierarchy();
            var targetHierarchy = new MockVsHierarchy();
            targetHierarchy.GetPropertyProjectValue.Globals.Dictionary["ProjectLinkReference"] = sourceHierarchy.GetPropertyProjectIdGuidValue.ToString();
            solution.Hierarchies.Add(sourceHierarchy);
            tracker.OnAfterOpenProject(sourceHierarchy, 0);
            tracker.OnAfterOpenProject(targetHierarchy, 0);
            Assert.IsTrue(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));

            tracker.OnBeforeCloseProject(targetHierarchy, 0);

            Assert.IsFalse(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));
        }

        [TestMethod]
        public void ShouldStopTrackingIfSourceProjectIsClosed()
        {
            var solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            var sourceHierarchy = new MockVsHierarchy();
            var targetHierarchy = new MockVsHierarchy();
            targetHierarchy.GetPropertyProjectValue.Globals.Dictionary["ProjectLinkReference"] = sourceHierarchy.GetPropertyProjectIdGuidValue.ToString();
            solution.Hierarchies.Add(sourceHierarchy);
            tracker.OnAfterOpenProject(sourceHierarchy, 0);
            tracker.OnAfterOpenProject(targetHierarchy, 0);
            Assert.IsTrue(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));

            tracker.OnBeforeCloseProject(sourceHierarchy, 0);

            Assert.IsFalse(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));
        }

        [TestMethod]
        public void ShouldRestartTrackingIfSourceProjectIsClosedAndReopened()
        {
            var solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            var sourceHierarchy = new MockVsHierarchy();
            var targetHierarchy = new MockVsHierarchy();
            targetHierarchy.GetPropertyProjectValue.Globals.Dictionary["ProjectLinkReference"] = sourceHierarchy.GetPropertyProjectIdGuidValue.ToString();
            solution.Hierarchies.Add(sourceHierarchy);
            tracker.OnAfterOpenProject(sourceHierarchy, 0);
            tracker.OnAfterOpenProject(targetHierarchy, 0);

            tracker.OnBeforeCloseProject(sourceHierarchy, 0);
            Assert.IsFalse(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));
            tracker.OnAfterOpenProject(sourceHierarchy, 0);

            Assert.IsTrue(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));
        }

        [TestMethod]
        public void ShouldNotRestartTrackingIfSourceAndTargetProjectsAreClosedAndSourceGetsReopened()
        {
            var solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            var sourceHierarchy = new MockVsHierarchy();
            var targetHierarchy = new MockVsHierarchy();
            targetHierarchy.GetPropertyProjectValue.Globals.Dictionary["ProjectLinkReference"] = sourceHierarchy.GetPropertyProjectIdGuidValue.ToString();
            solution.Hierarchies.Add(sourceHierarchy);
            tracker.OnAfterOpenProject(sourceHierarchy, 0);
            tracker.OnAfterOpenProject(targetHierarchy, 0);

            tracker.OnBeforeCloseProject(sourceHierarchy, 0);
            tracker.OnBeforeCloseProject(targetHierarchy, 0);
            tracker.OnAfterOpenProject(sourceHierarchy, 0);

            Assert.IsFalse(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));
        }

        [TestMethod]
        [ExpectedException(typeof(ProjectLinkerException))]
        public void ShouldNotLinkIfSourceIsAlreadyLinkedAsTarget()
        {
            var solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            var project1VsHierarchy = new MockVsHierarchy();
            var project2VsHierarchy = new MockVsHierarchy();
            var project3VsHierarchy = new MockVsHierarchy();
            solution.Hierarchies.Add(project1VsHierarchy);
            solution.Hierarchies.Add(project2VsHierarchy);
            solution.Hierarchies.Add(project3VsHierarchy);

            tracker.AddProjectLink(project1VsHierarchy, project2VsHierarchy);
            tracker.AddProjectLink(project2VsHierarchy, project3VsHierarchy);
        }

        [TestMethod]
        [ExpectedException(typeof(ProjectLinkerException))]
        public void ShouldNotLinkTwiceTheSameTarget()
        {
            var solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            var project1VsHierarchy = new MockVsHierarchy();
            var project2VsHierarchy = new MockVsHierarchy();
            var project3VsHierarchy = new MockVsHierarchy();
            solution.Hierarchies.Add(project1VsHierarchy);
            solution.Hierarchies.Add(project2VsHierarchy);
            solution.Hierarchies.Add(project3VsHierarchy);

            tracker.AddProjectLink(project1VsHierarchy, project3VsHierarchy);
            tracker.AddProjectLink(project2VsHierarchy, project3VsHierarchy);
        }

        [TestMethod]
        [ExpectedException(typeof(ProjectLinkerException))]
        public void ShouldNotLinkIfTargetIsAlreadyASource()
        {
            var solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            var project1VsHierarchy = new MockVsHierarchy();
            var project2VsHierarchy = new MockVsHierarchy();
            var project3VsHierarchy = new MockVsHierarchy();
            solution.Hierarchies.Add(project1VsHierarchy);
            solution.Hierarchies.Add(project2VsHierarchy);
            solution.Hierarchies.Add(project3VsHierarchy);

            tracker.AddProjectLink(project1VsHierarchy, project3VsHierarchy);
            tracker.AddProjectLink(project2VsHierarchy, project1VsHierarchy);
        }

        [TestMethod]
        public void ShouldGetEmptyLinkedProjectList()
        {
            var solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);

            var projectLinks = tracker.GetProjectLinks();

            Assert.IsNotNull(projectLinks);
            Assert.AreEqual(0, projectLinks.Count());
        }

        [TestMethod]
        public void ShouldGetLinkedProject()
        {
            var solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            var project1VsHierarchy = new MockVsHierarchy();
            var project2VsHierarchy = new MockVsHierarchy();
            solution.Hierarchies.Add(project1VsHierarchy);
            solution.Hierarchies.Add(project2VsHierarchy);

            tracker.AddProjectLink(project1VsHierarchy, project2VsHierarchy);

            var projectLinks = tracker.GetProjectLinks();

            Assert.AreEqual(1, projectLinks.Count());
            var projectLink = projectLinks.ElementAt(0);
            Assert.AreEqual(project1VsHierarchy.GetPropertyProjectIdGuidValue, projectLink.SourceProjectId);
            Assert.AreEqual(project2VsHierarchy.GetPropertyProjectIdGuidValue, projectLink.TargetProjectId);
        }

        [TestMethod]
        public void ShouldUnlinkProjects()
        {
            var solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            var sourceVsHierarchy = new MockVsHierarchy();
            var targetVsHierarchy = new MockVsHierarchy();
            solution.Hierarchies.Add(sourceVsHierarchy);
            solution.Hierarchies.Add(targetVsHierarchy);
            tracker.AddProjectLink(sourceVsHierarchy, targetVsHierarchy);
            targetVsHierarchy.GetPropertyProjectValue.Globals.set_VariablePersistsCalled = false;

            tracker.UnlinkProjects(sourceVsHierarchy.GetPropertyProjectIdGuidValue, targetVsHierarchy.GetPropertyProjectIdGuidValue);

            Assert.AreEqual(0, tracker.GetProjectLinks().Count());
            Assert.IsTrue(targetVsHierarchy.GetPropertyProjectValue.Globals.set_VariablePersistsCalled);
            Assert.IsFalse(targetVsHierarchy.GetPropertyProjectValue.Globals.set_VariablePersistsArgumentValue);
        }

        [TestMethod]
        public void ShouldRestoreLinksOnServiceInitialization()
        {
            var solution = new MockIVsSolution();
            var sourceHierarchy = new MockVsHierarchy();
            var targetHierarchy = new MockVsHierarchy();
            targetHierarchy.GetPropertyProjectValue.Globals.Dictionary["ProjectLinkReference"] = sourceHierarchy.GetPropertyProjectIdGuidValue.ToString();

            solution.Hierarchies.Add(sourceHierarchy);
            solution.Hierarchies.Add(targetHierarchy);

            var dteSolution = new MockSolution();
            dteSolution.Projects.List.Add(sourceHierarchy.GetPropertyProjectValue);
            dteSolution.Projects.List.Add(targetHierarchy.GetPropertyProjectValue);

            ProjectLinkTracker tracker = new ProjectLinkTracker(new MockDocumentTracker(), solution, new MockLogger(), dteSolution);

            var links = tracker.GetProjectLinks().ToList();
            Assert.AreEqual(1, links.Count);
            Assert.AreEqual(sourceHierarchy.GetPropertyProjectIdGuidValue, links[0].SourceProjectId);
            Assert.AreEqual(targetHierarchy.GetPropertyProjectIdGuidValue, links[0].TargetProjectId);
        }
    }

    internal class MockSolution : Solution4
    {
        public MockProjects Projects = new MockProjects();

        Projects Solution4.Projects
        {
            get { return this.Projects; }
        }

        #region _Solution Members

        Project _Solution.AddFromFile(string FileName, bool Exclusive)
        {
            throw new System.NotImplementedException();
        }

        Project _Solution.AddFromTemplate(string FileName, string Destination, string ProjectName, bool Exclusive)
        {
            throw new System.NotImplementedException();
        }

        AddIns _Solution.AddIns
        {
            get { throw new System.NotImplementedException(); }
        }

        void _Solution.Close(bool SaveFirst)
        {
            throw new System.NotImplementedException();
        }

        int _Solution.Count
        {
            get { throw new System.NotImplementedException(); }
        }

        void _Solution.Create(string Destination, string Name)
        {
            throw new System.NotImplementedException();
        }

        DTE _Solution.DTE
        {
            get { throw new System.NotImplementedException(); }
        }

        string _Solution.ExtenderCATID
        {
            get { throw new System.NotImplementedException(); }
        }

        object _Solution.ExtenderNames
        {
            get { throw new System.NotImplementedException(); }
        }

        string _Solution.FileName
        {
            get { throw new System.NotImplementedException(); }
        }

        ProjectItem _Solution.FindProjectItem(string FileName)
        {
            throw new System.NotImplementedException();
        }

        string _Solution.FullName
        {
            get { throw new System.NotImplementedException(); }
        }

        System.Collections.IEnumerator _Solution.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        Globals _Solution.Globals
        {
            get { throw new System.NotImplementedException(); }
        }

        bool _Solution.IsDirty
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        bool _Solution.IsOpen
        {
            get { throw new System.NotImplementedException(); }
        }

        Project _Solution.Item(object index)
        {
            throw new System.NotImplementedException();
        }

        void _Solution.Open(string FileName)
        {
            throw new System.NotImplementedException();
        }

        DTE _Solution.Parent
        {
            get { throw new System.NotImplementedException(); }
        }

        string _Solution.ProjectItemsTemplatePath(string ProjectKind)
        {
            throw new System.NotImplementedException();
        }

        Properties _Solution.Properties
        {
            get { throw new System.NotImplementedException(); }
        }

        void _Solution.Remove(Project proj)
        {
            throw new System.NotImplementedException();
        }

        void _Solution.SaveAs(string FileName)
        {
            throw new System.NotImplementedException();
        }

        bool _Solution.Saved
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        SolutionBuild _Solution.SolutionBuild
        {
            get { throw new System.NotImplementedException(); }
        }

        object _Solution.get_Extender(string ExtenderName)
        {
            throw new System.NotImplementedException();
        }

        string _Solution.get_TemplatePath(string ProjectType)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Solution4 Members

        Project Solution4.AddFromFile(string FileName, bool Exclusive = true)
        {
            throw new System.NotImplementedException();
        }

        Project Solution4.AddFromTemplate(string FileName, string Destination, string ProjectName, bool Exclusive = true)
        {
            throw new System.NotImplementedException();
        }

        public Project AddFromTemplateEx(string FileName, string Destination, string ProjectName, string SolutionName, bool Exclusive = true, uint Options = 0)
        {
            throw new System.NotImplementedException();
        }

        AddIns Solution4.AddIns
        {
            get { throw new System.NotImplementedException(); }
        }

        public Project AddSolutionFolder(string Name)
        {
            throw new System.NotImplementedException();
        }

        void Solution4.Close(bool SaveFirst = true)
        {
            throw new System.NotImplementedException();
        }

        int Solution4.Count
        {
            get { throw new System.NotImplementedException(); }
        }

        void Solution4.Create(string Destination, string Name)
        {
            throw new System.NotImplementedException();
        }

        DTE Solution4.DTE
        {
            get { throw new System.NotImplementedException(); }
        }

        string Solution4.ExtenderCATID
        {
            get { throw new System.NotImplementedException(); }
        }

        object Solution4.ExtenderNames
        {
            get { throw new System.NotImplementedException(); }
        }

        string Solution4.FileName
        {
            get { throw new System.NotImplementedException(); }
        }

        ProjectItem Solution4.FindProjectItem(string FileName)
        {
            throw new System.NotImplementedException();
        }

        string Solution4.FullName
        {
            get { throw new System.NotImplementedException(); }
        }

        System.Collections.IEnumerator Solution4.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public string GetProjectItemTemplate(string TemplateName, string Language)
        {
            throw new System.NotImplementedException();
        }

        public EnvDTE90.Templates GetProjectItemTemplates(string Language, string CustomDataSignature)
        {
            throw new System.NotImplementedException();
        }

        public string GetProjectTemplate(string TemplateName, string Language)
        {
            throw new System.NotImplementedException();
        }

        Globals Solution4.Globals
        {
            get { throw new System.NotImplementedException(); }
        }

        bool Solution4.IsDirty
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        bool Solution4.IsOpen
        {
            get { throw new System.NotImplementedException(); }
        }

        Project Solution4.Item(object index)
        {
            throw new System.NotImplementedException();
        }

        void Solution4.Open(string FileName)
        {
            throw new System.NotImplementedException();
        }

        DTE Solution4.Parent
        {
            get { throw new System.NotImplementedException(); }
        }

        string Solution4.ProjectItemsTemplatePath(string ProjectKind)
        {
            throw new System.NotImplementedException();
        }


        Properties Solution4.Properties
        {
            get { throw new System.NotImplementedException(); }
        }

        void Solution4.Remove(Project proj)
        {
            throw new System.NotImplementedException();
        }

        void Solution4.SaveAs(string FileName)
        {
            throw new System.NotImplementedException();
        }

        bool Solution4.Saved
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        SolutionBuild Solution4.SolutionBuild
        {
            get { throw new System.NotImplementedException(); }
        }

        object Solution4.get_Extender(string ExtenderName)
        {
            throw new System.NotImplementedException();
        }

        string Solution4.get_TemplatePath(string ProjectType)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Solution3 Members

        Project EnvDTE90.Solution3.AddFromFile(string FileName, bool Exclusive = false)
        {
            throw new System.NotImplementedException();
        }

        Project EnvDTE90.Solution3.AddFromTemplate(string FileName, string Destination, string ProjectName, bool Exclusive = false)
        {
            throw new System.NotImplementedException();
        }

        AddIns EnvDTE90.Solution3.AddIns
        {
            get { throw new System.NotImplementedException(); }
        }

        void EnvDTE90.Solution3.Close(bool SaveFirst = false)
        {
            throw new System.NotImplementedException();
        }

        int EnvDTE90.Solution3.Count
        {
            get { throw new System.NotImplementedException(); }
        }

        void EnvDTE90.Solution3.Create(string Destination, string Name)
        {
            throw new System.NotImplementedException();
        }

        DTE EnvDTE90.Solution3.DTE
        {
            get { throw new System.NotImplementedException(); }
        }

        string EnvDTE90.Solution3.ExtenderCATID
        {
            get { throw new System.NotImplementedException(); }
        }

        object EnvDTE90.Solution3.ExtenderNames
        {
            get { throw new System.NotImplementedException(); }
        }

        string EnvDTE90.Solution3.FileName
        {
            get { throw new System.NotImplementedException(); }
        }

        ProjectItem EnvDTE90.Solution3.FindProjectItem(string FileName)
        {
            throw new System.NotImplementedException();
        }

        string EnvDTE90.Solution3.FullName
        {
            get { throw new System.NotImplementedException(); }
        }

        System.Collections.IEnumerator EnvDTE90.Solution3.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        Globals EnvDTE90.Solution3.Globals
        {
            get { throw new System.NotImplementedException(); }
        }

        bool EnvDTE90.Solution3.IsDirty
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        bool EnvDTE90.Solution3.IsOpen
        {
            get { throw new System.NotImplementedException(); }
        }

        Project EnvDTE90.Solution3.Item(object index)
        {
            throw new System.NotImplementedException();
        }

        void EnvDTE90.Solution3.Open(string FileName)
        {
            throw new System.NotImplementedException();
        }

        DTE EnvDTE90.Solution3.Parent
        {
            get { throw new System.NotImplementedException(); }
        }

        string EnvDTE90.Solution3.ProjectItemsTemplatePath(string ProjectKind)
        {
            throw new System.NotImplementedException();
        }

        Projects EnvDTE90.Solution3.Projects
        {
            get { throw new System.NotImplementedException(); }
        }

        Properties EnvDTE90.Solution3.Properties
        {
            get { throw new System.NotImplementedException(); }
        }

        void EnvDTE90.Solution3.Remove(Project proj)
        {
            throw new System.NotImplementedException();
        }

        void EnvDTE90.Solution3.SaveAs(string FileName)
        {
            throw new System.NotImplementedException();
        }

        bool EnvDTE90.Solution3.Saved
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        SolutionBuild EnvDTE90.Solution3.SolutionBuild
        {
            get { throw new System.NotImplementedException(); }
        }

        object EnvDTE90.Solution3.get_Extender(string ExtenderName)
        {
            throw new System.NotImplementedException();
        }

        string EnvDTE90.Solution3.get_TemplatePath(string ProjectType)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Solution2 Members

        Project EnvDTE80.Solution2.AddFromFile(string FileName, bool Exclusive = false)
        {
            throw new System.NotImplementedException();
        }

        Project EnvDTE80.Solution2.AddFromTemplate(string FileName, string Destination, string ProjectName, bool Exclusive = false)
        {
            throw new System.NotImplementedException();
        }

        AddIns EnvDTE80.Solution2.AddIns
        {
            get { throw new System.NotImplementedException(); }
        }

        void EnvDTE80.Solution2.Close(bool SaveFirst = false)
        {
            throw new System.NotImplementedException();
        }

        int EnvDTE80.Solution2.Count
        {
            get { throw new System.NotImplementedException(); }
        }

        void EnvDTE80.Solution2.Create(string Destination, string Name)
        {
            throw new System.NotImplementedException();
        }

        DTE EnvDTE80.Solution2.DTE
        {
            get { throw new System.NotImplementedException(); }
        }

        string EnvDTE80.Solution2.ExtenderCATID
        {
            get { throw new System.NotImplementedException(); }
        }

        object EnvDTE80.Solution2.ExtenderNames
        {
            get { throw new System.NotImplementedException(); }
        }

        string EnvDTE80.Solution2.FileName
        {
            get { throw new System.NotImplementedException(); }
        }

        ProjectItem EnvDTE80.Solution2.FindProjectItem(string FileName)
        {
            throw new System.NotImplementedException();
        }

        string EnvDTE80.Solution2.FullName
        {
            get { throw new System.NotImplementedException(); }
        }

        System.Collections.IEnumerator EnvDTE80.Solution2.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        Globals EnvDTE80.Solution2.Globals
        {
            get { throw new System.NotImplementedException(); }
        }

        bool EnvDTE80.Solution2.IsDirty
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        bool EnvDTE80.Solution2.IsOpen
        {
            get { throw new System.NotImplementedException(); }
        }

        Project EnvDTE80.Solution2.Item(object index)
        {
            throw new System.NotImplementedException();
        }

        void EnvDTE80.Solution2.Open(string FileName)
        {
            throw new System.NotImplementedException();
        }

        DTE EnvDTE80.Solution2.Parent
        {
            get { throw new System.NotImplementedException(); }
        }

        string EnvDTE80.Solution2.ProjectItemsTemplatePath(string ProjectKind)
        {
            throw new System.NotImplementedException();
        }

        Projects EnvDTE80.Solution2.Projects
        {
            get { throw new System.NotImplementedException(); }
        }

        Properties EnvDTE80.Solution2.Properties
        {
            get { throw new System.NotImplementedException(); }
        }

        void EnvDTE80.Solution2.Remove(Project proj)
        {
            throw new System.NotImplementedException();
        }

        void EnvDTE80.Solution2.SaveAs(string FileName)
        {
            throw new System.NotImplementedException();
        }

        bool EnvDTE80.Solution2.Saved
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        SolutionBuild EnvDTE80.Solution2.SolutionBuild
        {
            get { throw new System.NotImplementedException(); }
        }

        object EnvDTE80.Solution2.get_Extender(string ExtenderName)
        {
            throw new System.NotImplementedException();
        }

        string EnvDTE80.Solution2.get_TemplatePath(string ProjectType)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region _Solution Members


        Projects _Solution.Projects
        {
            get { throw new System.NotImplementedException(); }
        }

        #endregion
    }

    internal class MockProjects : Projects
    {
        public List<Project> List = new List<Project>();

        #region Projects Members

        public int Count
        {
            get { throw new System.NotImplementedException(); }
        }

        public DTE DTE
        {
            get { throw new System.NotImplementedException(); }
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return List.GetEnumerator();
        }

        public Project Item(object index)
        {
            throw new System.NotImplementedException();
        }

        public string Kind
        {
            get { throw new System.NotImplementedException(); }
        }

        public DTE Parent
        {
            get { throw new System.NotImplementedException(); }
        }

        public Properties Properties
        {
            get { throw new System.NotImplementedException(); }
        }

        #endregion
    }

    internal class TestableProjectLinkTracker : ProjectLinkTracker
    {
        public TestableProjectLinkTracker(IVsTrackProjectDocuments2 documentTracker, IVsSolution solution)
            : base(documentTracker, solution, new MockLogger())
        {
        }

        public bool ProjectsAreLinked(IVsHierarchy sourceProject, IVsHierarchy targetProject)
        {
            return base.LinkExists(sourceProject, targetProject);
        }

        public void AddProjectSyncer(IProjectItemsSynchronizer syncher)
        {
            Synchronizers.Add(syncher);
        }
    }
}
