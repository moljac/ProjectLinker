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
using EnvDTE;

namespace Microsoft.Practices.VisualStudio.UnitTestLibrary.Mocks
{
    public class MockEnvDTEProjectItem : EnvDTE.ProjectItem
    {
        public MockEnvDTEProjectItem(MockVSHierarchy parentHierarchy)
        {
            Collection = new MockEnvDTEProjectItems(parentHierarchy);
        }

        public bool SaveAs(string NewFileName)
        {
            throw new System.NotImplementedException();
        }

        public Window Open(string ViewKind)
        {
            throw new System.NotImplementedException();
        }

        public void Remove()
        {
            throw new System.NotImplementedException();
        }

        public void ExpandView()
        {
            throw new System.NotImplementedException();
        }

        public void Save(string FileName)
        {
            throw new System.NotImplementedException();
        }

        public void Delete()
        {
            throw new System.NotImplementedException();
        }

        public bool IsDirty
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public short FileCount
        {
            get { throw new System.NotImplementedException(); }
        }

        public string Name { get; set; }

        public ProjectItems Collection { get; set; }

        public Properties Properties
        {
            get { throw new System.NotImplementedException(); }
        }

        public DTE DTE
        {
            get { throw new System.NotImplementedException(); }
        }

        public string Kind
        {
            get;
            set;
        }

        public ProjectItems ProjectItems
        {
            get { throw new System.NotImplementedException(); }
        }

        public object Object
        {
            get { throw new System.NotImplementedException(); }
        }

        public object ExtenderNames
        {
            get { throw new System.NotImplementedException(); }
        }

        public string ExtenderCATID
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool Saved
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public ConfigurationManager ConfigurationManager
        {
            get { throw new System.NotImplementedException(); }
        }

        public FileCodeModel FileCodeModel
        {
            get { throw new System.NotImplementedException(); }
        }

        public Document Document
        {
            get { throw new System.NotImplementedException(); }
        }

        public Project SubProject
        {
            get { throw new System.NotImplementedException(); }
        }

        public Project ContainingProject
        {
            get { throw new System.NotImplementedException(); }
        }

        public string get_FileNames(short index)
        {
            return Name;
        }

        public bool get_IsOpen(string ViewKind)
        {
            throw new System.NotImplementedException();
        }

        public object get_Extender(string ExtenderName)
        {
            throw new System.NotImplementedException();
        }
    }
}
