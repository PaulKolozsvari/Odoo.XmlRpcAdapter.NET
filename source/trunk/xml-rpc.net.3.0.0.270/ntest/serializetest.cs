using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using CookComputing.XmlRpc;
using NUnit.Framework;

namespace ntest
// TODO: test any culture dependencies
{
  [TestFixture]
  public class SerializeTest
  {
    //---------------------- int -------------------------------------------// 
    [Test]
    public void Int()
    {
      XmlReader rdr = Utils.Serialize("SerializeTest.testInt",
        12345,
        Encoding.UTF8, new MappingActions { NullMappingAction = NullMappingAction.Ignore });
      Type parsedType, parsedArrayType;
      object obj = Utils.Parse(rdr, null, MappingAction.Error, 
        out parsedType, out parsedArrayType);
      Assert.AreEqual(12345, obj);
    }

    //---------------------- i64 -------------------------------------------// 
    [Test]
    public void Int64()
    {
      XmlReader rdr = Utils.Serialize("SerializeTest.testInt64",
        123456789012, Encoding.UTF8, new MappingActions 
        { NullMappingAction = NullMappingAction.Ignore });
      Type parsedType, parsedArrayType;
      object obj = Utils.Parse(rdr, null, MappingAction.Error,
        out parsedType, out parsedArrayType);
      Assert.AreEqual(123456789012, obj);
    }

    //---------------------- string ----------------------------------------// 
    [Test]
    public void String()
    {
      XmlReader rdr = Utils.Serialize("SerializeTest.testString", 
        "this is a string",
        Encoding.UTF8, new MappingActions { NullMappingAction = NullMappingAction.Ignore });
      Type parsedType, parsedArrayType;
      object obj = Utils.Parse(rdr, null, MappingAction.Error, 
        out parsedType, out parsedArrayType);
      Assert.AreEqual("this is a string", obj);
    }


    [Test]
    [ExpectedException(typeof(XmlRpcException))]
    public void InvalidCharsString()
    {
      string str = new string('\a', 1);
      XmlReader rdr = Utils.Serialize("SerializeTest.testString", str,
        Encoding.UTF8, new MappingActions { NullMappingAction = NullMappingAction.Ignore });
    }

    //---------------------- boolean ---------------------------------------// 
    [Test]
    public void Boolean()
    {
      XmlReader rdr = Utils.Serialize("SerializeTest.testBoolean", 
        true,
        Encoding.UTF8, new MappingActions { NullMappingAction = NullMappingAction.Ignore });
      Type parsedType, parsedArrayType;
      object obj = Utils.Parse(rdr, null, MappingAction.Error, 
        out parsedType, out parsedArrayType);
      Assert.AreEqual(true, obj);
    }

    //---------------------- double ----------------------------------------// 
    [Test]
    public void Double()
    {
      XmlReader xdoc = Utils.Serialize("SerializeTest.testDouble", 
        543.21,
        Encoding.UTF8, new MappingActions { NullMappingAction = NullMappingAction.Ignore });
      Type parsedType, parsedArrayType;
      object obj = Utils.Parse(xdoc, null, MappingAction.Error, 
        out parsedType, out parsedArrayType);
      Assert.AreEqual(543.21, obj);
    }

    //---------------------- dateTime ------------------------------------// 
    [Test]
    public void DateTime()
    {
      CultureInfo oldci = Thread.CurrentThread.CurrentCulture;
      try
      {
        foreach (string locale in Utils.GetLocales())
        {
          CultureInfo ci = new CultureInfo(locale);
          Thread.CurrentThread.CurrentCulture = ci;
          DateTime testDate = new DateTime(2002, 7, 6, 11, 25, 37);
          XmlReader xdoc = Utils.Serialize("SerializeTest.testDateTime",
            testDate, Encoding.UTF8, 
            new MappingActions { NullMappingAction = NullMappingAction.Error });
          Type parsedType, parsedArrayType;
          object obj = Utils.Parse(xdoc, null, MappingAction.Error, 
            out parsedType, out parsedArrayType);
          Assert.AreEqual(testDate, obj);
        }
      }
      catch(Exception ex)
      {
        Assert.Fail("unexpected exception: " + ex.Message);
      }
      finally
      {
        Thread.CurrentThread.CurrentCulture = oldci;
      }
    }

    [Test]
    public void DateTimeWarekiCalendar()
    {
      CultureInfo oldci = Thread.CurrentThread.CurrentCulture;
      try
      {
        CultureInfo ci = new CultureInfo("ja-JP");
        Thread.CurrentThread.CurrentCulture = ci;
        ci.DateTimeFormat.Calendar = new JapaneseCalendar();
        XmlReader xdoc = Utils.Serialize("SerializeTest.testDateTime", 
          new DateTime(2002, 7, 6, 11, 25, 37),
          Encoding.UTF8, new MappingActions { NullMappingAction = NullMappingAction.Ignore });
        Type parsedType, parsedArrayType;
        object obj = Utils.Parse(xdoc, null, MappingAction.Error, 
          out parsedType, out parsedArrayType);
        Assert.AreEqual(new DateTime(2002, 7, 6, 11, 25, 37), obj);
      }
      finally
      {
        Thread.CurrentThread.CurrentCulture = oldci;
      }
    }
 
    //---------------------- base64 ----------------------------------------// 
    [Test]
    public void Base64()
    {
      byte[] testb = new Byte[] 
      {
        121, 111, 117, 32, 99, 97, 110, 39, 116, 32, 114, 101, 97, 100, 
        32, 116, 104, 105, 115, 33 
      };
      XmlReader xdoc = Utils.Serialize("SerializeTest.testBase64", 
        testb,
        Encoding.UTF8, new MappingActions { NullMappingAction = NullMappingAction.Ignore });
      Type parsedType, parsedArrayType;
      object obj = Utils.Parse(xdoc, null, MappingAction.Error, 
        out parsedType, out parsedArrayType);
      Assert.IsTrue(obj is byte[], "result is array of byte");
      byte[] ret = obj as byte[];
      Assert.IsTrue(ret.Length == testb.Length);
      for (int i = 0; i < testb.Length; i++)
        Assert.IsTrue(testb[i] == ret[i]);
    }

    [Test]
    public void ZeroLengthBas64()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { new byte[0] };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();

      Assert.AreEqual(
@"<?xml version=""1.0""?>
<methodCall>
  <methodName>Foo</methodName>
  <params>
    <param>
      <value>
        <base64 />
      </value>
    </param>
  </params>
</methodCall>", reqstr);
    }
    
    //---------------------- struct ----------------------------------------// 
    public struct Struct1
    {
      public int mi;
      public string ms;
      public bool mb;
      public double md;
      public DateTime mdt;
      public byte[] mb64;
      public int[] ma;
      public bool Equals(Struct1 str)
      {
        if (mi != str.mi || ms != str.ms || md != str.md || mdt != str.mdt)
          return false;
        if (mb64.Length != str.mb64.Length)
          return false;
        for (int i = 0; i < mb64.Length; i++)
          if (mb64[i] != str.mb64[i])
            return false;
        for (int i = 0; i < ma.Length; i++)
          if (ma[i] != str.ma[i])
            return false;
        return true;
      }
    }
    
    [Test]
    public void StructTest()
    {
      byte[] testb = new Byte[] 
      {
        121, 111, 117, 32, 99, 97, 110, 39, 116, 32, 114, 101, 97, 100, 
        32, 116, 104, 105, 115, 33 
      }; 
      
      Struct1 str1 = new Struct1();
      str1.mi = 34567;
      str1.ms = "another test string";
      str1.mb = true;
      str1.md = 8765.123;
      str1.mdt = new DateTime(2002, 7, 6, 11, 25, 37);
      str1.mb64 = testb;
      str1.ma = new int[] { 1, 2, 3, 4, 5 };
      XmlReader xdoc = Utils.Serialize("SerializeTest.testStruct", 
        str1,
        Encoding.UTF8, new MappingActions { NullMappingAction = NullMappingAction.Ignore });
      Type parsedType, parsedArrayType;    
      object obj = Utils.Parse(xdoc, typeof(Struct1), MappingAction.Error, 
        out parsedType, out parsedArrayType);
      Assert.IsTrue(obj is Struct1, "result is Struct1");
      Struct1 str2 = (Struct1)obj;
      Assert.IsTrue(str2.Equals(str1));
    }

    public struct Struct2
    {
      [XmlRpcMember("member_1")]
      public int member1;

      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public int? member2;

      [XmlRpcMember("member_3")]
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public int? member3;

      [XmlRpcMember("member_4")]
      public int member4 { get; set; }

    }

    [Test]
    public void XmlRpcMember()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { new Struct2 { member1 = 1, member4 = 4 } };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();

      Assert.AreEqual(
@"<?xml version=""1.0""?>
<methodCall>
  <methodName>Foo</methodName>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>member_4</name>
            <value>
              <i4>4</i4>
            </value>
          </member>
          <member>
            <name>member_1</name>
            <value>
              <i4>1</i4>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodCall>", reqstr);
}


    public struct Struct3
    {
      int _member1;
      public int member1 { get { return _member1; } set { _member1 = value; } } 

      int _member2;
      public int member2 { get { return _member2; }  } 

      int _member3;
      [XmlRpcMember("member-3")]
      public int member3 { get { return _member3; } set { _member3 = value; } } 

      int _member4;
      [XmlRpcMember("member-4")]
      public int member4 { get { return _member4; }  } 
    }

    [Test]
    public void StructProperties()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { new Struct3() };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();

      Assert.AreEqual(@"<?xml version=""1.0""?>
<methodCall>
  <methodName>Foo</methodName>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>member1</name>
            <value>
              <i4>0</i4>
            </value>
          </member>
          <member>
            <name>member2</name>
            <value>
              <i4>0</i4>
            </value>
          </member>
          <member>
            <name>member-3</name>
            <value>
              <i4>0</i4>
            </value>
          </member>
          <member>
            <name>member-4</name>
            <value>
              <i4>0</i4>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodCall>", reqstr);
    }
         
         
    public class TestClass
    {
      public int _int;
      public string _string;
    }

    [Test]
    public void Class()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      TestClass arg = new TestClass();
      arg._int = 456;
      arg._string = "Test Class";
      req.args = new Object[] { arg };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();

      Assert.AreEqual(
        @"<?xml version=""1.0""?>
<methodCall>
  <methodName>Foo</methodName>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>_int</name>
            <value>
              <i4>456</i4>
            </value>
          </member>
          <member>
            <name>_string</name>
            <value>
              <string>Test Class</string>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodCall>", reqstr);
    }

#if (!SILVERLIGHT)
    public struct Struct4
    {
      [NonSerialized]
      public int x;
      public int y;
    }

    public class Class4
    {
      [NonSerialized]
      public int x;
      public int y;
    }


    [Test]
    public void NonSerialized()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { new Struct4() };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();
      Assert.AreEqual(
@"<?xml version=""1.0""?>
<methodCall>
  <methodName>Foo</methodName>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>y</name>
            <value>
              <i4>0</i4>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodCall>", reqstr);
    }
#endif

#if (!SILVERLIGHT)
    public struct Struct5
    {
      [NonSerialized]
      public System.Data.DataSet ds;
      public int y;
    }

    [Test]
    public void NonSerializedWithInvalidType()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] 
      { 
        new Struct5 { ds = new System.Data.DataSet(), y = 1234 } 
      };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();
      Assert.AreEqual(
@"<?xml version=""1.0""?>
<methodCall>
  <methodName>Foo</methodName>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>y</name>
            <value>
              <i4>1234</i4>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodCall>", reqstr);
    }

    [Test]
    public void NonSerializedClass()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { new Class4() };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();
      Assert.AreEqual(
@"<?xml version=""1.0""?>
<methodCall>
  <methodName>Foo</methodName>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>y</name>
            <value>
              <i4>0</i4>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodCall>", reqstr);
    }
#endif

    public class RecursiveMember
    {
      public string Level;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public RecursiveMember childExample;
    }

    [Test]
    public void RecursiveMemberTest()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      var example = new RecursiveMember
      {
        Level = "1",
        childExample = new RecursiveMember
        {
          Level = "2",
          childExample = new RecursiveMember
          {
            Level = "3",
          }
        }
      };
      req.args = new Object[] { example };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.UseStringTag = false;
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();
      Assert.AreEqual(
@"<?xml version=""1.0""?>
<methodCall>
  <methodName>Foo</methodName>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>Level</name>
            <value>1</value>
          </member>
          <member>
            <name>childExample</name>
            <value>
              <struct>
                <member>
                  <name>Level</name>
                  <value>2</value>
                </member>
                <member>
                  <name>childExample</name>
                  <value>
                    <struct>
                      <member>
                        <name>Level</name>
                        <value>3</value>
                      </member>
                    </struct>
                  </value>
                </member>
              </struct>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodCall>", reqstr);
    }

    public class RecursiveArrayMember
    {
      public string Level;
      public RecursiveArrayMember[] childExamples;
    }

    [Test]
    public void RecursiveArrayMemberTest()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      var example = new RecursiveArrayMember
      {
        Level = "1",
        childExamples = new RecursiveArrayMember[]
        {
          new RecursiveArrayMember
          {
            Level = "1-1",
            childExamples = new RecursiveArrayMember[]
            {
              new RecursiveArrayMember
              {
                Level = "1-1-1",
                childExamples = new RecursiveArrayMember[]
                {
                }
              },
              new RecursiveArrayMember
              {
                Level = "1-1-2",
                childExamples = new RecursiveArrayMember[]
                {
                }
              }
            }
          },
          new RecursiveArrayMember
          {
            Level = "1-2",
            childExamples = new RecursiveArrayMember[]
            {
              new RecursiveArrayMember
              {
                Level = "1-2-1",
                childExamples = new RecursiveArrayMember[]
                {
                }
              },
              new RecursiveArrayMember
              {
                Level = "1-2-2",
                childExamples = new RecursiveArrayMember[]
                {
                }
              }
            }
          }
        }
      };
      req.args = new Object[] { example };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.UseStringTag = false;
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();
      Assert.AreEqual(
@"<?xml version=""1.0""?>
<methodCall>
  <methodName>Foo</methodName>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>Level</name>
            <value>1</value>
          </member>
          <member>
            <name>childExamples</name>
            <value>
              <array>
                <data>
                  <value>
                    <struct>
                      <member>
                        <name>Level</name>
                        <value>1-1</value>
                      </member>
                      <member>
                        <name>childExamples</name>
                        <value>
                          <array>
                            <data>
                              <value>
                                <struct>
                                  <member>
                                    <name>Level</name>
                                    <value>1-1-1</value>
                                  </member>
                                  <member>
                                    <name>childExamples</name>
                                    <value>
                                      <array>
                                        <data />
                                      </array>
                                    </value>
                                  </member>
                                </struct>
                              </value>
                              <value>
                                <struct>
                                  <member>
                                    <name>Level</name>
                                    <value>1-1-2</value>
                                  </member>
                                  <member>
                                    <name>childExamples</name>
                                    <value>
                                      <array>
                                        <data />
                                      </array>
                                    </value>
                                  </member>
                                </struct>
                              </value>
                            </data>
                          </array>
                        </value>
                      </member>
                    </struct>
                  </value>
                  <value>
                    <struct>
                      <member>
                        <name>Level</name>
                        <value>1-2</value>
                      </member>
                      <member>
                        <name>childExamples</name>
                        <value>
                          <array>
                            <data>
                              <value>
                                <struct>
                                  <member>
                                    <name>Level</name>
                                    <value>1-2-1</value>
                                  </member>
                                  <member>
                                    <name>childExamples</name>
                                    <value>
                                      <array>
                                        <data />
                                      </array>
                                    </value>
                                  </member>
                                </struct>
                              </value>
                              <value>
                                <struct>
                                  <member>
                                    <name>Level</name>
                                    <value>1-2-2</value>
                                  </member>
                                  <member>
                                    <name>childExamples</name>
                                    <value>
                                      <array>
                                        <data />
                                      </array>
                                    </value>
                                  </member>
                                </struct>
                              </value>
                            </data>
                          </array>
                        </value>
                      </member>
                    </struct>
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


    public struct DefaultNullMappingStruct
    {
      public string s;
      public int? i;
    }

    [Test]
    [ExpectedException(typeof(XmlRpcMappingSerializeException))]
    public void DefaultNullMapping()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { new DefaultNullMappingStruct() };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();
      Assert.AreEqual(
@"<?xml version=""1.0""?>
<methodCall>
  <methodName>Foo</methodName>
  <params>
    <param>
      <value>
        <struct>
        </struct>
      </value>
    </param>
  </params>
</methodCall>", reqstr);
    }

    [XmlRpcNullMapping(NullMappingAction.Ignore)]
    public struct NullMappingIgnoreStruct
    {
      public string s;
      public int? i;
    }

    [Test]
    public void NullMappingIgnore()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { new NullMappingIgnoreStruct() };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();
      Assert.AreEqual(
@"<?xml version=""1.0""?>
<methodCall>
  <methodName>Foo</methodName>
  <params>
    <param>
      <value>
        <struct />
      </value>
    </param>
  </params>
</methodCall>", reqstr);
    }

    [XmlRpcNullMapping(NullMappingAction.Nil)]
    public struct NullMappingNilStruct
    {
      public string s;
      public int? i;
    }

    [Test]
    public void NullMappingNil()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { new NullMappingNilStruct() };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();
      Assert.AreEqual(
@"<?xml version=""1.0""?>
<methodCall>
  <methodName>Foo</methodName>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>s</name>
            <value>
              <nil />
            </value>
          </member>
          <member>
            <name>i</name>
            <value>
              <nil />
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodCall>", reqstr);
    }


    //---------------------- XmlRpcStruct ------------------------------------// 
    [Test]
    public void XmlRpcStruct()
    {
      XmlRpcStruct xmlRpcStruct = new XmlRpcStruct();
      xmlRpcStruct["mi"] = 34567;
      xmlRpcStruct["ms"] = "another test string";

      XmlReader xdoc = Utils.Serialize("SerializeTest.testXmlRpcStruct",
        xmlRpcStruct, Encoding.UTF8, 
        new MappingActions { NullMappingAction = NullMappingAction.Ignore });
      Type parsedType, parsedArrayType;
      object obj = Utils.Parse(xdoc, typeof(XmlRpcStruct), MappingAction.Error,
        out parsedType, out parsedArrayType);
      Assert.IsTrue(obj is XmlRpcStruct, "result is XmlRpcStruct");
      xmlRpcStruct = obj as XmlRpcStruct;
      Assert.IsTrue(xmlRpcStruct["mi"] is int);
      Assert.AreEqual((int)xmlRpcStruct["mi"], 34567);
      Assert.IsTrue(xmlRpcStruct["ms"] is string);
      Assert.AreEqual((string)xmlRpcStruct["ms"], "another test string");
    }

    [Test]
    [ExpectedException(typeof(XmlRpcUnsupportedTypeException))]
    public void XmlRpcStructIListValue()
    {
      IList<string> list = new List<string>();
      list.Add("one");
      list.Add("two");
      XmlRpcStruct xmlRpcStruct = new XmlRpcStruct();
      xmlRpcStruct["list"] = list;

      XmlReader xdoc = Utils.Serialize("SerializeTest.XmlRpcStructIListValue",
        xmlRpcStruct, Encoding.UTF8,
        new MappingActions { NullMappingAction = NullMappingAction.Ignore });
    }



    //---------------------- HashTable----------------------------------------// 
    [Test]
    [ExpectedException(typeof(XmlRpcUnsupportedTypeException))]
    public void Hashtable()
    {
      Hashtable hashtable = new Hashtable();
      hashtable["mi"] = 34567;
      hashtable["ms"] = "another test string";

      XmlReader xdoc = Utils.Serialize("SerializeTest.testXmlRpcStruct",
        hashtable, Encoding.UTF8, 
        new MappingActions { NullMappingAction = NullMappingAction.Ignore });
    }  


    //---------------------- XmlRpcInt -------------------------------------// 
    [Test]
    public void XmlRpcInt()
    {
      XmlReader xdoc = Utils.Serialize("SerializeTest.testXmlRpcInt", 
        new int?(12345),
        Encoding.UTF8, new MappingActions { NullMappingAction = NullMappingAction.Ignore });
      Type parsedType, parsedArrayType;
      object obj = Utils.Parse(xdoc, null, MappingAction.Error, 
        out parsedType, out parsedArrayType);
      Assert.AreEqual(12345, obj);
    }
  
    //---------------------- XmlRpcBoolean --------------------------------// 
    [Test]
    public void XmlRpcBoolean()
    {
      XmlReader xdoc = Utils.Serialize("SerializeTest.testXmlRpcBoolean", 
        new bool?(true),
        Encoding.UTF8, new MappingActions { NullMappingAction = NullMappingAction.Ignore });
      Type parsedType, parsedArrayType;
      object obj = Utils.Parse(xdoc, null, MappingAction.Error, 
        out parsedType, out parsedArrayType);
      Assert.AreEqual(true, obj);
    }

    //---------------------- XmlRpcDouble ----------------------------------// 
    [Test]
    public void XmlRpcDouble()
    {
      XmlReader xdoc = Utils.Serialize("SerializeTest.testXmlRpcDouble", 
        new double?(543.21),
        Encoding.UTF8, new MappingActions { NullMappingAction = NullMappingAction.Ignore });
      Type parsedType, parsedArrayType;
      object obj = Utils.Parse(xdoc, null, MappingAction.Error, 
        out parsedType, out parsedArrayType);
      Assert.AreEqual(543.21, obj);
    }

    [Test]
    public void XmlRpcDouble_ForeignCulture()
    {
      CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
      XmlReader xdoc;
      try
      {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-BE");
        double? xsd = new double?(543.21);
        //Console.WriteLine(xsd.ToString());
        xdoc = Utils.Serialize(
          "SerializeTest.testXmlRpcDouble_ForeignCulture", 
          new double?(543.21),
          Encoding.UTF8, new MappingActions { NullMappingAction = NullMappingAction.Ignore });
      }
      catch(Exception)
      {
        throw;
      }
      finally
      {
        Thread.CurrentThread.CurrentCulture = currentCulture;
      }
      Type parsedType, parsedArrayType;
      object obj = Utils.Parse(xdoc, null, MappingAction.Error, 
        out parsedType, out parsedArrayType);
      Assert.AreEqual(543.21, obj);
    }

    //---------------------- XmlRpcDateTime ------------------------------// 
    [Test]
    public void XmlRpcDateTime()
    {
      XmlReader xdoc = Utils.Serialize("SerializeTest.testXmlRpcDateTime", 
        new DateTime(2002, 7, 6, 11, 25, 37),
        Encoding.UTF8, new MappingActions { NullMappingAction = NullMappingAction.Ignore });
      Type parsedType, parsedArrayType;
      object obj = Utils.Parse(xdoc, null, MappingAction.Error, 
        out parsedType, out parsedArrayType);
      Assert.AreEqual(new DateTime(2002, 7, 6, 11, 25, 37), obj);
    }

    //---------------------- null parameter ----------------------------------// 
    [Test]
    public void NullParameter()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { null };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();

      Assert.AreEqual(
@"<?xml version=""1.0""?>
<methodCall>
  <methodName>Foo</methodName>
  <params>
    <param>
      <value>
        <nil />
      </value>
    </param>
  </params>
</methodCall>", reqstr);
    }

    //---------------------- formatting ----------------------------------// 
    [Test]
    public void DefaultFormatting()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { 1234567 };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();

      Assert.AreEqual(
@"<?xml version=""1.0""?>
<methodCall>
  <methodName>Foo</methodName>
  <params>
    <param>
      <value>
        <i4>1234567</i4>
      </value>
    </param>
  </params>
</methodCall>", reqstr);
    }

    [Test]
    public void IncreasedIndentation()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { 1234567 };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.Indentation = 4;
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();

      Assert.AreEqual(
        @"<?xml version=""1.0""?>
<methodCall>
    <methodName>Foo</methodName>
    <params>
        <param>
            <value>
                <i4>1234567</i4>
            </value>
        </param>
    </params>
</methodCall>", reqstr);
    }

    [Test]
    public void NoIndentation()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { 1234567 };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.UseIndentation = false;
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();

      Assert.AreEqual(
        "<?xml version=\"1.0\"?><methodCall><methodName>Foo</methodName>"+
        "<params><param><value><i4>1234567</i4></value></param></params>"+
        "</methodCall>", reqstr);
    }

    [Test]
    public void StructOrderTest()
    {
      byte[] testb = new Byte[] 
      {
        121, 111, 117, 32, 99, 97, 110, 39, 116, 32, 114, 101, 97, 100, 
        32, 116, 104, 105, 115, 33 
      };

      Struct1 str1 = new Struct1();
      str1.mi = 34567;
      str1.ms = "another test string";
      str1.mb = true;
      str1.md = 8765.123;
      str1.mdt = new DateTime(2002, 7, 6, 11, 25, 37);
      str1.mb64 = testb;
      str1.ma = new int[] { 1, 2, 3, 4, 5 };

      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { str1 };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();
      Assert.Less(reqstr.IndexOf(">mi</"), reqstr.IndexOf(">ms</"));
      Assert.Less(reqstr.IndexOf(">ms</"), reqstr.IndexOf(">mb</"));
      Assert.Less(reqstr.IndexOf(">mb</"), reqstr.IndexOf(">md</"));
      Assert.Less(reqstr.IndexOf(">md</"), reqstr.IndexOf(">mdt</"));
      Assert.Less(reqstr.IndexOf(">mdt</"), reqstr.IndexOf(">mb64</"));
      Assert.Less(reqstr.IndexOf(">mb64</"), reqstr.IndexOf(">ma</"));
    }

    [Test]
    public void StringNoStringTag()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { "string no string tag" };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.UseStringTag = false;
      ser.Indentation = 4;
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();

      Assert.AreEqual(
        @"<?xml version=""1.0""?>
<methodCall>
    <methodName>Foo</methodName>
    <params>
        <param>
            <value>string no string tag</value>
        </param>
    </params>
</methodCall>", reqstr);
    }

    [Test]
    public void StringStringTag()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { "string string tag" };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.UseStringTag = true;
      ser.Indentation = 4;
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();

      Assert.AreEqual(
        @"<?xml version=""1.0""?>
<methodCall>
    <methodName>Foo</methodName>
    <params>
        <param>
            <value>
                <string>string string tag</string>
            </value>
        </param>
    </params>
</methodCall>", reqstr);
    }

    [Test]
    public void StringDefaultTag()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { "string default tag" };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.Indentation = 4;
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();

      Assert.AreEqual(
        @"<?xml version=""1.0""?>
<methodCall>
    <methodName>Foo</methodName>
    <params>
        <param>
            <value>
                <string>string default tag</string>
            </value>
        </param>
    </params>
</methodCall>", reqstr);
    }

    [Test]
    public void UseInt()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { 1234 };
      req.method = "Foo";
      var ser = new XmlRpcRequestSerializer();
      ser.Indentation = 4;
      ser.UseIntTag = true;
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();

      Assert.AreEqual(
        @"<?xml version=""1.0""?>
<methodCall>
    <methodName>Foo</methodName>
    <params>
        <param>
            <value>
                <int>1234</int>
            </value>
        </param>
    </params>
</methodCall>", reqstr);
    }

    //---------------------- struct params -----------------------------------// 
    [XmlRpcMethod(StructParams=true)]
    public int Foo(int x, string y, double z)
    {
      return 1;
    }

    [Test]
    public void StructParams()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { 1234, "test", 10.1 };
      req.method = "Foo";
      req.mi = this.GetType().GetMethod("Foo");
      var ser = new XmlRpcRequestSerializer();
      ser.Indentation = 2;
      ser.UseIntTag = true;
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();

      Assert.AreEqual(
        @"<?xml version=""1.0""?>
<methodCall>
  <methodName>Foo</methodName>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>x</name>
            <value>
              <int>1234</int>
            </value>
          </member>
          <member>
            <name>y</name>
            <value>
              <string>test</string>
            </value>
          </member>
          <member>
            <name>z</name>
            <value>
              <double>10.1</double>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodCall>", reqstr);
    }

    [XmlRpcMethod(StructParams = true)]
    public int FooWithParams(int x, string y, params double[] z)
    {
      return 1;
    }

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidParametersException))]
    public void StructParamsWithParams()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { 1234, "test", new double[] { 10.1 } };
      req.method = "FooWithParams";
      req.mi = this.GetType().GetMethod("FooWithParams");
      var ser = new XmlRpcRequestSerializer();
      ser.Indentation = 2;
      ser.UseIntTag = true;
      ser.SerializeRequest(stm, req);
    }

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidParametersException))]
    public void StructParamsTooManyParams()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { 1234, "test", 10.1, "lopol" };
      req.method = "Foo";
      req.mi = this.GetType().GetMethod("Foo");
      var ser = new XmlRpcRequestSerializer();
      ser.Indentation = 2;
      ser.UseIntTag = true;
      ser.SerializeRequest(stm, req);
    }


    [XmlRpcMethod("artist.getInfo", StructParams = true)]
    public string getInfo(string artist, string api_key)
    {
      return "";
    }

    [Test]
    public void StructParamsGetInfo()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[] { "Bob Dylan", "abcd1234" };
      req.method = "artist.getInfo";
      req.mi = this.GetType().GetMethod("getInfo");
      var ser = new XmlRpcRequestSerializer();
      ser.Indentation = 2;
      ser.UseIntTag = true;
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();

      Assert.AreEqual(
        @"<?xml version=""1.0""?>
<methodCall>
  <methodName>artist.getInfo</methodName>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>artist</name>
            <value>
              <string>Bob Dylan</string>
            </value>
          </member>
          <member>
            <name>api_key</name>
            <value>
              <string>abcd1234</string>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodCall>", reqstr);
    }

    [XmlRpcMethod("system.pid")]
    public string getPid()
    {
      return "1234";
    }

    [Test]
    public void NoParams()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[0];
      req.method = "system.pid";
      req.mi = this.GetType().GetMethod("getPid");
      var ser = new XmlRpcRequestSerializer();
      ser.Indentation = 2;
      ser.UseIntTag = true;
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();

      Assert.AreEqual(
        @"<?xml version=""1.0""?>
<methodCall>
  <methodName>system.pid</methodName>
  <params />
</methodCall>", reqstr);
    }

        [Test]
    public void NoParams2()
    {
      Stream stm = new MemoryStream();
      XmlRpcRequest req = new XmlRpcRequest();
      req.args = new Object[0];
      req.method = "system.pid";
      req.mi = this.GetType().GetMethod("getPid");
      var ser = new XmlRpcRequestSerializer();
      ser.Indentation = 2;
      ser.UseIntTag = true;
      ser.UseEmptyParamsTag = false;
      ser.SerializeRequest(stm, req);
      stm.Position = 0;
      TextReader tr = new StreamReader(stm);
      string reqstr = tr.ReadToEnd();

      Assert.AreEqual(
        @"<?xml version=""1.0""?>
<methodCall>
  <methodName>system.pid</methodName>
</methodCall>", reqstr);
    }

     public void EmptyStringTag()
     {
         Stream stm = new MemoryStream();
         XmlRpcRequest req = new XmlRpcRequest();
         req.args = new Object[] { "" };
         req.method = "Foo";
         var ser = new XmlRpcRequestSerializer();
         ser.UseStringTag = true;
         ser.Indentation = 4;
         ser.UseEmptyElementTags = false; 
         ser.SerializeRequest(stm, req);
         stm.Position = 0;
         TextReader tr = new StreamReader(stm);
         string reqstr = tr.ReadToEnd();
 
         Assert.AreEqual(
           @"<?xml version=""1.0""?>
 <methodCall>
     <methodName>Foo</methodName>
     <params>
         <param>
             <value>
                 <string />
             </value>
         </param>
     </params>
 </methodCall>", reqstr);
     }
 
     [Test]
     public void EmptyStringTagNoEmptyTag()
     {
         Stream stm = new MemoryStream();
         XmlRpcRequest req = new XmlRpcRequest();
         req.args = new Object[] { "" };
         req.method = "Foo";
         var ser = new XmlRpcRequestSerializer();
         ser.UseStringTag = true;
         ser.Indentation = 4;
         ser.UseEmptyElementTags = false;
         ser.SerializeRequest(stm, req);
         stm.Position = 0;
         TextReader tr = new StreamReader(stm);
         string reqstr = tr.ReadToEnd();
 
         Assert.AreEqual(
           @"<?xml version=""1.0""?>
<methodCall>
    <methodName>Foo</methodName>
    <params>
        <param>
            <value>
                <string></string>
            </value>
        </param>
    </params>
</methodCall>", reqstr);
     }
 
     [Test]
     public void EmptyStringTagNoEmptyTagNoIndentation()
     {
         Stream stm = new MemoryStream();
         XmlRpcRequest req = new XmlRpcRequest();
         req.args = new Object[] { "" };
         req.method = "Foo";
         var ser = new XmlRpcRequestSerializer();
         ser.UseStringTag = true;
         ser.UseIndentation = false;
         ser.UseEmptyElementTags = false;
         ser.SerializeRequest(stm, req);
         stm.Position = 0;
         TextReader tr = new StreamReader(stm);
         string reqstr = tr.ReadToEnd();
 
         Assert.AreEqual(
           "<?xml version=\"1.0\"?><methodCall><methodName>Foo</methodName>"+
           "<params><param><value><string></string></value></param></params>"+
           "</methodCall>", reqstr);
     }

     [Test]
     public void EmptyValueTagNoEmptyTag()
     {
       Stream stm = new MemoryStream();
       XmlRpcRequest req = new XmlRpcRequest();
       req.args = new Object[] { "" };
       req.method = "Foo";
       var ser = new XmlRpcRequestSerializer();
       ser.UseStringTag = false;
       ser.Indentation = 4;
       ser.SerializeRequest(stm, req);
       stm.Position = 0;
       TextReader tr = new StreamReader(stm);
       string reqstr = tr.ReadToEnd();

       Assert.AreEqual(
         @"<?xml version=""1.0""?>
<methodCall>
    <methodName>Foo</methodName>
    <params>
        <param>
            <value></value>
        </param>
    </params>
</methodCall>", reqstr);
     }

     [Test]
     public void CRLFValue()
     {
       Stream stm = new MemoryStream();
       XmlRpcRequest req = new XmlRpcRequest();
       req.args = new Object[] { "\r\n" };
       req.method = "Foo";
       var ser = new XmlRpcRequestSerializer();
       ser.UseStringTag = false;
       ser.Indentation = 4;
       ser.SerializeRequest(stm, req);
       stm.Position = 0;
       TextReader tr = new StreamReader(stm);
       string reqstr = tr.ReadToEnd();

       Assert.AreEqual(
         @"<?xml version=""1.0""?>
<methodCall>
    <methodName>Foo</methodName>
    <params>
        <param>
            " + "<value>\r\n</value>"+ @"
        </param>
    </params>
</methodCall>", reqstr);
     }

     [Test]
     public void CRLFString()
     {
       Stream stm = new MemoryStream();
       XmlRpcRequest req = new XmlRpcRequest();
       req.args = new Object[] { "\r\n" };
       req.method = "Foo";
       var ser = new XmlRpcRequestSerializer();
       ser.UseStringTag = true;
       ser.Indentation = 4;
       ser.SerializeRequest(stm, req);
       stm.Position = 0;
       TextReader tr = new StreamReader(stm);
       string reqstr = tr.ReadToEnd();

       Assert.AreEqual(
         @"<?xml version=""1.0""?>
<methodCall>
    <methodName>Foo</methodName>
    <params>
        <param>
            <value>
                " + "<string>\r\n</string>" + @"
            </value>
        </param>
    </params>
</methodCall>", reqstr);
     }

     [Test]
     public void CRString()
     {
       Stream stm = new MemoryStream();
       XmlRpcRequest req = new XmlRpcRequest();
       req.args = new Object[] { "\r" };
       req.method = "Foo";
       var ser = new XmlRpcRequestSerializer();
       ser.UseStringTag = true;
       ser.Indentation = 4;
       ser.SerializeRequest(stm, req);
       stm.Position = 0;
       TextReader tr = new StreamReader(stm);
       string reqstr = tr.ReadToEnd();

       Assert.AreEqual(
         @"<?xml version=""1.0""?>
<methodCall>
    <methodName>Foo</methodName>
    <params>
        <param>
            <value>
                " + "<string>\r</string>" + @"
            </value>
        </param>
    </params>
</methodCall>", reqstr);
     }

     [Test]
     public void LFString()
     {
       Stream stm = new MemoryStream();
       XmlRpcRequest req = new XmlRpcRequest();
       req.args = new Object[] { "\n" };
       req.method = "Foo";
       var ser = new XmlRpcRequestSerializer();
       ser.UseStringTag = true;
       ser.Indentation = 4;
       ser.SerializeRequest(stm, req);
       stm.Position = 0;
       TextReader tr = new StreamReader(stm);
       string reqstr = tr.ReadToEnd();

       Assert.AreEqual(
         @"<?xml version=""1.0""?>
<methodCall>
    <methodName>Foo</methodName>
    <params>
        <param>
            <value>
                " + "<string>\n</string>" + @"
            </value>
        </param>
    </params>
</methodCall>", reqstr);
     }


    public struct TestPropertyMemberName
    {
      [XmlRpcMember("title1")]
      public string Title1;
      [XmlRpcMember("title2")]
      public string Title2 { get; set; }
    }

    [Test]
    public void PropertyMemberName()
    {
       Stream stm = new MemoryStream();
       XmlRpcRequest req = new XmlRpcRequest();
       req.args = new Object[] { new TestPropertyMemberName { Title1 = "abc", Title2 = "def"} };
       req.method = "Foo";
       var ser = new XmlRpcRequestSerializer();
       ser.UseStringTag = true;
       ser.Indentation = 2;
       ser.SerializeRequest(stm, req);
       stm.Position = 0;
       TextReader tr = new StreamReader(stm);
       string reqstr = tr.ReadToEnd();
       Assert.AreEqual(
@"<?xml version=""1.0""?>
<methodCall>
  <methodName>Foo</methodName>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>title2</name>
            <value>
              <string>def</string>
            </value>
          </member>
          <member>
            <name>title1</name>
            <value>
              <string>abc</string>
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
