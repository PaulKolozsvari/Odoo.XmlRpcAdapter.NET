using System;
using System.IO;
using CookComputing.XmlRpc;
using NUnit.Framework;

namespace ntest
{
  [TestFixture]
  class NilTestServer
  {
    [Test]
    public void DeserializeResponseNilMethod()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><nil /></value>
    </param>
  </params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      var deserializer = new XmlRpcResponseDeserializer();
      XmlRpcResponse response = deserializer.DeserializeResponse(sr, this.GetType());

      Assert.IsNull(response.retVal, "return value is null");
    }

    [Test]
    public void DeserializeResponseStructWithNil()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>lowerBound</name>
            <value><nil/></value>
          </member>
          <member>
            <name>upperBound</name>
            <value><nil/></value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodResponse>";
      StringReader sr = new StringReader(xml);
      var deserializer = new XmlRpcResponseDeserializer();
      XmlRpcResponse response = deserializer.DeserializeResponse(sr, typeof(ServerBounds));
      Assert.IsInstanceOf<ServerBounds>(response.retVal);
      ServerBounds bounds = response.retVal as ServerBounds;
      Assert.IsNull(bounds.lowerBound);
      Assert.IsNull(bounds.upperBound);
    }

    [Test]
    public void DeserializeRequestStructWithNil()
    {
      string xml = @"<?xml version=""1.0""?>
<methodCall>
    <methodName>StructWithArrayMethod</methodName>
    <params>
        <param>
            <value>
                <struct>
                    <member>
                        <name>ints</name>
                        <value>
                            <array>
                                <data>
                                    <value>
                                        <i4>1</i4>
                                    </value>
                                    <value>
                                        <nil />
                                    </value>
                                    <value>
                                        <i4>3</i4>
                                    </value>
                                </data>
                            </array>
                        </value>
                    </member>
                </struct>
            </value>
        </param>
    </params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      var deserializer = new XmlRpcRequestDeserializer();
      XmlRpcRequest request = deserializer.DeserializeRequest(sr, this.GetType());

      Assert.AreEqual(request.method, "StructWithArrayMethod", "method is TestString");
      Assert.AreEqual(1, request.args.Length);
      Assert.IsInstanceOf<StructWithArray>(request.args[0], "argument is StructWithArray");
      int?[] arg = ((StructWithArray)request.args[0]).ints;
      Assert.AreEqual(1, arg[0]);
      Assert.IsNull(arg[1]);
      Assert.AreEqual(3, arg[2]);
    }

    [Test]
    public void DeserializeRequestNilMethod()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>NilMethod</methodName> 
  <params>
    <param>
      <value><nil /></value>
    </param>
    <param>
      <value><int>12345</int></value>
    </param>
  </params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      var deserializer = new XmlRpcRequestDeserializer();
      XmlRpcRequest request = deserializer.DeserializeRequest(sr, this.GetType());

      Assert.AreEqual(request.method, "NilMethod", "method is TestString");
      Assert.IsNull(request.args[0], "argument is null");
      Assert.AreEqual(12345, (int)request.args[1], "argument is 12345");
    }


    [XmlRpcNullMapping(NullMappingAction.Nil)]
    public struct StructWithArray
    {
      public int?[] ints;
    }

    [XmlRpcMethod]
    public void StructWithArrayMethod(StructWithArray x)
    {
    }

    [XmlRpcMethod]
    public int? NilMethod(int? x, int? y)
    {
      return null;
    }
  }


  [XmlRpcNullMapping(NullMappingAction.Nil)]
  class ServerBounds
  {
    public int? lowerBound;
    public int? upperBound;
  }

}
