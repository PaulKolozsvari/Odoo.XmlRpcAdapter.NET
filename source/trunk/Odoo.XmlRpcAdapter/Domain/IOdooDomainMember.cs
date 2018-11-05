namespace Odoo.XmlRpcAdapter.Domain
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    /// <summary>
    /// A domain is a list of criteria, each criterion being a triple (either a list or a tuple) of (field_name, operator, value)
    /// https://www.odoo.com/documentation/12.0/reference/orm.html#domains
    /// 
    /// Python Tuple in Java XMLRPC
    /// https://stackoverflow.com/questions/42105943/python-tuple-in-java-xmlrpc
    /// </summary>
    public interface IOdooDomainMember
    {
        #region Methods

        object ToOdooObject();

        #endregion //Methods
    }
}
