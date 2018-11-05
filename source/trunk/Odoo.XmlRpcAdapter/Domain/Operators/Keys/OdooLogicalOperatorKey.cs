namespace Odoo.XmlRpcAdapter.Domain.Operators.Keys
{
    /// <summary>
    /// Domain filters can be combined using logical operators in prefix form. Python Example:
    /// 
    /// To search for partners named ABC, from belgium or germany, whose language is not english:
    /// 
    /// [('name','=','ABC'),
    /// ('language.code','!=','en_US'),
    /// '|',('country_id.code','=','be'),
    /// ('country_id.code','=','de')]
    /// 
    /// This domain is interpreted as:
    /// 
    /// (name is 'ABC')
    /// AND(language is NOT english)
    /// AND(country is Belgium OR Germany)
    /// </summary>
    public enum OdooLogicalOperatorKey
    {
        /// <summary>
        /// "&" : Logical AND, default operation to combine criteria following one another. Arity 2 (uses the next 2 criteria or combinations).
        /// </summary>
        And,
        /// <summary>
        /// "|" : logical OR, arity 2.
        /// 
        /// How to use logical OR operator with XML-RPC ?
        /// https://www.odoo.com/forum/help-1/question/how-to-use-logical-or-operator-with-xml-rpc-25694
        /// </summary>
        Or,
        /// <summary>
        /// "!" : logical NOT, arity 1.
        /// </summary>
        Not
    }
}
