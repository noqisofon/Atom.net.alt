/* -*- encoding: utf-8 -*- */
using System;
using System.Collections.Generic;
using System.Text;


namespace Atom.Core {


    using Atom.Utils;


    /// <summary>
    ///
    /// </summary>
    internal class AtomGenerator : AtomElement {
        #region constructor
        /// <summary>
        /// 
        /// </summary>
        internal AtomGenerator() {
            base.LocalName = "generator";

            this.content_ = DefaultValues.GeneratorMessage;
            this.url_ = DefaultValues.GeneratorUri;
            this.version_ = DefaultValues.GeneratorVersion;
        }
        #endregion constructor


        #region ToString-method
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            try {
                this.writeStartElement();
                base.buffer.Append( this.content_ );
                this.writeEndElement();

                return base.buffer.ToString();
            } finally {
                base.buffer.Length = 0;
            }
        }
        #endregion ToString-method


        #region ToString-helper-method
        /// <summary>
        /// 
        /// </summary>
        protected internal override void writeStartElement() {
            base.buffer.AppendFormat( "<{0}", base.LocalName );
            base.buffer.Append( ">" );
        }


        /// <summary>
        /// 
        /// </summary>
        protected internal override void writeEndElement() {
            base.buffer.AppendFormat( "</{0}>", this.LocalName ).AppendLine();
        }
        #endregion ToString-helper-method


        #region fields
        /// <summary>
        /// 
        /// </summary>
        private string content_ = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        private Uri url_ = DefaultValues.Uri;
        /// <summary>
        /// 
        /// </summary>
        private string version_ = string.Empty;
        #endregion fields
    }


}
