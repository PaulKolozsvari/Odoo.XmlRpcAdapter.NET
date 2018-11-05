### Odoo XML RPC Adapter for .NET

This library is a .NET wrapper for the xmlrpc.net SDK (http://xml-rpc.net) which enables simplified XML RPC calls to Odoo (www.odoo.com).
It allows for quick and easy calls to all the Odoo RPC functions listed in the Odoo documentation: https://www.odoo.com/documentation/12.0/webservices/odoo.html

Has been tested on Odoo version 11 & 12.

External dependencies (included in this repository): 

  - [XML-RPC.NET version xml-rpc.net.3.0.0.270](http://xml-rpc.net)

  - [Figlut Server Toolkit](https://github.com/PaulKolozsvari/Figlut-Suite)

Sample (included in this reporsitory)

```C#

static void Main(string[] args)
        {
            try
            {
                OdooXmlRpcAdapter odoo = new OdooXmlRpcAdapter(
                    "http://docker.odoo.zone:9669/xmlrpc/2/common",
                    "http://docker.odoo.zone:9669/xmlrpc/2/object",
                    "samd_dev",
                    "admin",
                    "admin",
                    "fixers");
                if (!odoo.CheckAccessRights())
                {
                    throw new Exception("Access rights to web service not granted.");
                }
                int[] ids1 = odoo.Search(
                    "res.partner",
                    0,
                    5,
                    new object[] { "is_company", "=", true },
                    new object[] { "customer", "=", true });

                int[] ids2 = odoo.Search(
                    "res.partner",
                    0,
                    5,
                    OdooLogicalOperator.Or,
                    new OdooDomainExpression("is_company", OdooComparisonOperator.Equals, true),
                    new OdooDomainExpression("customer", OdooComparisonOperator.Equals, true));

                int count = odoo.SearchCount(
                    "res.partner",
                    null,
                    null,
                    OdooLogicalOperator.Or,
                    new OdooDomainExpression("is_company", OdooComparisonOperator.Equals, true),
                    new OdooDomainExpression("customer", OdooComparisonOperator.Equals, true));

                XmlRpcStruct[] records1 = odoo.Read(
                    "res.partner",
                    ids2);

                XmlRpcStruct[] records2 = odoo.Read(
                    "res.partner",
                    ids2,
                    new string[] { "name", "country_id", "comment" });

                XmlRpcStruct[] recordsSearchRead = odoo.SearchRead(
                    "res.partner",
                    0,
                    5,
                    new string[] { "name", "country_id", "comment" },
                    OdooLogicalOperator.Or,
                    new OdooDomainExpression("is_company", OdooComparisonOperator.Equals, true),
                    new OdooDomainExpression("customer", OdooComparisonOperator.Equals, true));

                XmlRpcStruct fieldsGetRecordsAll = odoo.FieldsGet(
                    "res.partner");

                XmlRpcStruct fieldsGetRecordsFiltered = odoo.FieldsGet(
                    "res.partner",
                    new string[] { "string", "help", "type", "readonly" });

                XmlRpcStruct r1 = new XmlRpcStruct();
                r1.Add("name", "New Partner");
                int recordId = odoo.Create(
                    "res.partner",
                    r1);

                XmlRpcStruct r2 = new XmlRpcStruct
                {
                    { "name", "Newer Partner" }
                };
                bool writeResult = odoo.Write(
                    "res.partner",
                    recordId,
                    r2);

                OdooNameGetResult nameGetResult = odoo.NameGet(
                    "res.partner",
                    recordId);

                object deleteResult = odoo.Unlink(
                    "res.partner",
                    new int[] { nameGetResult.RecordIdentifier },
                    true);

                nameGetResult = odoo.NameGet(
                    "res.partner",
                    recordId);
            }
            catch (XmlRpcFaultException fEx)
            {
                Console.Error.WriteLine(fEx.Message);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }

```

**Voice technology stack overview:** 