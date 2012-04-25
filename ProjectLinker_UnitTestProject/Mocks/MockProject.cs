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
    class MockProject : Project
    {
        static Random random = new Random();
        public MockProjectItems ProjectItems;
        public MockProperties Properties;
        public MockGlobals Globals = new MockGlobals();

        public MockProject()
        {
            Properties = new MockProperties(this);
            FullName = string.Format(@"c:\mockProjectPath\{0}\{0}.csproj", random.Next());
            ProjectItems = new MockProjectItems(this);
        }

        public MockProject(string fullName)
            : this()
        {
            FullName = fullName;
        }

        public string FullName { get; set; }

        Globals Project.Globals
        {
            get { return Globals; }
        }

        Properties Project.Properties
        {
            get { return Properties; }
        }

        ProjectItems Project.ProjectItems
        {
            get { return ProjectItems; }
        }

        public string Name
        {
            get
            {
                return Path.GetFileNameWithoutExtension(FullName);
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string Project.UniqueName
        {
            get { return this.FullName; }
        }

        CodeModel Project.CodeModel
        {
            get { throw new NotImplementedException(); }
        }

        Projects Project.Collection
        {
            get { throw new NotImplementedException(); }
        }

        ConfigurationManager Project.ConfigurationManager
        {
            get { throw new NotImplementedException(); }
        }

        DTE Project.DTE
        {
            get { throw new NotImplementedException(); }
        }

        void Project.Delete()
        {
            throw new NotImplementedException();
        }

        string Project.ExtenderCATID
        {
            get { throw new NotImplementedException(); }
        }

        object Project.ExtenderNames
        {
            get { throw new NotImplementedException(); }
        }

        string Project.FileName
        {
            get { throw new NotImplementedException(); }
        }

        bool Project.IsDirty
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

        string Project.Kind
        {
            get { return VSLangProj.PrjKind.prjKindCSharpProject; }
        }

        object Project.Object
        {
            get { throw new NotImplementedException(); }
        }

        ProjectItem Project.ParentProjectItem
        {
            get { throw new NotImplementedException(); }
        }

        void Project.Save(string FileName)
        {
            throw new NotImplementedException();
        }

        void Project.SaveAs(string NewFileName)
        {
            throw new NotImplementedException();
        }

        bool Project.Saved
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

        object Project.get_Extender(string ExtenderName)
        {
            throw new NotImplementedException();
        }
    }
}