using System;
using System.IO;
using System.Reflection;
using CookComputing.XmlRpc;
using NUnit.Framework;

namespace ntest
{
  [TestFixture]
  public class EnumTestServer
  {
    [return: XmlRpcEnumMapping(EnumMapping.String)]
    public IntEnum MappingReturnOnMethod()
    {
      return IntEnum.One;
    }

    [Test]
    public void SerializeResponseOnMethod()
    {
      var deserializer = new XmlRpcResponseSerializer();
      var response = new XmlRpcResponse(IntEnum.One,
        GetType().GetMethod("MappingReturnOnMethod"));
      var stm = new MemoryStream();
      deserializer.SerializeResponse(stm, response);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();
      Assert.AreEqual(
@"<?xml version=""1.0""?>
<methodResponse>
  <params>
    <param>
      <value>
        <string>One</string>
      </value>
    </param>
  </params>
</methodResponse>", reqstr);
    }

    [Test]
    public void SerializeResponseOnType()
    {
      var deserializer = new XmlRpcResponseSerializer();
      var proxy = XmlRpcProxyGen.Create<TestMethods2>();
      MethodInfo mi = proxy.GetType().GetMethod("Bar");
      var response = new XmlRpcResponse(IntEnum.Three, mi);
      var stm = new MemoryStream();
      deserializer.SerializeResponse(stm, response);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();
      Assert.AreEqual(
@"<?xml version=""1.0""?>
<methodResponse>
  <params>
    <param>
      <value>
        <string>Three</string>
      </value>
    </param>
  </params>
</methodResponse>", reqstr);
    }
  }
}
