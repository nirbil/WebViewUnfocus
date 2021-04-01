using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WebViewUnfocus
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CancellationTokenSource _currCts = null;
        
        public MainPage()
        {
            this.InitializeComponent();
            var webView = new WebView(WebViewExecutionMode.SeparateProcess)
            {
                Source = new Uri("https://www.bing.com")
            };
            webView.SetValue(Grid.RowProperty, 1);
            WebViewWrapper.Content = webView;
        }

        private void SchedFocusClicked(object sender, RoutedEventArgs e)
        {
            if (_currCts != null)
            {
                _currCts.Cancel();
            }

            _currCts = new CancellationTokenSource();
            CountdownActivateFocus(_currCts, SchedBtn, "Schedule focus timer(x)", 3, false);
        }

        private void SchedFocus2Clicked(object sender, RoutedEventArgs e)
        {
            if (_currCts != null)
            {
                _currCts.Cancel();
            }

            _currCts = new CancellationTokenSource();
            CountdownActivateFocus(_currCts, SchedBtn2, "Schedule focus timer with activation (x)", 3, true);
        }

        private void CountdownActivateFocus(CancellationTokenSource cts, Button targetButton, string buttonTextTemplate, int countdown, bool activate)
        {
            
            _= Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (cts.IsCancellationRequested)
                {
                    return;
                }

                targetButton.Content = buttonTextTemplate.Replace("x", countdown.ToString());
                countdown--;
                if (countdown >= 0)
                {
                    Task.Delay(1000).ContinueWith((task) =>
                    {
                        CountdownActivateFocus(cts, targetButton, buttonTextTemplate, countdown, activate);
                    });
                }

                else
                {
                    FocusTarget.Focus(FocusState.Programmatic);
                    if (activate)
                    {
                        _ = Windows.System.Launcher.LaunchUriAsync(new Uri("mycustomprotocol:"));
                    }
                }
            });
        }
    }
}
