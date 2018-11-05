using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using CookComputing.XmlRpc;
using System.Reflection;

namespace StateNameWindowsPhoneClient
{
  public partial class MainPage : PhoneApplicationPage
  {
    // Constructor
    public MainPage()
    {
      InitializeComponent();
    }

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
      Output.Visibility = Visibility.Collapsed;
      Error.Visibility = Visibility.Collapsed;
      int number;
      bool ok = Int32.TryParse(InputNumber.Text, out number);
      if (!ok)
      {
        Error.Text = "Enter a number";
        Error.Visibility = Visibility.Visible;
        return;
      }
      var proxy = new StateNameProxy();
      proxy.BeginGetName(number, asr =>
      {
        Dispatcher.BeginInvoke(delegate() 
          {
            try
            {
              StateName.Text = proxy.EndGetName(asr);
              StateNumber.Text = number.ToString();
              Output.Visibility = Visibility.Visible;
            }
            catch (XmlRpcFaultException fex)
            {
              Error.Text = "[" + fex.FaultCode.ToString() + "] " + fex.FaultString;
              Error.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
              Error.Text = ex.Message;
              Error.Visibility = Visibility.Visible;
            }
          });
      });
    }

    private void OnInputNumberChanged(object sender, TextChangedEventArgs e)
    {
      Error.Visibility = Visibility.Collapsed;
    }
  }

  [XmlRpcUrl("http://www.cookcomputing.com/xmlrpcsamples/RPC2.ashx")]
  public class StateNameProxy : XmlRpcClientProtocol 
  {
    [XmlRpcBegin("examples.getStateName")]
   public IAsyncResult BeginGetName(int number, AsyncCallback acb)
    {
      return this.BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { number }, acb, null);
    }
    [XmlRpcEnd]
    public string EndGetName(IAsyncResult iasr)
    {
      string ret = (string)this.EndInvoke(iasr);
      return ret;
    }
  }
}