using System;
using CookComputing.XmlRpc;
using NUnit.Framework;
using System.IO;

namespace ntest
{
  public enum ItfEnum : int
  {
    Zero,
    One,
    Two,
    Three,
    Four,
  }

  public class ItfEnumClass
  {
    public ItfEnum IntEnum { get; set; }
    public ItfEnum intEnum;
    public ItfEnum[] IntEnums { get; set; }
    public ItfEnum[] intEnums;
  }

  [XmlRpcEnumMapping(EnumMapping.String)]
  public interface IEnumStringInterfaceMapping
  {
    [XmlRpcEnumMapping(EnumMapping.String)]
     void MappingOnMethod(ItfEnum param1, ItfEnum[] param2, 
      ItfEnum param3);
  }

  class EnumStringInterfaceMapping : IEnumStringInterfaceMapping
  {
    public void MappingOnMethod(ItfEnum param1, ItfEnum[] param2, ItfEnum param3)
    {
      throw new NotImplementedException();
    }

    [Test]
    public void SerializeWithMappingOnInterface()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] 
      { 
        IntEnum.Zero,
        new IntEnum[] { IntEnum.One, IntEnum.Two },
        new ItfEnumClass
        { 
          IntEnum = ItfEnum.One, 
          intEnum = ItfEnum.Two,
          IntEnums = new ItfEnum[] { ItfEnum.One, ItfEnum.Two },
          intEnums = new ItfEnum[] { ItfEnum.Three, ItfEnum.Four },
        } 
      };
      req.method = "MappingOnMethod";
      req.mi = this.GetType().GetMethod("MappingOnMethod");
      var ser = new XmlRpcRequestSerializer();
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();

      Assert.AreEqual(
        @"<?xml version=""1.0""?>
<methodCall>
  <methodName>MappingOnMethod</methodName>
  <params>
    <param>
      <value>
        <string>Zero</string>
      </value>
    </param>
    <param>
      <value>
        <array>
          <data>
            <value>
              <string>One</string>
            </value>
            <value>
              <string>Two</string>
            </value>
          </data>
        </array>
      </value>
    </param>
    <param>
      <value>
        <struct>
          <member>
            <name>IntEnum</name>
            <value>
              <string>One</string>
            </value>
          </member>
          <member>
            <name>IntEnums</name>
            <value>
              <array>
                <data>
                  <value>
                    <string>One</string>
                  </value>
                  <value>
                    <string>Two</string>
                  </value>
                </data>
              </array>
            </value>
          </member>
          <member>
            <name>intEnum</name>
            <value>
              <string>Two</string>
            </value>
          </member>
          <member>
            <name>intEnums</name>
            <value>
              <array>
                <data>
                  <value>
                    <string>Three</string>
                  </value>
                  <value>
                    <string>Four</string>
                  </value>
                </data>
              </array>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodCall>", reqstr);
    }
  }
}
