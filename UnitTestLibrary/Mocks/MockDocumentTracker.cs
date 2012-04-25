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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.Practices.VisualStudio.UnitTestLibrary.Mocks
{
    public class MockDocumentTracker : IVsTrackProjectDocuments2
    {
        IVsTrackProjectDocumentsEvents2 _eventSink;
        public bool AdviseTrackProjectDocumentsEventsCalled;
        public IVsTrackProjectDocumentsEvents2 AdviseTrackProjectDocumentsEventsArgumentEventSink;


        public virtual void NotifyAddFile(IVsProject project, string file)
        {
            NotifyAddFiles(project, new[] { file });
        }

        public virtual void NotifyAddFiles(IVsProject project, string[] files)
        {
            var addFlags = new VSADDFILEFLAGS[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                addFlags[i] = VSADDFILEFLAGS.VSADDFILEFLAGS_NoFlags;
            }

            _eventSink.OnAfterAddFilesEx(
                1,
                files.Length,
                new[] { project },
                new[] { 0 },
                files,
                addFlags);
        }

        public void NotifyAddDirectory(IVsProject project, string[] directories)
        {
            var addFlags = new VSADDDIRECTORYFLAGS[directories.Length];

            for (int i = 0; i < directories.Length; i++)
            {
                addFlags[i] = VSADDDIRECTORYFLAGS.VSADDDIRECTORYFLAGS_NoFlags;
            }

            _eventSink.OnAfterAddDirectoriesEx(
                1,
                directories.Length,
                new[] { project },
                new[] { 0 },
                directories,
                addFlags);
        }


        public void NotifyRemoveFiles(IVsProject project, string[] files)
        {
            var addFlags = new VSREMOVEFILEFLAGS[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                addFlags[i] = VSREMOVEFILEFLAGS.VSREMOVEFILEFLAGS_NoFlags;
            }

            _eventSink.OnAfterRemoveFiles(
                1,
                files.Length,
                new[] { project },
                new[] { 0 },
                files,
                addFlags);
        }

        public int BeginBatch()
        {
            throw new System.NotImplementedException();
        }

        public int EndBatch()
        {
            throw new System.NotImplementedException();
        }

        public int Flush()
        {
            throw new System.NotImplementedException();
        }

        public int OnQueryAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults)
        {
            throw new System.NotImplementedException();
        }

        public int OnAfterAddFilesEx(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags)
        {
            throw new System.NotImplementedException();
        }

        public int OnAfterAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments)
        {
            throw new System.NotImplementedException();
        }

        public int OnAfterAddDirectoriesEx(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags)
        {
            throw new System.NotImplementedException();
        }

        public int OnAfterAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments)
        {
            throw new System.NotImplementedException();
        }

        public int OnAfterRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags)
        {
            throw new System.NotImplementedException();
        }

        public int OnAfterRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags)
        {
            throw new System.NotImplementedException();
        }

        public int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults)
        {
            throw new System.NotImplementedException();
        }

        public int OnQueryRenameFile(IVsProject pProject, string pszMkOldName, string pszMkNewName, VSRENAMEFILEFLAGS flags, out int pfRenameCanContinue)
        {
            throw new System.NotImplementedException();
        }

        public int OnAfterRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags)
        {
            throw new System.NotImplementedException();
        }

        public int OnAfterRenameFile(IVsProject pProject, string pszMkOldName, string pszMkNewName, VSRENAMEFILEFLAGS flags)
        {
            throw new System.NotImplementedException();
        }

        public int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
        {
            throw new System.NotImplementedException();
        }

        public int OnAfterRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags)
        {
            throw new System.NotImplementedException();
        }

        public int AdviseTrackProjectDocumentsEvents(IVsTrackProjectDocumentsEvents2 pEventSink, out uint pdwCookie)
        {
            AdviseTrackProjectDocumentsEventsCalled = true;
            AdviseTrackProjectDocumentsEventsArgumentEventSink = pEventSink;
            pdwCookie = 1;
            _eventSink = pEventSink;

            return VSConstants.S_OK;
        }

        public int UnadviseTrackProjectDocumentsEvents(uint dwCookie)
        {
            throw new System.NotImplementedException();
        }

        public int OnQueryAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults)
        {
            throw new System.NotImplementedException();
        }

        public int OnQueryRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults)
        {
            throw new System.NotImplementedException();
        }

        public int OnQueryRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
        {
            throw new System.NotImplementedException();
        }

        public int OnAfterSccStatusChanged(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, uint[] rgdwSccStatus)
        {
            throw new System.NotImplementedException();
        }


    }
}