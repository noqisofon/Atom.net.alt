/* -*- encoding: utf-8 -*- */
using System;
using System.IO;
using System.Text;
using System.Xml;


namespace Atom {


    using Atom.Core;
    using Atom.Utils;


    /// <summary>
    ///
    /// </summary>
    internal class AtomWriter {
        #region constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text_writer"></param>
        internal AtomWriter(TextWriter text_writer)
            : this( new XmlTextWriter( text_writer ) ) {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="write_stream"></param>
        /// <param name="encoding"></param>
        internal AtomWriter(Stream write_stream, Encoding encoding)
            : this( new XmlTextWriter( write_stream, encoding ) ) {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="encoding"></param>
        internal AtomWriter(string filename, Encoding encoding)
            : this( new XmlTextWriter( filename, encoding ) ) {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        internal AtomWriter(XmlWriter writer) {
            this.writer_ = (XmlTextWriter)writer;

            Initialize();
        }
        #endregion constructors


        #region internal-methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="feed"></param>
        internal void write(AtomFeed feed) {
            this.WriteFeed( feed );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        internal void write(AtomEntry entry) {
            this.WriteEntry( entry );
        }
        #endregion internal-methods


        #region private-methods
        /// <summary>
        /// 
        /// </summary>
        private void WriteHeader() {
            this.writer_.WriteStartDocument();
            this.writer_.WriteComment( DefaultValues.GeneratorMessage );
            this.writer_.WriteRaw( Environment.NewLine );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="feed"></param>
        private void WriteFeed(AtomFeed feed) {
            if ( this.writer_ == null )
                throw new InvalidOperationException( "AtomWriter has been closed, and can not be written to." );

            this.WriteHeader();

            if ( feed == null )
                throw new RequiredElementNotFoundException( "AtomFeed cannot be null." );

            this.writer_.WriteRaw( feed.ToString() );

            this.writer_.Flush();
            this.writer_.Close();

            this.writer_ = null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        private void WriteEntry(AtomEntry entry) {
            if ( this.writer_ == null )
                throw new InvalidOperationException( "AtomWriter has been closed, and can not be written to." );
            if ( entry == null )
                throw new RequiredElementNotFoundException( "AtomEntry cannot be null." );

            this.writer_.WriteRaw( entry.ToString() );

            this.writer_.Flush();
            this.writer_.Close();

            this.writer_ = null;
        }


        /// <summary>
        /// 
        /// </summary>
        private void Initialize() {
            this.writer_.Formatting = Formatting.Indented;
            this.writer_.Indentation = 2;
        }
        #endregion private-methods


        /// <summary>
        /// 
        /// </summary>
        private XmlTextWriter writer_ = null;
    }


}
