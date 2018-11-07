namespace Odoo.XmlRpcAdapter
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CookComputing.XmlRpc;
    using Figlut.Server.Toolkit.Data;
    using Odoo.XmlRpcAdapter.XmlRpcProxy;
    using Odoo.XmlRpcAdapter.Domain;
    using Odoo.XmlRpcAdapter.Contants;
    using Odoo.XmlRpcAdapter.Results;
    using Odoo.XmlRpcAdapter.Domain.Operators.Mappers;

    #endregion //Using Directives

    /// <summary>
    /// Odoo Web Services (XML RPC): https://www.odoo.com/documentation/12.0/webservices/odoo.html
    /// Odoo ORM API: https://www.odoo.com/documentation/12.0/reference/orm.html
    /// </summary>
    public class OdooXmlRpcAdapter
    {
        #region Constructors

        public OdooXmlRpcAdapter(
            string xmlRpcCommonUrl,
            string xmlRpcObjectUrl,
            string databaseName,
            string userName,
            string password,
            string userAgent)
        {
            XmlRpcCommonUrl = xmlRpcCommonUrl;
            XmlRpcObjectUrl = xmlRpcObjectUrl;
            DatabaseName = databaseName;
            UserName = userName;
            Password = password;
        }

        #endregion //Constructors

        #region Fields

        private string _xmlRpcCommonUrl;
        private string _xmlRpcObjectUrl;

        private string _databaseName;
        private string _userName;
        private string _password;
        private string _userAgent;

        #region Derived Fields

        private Nullable<int> _userId;

        #endregion //Derived Fields

        #endregion //Fields

        public string XmlRpcCommonUrl
        {
            get { return _xmlRpcCommonUrl; }
            set
            {
                DataValidator.ValidateStringNotEmptyOrNull(value, EntityReader<OdooXmlRpcAdapter>.GetPropertyName(p => p.XmlRpcCommonUrl, false), nameof(OdooXmlRpcAdapter));
                _xmlRpcCommonUrl = value;
            }
        }

        public string XmlRpcObjectUrl
        {
            get { return _xmlRpcObjectUrl; }
            set
            {
                DataValidator.ValidateStringNotEmptyOrNull(value, EntityReader<OdooXmlRpcAdapter>.GetPropertyName(p => p.XmlRpcObjectUrl, false), nameof(OdooXmlRpcAdapter));
                _xmlRpcObjectUrl = value;
            }
        }

        public string DatabaseName
        {
            get { return _databaseName; }
            set
            {
                DataValidator.ValidateStringNotEmptyOrNull(value, EntityReader<OdooXmlRpcAdapter>.GetPropertyName(p => p.DatabaseName, false), nameof(OdooXmlRpcAdapter));
                _databaseName = value;
            }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                DataValidator.ValidateStringNotEmptyOrNull(value, EntityReader<OdooXmlRpcAdapter>.GetPropertyName(p => p.UserName, false), nameof(OdooXmlRpcAdapter));
                _userName = value;
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                DataValidator.ValidateStringNotEmptyOrNull(value, EntityReader<OdooXmlRpcAdapter>.GetPropertyName(p => p.Password, false), nameof(OdooXmlRpcAdapter));
                _password = value;
            }
        }

        public string UserAgent
        {
            get { return _userAgent; }
            set
            {
                DataValidator.ValidateStringNotEmptyOrNull(value, EntityReader<OdooXmlRpcAdapter>.GetPropertyName(p => p.UserAgent, false), nameof(OdooXmlRpcAdapter));
                _userAgent = value;
            }
        }

        public Nullable<int> UserId
        {
            get
            {
                if (!_userId.HasValue)
                {
                    _userId = Authenticate();
                }
                return _userId;
            }
        }

        #region Methods

        #region Utility Methods

        public IOdooXmlRpcCommonProxy CreateCommonProxy()
        {
            IOdooXmlRpcCommonProxy proxy = XmlRpcProxyGen.Create<IOdooXmlRpcCommonProxy>();
            proxy.Url = _xmlRpcCommonUrl;
            return proxy;
        }

        public IOdooXmlRpcObjectProxy CreateObjectProxy()
        {
            IOdooXmlRpcObjectProxy proxy = XmlRpcProxyGen.Create<IOdooXmlRpcObjectProxy>();
            proxy.Url = _xmlRpcObjectUrl;
            return proxy;
        }

        #region Static Methods

        public static T ConvertOdooResult<T>(object odooResult)
        {
            T result;
            try
            {
                result = (T)odooResult;
            }
            catch (Exception ex)
            {
                if (odooResult == null)
                {
                    throw new InvalidCastException(
                        $"Could not convert {nameof(odooResult)} 'null' of type {odooResult.GetType().FullName} to type {typeof(T).FullName}.",
                        ex);
                }
                else
                {
                    throw new InvalidCastException(
                        $"Could not convert {nameof(odooResult)} '{odooResult.ToString()}' of type {odooResult.GetType().FullName} to type {typeof(T).FullName}.",
                        ex);
                }
            }
            return result;
        }

        /// <summary>
        /// Wraps the array of domain members within an outer jagged array.
        /// </summary>
        /// <param name="domainMembers"></param>
        /// <returns></returns>
        public static object[] EncapsulateDomainMembers(IOdooDomainMember[] domainMembers)
        {
            List<object> resultList = new List<object>();
            domainMembers.ToList().ForEach(p => resultList.Add(p.ToOdooObject()));
            object[] result = resultList.ToArray();
            return result;
        }

        public static XmlRpcStruct GetFieldNamesXmlRpcStruct(object[] fieldNames, Nullable<int> paginationOffset, Nullable<int> paginationLimit)
        {
            XmlRpcStruct result = new XmlRpcStruct();
            result.Add("fields", fieldNames);
            if (paginationOffset.HasValue)
            {
                result.Add("offset", paginationOffset.Value);
            }
            if (paginationLimit.HasValue)
            {
                result.Add("limit", paginationLimit.Value);
            }
            return result;
        }

        #endregion //Static Methods

        #endregion //Utility Methods

        #region XML RPC Functions

        public int Authenticate()
        {
            return CreateCommonProxy().authenticate(DatabaseName, UserName, Password, UserAgent);
        }

        public bool CheckAccessRights()
        {
            object result = CreateObjectProxy().execute(
                DatabaseName,
                UserId.Value,
                Password,
                "res.partner",
                "check_access_rights",
                OdooObjectMethodName.READ,
                null,
                null);
            return ConvertOdooResult<bool>(result);
        }

        /// <summary>
        /// Run the Execute XML RPC function.
        /// </summary>
        /// <typeparam name="T">The expected Type of object to return.</typeparam>
        /// <param name="modelName">Name of the table to query.</param>
        /// <param name="methodName">The name of the Odoo object method to call.</param>
        /// <param name="domainMembers">The domain filter criteria made of of domain expressions and logical operators.</param>
        /// <param name="parameters">Additional parameters to pass to the method.</param>
        /// <returns>Returns an object of the T type specified.</returns>
        public T Execute<T>(string modelName, string methodName, object[][] domainMembers, params object[] parameters)
        {
            object result = CreateObjectProxy().execute(DatabaseName, UserId.Value, Password, modelName, methodName, domainMembers, parameters);
            return ConvertOdooResult<T>(result);
        }

        /// <summary>
        /// Run the Execute XML RPC function.
        /// </summary>
        /// <typeparam name="T">The expected Type of object to return.</typeparam>
        /// <param name="modelName">Name of the table to query.</param>
        /// <param name="methodName">The name of the Odoo object method to call.</param>
        /// <param name="domainMembers">The domain filter criteria made of of domain expressions and logical operators.</param>
        /// <param name="parameters">Additional parameters to pass to the method.</param>
        /// <returns>Returns an object of the T type specified.</returns>
        public T Execute<T>(string modelName, string methodName, IOdooDomainMember[] domainMembers, params object[] parameters)
        {
            object result = CreateObjectProxy().execute(DatabaseName, UserId.Value, Password, modelName, methodName, EncapsulateDomainMembers(domainMembers), parameters);
            return ConvertOdooResult<T>(result);
        }

        /// <summary>
        /// Run the Execute XML RPC function.
        /// </summary>
        /// <typeparam name="T">The expected Type of object to return.</typeparam>
        /// <param name="modelName">Name of the table to query.</param>
        /// <param name="methodName">The name of the Odoo object method to call.</param>
        /// <param name="recordIdentifiers">The record identifers related to the call e.g. used for example on the read method to query for specific records.</param>
        /// <param name="parameters">Additional parameters to pass to the method.</param>
        /// <returns>Returns an object of the T type specified.</returns>
        public T Execute<T>(string modelName, string methodName, int[] recordIdentifiers, params object[] parameters)
        {
            object result = CreateObjectProxy().execute(DatabaseName, UserId.Value, Password, modelName, methodName, recordIdentifiers, parameters);
            return ConvertOdooResult<T>(result);
        }

        /// <summary>
        /// Run the Execute XML RPC function.
        /// </summary>
        /// <typeparam name="T">The expected Type of object to return.</typeparam>
        /// <param name="modelName">Name of the table to query.</param>
        /// <param name="methodName">The name of the Odoo object method to call.</param>
        /// <param name="recordIdentifiers">The record identifers related to the call e.g. used for example on the read method to query for specific records.</param>
        /// <returns>Returns an object of the T type specified.</returns>
        public T Execute<T>(string modelName, string methodName, int[] recordIdentifiers)
        {
            object result = CreateObjectProxy().execute(DatabaseName, UserId.Value, Password, modelName, methodName, recordIdentifiers);
            return ConvertOdooResult<T>(result);
        }

        /// <summary>
        /// Run the Execute XML RPC function.
        /// </summary>
        /// <typeparam name="T">The expected Type of object to return.</typeparam>
        /// <param name="modelName">Name of the table to query.</param>
        /// <param name="methodName">The name of the Odoo object method to call.</param>
        /// <param name="parameters">Additional parameters to pass to the method.</param>
        /// <returns>Returns an object of the T type specified.</returns>
        public T Execute<T>(string modelName, string methodName, params object[] parameters)
        {
            object result = CreateObjectProxy().execute(DatabaseName, UserId.Value, Password, modelName, methodName, parameters);
            return ConvertOdooResult<T>(result);
        }

        #endregion //XML RPC Functions

        #region Execute Function Methods

        /// <summary>
        /// Search for identifiers (integers) of records.
        /// </summary>
        /// <param name="modelName">Name of the table to query.</param>
        /// <param name="paginationOffset">For quering a subset of the records, this is the offtset to start with.</param>
        /// <param name="paginationLimit">The maximum number of records to return.</param>
        /// <param name="domainMembers">The domain filter criteria made of of domain expressions and logical operators.</param>
        /// <returns>Returns an array of record identifiers of the records queried based on the specified criteria.</returns>
        public int[] Search(string modelName, Nullable<int> paginationOffset, Nullable<int> paginationLimit, params object[][] domainMembers)
        {
            object result = Execute<object>(modelName, OdooObjectMethodName.SEARCH, domainMembers, paginationOffset, paginationLimit);
            if (result.GetType().Equals(typeof(int[])))
            {
                return ConvertOdooResult<int[]>(result);
            }
            else if (result.GetType().Equals(typeof(object[])))
            {
                return new int[] { };
            }
            throw new XmlRpcUnexpectedTypeException($"Search returned object of unexpected type: {result.GetType().FullName}.");
        }

        /// <summary>
        /// Search for identifiers (integers) of records.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="modelName">Name of the table to query.</param>
        /// <param name="paginationOffset">For quering a subset of the records, this is the offtset to start with.</param>
        /// <param name="paginationLimit">The maximum number of records to return.</param>
        /// <param name="domainMembers">The domain filter criteria made of of domain expressions and logical operators.</param>
        /// <returns>Returns an array of record identifiers of the records queried based on the specified criteria.</returns>
        public int[] Search(string modelName, Nullable<int> paginationOffset, Nullable<int> paginationLimit, params IOdooDomainMember[] domainMembers)
        {
            object result = Execute<object>(modelName, OdooObjectMethodName.SEARCH, domainMembers, paginationOffset, paginationLimit);
            if (result.GetType().Equals(typeof(int[])))
            {
                return ConvertOdooResult<int[]>(result);
            }
            else if (result.GetType().Equals(typeof(object[])))
            {
                return new int[] { };
            }
            throw new XmlRpcUnexpectedTypeException($"Search returned object of unexpected type: {result.GetType().FullName}.");
        }

        /// <summary>
        /// Count the number of records.
        /// </summary>
        /// <param name="modelName">Name of the table to query.</param>
        /// <param name="paginationOffset">For quering a subset of the records, this is the offtset to start with.</param>
        /// <param name="paginationLimit">The maximum number of records to return.</param>
        /// <param name="domainMembers">The domain filter criteria made of of domain expressions and logical operators.</param>
        /// <returns>Returns the number of records that exist based on the criteria specified.</returns>
        public int SearchCount(string modelName, Nullable<int> paginationOffset, Nullable<int> paginationLimit, params IOdooDomainMember[] domainMembers)
        {
            return Execute<int>(modelName, OdooObjectMethodName.SEARCH_COUNT, domainMembers, paginationOffset, paginationLimit);
        }

        /// <summary>
        /// Reads records based on the identifiers passed in. The identifers should be retrieved from search function.
        /// </summary>
        /// <param name="modelName">Name of the table to query.</param>
        /// <param name="recordIdentifiers">The identifiers of the records to read and return as XML RPC structs.</param>
        /// <returns>Returns an array of XML RPC structs representing the objects queried.</returns>
        public XmlRpcStruct[] Read(string modelName, int[] recordIdentifiers)
        {
            return Execute<XmlRpcStruct[]>(modelName, OdooObjectMethodName.READ, recordIdentifiers);
        }

        /// <summary>
        /// Reads records or specific fields out of records based on the identifiers passed in. The identifers should be retrieved from search function.
        /// </summary>
        /// <param name="modelName">Name of the table to query.</param>
        /// <param name="recordIdentifiers">The identifiers of the records to read and return as XML RPC structs.</param>
        /// <param name="fieldNamse">The table fields to include in the XML RPC struct records that will be returned.</param>
        /// <returns>Returns an array of XML RPC structs representing the objects queried.</returns>
        public XmlRpcStruct[] Read(string modelName, int[] recordIdentifiers, string[] fieldNames)
        {
            return Execute<XmlRpcStruct[]>(modelName, OdooObjectMethodName.READ, recordIdentifiers, fieldNames, new object[] { });
        }

        /// <summary>
        /// Search and read records or specific fields of those records.
        /// </summary>
        /// <param name="modelName">Name of the table to query.</param>
        /// <param name="paginationOffset">For quering a subset of the records, this is the offtset to start with.</param>
        /// <param name="paginationLimit">The maximum number of records to return.</param>
        /// <param name="fieldNames">The table fields to include in the XML RPC struct records that will be returned.</param>
        /// <returns>Returns an array of XML RPC structs representing the objects queried.</returns>
        public XmlRpcStruct[] SearchRead(string modelName, Nullable<int> paginationOffset, Nullable<int> paginationLimit, string[] fieldNames, params IOdooDomainMember[] domainMembers)
        {
            return Execute<XmlRpcStruct[]>(modelName, OdooObjectMethodName.SEARCH_READ, domainMembers, fieldNames, paginationOffset, paginationLimit);
        }

        /// <summary>
        /// Can be used to query and inspect a model’s metadata for a list fields and check which ones seem to be of interest.
        /// 
        /// Because it returns a large amount of meta-information (it is also used by client programs) it should be filtered before printing, 
        /// the most interesting items for a human user are string (the field’s label), help (a help text if available) and 
        /// type (to know which values to expect, or to send when updating a record)
        /// </summary>
        /// <param name="modelName">Name of the table to query.</param>
        /// <returns>Returns an XML RPC struct representing the fields' metadata. The keys of the dictionary will be the field names while the values 
        /// for each key will be another dictionary of attribute names and their values.</returns>
        public XmlRpcStruct FieldsGet(string modelName)
        {
            return Execute<XmlRpcStruct>(modelName, OdooObjectMethodName.FIELDS_GET);
        }

        /// <summary>
        /// Can be used to query and inspect a model’s metadata for a list fields and check which ones seem to be of interest.
        /// 
        /// Because it returns a large amount of meta-information (it is also used by client programs) it should be filtered before printing, 
        /// the most interesting items for a human user are string (the field’s label), help (a help text if available) and 
        /// type (to know which values to expect, or to send when updating a record)
        /// </summary>
        /// <param name="modelName">Name of the table to query.</param>
        /// <param name="attributeNames">The names of the attributes to include about a table's fields.</param>
        /// <returns>Returns an XML RPC struct representing the fields' metadata. The keys of the dictionary will be the field names while the values 
        /// for each key will be another dictionary of attribute names and their values.</returns>
        public XmlRpcStruct FieldsGet(string modelName, string[] attributeNames)
        {
            return Execute<XmlRpcStruct>(modelName, OdooObjectMethodName.FIELDS_GET, new object[] { }, attributeNames);
        }

        /// <summary>
        /// Creates a new record.
        /// </summary>
        /// <param name="modelName">Name of the table to insert a record into.</param>
        /// <param name="record">The XML RPC struct with the keys being the name of the columns and values being the values of the columns.</param>
        /// <returns>Returns the identifier of the new record that was created.</returns>
        public int Create(string modelName, XmlRpcStruct record)
        {
            return Execute<int>(modelName, OdooObjectMethodName.CREATE, record);
        }

        /// <summary>
        /// Updates a record specified by the recordIdentifier in a specific table.
        /// </summary>
        /// <param name="modelName">The name of the table to update the record in.</param>
        /// <param name="recordIdentifier">The identifier of the record.</param>
        /// <param name="record">The XML RPC struct with the keys being the name of the columns and values being the values of the columns.</param>
        /// <returns></returns>
        public bool Write(string modelName, int recordIdentifier, XmlRpcStruct record)
        {
            return Execute<bool>(modelName, OdooObjectMethodName.WRITE, new int[] { recordIdentifier }, record);
        }

        /// <summary>
        /// Gets the name of a record after having updated it with the write method.
        /// </summary>
        /// <param name="modelName">The name of the table where the record is.</param>
        /// <param name="recordIdentifier">The identifier of the record.</param>
        /// <returns>Returns a jagged array with the ID and name of the record.</returns>
        public OdooNameGetResult NameGet(string modelName, int recordIdentifier)
        {
            object[][] result = Execute<object[][]>(modelName, OdooObjectMethodName.NAME_GET, new int[] { recordIdentifier });
            int id = ConvertOdooResult<int>(result[0][0]);
            string name = ConvertOdooResult<string>(result[0][1]);
            return new OdooNameGetResult(id, name);
        }

        /// <summary>
        /// Delete records. Records can be deleted in bulk by providing their ids to unlink.
        /// </summary>
        /// <param name="modelName">The name of the table from where to delete records.</param>
        /// <param name="recordIdentifiers">The identifiers of the records to be deleted.</param>
        /// <returns></returns>
        public bool Unlink(string modelName, int[] recordIdentifiers, bool validateRecordsDeleted)
        {
            bool result = Execute<bool>(modelName, OdooObjectMethodName.UNLINK, recordIdentifiers);
            if (validateRecordsDeleted)
            {
                foreach (int id in recordIdentifiers)
                {
                    int[] searchResult = Search(modelName, null, null, new OdooDomainExpression("id", OdooComparisonOperator.Equals, id));
                    if (searchResult.Length != 0)
                    {
                        throw new Exception($"Record with identifier of {id} could not be deleted.");
                    }
                }
            }
            return result;
        }

        #endregion //Execute Function Methods

        #endregion //Methods
    }
}
