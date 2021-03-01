﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PnP.Core.Model
{
    /// <summary>
    /// Paged data retrieval can be done using the methods provided via this interface
    /// </summary>
    internal interface ISupportPaging
    {
        /// <summary>
        /// Determines whether paging is possible
        /// </summary>
        bool CanPage
        {
            get;
        }
    }

}
