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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Practices.ProjectLinker;
using Microsoft.Practices.ProjectLinker.Utility;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectLinker.Tests.Mocks;
using Constants = EnvDTE.Constants;

namespace ProjectLinker.Tests
{
    [TestClass]
    public class ProjectItemsSyncherFixture
    {
        [TestMethod]
        public void ShouldAddItemToTargetWhenAddingToSourceProject()
        {
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject();
            var mockLogger = new MockLogger();
            sourceProject.ProjectItems.AddProjectItem(new MockProjectItem("ABC.txt"));

            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());
            string fileToAdd = Path.Combine(@"c:\mockPath1", @"ABC.txt");

            syncher.FileAddedToSource(fileToAdd);

            Assert.IsTrue(targetProject.ProjectItems.AddFromFileCalled);
            Assert.AreEqual(1, targetProject.ProjectItems.Count);
            StringAssert.EndsWith(targetProject.ProjectItems.Item(0).Name, "ABC.txt");
            Assert.AreEqual(1, mockLogger.MessageLog.Count);
            StringAssert.Contains(mockLogger.MessageLog[0], "added");
            StringAssert.Contains(mockLogger.MessageLog[0], "ABC.txt");
            StringAssert.Contains(mockLogger.MessageLog[0], targetProject.Name);
        }

        [TestMethod]
        public void ShouldAddMultipleItemsToTarget()
        {
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject();
            sourceProject.ProjectItems.AddProjectItem(new MockProjectItem("ABC.txt"));
            sourceProject.ProjectItems.AddProjectItem(new MockProjectItem("123.txt"));

            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, new MockLogger(), null, new MockProjectItemsFilter());
            string fileToAdd1 = Path.Combine(@"c:\mockPath1", @"ABC.txt");
            string fileToAdd2 = Path.Combine(@"c:\mockPath1", @"123.txt");

            syncher.FileAddedToSource(fileToAdd1);
            syncher.FileAddedToSource(fileToAdd2);

