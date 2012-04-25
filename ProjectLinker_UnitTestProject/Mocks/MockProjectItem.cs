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
using EnvDTE;

namespace ProjectLinker.Tests.Mocks
{
    class MockProjectItem : ProjectItem
    {
        public MockProjectItems ProjectItems;
        public bool DeleteCalled;
        public MockProperties MockProperties;

        public MockProjectItem(string name)
            : this(name, false)
        {
        }

        public MockProjectItem(string pathOrName, bool isLink)
        {
            Name = Path.GetFileName(pathOrName);
            MockProperties = new MockProperties(this);
            ProjectItems = new MockProjectItems(this);
            if (isLink)
            {
                MockProperties.PropertiesList.Add(new MockProperty("FullPath", pathOrName));
                MockProperties.PropertiesList.Add(new MockProperty("IsLink", true));
                Kind = Constants.vsProjectItemKindPhysicalFile;
            }
        }

        public string Name { get; set; }
        public string Kind { get; set; }

        ProjectItems ProjectItem.ProjectItems
        {
            get { return ProjectItems; }
        }

        public ProjectItems Collection { get; set; }

        public ConfigurationManager ConfigurationManager
        {
            get { throw new NotImplementedException(); }
        }

        public Project ContainingProject
        {
            get { throw new NotImplementedException(); }
        }

        public DTE DTE
        {
            get { throw new NotImplementedException(); }
        }

        public void Delete()
        {
            DeleteCalled = true;
        }

        public Document Document
        {
            get { throw new NotImplementedException(); }
        }

        public void ExpandView()
        {
            throw new NotImplementedException();
        }

        public string ExtenderCATID
        {
            get { throw new NotImplementedException(); }
        }

        public object ExtenderNames
        {
            get { throw new NotImplementedException(); }
        }

        public FileCodeModel FileCodeModel
        {
            get { throw new NotImplementedException(); }
        }

        public short FileCount
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsDirty
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public object Object
        {
            get { throw new NotImplementedException(); }
        }

        public Window Open(string ViewKind)
        {
            throw new NotImplementedException();
        }

        public Properties Properties
        {
            get { return MockProperties; }
        }

        public void Remove()
        {
            throw new NotImplementedException();
        }

        public void Save(string FileName)
        {
            throw new NotImplementedException();
        }

        public bool SaveAs(string NewFileName)
        {
            throw new NotImplementedException();
        }

        public bool Saved
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Project SubProject
        {
            get { throw new NotImplementedException(); }
        }


        public object get_Extender(string ExtenderName)
        {
            throw new NotImplementedException();
        }

        public string get_FileNames(short index)
        {
            throw new NotImplementedException();
        }

        public bool get_IsOpen(string ViewKind)
        {
            throw new NotImplementedException();
        }
    }
}