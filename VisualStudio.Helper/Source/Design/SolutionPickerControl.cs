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
using System.Diagnostics;
using System.Windows.Forms;


namespace Microsoft.Practices.ProjectLinker.VisualStudio.Helper.Design
{
    /// <summary>
    /// User control that allows selection of a valid target given a filter.
    /// </summary>
    public partial class SolutionPickerControl : UserControl
    {
        ISolutionPickerFilter filter;
        ISolutionPickerFilter onSelectFilter = new DefaultProjectsOnlyFilter();

        IHierarchyNode root;

        /// <summary>
        /// Empty constructor for design-time support.
        /// </summary>
        public SolutionPickerControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event rised whenever the user selects a new node in the tree.
        /// </summary>
        public event EventHandler SelectionChanged;

        /// <summary>
        /// Initializes the control receiving the root value 
        /// to customize the behavior of the control.
        /// </summary>
        /// <param name="root"></param>
        public SolutionPickerControl(IHierarchyNode root)
            : this(root, null)
        {
        }

        /// <summary>
        /// Initializes the control receiving the root value and a filter
        /// to customize the behavior of the control.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="filter"></param>
        public SolutionPickerControl(IHierarchyNode root, ISolutionPickerFilter filter)
        {
            Initialize(root, filter);
        }

        private void Initialize(IHierarchyNode rootNode, ISolutionPickerFilter pickerFilter)
        {
            this.filter = pickerFilter;
            this.root = rootNode;
            InitializeComponent();
            this.SuspendLayout();
            CreateNode(solutionTree.Nodes, this.root);
            solutionTree.Nodes[0].Expand();
            this.ResumeLayout(false);
        }

        private readonly object childrenTag = new object();

        private TreeNode CreateNode(TreeNodeCollection parentCollection, IHierarchyNode hierarchyNode)
        {
            Debug.Assert(hierarchyNode.Icon != null);
            TreeNode node = new TreeNode(hierarchyNode.Name);
            if (!treeIcons.Images.ContainsKey(hierarchyNode.IconKey) && hierarchyNode.Icon != null)
            {
                treeIcons.Images.Add(hierarchyNode.IconKey, hierarchyNode.Icon);
            }
            node.ImageKey = hierarchyNode.IconKey;
            node.SelectedImageKey = hierarchyNode.IconKey;
            node.Name = hierarchyNode.Name;
            node.Tag = hierarchyNode;
            if (hierarchyNode.HasChildren)
            {
                bool filterAll = true;
                foreach (IHierarchyNode child in hierarchyNode.Children)
                {
                    if (!Filter(child))
                    {
                        filterAll = false;
                        break;
                    }
                }
                if (!filterAll)
                {
                    TreeNode firstChildNode = new TreeNode();
                    firstChildNode.Tag = childrenTag;
                    node.Nodes.Add(firstChildNode);
                }
            }
            parentCollection.Add(node);
            return node;
        }

        /// <summary>
        /// Gets the target selected in the treeview.
        /// </summary>
        public IHierarchyNode SelectedTarget
        {
            get
            {
                return this.solutionTree.SelectedNode == null ?
                        null : (IHierarchyNode)this.solutionTree.SelectedNode.Tag;
            }
        }

        private void OnBeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Tag == this.childrenTag)
            {
                e.Node.Nodes.Remove(e.Node.Nodes[0]);
                IHierarchyNode hierarchyNode = (IHierarchyNode)e.Node.Tag;
                foreach (IHierarchyNode child in hierarchyNode.Children)
                {
                    if (!Filter(child))
                    {
                        CreateNode(e.Node.Nodes, child);
                    }
                }
            }
        }

        private bool Filter(IHierarchyNode node)
        {
            if (filter != null && filter.Filter(node))
            {
                return true;
            }
            return false;
        }

        private bool SelectFilter(IHierarchyNode node)
        {
            if (onSelectFilter != null & onSelectFilter.Filter(node))
            {
                return true;
            }

            return false;
        }


        internal class DefaultProjectsOnlyFilter : ISolutionPickerFilter
        {
            #region ISolutionPickerFilter Members

            public bool Filter(IHierarchyNode node)
            {
                return (!(node.ExtObject is EnvDTE.Project) ||
                        node.TypeGuid == Microsoft.VisualStudio.VSConstants.GUID_ItemType_VirtualFolder);
            }

            #endregion
        }

        private void OnSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            IHierarchyNode node = e.Node.Tag as IHierarchyNode;

            if (SelectionChanged != null && node != null)
            {
                SelectionChanged(this, new EventArgs());
            }
        }
    }
}
