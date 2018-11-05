using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace CookComputing.XmlRpc
{
  public static class XmlRpcXmlReader
  {
    public static XmlReader Create(Stream stm)
    {
      XmlReader xmlRdr = XmlReader.Create(stm, GetSettings());
      return xmlRdr;
    }

    public static XmlReader Create(TextReader textReader)
    {
      XmlReader xmlRdr = XmlReader.Create(textReader, GetSettings());
      return xmlRdr;
    }

    static XmlReaderSettings GetSettings()
    {
      return new XmlReaderSettings
      {
        IgnoreComments = true,
        IgnoreProcessingInstructions = true,
        IgnoreWhitespace = false,
        CheckCharacters = false,
        DtdProcessing = DtdProcessing.Prohibit,
      };
    }
  }
}
