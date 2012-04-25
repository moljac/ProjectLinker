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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using EnvDTE;

namespace Microsoft.Practices.VisualStudio.UnitTestLibrary.Mocks
{
    public class MockEnvDTEProjectItems : EnvDTE.ProjectItems
    {
        private readonly MockVSHierarchy parentHierarchy;

        public MockEnvDTEProjectItems(MockVSHierarchy parentHierarchy)
        {
            this.parentHierarchy = parentHierarchy;
        }

        public ProjectItem Item(object index)
        {
            throw new System.NotImplementedException();
        }

        IEnumerator EnvDTE.ProjectItems.GetEnumerator()
        {
            return new MockProjectItemsEnumerator(parentHierarchy);
        }

        private class MockProjectItemsEnumerator : IEnumerator<MockEnvDTEProjectItem>
        {
            private readonly MockVSHierarchy hierarchy;
            private List<string>.Enumerator hierarchyChildrenEnumerator;

            public MockProjectItemsEnumerator(MockVSHierarchy hierarchy)
            {
                this.hierarchy = hierarchy;
                hierarchyChildrenEnumerator = hierarchy.Children.GetEnumerator();
            }

            public bool MoveNext()
            {
                return hierarchyChildrenEnumerator.MoveNext();
            }

            public void Reset()
            {
                hierarchyChildrenEnumerator.Dispose();
                hierarchyChildrenEnumerator = hierarchy.Children.GetEnumerator();
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public MockEnvDTEProjectItem Current
            {
                get
                {
                    return new MockEnvDTEProjectItem(hierarchy) { Name = hierarchyChildrenEnumerator.Current };
                }
            }

            public void Dispose()
            {
                hierarchyChildrenEnumerator.Dispose();
            }
        }

        public ProjectItem AddFromFile(string FileName)
        {
            var newItem = new MockEnvDTEProjectItem(parentHierarchy) { Name = FileName };
            parentHierarchy.AddChild(Path.GetFileName(FileName));
            return newItem;
        }

        public ProjectItem AddFolder(string name, string kind)
        {
            var newItem = new MockEnvDTEProjectItem(parentHierarchy)
                              {
                                  Name = name,
                                  Kind = EnvDTE.Constants.vsProjectItemKindPhysicalFolder
                              };
            parentHierarchy.AddFolder(Path.GetFileName(name));
            return newItem;
        }

        public ProjectItem AddFromTemplate(string FileName, string Name)
        {
            throw new System.NotImplementedException();
        }

        public ProjectItem AddFromDirectory(string Directory)
        {
            throw new System.NotImplementedException();
        }

        public ProjectItem AddFromFileCopy(string FilePath)
        {
            throw new System.NotImplementedException();
        }

        public object Parent
        {
            get { return null; }
        }

        public int Count
        {
            get { return parentHierarchy.Children.Count; }
        }

        public DTE DTE
        {
            get { throw new System.NotImplementedException(); }
        }

        public string Kind
        {
            get { throw new System.NotImplementedException(); }
        }

        public Project ContainingProject
        {
            get { throw new System.NotImplementedException(); }
        }

        public IEnumerator GetEnumerator()
        {
            return new MockProjectItemsEnumerator(parentHierarchy);
        }
    }
}
