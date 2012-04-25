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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using ProjectLinker.Tests.Mocks;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

internal class MockVsHierarchy : IVsProject, IVsHierarchy
{
    public MockProject GetPropertyProjectValue = new MockProject();
    public Guid GetPropertyProjectIdGuidValue = Guid.NewGuid();
    public bool AddItemCalled = false;
    public uint AddItemArgumentItemidLoc;
    public VSADDITEMOPERATION AddItemArgumentAddItemOperation;
    public string AddItemArgumentItemName;
    public uint AddItemArgumentFilesToOpen;
    public string[] AddItemArgumentArrayFilesToOpen;
    public bool GetPropertyCalled;
    public uint GetPropertyArgumentItemId;
    public int GetPropertyArgumentPropId;
    public string GetPropertyProjectName;

    public int GetProperty(uint itemid, int propid, out object pvar)
    {
        GetPropertyCalled = true;
        GetPropertyArgumentItemId = itemid;
        GetPropertyArgumentPropId = propid;
        switch (propid)
        {
            case (int)__VSHPROPID.VSHPROPID_ExtObject:
                pvar = GetPropertyProjectValue;
                break;
            case (int)__VSHPROPID.VSHPROPID_ProjectIDGuid:
                pvar = GetPropertyProjectIdGuidValue;
                break;
            case (int)__VSHPROPID.VSHPROPID_Name:
                pvar = GetPropertyProjectName;
                break;
            default:
                pvar = null;
                break;
        }
        return 0;
    }

    public int AddItem(uint itemidLoc, VSADDITEMOPERATION dwAddItemOperation, string pszItemName, uint cFilesToOpen, string[] rgpszFilesToOpen, IntPtr hwndDlgOwner, VSADDRESULT[] pResult)
    {
        AddItemCalled = true;

        AddItemArgumentItemidLoc = itemidLoc;
        AddItemArgumentAddItemOperation = dwAddItemOperation;
        AddItemArgumentItemName = pszItemName;
        AddItemArgumentFilesToOpen = cFilesToOpen;
        AddItemArgumentArrayFilesToOpen = rgpszFilesToOpen;

        return VSConstants.S_OK;
    }

    #region IVsProject, IVsHierarchy members

    public int IsDocumentInProject(string pszMkDocument, out int pfFound, VSDOCUMENTPRIORITY[] pdwPriority, out uint pitemid)
    {
        throw new NotImplementedException();
    }

    public int GetMkDocument(uint itemid, out string pbstrMkDocument)
    {
        throw new NotImplementedException();
    }

    public int OpenItem(uint itemid, ref Guid rguidLogicalView, IntPtr punkDocDataExisting, out IVsWindowFrame ppWindowFrame)
    {
        throw new NotImplementedException();
    }

    public int GetItemContext(uint itemid, out IServiceProvider ppSP)
    {
        throw new NotImplementedException();
    }

    public int GenerateUniqueItemName(uint itemidLoc, string pszExt, string pszSuggestedRoot, out string pbstrItemName)
    {
        throw new NotImplementedException();
    }

    public int AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie)
    {
        throw new NotImplementedException();
    }

    public int Close()
    {
        throw new NotImplementedException();
    }

    public int GetCanonicalName(uint itemid, out string pbstrName)
    {
        throw new NotImplementedException();
    }

    public int GetGuidProperty(uint itemid, int propid, out Guid pguid)
    {
        object retVal;
        GetProperty(itemid, propid, out retVal);
        pguid = (Guid)retVal;
        return VSConstants.S_OK;
    }

    public int GetNestedHierarchy(uint itemid, ref Guid iidHierarchyNested, out IntPtr ppHierarchyNested, out uint pitemidNested)
    {
        throw new NotImplementedException();
    }

    public int GetSite(out IServiceProvider ppSP)
    {
        throw new NotImplementedException();
    }

    public int ParseCanonicalName(string pszName, out uint pitemid)
    {
        throw new NotImplementedException();
    }

    public int QueryClose(out int pfCanClose)
    {
        throw new NotImplementedException();
    }

    public int SetGuidProperty(uint itemid, int propid, ref Guid rguid)
    {
        throw new NotImplementedException();
    }

    public int SetProperty(uint itemid, int propid, object var)
    {
        throw new NotImplementedException();
    }

    public int SetSite(IServiceProvider psp)
    {
        throw new NotImplementedException();
    }

    public int UnadviseHierarchyEvents(uint dwCookie)
    {
        throw new NotImplementedException();
    }

    public int Unused0()
    {
        throw new NotImplementedException();
    }

    public int Unused1()
    {
        throw new NotImplementedException();
    }

    public int Unused2()
    {
        throw new NotImplementedException();
    }

    public int Unused3()
    {
        throw new NotImplementedException();
    }

    public int Unused4()
    {
        throw new NotImplementedException();
    }

    #endregion IVsProject, IVsHierarchy members
}