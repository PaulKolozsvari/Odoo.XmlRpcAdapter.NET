namespace Odoo.XmlRpcAdapter.Contants
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class OdooObjectMethodName
    {
        #region Constants

        /// <summary>
        /// Search for identifiers (integers) of records.
        /// </summary>
        public const string SEARCH = "search";

        /// <summary>
        /// Count the number of records.
        /// </summary>
        public const string SEARCH_COUNT = "search_count";

        /// <summary>
        /// Reads the fields of records by passing in the identifiers retrieved from the search method.
        /// </summary>
        public const string READ = "read";

        /// <summary>
        /// Can be used to inspect a model’s fields and check which ones seem to be of interest.
        /// 
        /// Because it returns a large amount of meta-information (it is also used by client programs) 
        /// it should be filtered before printing, the most interesting items for a human user are string (the field’s label), 
        /// help (a help text if available) and type (to know which values to expect, or to send when updating a record):
        /// </summary>
        public const string FIELDS_GET = "fields_get";

        /// <summary>
        /// Search and read in a single call.
        /// </summary>
        public const string SEARCH_READ = "search_read";

        /// <summary>
        /// Insert a record
        /// </summary>
        public const string CREATE = "create";

        /// <summary>
        /// Update a record
        /// </summary>
        public const string WRITE = "write";

        /// <summary>
        /// # get record name after having changed it
        /// </summary>
        public const string NAME_GET = "name_get";

        /// <summary>
        /// Delete records. Records can be deleted in bulk by providing their ids to unlink.
        /// </summary>
        public const string UNLINK = "unlink";

        #endregion //Constants
    }
}