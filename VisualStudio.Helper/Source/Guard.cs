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
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Practices.ProjectLinker.VisualStudio.Helper.Properties;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.Practices.ProjectLinker.VisualStudio.Helper
{
    static class Guard
    {
        public static void ArgumentNotNullOrEmptyString(string value, string paramName)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentNullException(paramName,
                                                String.Format(CultureInfo.CurrentCulture,
                                                              Resources.General_ArgumentEmpty, paramName));

        }

        public static void ArgumentNotNull(object value, string paramName)
        {
            if (value == null)
                throw new ArgumentNullException(paramName,
                                                String.Format(CultureInfo.CurrentCulture,
                                                              Resources.General_ArgumentEmpty, paramName));

        }
    }
}
