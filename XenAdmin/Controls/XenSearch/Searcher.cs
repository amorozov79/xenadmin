/* Copyright (c) Citrix Systems Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.ComponentModel;
using System.Windows.Forms;
using XenAdmin.XenSearch;

namespace XenAdmin.Controls.XenSearch
{
    public partial class Searcher : UserControl
    {
        private int _maxHeight = 400;

        public event EventHandler SearchChanged;
        public event EventHandler SearchForChanged;

        public Searcher()
        {
            InitializeComponent();

            QueryElement.Searcher = this;
            GroupingControl.Searcher = this;

            QueryElement.QueryChanged += QueryElementQueryChanged;
            GroupingControl.GroupingChanged += GroupingControlGroupingChanged;
            searchFor.QueryChanged += SearchForQueryChanged;
        }

        private void GroupingControlGroupingChanged(object sender, EventArgs e)
        {
            OnSearchChanged(EventArgs.Empty);
        }

        private void QueryElementQueryChanged(object sender, EventArgs e)
        {
            OnSearchChanged(EventArgs.Empty);
        }

        private void SearchForQueryChanged(object sender, EventArgs e)
        {
            OnSearchForChanged(EventArgs.Empty);
            OnSearchChanged(EventArgs.Empty);
        }

        [Browsable(false)]
        public Search Search
        {
            set
            {
                if (value != null)
                {
                    // It's actually necessary for searchFor to come first here. Otherwise when
                    // we set the searchFor, it will remove any inapplicable filters and groups,
                    // which is not the desired behaviour for legacy saved searches.
                    if (searchFor != null)
                    {
                        searchFor.QueryScope = value.Query.QueryScope;
                    }

                    if (QueryElement != null)
                    {
                        QueryElement.QueryFilter = value.Query.QueryFilter;
                    }

                    if (GroupingControl != null)
                    {
                        GroupingControl.Grouping = value.Grouping;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum height for this control. The contents of this control will autoscroll inside this height.
        /// </summary>
        /// <value>The maximum height for this control.</value>
        public int MaxHeight
        {
            get { return _maxHeight; }
            set { _maxHeight = value; }
        }

        public QueryScope QueryScope
        {
            get { return searchFor.QueryScope; }
        }

        public QueryFilter QueryFilter
        {
            get { return QueryElement.QueryFilter; }
        }

        public Grouping Grouping
        {
            get { return GroupingControl.Grouping; }
        }

        protected virtual void OnSearchChanged(EventArgs e)
        {
            EventHandler handler = SearchChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnSearchForChanged(EventArgs e)
        {
            EventHandler handler = SearchForChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void QueryElement_Resize(object sender, EventArgs e)
        {
            // this control has AutoScroll set to true.

            int contentsHeight = QueryElement.Top + QueryElement.Height + 73;
            GroupsLabel.Top = contentsHeight - 64;
            GroupingControl.Top = contentsHeight - 41;

            Height = Math.Min(_maxHeight, contentsHeight);
        }
    }
}