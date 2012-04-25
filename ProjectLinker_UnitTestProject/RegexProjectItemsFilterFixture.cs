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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectLinker.Tests
{
    [TestClass]
    public class RegexProjectItemsFilterFixture
    {
        [TestMethod]
        public void ShouldCreateFilter()
        {
            var filter = new RegexProjectItemsFilter(new[] {".*"});
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowOnInvalidArgument()
        {
            var filter = new RegexProjectItemsFilter(null);
        }

        [TestMethod]
        public void ShouldFilterOutItem()
        {
            var filter = new RegexProjectItemsFilter(new[] {@"\.xaml"});
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\myTestString.xaml"));
        }

        [TestMethod]
        public void ShouldFilterInItem()
        {
            var filter = new RegexProjectItemsFilter(new[] { @"\.xaml" });
            Assert.IsTrue(filter.CanBeSynchronized(@"c:\myTestString.cs"));
        }
    }
}
