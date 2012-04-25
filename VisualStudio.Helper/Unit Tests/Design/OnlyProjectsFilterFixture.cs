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
using Microsoft.Practices.ProjectLinker.VisualStudio.Helper.Design;
using Microsoft.Practices.VisualStudio.UnitTestLibrary.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.ProjectLinker.VisualStudio.Helper.Tests.Design
{
    [TestClass]
    public class OnlyProjectsFilterFixture
    {
        [TestMethod]
        public void ShouldFilterOutChildren()
        {
            var solutionNode = new MockHierarchyNode() { IsSolution = true };
            var projectNode = new MockHierarchyNode() { ExtObject = new MockProject() };
            var itemNode = new MockHierarchyNode() { ExtObject = new object() };

            OnlyProjectsFilter target = new OnlyProjectsFilter();
            Assert.IsFalse(target.Filter(solutionNode));
            Assert.IsTrue(target.Filter(itemNode));
            Assert.IsFalse(target.Filter(projectNode));
        }

        internal class MockProject : Project
        {
            CodeModel Project.CodeModel
            {
                get { throw new System.NotImplementedException(); }
            }

            Projects Project.Collection
            {
                get { throw new System.NotImplementedException(); }
            }

            ConfigurationManager Project.ConfigurationManager
            {
                get { throw new System.NotImplementedException(); }
            }

            DTE Project.DTE
            {
                get { throw new System.NotImplementedException(); }
            }

            void Project.Delete()
            {
                throw new System.NotImplementedException();
            }

            string Project.ExtenderCATID
            {
                get { throw new System.NotImplementedException(); }
            }

            object Project.ExtenderNames
            {
                get { throw new System.NotImplementedException(); }
            }

            string Project.FileName
            {
                get { throw new System.NotImplementedException(); }
            }

            string Project.FullName
            {
                get { throw new System.NotImplementedException(); }
            }

            Globals Project.Globals
            {
                get { throw new System.NotImplementedException(); }
            }

            bool Project.IsDirty
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

            string Project.Kind
            {
                get { throw new System.NotImplementedException(); }
            }

            string Project.Name
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

            object Project.Object
            {
                get { throw new System.NotImplementedException(); }
            }

            ProjectItem Project.ParentProjectItem
            {
                get { throw new System.NotImplementedException(); }
            }

            ProjectItems Project.ProjectItems
            {
                get { throw new System.NotImplementedException(); }
            }

            EnvDTE.Properties Project.Properties
            {
                get { throw new System.NotImplementedException(); }
            }

            void Project.Save(string FileName)
            {
                throw new System.NotImplementedException();
            }

            void Project.SaveAs(string NewFileName)
            {
                throw new System.NotImplementedException();
            }

            bool Project.Saved
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

            string Project.UniqueName
            {
                get { throw new System.NotImplementedException(); }
            }

            object Project.get_Extender(string ExtenderName)
            {
                throw new System.NotImplementedException();
            }
        }
    }


}