using System;
using System.IO;
using CookComputing.XmlRpc;
using NUnit.Framework;
using System.Xml;
using System.Text;

namespace ntest
{
  [TestFixture]
  public class ArrayTest
  {
    string expectedJagged =
@"<value>
  <array>
    <data>
      <value>
        <array>
          <data />
        </array>
      </value>
      <value>
        <array>
          <data>
            <value>
              <i4>1</i4>
            </value>
          </data>
        </array>
      </value>
      <value>
        <array>
          <data>
            <value>
              <i4>2</i4>
            </value>
            <value>
              <i4>3</i4>
            </value>
          </data>
        </array>
      </value>
    </data>
  </array>
</value>";

    string expectedMultiDim =
@"<value>
  <array>
    <data>
      <value>
        <array>
          <data>
            <value>
              <i4>1</i4>
            </value>
            <value>
              <i4>2</i4>
            </value>
          </data>
        </array>
      </value>
      <value>
        <array>
          <data>
            <value>
              <i4>3</i4>
            </value>
            <value>
              <i4>4</i4>
            </value>
          </data>
        </array>
      </value>
      <value>
        <array>
          <data>
            <value>
              <i4>5</i4>
            </value>
            <value>
              <i4>6</i4>
            </value>
          </data>
        </array>
      </value>
    </data>
  </array>
</value>";

    string expectedEmptyArray =
@"<value>
  <array>
    <data />
  </array>
</value>";

    [Test]
    public void SerializeJagged()
    {
      var jagged = new int[][] 
      {
        new int[] {},
        new int[] {1},
        new int[] {2, 3}
      };
      string xml = Utils.SerializeValue(jagged, true);
      Assert.AreEqual(expectedJagged, xml);
    }

    [Test]
    public void DeserializeJagged()
    {
      object retVal = Utils.ParseValue(expectedJagged, typeof(int[][]));
      Assert.IsInstanceOf<int[][]>(retVal);
      int[][] ret = (int[][])retVal;
      Assert.IsTrue(ret[0].Length == 0);
      Assert.IsTrue(ret[1].Length == 1);
      Assert.IsTrue(ret[2].Length == 2);
      Assert.AreEqual(1, ret[1][0]);
      Assert.AreEqual(2, ret[2][0]);
      Assert.AreEqual(3, ret[2][1]);
    }

    [Test]
    public void SerializeMultiDim()
    {
      int[,] multiDim = new int[3, 2] { {1, 2}, {3, 4}, {5, 6} };
      string xml = Utils.SerializeValue(multiDim, true);
      Assert.AreEqual(expectedMultiDim, xml);
    }

    [Test]
    public void DeserializeMultiDim()
    {
      object retVal = Utils.ParseValue(expectedMultiDim, typeof(int[,]));
      Assert.IsInstanceOf<int[,]>(retVal);
      int[,] ret = (int[,])retVal;
      Assert.AreEqual(1, ret[0, 0]);
      Assert.AreEqual(2, ret[0, 1]);
      Assert.AreEqual(3, ret[1, 0]);
      Assert.AreEqual(4, ret[1, 1]);
      Assert.AreEqual(5, ret[2, 0]);
      Assert.AreEqual(6, ret[2, 1]);
    }

    [Test]
    public void SerializeEmpty()
    {
      var empty = new int[] 
      {
      };
      string xml = Utils.SerializeValue(empty, true);
      Assert.AreEqual(expectedEmptyArray, xml);
    }

    [Test]
    public void DeserializeEmpty()
    {
      object retVal = Utils.ParseValue(expectedEmptyArray, typeof(int[]));
      Assert.IsInstanceOf<int[]>(retVal);
      int[] ret = (int[])retVal;
      Assert.IsTrue(ret.Length == 0);
    }

