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

namespace Microsoft.Practices.ProjectLinker.Commands
{
    internal static class AddProjectLinkCommandConstants
    {
        public const uint cmdidAddProjectLinkCommand = 0x62FF;
        public const string guidAddProjectLinkCommandString = "F3493348-D763-4454-9282-8759F60AE796";
        public static readonly Guid guidAddProjectLinkCommand = new Guid(guidAddProjectLinkCommandString);
    }
}