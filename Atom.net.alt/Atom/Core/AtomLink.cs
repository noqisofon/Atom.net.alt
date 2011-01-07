/* -*- encoding: utf-8 -*- */
using System;
using System.Collections.Generic;
using System.Text;


namespace Atom.Core {


    /// <summary>
    ///
    /// </summary>
    public class AtomLink : AtomElement {
        #region fields
        /// <summary>
        /// 
        /// </summary>
        private Relationship rel_ = DefaultValues.Rel;
        /// <summary>
        /// 
        /// </summary>
        private MediaType type_ = DefaultValues.MediaType;
        #endregion fields
    }


}