    //---------------------- array -----------------------------------------// 
    [Test]
    public void MixedArray_NullType()
    {
      Type parsedType, parsedArrayType;
      string xml = @"<?xml version=""1.0"" ?>
<value>
  <array>
    <data>
      <value><i4>12</i4></value>
      <value><string>Egypt</string></value>
      <value><boolean>0</boolean></value>
    </data>
  </array>
</value>";
      object obj = Utils.Parse(xml, null, MappingAction.Error,
        out parsedType, out parsedArrayType);
      Assert.IsTrue(obj is object[], "result is array of object");
      object[] ret = obj as object[];
      Assert.AreEqual(12, ret[0]);
      Assert.AreEqual("Egypt", ret[1]);
      Assert.AreEqual(false, ret[2]);
    }

    [Test]
    public void MixedArray_ObjectArrayType()
    {
      Type parsedType, parsedArrayType;
      string xml = @"<?xml version=""1.0"" ?>
<value>
  <array>
    <data>
      <value><i4>12</i4></value>
      <value><string>Egypt</string></value>
      <value><boolean>0</boolean></value>
    </data>
  </array>
</value>";
      object obj = Utils.Parse(xml, typeof(object[]), MappingAction.Error,
        out parsedType, out parsedArrayType);
      Assert.IsTrue(obj is object[], "result is array of object");
      object[] ret = obj as object[];
      Assert.AreEqual(12, ret[0]);
      Assert.AreEqual("Egypt", ret[1]);
      Assert.AreEqual(false, ret[2]);
    }

    [Test]
    public void MixedArray_ObjectType()
    {
      Type parsedType, parsedArrayType;
      string xml = @"<?xml version=""1.0"" ?>
<value>
  <array>
    <data>
      <value><i4>12</i4></value>
      <value><string>Egypt</string></value>
      <value><boolean>0</boolean></value>
    </data>
  </array>
</value>";
      object obj = Utils.Parse(xml, typeof(object), MappingAction.Error,
        out parsedType, out parsedArrayType);
      Assert.IsTrue(obj is object[], "result is array of object");
      object[] ret = obj as object[];
      Assert.AreEqual(12, ret[0]);
      Assert.AreEqual("Egypt", ret[1]);
      Assert.AreEqual(false, ret[2]);
    }

    [Test]
    public void HomogArray_NullType()
    {
      Type parsedType, parsedArrayType;
      string xml = @"<?xml version=""1.0"" ?>
<value>
  <array>
    <data>
      <value><i4>12</i4></value>
      <value><i4>13</i4></value>
      <value><i4>14</i4></value>
    </data>
  </array>
</value>";
      object obj = Utils.Parse(xml, null, MappingAction.Error,
        out parsedType, out parsedArrayType);
      Assert.IsTrue(obj is int[], "result is array of int");
      int[] ret = obj as int[];
      Assert.AreEqual(12, ret[0]);
      Assert.AreEqual(13, ret[1]);
      Assert.AreEqual(14, ret[2]);
    }

    [Test]
    public void HomogArray_IntArrayType()
    {
      Type parsedType, parsedArrayType;
      string xml = @"<?xml version=""1.0"" ?>
<value>
  <array>
    <data>
      <value><i4>12</i4></value>
      <value><i4>13</i4></value>
      <value><i4>14</i4></value>
    </data>
  </array>
</value>";
      object obj = Utils.Parse(xml, typeof(int[]), MappingAction.Error,
        out parsedType, out parsedArrayType);
      Assert.IsTrue(obj is int[], "result is array of int");
      int[] ret = obj as int[];
      Assert.AreEqual(12, ret[0]);
      Assert.AreEqual(13, ret[1]);
      Assert.AreEqual(14, ret[2]);
    }

