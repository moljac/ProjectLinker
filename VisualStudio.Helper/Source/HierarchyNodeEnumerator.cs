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
using System.Diagnostics;
using Microsoft.VisualStudio;

namespace Microsoft.Practices.ProjectLinker.VisualStudio.Helper
{
    internal sealed class HierarchyNodeEnumerator : IEnumerator<IHierarchyNode>
    {
        HierarchyNode parent;
        uint currentItemId;

        public HierarchyNodeEnumerator(HierarchyNode parent)
        {
            Debug.Assert(parent != null);
            this.parent = parent;
            this.Reset();
        }

        #region IEnumerator<HierarchyNode> Members

        public IHierarchyNode Current
        {
            get
            {
                if (currentItemId != VSConstants.VSITEMID_NIL)
                {
                    return new HierarchyNode(parent, currentItemId);
                }
                return null;
            }
        }

        #endregion

        #region IDisposable Members

        private bool disposed;

        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // Take yourself off the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }
                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.

                // Note that this is not thread safe.
                // Another thread could start disposing the object
                // after the managed resources are disposed,
                // but before the disposed flag is set to true.
                // If thread safety is necessary, it must be
                // implemented by the client.

            }
            disposed = true;
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method 
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~HierarchyNodeEnumerator()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

        public bool MoveNext()
        {
            if (this.currentItemId == VSConstants.VSITEMID_NIL)
            {
                this.currentItemId = parent.FirstChildId;
            }
            else
            {
                this.currentItemId = this.parent.GetNextChildId(currentItemId);
            }
            return (this.currentItemId != VSConstants.VSITEMID_NIL);
        }

        public void Reset()
        {
            this.currentItemId = VSConstants.VSITEMID_NIL;
        }

        #endregion
    }
}
