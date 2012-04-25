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
using System.Drawing;
using Microsoft.Practices.ProjectLinker.VisualStudio.Helper;

namespace Microsoft.Practices.VisualStudio.UnitTestLibrary.Mocks
{
    public class MockHierarchyNode : IHierarchyNode
    {
        public object GetObjectReturnValue;

        public string SolutionRelativeName { get; set; }

        T IHierarchyNode.GetObject<T>()
        {
            return GetObjectReturnValue as T;
        }

        private Guid _projectGuid = Guid.NewGuid();

        public Guid TypeGuid { get; set; }

        public Guid ProjectGuid
        {
            get { return _projectGuid; }
            set { _projectGuid = value; }
        }

        public object ExtObject { get; set; }

        public bool IsSolution { get; set; }

        Icon IHierarchyNode.Icon
        {
            get { throw new NotImplementedException(); }
        }

        string IHierarchyNode.Name
        {
            get { throw new NotImplementedException(); }
        }

        string IHierarchyNode.IconKey
        {
            get { throw new NotImplementedException(); }
        }

        bool IHierarchyNode.HasChildren
        {
            get { throw new NotImplementedException(); }
        }

        IEnumerable<IHierarchyNode> IHierarchyNode.Children
        {
            get { throw new NotImplementedException(); }
        }

        uint IHierarchyNode.ItemId
        {
            get { throw new NotImplementedException(); }
        }
    }
}