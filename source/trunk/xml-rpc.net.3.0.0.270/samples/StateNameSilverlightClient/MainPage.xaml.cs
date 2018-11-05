using System;
using System.Windows;
using System.Windows.Controls;
using CookComputing.XmlRpc;

namespace StateNameSilverlightClient
{
  public partial class MainPage : UserControl
  {
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
      var proxy = XmlRpcProxyGen.Create<IStateName>();
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
  public interface IStateName : IXmlRpcProxy
  {
    [XmlRpcBegin("examples.getStateName")]
    IAsyncResult BeginGetName(int number, AsyncCallback acb);
    [XmlRpcEnd]
    string EndGetName(IAsyncResult iasr);
  }
}