            Assert.IsTrue(targetProject.ProjectItems.AddFromFileCalled);
            Assert.AreEqual(2, targetProject.ProjectItems.Count);
            StringAssert.EndsWith(targetProject.ProjectItems.Item(0).Name, "ABC.txt");
            StringAssert.EndsWith(targetProject.ProjectItems.Item(1).Name, "123.txt");
        }

        [TestMethod]
        public void ShouldAddFileInCorrectSubfolder()
        {
            var mockLogger = new MockLogger();
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject();
            var sourceFolder = new MockProjectItem("MyFolder") { Kind = Constants.vsProjectItemKindPhysicalFolder };
            sourceProject.ProjectItems.AddProjectItem(sourceFolder);
            sourceFolder.ProjectItems.AddProjectItem(new MockProjectItem("MyFile.txt"));

            var targetFolder = new MockProjectItem("MyFolder") { Kind = Constants.vsProjectItemKindPhysicalFolder };
            targetProject.ProjectItems.AddProjectItem(targetFolder);

            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());
            string fileToAdd = @"c:\mockPath1\MyFolder\MyFile.txt";

            syncher.FileAddedToSource(fileToAdd);

            Assert.IsTrue(targetFolder.ProjectItems.AddFromFileCalled);
            Assert.AreEqual(1, targetFolder.ProjectItems.Count);
            StringAssert.EndsWith(targetFolder.ProjectItems.Item(0).Name, "MyFile.txt");
            Assert.AreEqual(1, mockLogger.MessageLog.Count);
            StringAssert.Contains(mockLogger.MessageLog[0], "added");
            StringAssert.Contains(mockLogger.MessageLog[0], @"MyFolder\MyFile.txt");
        }

        [TestMethod]
        public void ShouldRemoveLinkedFileWhenDeletingFromSource()
        {
            string sourceFile = @"c:\mockPath1\MyClass.cs";
            var mockLogger = new MockLogger();
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject();
            var targetFile = new MockProjectItem(sourceFile, true);
            targetProject.ProjectItems.AddProjectItem(targetFile);

            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());
            Assert.AreEqual(1, targetProject.ProjectItems.Count);

            syncher.FileRemovedFromSource(sourceFile);

            Assert.IsTrue(targetFile.DeleteCalled);
            Assert.AreEqual(1, mockLogger.MessageLog.Count);
            StringAssert.Contains(mockLogger.MessageLog[0], "removed");
            StringAssert.Contains(mockLogger.MessageLog[0], @"MyClass.cs");
            StringAssert.Contains(mockLogger.MessageLog[0], targetProject.Name);
        }

        [TestMethod]
        public void ShouldNotRemoveFileIfTargetFileIsNotALink()
        {
            var mockLogger = new MockLogger();
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject();
            var targetFile = new MockProjectItem("MyClass.cs") { Kind = Constants.vsProjectItemKindPhysicalFile };
            targetProject.ProjectItems.AddProjectItem(targetFile);

            string sourceFile = Path.Combine(@"c:\mockPath1", @"MyClass.cs");
            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());
            Assert.AreEqual(1, targetProject.ProjectItems.Count);

            syncher.FileRemovedFromSource(sourceFile);

            Assert.IsFalse(targetFile.DeleteCalled);
            Assert.AreEqual(1, mockLogger.MessageLog.Count);
            StringAssert.Contains(mockLogger.MessageLog[0], "not linked");
            StringAssert.Contains(mockLogger.MessageLog[0], @"MyClass.cs");
            StringAssert.Contains(mockLogger.MessageLog[0], targetProject.Name);
        }

        [TestMethod]
        public void ShouldRemoveLinkedFileInSubFolderWhenDeletingFromSource()
        {
            string sourceFile = @"c:\mockPath1\SubFolder\MyClass.cs";
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject();
            var subFolder = new MockProjectItem("SubFolder") { Kind = Constants.vsProjectItemKindPhysicalFolder };
            var targetFile = new MockProjectItem(sourceFile, true);
            subFolder.ProjectItems.AddProjectItem(targetFile);
            targetProject.ProjectItems.AddProjectItem(subFolder);

            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, new MockLogger(), null, new MockProjectItemsFilter());

            syncher.FileRemovedFromSource(sourceFile);

            Assert.IsTrue(targetFile.DeleteCalled);
        }

        [TestMethod]
        public void ShouldRenameLinkedFile()
        {
            string oldSourceFile = Path.Combine(@"c:\mockPath1", @"MyOldFilename.cs");
            string newSourceFile = Path.Combine(@"c:\mockPath1", @"MyNewFilename.cs");

            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject();
            sourceProject.ProjectItems.AddProjectItem(new MockProjectItem("MyNewFilename.cs"));
            var targetFile = new MockProjectItem(oldSourceFile, true);
            targetProject.ProjectItems.AddProjectItem(targetFile);

            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, new MockLogger(), null, new MockProjectItemsFilter());
            Assert.AreEqual(1, targetProject.ProjectItems.Count);

            syncher.FileRenamedInSource(oldSourceFile, newSourceFile);

            Assert.IsTrue(targetFile.DeleteCalled);
            Assert.IsTrue(targetProject.ProjectItems.AddFromFileCalled);
            Assert.IsNotNull(targetProject.ProjectItems.FirstOrDefault(x => x.Name.EndsWith("MyNewFilename.cs")));
        }

        [TestMethod]
        public void ShouldCreateFolderStructureWhenAddingLinkedFile()
        {
            var mockLogger = new MockLogger();
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject();
            var sourceFolder = new MockProjectItem("MyFolder") { Kind = Constants.vsProjectItemKindPhysicalFolder };
            sourceProject.ProjectItems.AddProjectItem(sourceFolder);
            sourceFolder.ProjectItems.AddProjectItem(new MockProjectItem("MyFile.txt"));

            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());
            string fileToAdd = @"c:\mockPath1\MyFolder\MyFile.txt";

            syncher.FileAddedToSource(fileToAdd);

            Assert.IsNotNull(targetProject.ProjectItems.Item(0));
            Assert.AreEqual(targetProject.ProjectItems.Item(0).Name, "MyFolder");
            Assert.IsNotNull(targetProject.ProjectItems.Item(0).ProjectItems);
            Assert.AreEqual(1, targetProject.ProjectItems.Item(0).ProjectItems.Count);
            StringAssert.EndsWith(targetProject.ProjectItems.Item(0).ProjectItems.Item(0).Name, "MyFile.txt");
            Assert.IsTrue(mockLogger.MessageLog.Count > 1);
            string loggedMessage = mockLogger.MessageLog.FirstOrDefault(x => x.IndexOf("folder", StringComparison.OrdinalIgnoreCase) >= 0);
            Assert.IsNotNull(loggedMessage);
            StringAssert.Contains(loggedMessage, "created");
            Assert.AreEqual(-1, loggedMessage.IndexOf("MyFile.txt"));

        }

        [TestMethod]
        public void ShouldRemoveLinkedFolderWhenDeletingFolderFromSource()
        {
            var mockLogger = new MockLogger();
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject();
            var folder = new MockProjectItem("MyFolder") { Kind = Constants.vsProjectItemKindPhysicalFolder };
            targetProject.ProjectItems.AddProjectItem(folder);

            string sourceFolder = Path.Combine(@"c:\mockPath1", @"MyFolder");
            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());
            Assert.AreEqual(1, targetProject.ProjectItems.Count);

            syncher.DirectoryRemovedFromSource(sourceFolder);

            Assert.IsTrue(folder.DeleteCalled);
            Assert.AreEqual(1, mockLogger.MessageLog.Count);
            StringAssert.Contains(mockLogger.MessageLog[0], "removed");
            StringAssert.Contains(mockLogger.MessageLog[0], @"MyFolder");
            StringAssert.Contains(mockLogger.MessageLog[0], targetProject.Name);
        }

        [TestMethod]
        public void ShouldAddDirectoryToTargetWhenAddingToSourceProject()
        {
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject();

            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, new MockLogger(), null, new MockProjectItemsFilter());
            string directoryToAdd = @"c:\mockPath1\MyFolder\";

            syncher.DirectoryAddedToSource(directoryToAdd);

            Assert.IsTrue(targetProject.ProjectItems.AddFolderCalled);
            Assert.AreEqual(1, targetProject.ProjectItems.Count);
            StringAssert.EndsWith(targetProject.ProjectItems.Item(0).Name, "MyFolder");
        }

        [TestMethod]
        public void ShouldAddSubDirectoryToTargetWhenAddingToSourceProject()
        {
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject();

            targetProject.ProjectItems.AddFolder("Folder1", Constants.vsProjectItemKindPhysicalFolder);
            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, new MockLogger(), null, new MockProjectItemsFilter());
            string directoryToAdd = @"c:\mockPath1\Folder1\Folder2";

            syncher.DirectoryAddedToSource(directoryToAdd);

            Assert.IsTrue(targetProject.ProjectItems.AddFolderCalled);
            Assert.AreEqual(1, targetProject.ProjectItems.Item(0).ProjectItems.Count);
            StringAssert.EndsWith(targetProject.ProjectItems.Item(0).ProjectItems.Item(0).Name, "Folder2");
        }

        [TestMethod]
        public void ShouldNotRemoveLinkedFolderWithNestedFilesWhenDeletingFolderFromSource()
        {
            var mockLogger = new MockLogger();
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject();

            var folder = new MockProjectItem("Folder") { Kind = Constants.vsProjectItemKindPhysicalFolder };
            var subFolder = new MockProjectItem("SubFolder") { Kind = Constants.vsProjectItemKindPhysicalFolder };
            subFolder.ProjectItems.AddProjectItem(new MockProjectItem("File.txt") { Kind = Constants.vsProjectItemKindPhysicalFile });
            folder.ProjectItems.AddProjectItem(subFolder);

            targetProject.ProjectItems.AddProjectItem(folder);

            string sourceFolder = Path.Combine(@"c:\mockPath1", @"Folder");
            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());
            Assert.AreEqual(1, targetProject.ProjectItems.Count);

            syncher.DirectoryRemovedFromSource(sourceFolder);

            Assert.IsFalse(folder.DeleteCalled);
            Assert.AreEqual(1, mockLogger.MessageLog.Count);
            StringAssert.Contains(mockLogger.MessageLog[0], "not removed");
            StringAssert.Contains(mockLogger.MessageLog[0], @"Folder");
            StringAssert.Contains(mockLogger.MessageLog[0], targetProject.Name);
        }

        [TestMethod]
        public void RemoveDirectoryWithEmptySubFolderShouldDeleteWholeFolderStructure()
        {
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject();
            var folder = new MockProjectItem("MyFolder") { Kind = Constants.vsProjectItemKindPhysicalFolder };
            var subFolder = new MockProjectItem("EmptySubFolder") { Kind = Constants.vsProjectItemKindPhysicalFolder };
            folder.ProjectItems.AddProjectItem(subFolder);
            targetProject.ProjectItems.AddProjectItem(folder);

            string sourceFolder = Path.Combine(@"c:\mockPath1", @"MyFolder");
            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, new MockLogger(), null, new MockProjectItemsFilter());
            Assert.AreEqual(1, targetProject.ProjectItems.Count);

            syncher.DirectoryRemovedFromSource(sourceFolder);

            Assert.IsTrue(folder.DeleteCalled);
        }

        [TestMethod]
        public void ShouldIncludeDirectoryInTargetWhenItsPresentInFileSystem()
        {
            var mockSolution = new MockIVsSolution();
            var mockTargetVsHierarchy = new MockVsHierarchy();
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject(@"c:\mockPath2\project1.csproj");
            mockTargetVsHierarchy.GetPropertyProjectValue = targetProject;
            mockSolution.Hierarchies.Add(mockTargetVsHierarchy);
            var mockHierarchyHelper = new MockHierarchyHelper();
            var mockLogger = new MockLogger();
            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, mockSolution, mockHierarchyHelper, new MockProjectItemsFilter());
            string directoryToAdd = @"c:\mockPath1\MyFolder\";
            targetProject.ProjectItems.ThrowOnAddFolder = true;

            syncher.DirectoryAddedToSource(directoryToAdd);

            Assert.IsTrue(mockTargetVsHierarchy.AddItemCalled);
            Assert.AreEqual(VSConstants.VSITEMID_ROOT, mockTargetVsHierarchy.AddItemArgumentItemidLoc);
            Assert.AreEqual(string.Empty, mockTargetVsHierarchy.AddItemArgumentItemName);
            Assert.AreEqual<uint>(1, mockTargetVsHierarchy.AddItemArgumentFilesToOpen);
            Assert.AreEqual(@"c:\mockPath2\MyFolder", mockTargetVsHierarchy.AddItemArgumentArrayFilesToOpen[0]);
        }

        [TestMethod]
        public void ShouldIncludeSubDirectoryInTargetWhenItsPresentInFileSystem()
        {
            var mockSolution = new MockIVsSolution();
            var mockTargetVsHierarchy = new MockVsHierarchy();
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject(@"c:\mockPath2\project1.csproj");
            mockTargetVsHierarchy.GetPropertyProjectValue = targetProject;
            mockSolution.Hierarchies.Add(mockTargetVsHierarchy);

            targetProject.ProjectItems.AddFolder("Folder1", Constants.vsProjectItemKindPhysicalFolder);
            MockProjectItem folder1 = targetProject.ProjectItems.Item(0) as MockProjectItem;
            folder1.ProjectItems.ThrowOnAddFolder = true;

            var mockHierarchyHelper = new MockHierarchyHelper();
            var mockLogger = new MockLogger();
            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, mockSolution, mockHierarchyHelper, new MockProjectItemsFilter());
            string directoryToAdd = @"c:\mockPath1\Folder1\Folder2\";

            syncher.DirectoryAddedToSource(directoryToAdd);

            Assert.IsTrue(mockTargetVsHierarchy.AddItemCalled);
            Assert.AreEqual(VSConstants.VSITEMID_ROOT, mockTargetVsHierarchy.AddItemArgumentItemidLoc);
            Assert.AreEqual(string.Empty, mockTargetVsHierarchy.AddItemArgumentItemName);
            Assert.AreEqual<uint>(1, mockTargetVsHierarchy.AddItemArgumentFilesToOpen);
            Assert.AreEqual(@"c:\mockPath2\Folder1\Folder2", mockTargetVsHierarchy.AddItemArgumentArrayFilesToOpen[0]);
            Assert.AreEqual(1, mockLogger.MessageLog.Count);
            StringAssert.Contains(mockLogger.MessageLog[0], "already exists");
            StringAssert.Contains(mockLogger.MessageLog[0], "included");
            StringAssert.Contains(mockLogger.MessageLog[0], @"Folder2");
            StringAssert.Contains(mockLogger.MessageLog[0], targetProject.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(COMException))]
        public void ShouldThrowIfCOMExceptionErrorCodeIsNotTheExpected()
        {
            var mockSolution = new MockIVsSolution();
            var mockTargetVsHierarchy = new MockVsHierarchy();
            mockSolution.Hierarchies.Add(mockTargetVsHierarchy);
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject(@"c:\mockPath2\project1.csproj");
            targetProject.ProjectItems.ThrowOnAddFolder = true;
            targetProject.ProjectItems.ErrorCode = VSConstants.S_FALSE;

            var mockHierarchyHelper = new MockHierarchyHelper();
            var mockLogger = new MockLogger();
            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, mockSolution, mockHierarchyHelper, new MockProjectItemsFilter());
            string directoryToAdd = @"c:\mockPath1\MyFolder\";

            syncher.DirectoryAddedToSource(directoryToAdd);
        }

        [TestMethod]
        public void ShouldCallRemoveAndAddDirectoryWhenDirectoryIsRenamedInSource()
        {
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject();
            var folder = new MockProjectItem("oldFolder") { Kind = Constants.vsProjectItemKindPhysicalFolder };
            targetProject.ProjectItems.AddProjectItem(folder);

            string oldFolderName = Path.Combine(@"c:\mockPath1", @"oldFolder");
            string newFolderName = @"c:\mockPath1\newFolder\";

            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, new MockLogger(), null, new MockProjectItemsFilter());

            syncher.DirectoryRenamedInSource(oldFolderName, newFolderName);

            Assert.IsTrue(folder.DeleteCalled);
            Assert.IsTrue(targetProject.ProjectItems.AddFolderCalled);
        }

        [TestMethod]
        public void ShouldFilterAddedFileOnSource()
        {
            string fileToAdd = Path.Combine(@"c:\mockPath1", @"ABC.xaml");
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            sourceProject.ProjectItems.AddProjectItem(new MockProjectItem("ABC.xaml"));
            var targetProject = new MockProject();
            var mockProjectItemsFilter = new MockProjectItemsFilter();
            mockProjectItemsFilter.IsSynchronizableReturnValue = false;
            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, new MockLogger(), null, null, mockProjectItemsFilter);
            Assert.IsFalse(mockProjectItemsFilter.IsSynchronizableCalled);

            syncher.FileAddedToSource(fileToAdd);

            Assert.IsFalse(targetProject.ProjectItems.AddFromFileCalled);
            Assert.IsTrue(mockProjectItemsFilter.IsSynchronizableCalled);
        }

        [TestMethod]
        public void ShouldFilterAddedDirectoryOnSource()
        {
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject();

            var mockProjectItemsFilter = new MockProjectItemsFilter();
            mockProjectItemsFilter.IsSynchronizableReturnValue = false;
            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, new MockLogger(), null, null, mockProjectItemsFilter);
            string directoryToAdd = Path.Combine(@"c:\mockPath1", "MyFolder");

            syncher.DirectoryAddedToSource(directoryToAdd);

            Assert.IsFalse(targetProject.ProjectItems.AddFolderCalled);
            Assert.IsTrue(mockProjectItemsFilter.IsSynchronizableCalled);
        }

        [TestMethod]
        public void ShouldLogMessageIfFileDoesNotExistInTargetProject()
        {
            var mockLogger = new MockLogger();
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject();

            string sourceFile = Path.Combine(@"c:\mockPath1", @"MyClass.cs");
            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());

            syncher.FileRemovedFromSource(sourceFile);

            Assert.AreEqual(1, mockLogger.MessageLog.Count);
            StringAssert.Contains(mockLogger.MessageLog[0], "not linked");
            StringAssert.Contains(mockLogger.MessageLog[0], @"MyClass.cs");
            StringAssert.Contains(mockLogger.MessageLog[0], targetProject.Name);
        }

        [TestMethod]
        public void ShouldAddItemToTargetWhenAddingLinkedFileToSourceProject()
        {
            string fileToAdd = @"c:\alternativeExternalPath\file.txt";
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject();
            var mockLogger = new MockLogger();
            var linkedSourceFile = new MockProjectItem("file.txt");
            linkedSourceFile.MockProperties.PropertiesList.Add(new MockProperty("FullPath", fileToAdd));
            sourceProject.ProjectItems.AddProjectItem(linkedSourceFile);

            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());

            syncher.FileAddedToSource(fileToAdd);

            Assert.IsTrue(targetProject.ProjectItems.AddFromFileCalled);
            Assert.AreEqual(1, targetProject.ProjectItems.Count);
            StringAssert.EndsWith(targetProject.ProjectItems.Item(0).Name, "file.txt");
        }

        [TestMethod]
        public void ShouldAddFileThatIsLinkInCorrectSubfolder()
        {
            string fileToAdd = @"c:\alternativeExternalPath\file.txt";
            var mockLogger = new MockLogger();
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject();
            var sourceFolder = new MockProjectItem("MyFolder") { Kind = Constants.vsProjectItemKindPhysicalFolder };
            sourceProject.ProjectItems.AddProjectItem(sourceFolder);
            var linkedSourceFile = new MockProjectItem("file.txt");
            linkedSourceFile.MockProperties.PropertiesList.Add(new MockProperty("FullPath", fileToAdd));
            sourceFolder.ProjectItems.AddProjectItem(linkedSourceFile);

            var targetFolder = new MockProjectItem("MyFolder") { Kind = Constants.vsProjectItemKindPhysicalFolder };
            targetProject.ProjectItems.AddProjectItem(targetFolder);

            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());

            syncher.FileAddedToSource(fileToAdd);

            Assert.IsTrue(targetFolder.ProjectItems.AddFromFileCalled);
            Assert.AreEqual(1, targetFolder.ProjectItems.Count);
            StringAssert.EndsWith(targetFolder.ProjectItems.Item(0).Name, "file.txt");
        }

        [TestMethod]
        public void ShouldRemoveLinkedFileEvenWhenSourceIsALink()
        {
            string sourceFile = @"c:\alternativeExternalPath\file.txt";
            var mockLogger = new MockLogger();
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject();
            var targetFile = new MockProjectItem(sourceFile, true);
            targetProject.ProjectItems.AddProjectItem(targetFile);

            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());
            Assert.AreEqual(1, targetProject.ProjectItems.Count);

            syncher.FileRemovedFromSource(sourceFile);

            Assert.IsTrue(targetFile.DeleteCalled);
            Assert.AreEqual(1, mockLogger.MessageLog.Count);
            StringAssert.Contains(mockLogger.MessageLog[0], "removed");
            StringAssert.Contains(mockLogger.MessageLog[0], @"file.txt");
            StringAssert.Contains(mockLogger.MessageLog[0], targetProject.Name);
        }

        [TestMethod]
        public void ShouldUseRelativePathWhenEvaluatingFilter()
        {
            var sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            var targetProject = new MockProject();
            var mockLogger = new MockLogger();
            var mockFilter = new MockProjectItemsFilter();
            var sourceFolder = new MockProjectItem("MyFolder") { Kind = Constants.vsProjectItemKindPhysicalFolder };
            sourceProject.ProjectItems.AddProjectItem(sourceFolder);
            sourceFolder.ProjectItems.AddProjectItem(new MockProjectItem("ABC.txt"));

            var syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, mockFilter);
            string fileToAdd = @"c:\mockPath1\MyFolder\ABC.txt";

            syncher.FileAddedToSource(fileToAdd);

            Assert.IsTrue(mockFilter.IsSynchronizableCalled);
            Assert.AreEqual(@"MyFolder\ABC.txt", mockFilter.IsSynchronizableArgument);
        }

        /*
         * Delete a folder that contains files excluded from the project (analyze best option)
         */
    }

    internal class MockProjectItemsFilter : IProjectItemsFilter
    {
        public bool IsSynchronizableReturnValue = true;
        public bool IsSynchronizableCalled = false;
        public string IsSynchronizableArgument;

        public bool CanBeSynchronized(string relativePath)
        {
            IsSynchronizableCalled = true;
            IsSynchronizableArgument = relativePath;
            return IsSynchronizableReturnValue;
        }
    }


    internal class MockHierarchyHelper : IHierarchyHelper
    {
        public uint GetItemId(IVsHierarchy projectHierarchy, string folderRelativePath)
        {
            return VSConstants.VSITEMID_ROOT;
        }
    }
}
