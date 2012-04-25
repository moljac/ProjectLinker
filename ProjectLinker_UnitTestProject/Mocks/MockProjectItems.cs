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
using System.Runtime.InteropServices;
using EnvDTE;

namespace ProjectLinker.Tests.Mocks
{
    class MockProjectItems : ProjectItems, IEnumerable<ProjectItem>
    {
        private List<ProjectItem> projectItems = new List<ProjectItem>();
        public bool AddFromFileCalled;
        public bool AddFolderCalled;
        public bool ThrowOnAddFolder;
        public int ErrorCode = -2147467259;

        public MockProjectItems(object parent)
        {
            Parent = parent;
        }

        public void AddProjectItem(MockProjectItem projectItem)
        {
            if (projectItem.Collection != null)
                throw new InvalidOperationException("Invalid use of the mock object");
            projectItem.Collection = this;
            projectItems.Add(projectItem);
        }

        public ProjectItem AddFolder(string Name, string Kind)
        {
            AddFolderCalled = true;
            if (ThrowOnAddFolder)
                throw new COMException("Folder Exists in File System (from MockProjectItems)", ErrorCode);

            var projectItem = new MockProjectItem(Name) { Kind = Kind };
            AddProjectItem(projectItem);
            return projectItem;
        }

        public ProjectItem AddFromDirectory(string Directory)
        {
            throw new System.NotImplementedException();
        }

        public ProjectItem AddFromFile(string FileName)
        {
            AddFromFileCalled = true;
            AddProjectItem(new MockProjectItem(FileName)); ;
            return null;
        }

        public ProjectItem AddFromFileCopy(string FilePath)
        {
            throw new System.NotImplementedException();
        }

        public ProjectItem AddFromTemplate(string FileName, string Name)
        {
            throw new System.NotImplementedException();
        }

        public Project ContainingProject
        {
            get { throw new System.NotImplementedException(); }
        }

        public int Count
        {
            get { return projectItems.Count; }
        }

        public DTE DTE
        {
            get { throw new System.NotImplementedException(); }
        }

        IEnumerator<ProjectItem> IEnumerable<ProjectItem>.GetEnumerator()
        {
            foreach (ProjectItem item in projectItems)
            {
                yield return item;
            }
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return ((IEnumerable<ProjectItem>)this).GetEnumerator();
        }

        public ProjectItem Item(object index)
        {
            if (index is int)
            {
                return projectItems[(int)index];
            }
            else if (index is string)
            {
                var item = new MockProjectItem("Stub mock created dynamically by calling Item(string)");
                AddProjectItem(item);
                return item;
            }
            return null;

        }

        public string Kind
        {
            get { throw new System.NotImplementedException(); }
        }

        public object Parent { get; set; }
    }
}