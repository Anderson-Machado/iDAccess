using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace App2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void btEnter_Click(object sender, RoutedEventArgs e)
        {
            LoginRequest acesso = new LoginRequest();
            acesso.login = txtLogin.Text;
            acesso.password = txtPassword.Text;

            object result = WebJson.JsonCommand("http://" + txtIP.Text + "/login.fcgi", acesso, typeof(LoginResult));
            if (result is LoginResult)
            {
                LoginResult dados = (LoginResult)result;
                if (dados.session != null)
                    tInfo.Text = "Sessão: " + dados.session;
                else
                    tInfo.Text = "Erro no Login: " + dados.error;
            }
            else if (result != null)
                tInfo.Text = "Erro Desconhecido: " + result;
            else
                tInfo.Text = "";
        }

        private void btRele_Click(object sender, RoutedEventArgs e)
        {
            if (WebJson.lr == null)
                tInfo.Text = "Efetue login primeiro";
            else
            {
                ActionsRequest ar = new ActionsRequest();
                ar.actions = new ActionItem[] { new ActionItem() { action = "door", parameters = "door=1" } };
                WebJson.JsonCommand("http://192.168.0.200/execute_actions.fcgi?session=" + WebJson.lr.session, ar, null);
            }
        }
    }
}