    [Test]
    public void HomogArray_ObjectArrayType()
    {
      Type parsedType, parsedArrayType;
      string xml = @"<?xml version=""1.0"" ?>
<value>
  <array>
    <data>
      <value><i4>12</i4></value>
      <value><i4>13</i4></value>
      <value><i4>14</i4></value>
    </data>
  </array>
</value>";
      object obj = Utils.Parse(xml, typeof(object[]), MappingAction.Error,
      out parsedType, out parsedArrayType);
      Assert.IsTrue(obj is object[], "result is array of object");
      object[] ret = obj as object[];
      Assert.AreEqual(12, ret[0]);
      Assert.AreEqual(13, ret[1]);
      Assert.AreEqual(14, ret[2]);
    }

    [Test]
    public void HomogArray_ObjectType()
    {
      Type parsedType, parsedArrayType;
      string xml = @"<?xml version=""1.0"" ?>
<value>
  <array>
    <data>
      <value><i4>12</i4></value>
      <value><i4>13</i4></value>
      <value><i4>14</i4></value>
    </data>
  </array>
</value>";
      object obj = Utils.Parse(xml, typeof(object), MappingAction.Error,
        out parsedType, out parsedArrayType);
      Assert.IsTrue(obj is int[], "result is array of int");
      int[] ret = obj as int[];
      Assert.AreEqual(12, ret[0]);
      Assert.AreEqual(13, ret[1]);
      Assert.AreEqual(14, ret[2]);
    }

    [Test]
    public void JaggedArray()
    {
      Type parsedType, parsedArrayType;
      string xml = @"<?xml version=""1.0"" ?>
 <value>
   <array>
     <data>
       <value>
         <array>
           <data>
             <value>
               <i4>1213028</i4>
             </value>
             <value>
               <string>products</string>
             </value>
           </data>
         </array>
       </value>
       <value>
         <array>
           <data>
             <value>
               <i4>666</i4>
             </value>
           </data>
         </array>
       </value>
     </data>
   </array>
 </value>";
      object obj = Utils.Parse(xml, typeof(object[][]), MappingAction.Error,
        out parsedType, out parsedArrayType);
      Assert.IsTrue(obj is object[][]);
      object[][] ret = (object[][])obj;
      Assert.AreEqual(1213028, ret[0][0]);
      Assert.AreEqual("products", ret[0][1]);
      Assert.AreEqual(666, ret[1][0]);
    }

    //---------------------- array -----------------------------------------// 
    [Test]
    public void Array()
    {
      object[] testary = new Object[] { 12, "Egypt", false };
      XmlReader xdoc = Utils.Serialize("SerializeTest.testArray",
      testary,
      Encoding.UTF8, new MappingActions { NullMappingAction = NullMappingAction.Ignore });
      Type parsedType, parsedArrayType;
      object obj = Utils.Parse(xdoc, null, MappingAction.Error,
        out parsedType, out parsedArrayType);
      Assert.IsTrue(obj is object[], "result is array of object");
      object[] ret = obj as object[];
      Assert.AreEqual(12, ret[0]);
      Assert.AreEqual("Egypt", ret[1]);
      Assert.AreEqual(false, ret[2]);
    }

    //---------------------- array -----------------------------------------// 
    [Test]
    public void MultiDimArray()
    {
      int[,] myArray = new int[,] { { 1, 2 }, { 3, 4 } };
      XmlReader xdoc = Utils.Serialize("SerializeTest.testMultiDimArray",
        myArray,
        Encoding.UTF8, new MappingActions { NullMappingAction = NullMappingAction.Ignore });
      Type parsedType, parsedArrayType;
      object obj = Utils.Parse(xdoc, typeof(int[,]), MappingAction.Error,
        out parsedType, out parsedArrayType);
      Assert.IsTrue(obj is int[,], "result is 2 dim array of int");
      int[,] ret = obj as int[,];
      Assert.AreEqual(1, ret[0, 0]);
      Assert.AreEqual(2, ret[0, 1]);
      Assert.AreEqual(3, ret[1, 0]);
      Assert.AreEqual(4, ret[1, 1]);
    }    
 
  }
}