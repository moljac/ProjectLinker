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
using Microsoft.Practices.ProjectLinker;
using Microsoft.Practices.ProjectLinker.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectLinker.Tests.Mocks;

namespace ProjectLinker.Tests.Utility
{
    [TestClass]
    public class FilterManagerFixture
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ShouldValidateArgument()
        {
            FilterManager.GetFilterForProject(null);
        }

        [TestMethod]
        public void ShouldRetrieveDefaultFilter()
        {
            var mockProject = new MockProject();
            
            var result = FilterManager.GetFilterForProject(mockProject);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof (RegexProjectItemsFilter));
        }

        [TestMethod]
        public void DefaultFilterShouldFilterOutFiles()
        {
            var mockProject = new MockProject();
            
            var filter = FilterManager.GetFilterForProject(mockProject);

            Assert.IsFalse(filter.CanBeSynchronized(@"c:\MyFolder\abc.silverlight.cs"));
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\MyFolder\abc.desktop"));
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\MyFolder.Silverlight\abc.cs"));
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\MyFolder.desktop\abc.cs"));
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\silverlight\abc.cs"));
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\Desktop\abc.cs"));
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\MyFolder\abc.xaml"));
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\Service References\abc.xaml"));
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\Web References\abc.xaml"));
            Assert.IsTrue(filter.CanBeSynchronized(@"c:\MyFolder\Service References\abc.cs"));
            Assert.IsTrue(filter.CanBeSynchronized(@"c:\MyFolder\Web References\abc.cs"));
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\MyFolder\abc.clientconfig"));
        }

        [TestMethod]
        public void ShouldRetrievePersistedFilters()
        {
            var mockProject = new MockProject();
            mockProject.Globals.Dictionary["ProjectLinkerExcludeFilter"] = @"\.excludeMe;\.excludeMeToo";
            
            var filter = FilterManager.GetFilterForProject(mockProject);

            Assert.IsFalse(filter.CanBeSynchronized(@"c:\MyFolder\abc.silverlight.excludeMe"));
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\MyFolder\abc.excludeMeToo"));
            Assert.IsTrue(filter.CanBeSynchronized(@"c:\MyFolder\abc.silverlight.cs"));
            Assert.IsTrue(filter.CanBeSynchronized(@"c:\MyFolder\abc.desktop"));
            Assert.IsTrue(filter.CanBeSynchronized(@"c:\MyFolder.Silverlight\abc.cs"));
            Assert.IsTrue(filter.CanBeSynchronized(@"c:\MyFolder.desktop\abc.cs"));
            Assert.IsTrue(filter.CanBeSynchronized(@"c:\silverlight\abc.cs"));
            Assert.IsTrue(filter.CanBeSynchronized(@"c:\Desktop\abc.cs"));
            Assert.IsTrue(filter.CanBeSynchronized(@"c:\MyFolder\abc.xaml"));
        }

        [TestMethod]
        public void ShouldPersistDefaultFiltersOnProject()
        {
            var mockProject = new MockProject();
            var defaultFilterExpressions = @"\\?desktop(\\.*)?$;\\?silverlight(\\.*)?$;\.desktop;\.silverlight;\.xaml;^service references(\\.*)?$;\.clientconfig;^web references(\\.*)?$";

            var filter = FilterManager.GetFilterForProject(mockProject);

            Assert.IsTrue(mockProject.Globals.set_VariablePersistsCalled);
            Assert.AreEqual(defaultFilterExpressions, mockProject.Globals.Dictionary["ProjectLinkerExcludeFilter"]);
        }
    }
}
