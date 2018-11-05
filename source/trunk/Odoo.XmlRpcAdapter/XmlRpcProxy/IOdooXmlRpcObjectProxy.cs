namespace Odoo.XmlRpcAdapter.XmlRpcProxy
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CookComputing.XmlRpc;

    #endregion //Using Directives

    [XmlRpcUrl("http://docker.odoo.zone:9669/xmlrpc/2/object")]
    public interface IOdooXmlRpcObjectProxy : IXmlRpcProxy
    {
        #region Methods

        [XmlRpcMethod]
        object execute(string database, int userId, string password, string modelName, string method_name, object[] domainMembers, params object[] parameters);

        [XmlRpcMethod]
        object execute(string database, int userId, string password, string modelName, string method_name, int[] recordIdentifiers, params object[] parameters);

        [XmlRpcMethod]
        object execute(string database, int userId, string password, string modelName, string method_name, int[] recordIdentifiers);

        [XmlRpcMethod]
        object execute(string database, int userId, string password, string modelName, string method_name, params object[] parameters);

        #endregion //Methods
    }
}
