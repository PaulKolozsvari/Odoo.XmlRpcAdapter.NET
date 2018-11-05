namespace Odoo.XmlRpcAdapter.XmlRpcProxy
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CookComputing.XmlRpc;
    using System.Collections;

    #endregion //Using Directives

    [XmlRpcUrl("http://docker.odoo.zone:9669/xmlrpc/2/common")]
    public interface IOdooXmlRpcCommonProxy : IXmlRpcProxy
    {
        #region Methods

        [XmlRpcMethod("authenticate")]
        int authenticate(string database, string username, string password, params object[] user_agent_env);

        #endregion //Methods
    }
}
