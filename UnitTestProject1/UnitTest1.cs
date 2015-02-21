using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Connect_Simple()
        {
            System.Net.ServicePointManager.Expect100Continue = false;
            try
            {
                var request = WebRequest.Create("http://192.168.0.200/login.fcgi");
                request.ContentType = "application/json";
                request.Method = "POST";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write("{\"login\":\"admin\",\"password\":\"admin\"}");
                }

                var response = request.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    string cResult = streamReader.ReadToEnd();
                    Console.WriteLine(cResult);
                    Assert.IsTrue(cResult.Contains("session"), "Login Invalido");
                }
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        Console.WriteLine(text);
                    }
                }
                Assert.Fail("Erro Web: " + e.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("Erro Geral: " + e.Message);
            }
        }

        [TestMethod]
        public void Connect_Json()
        {
            LoginRequest acesso = new LoginRequest();
            acesso.login = "admin";
            acesso.password = "admin";

            object result = WebJson.JsonCommand("http://192.168.0.200/login.fcgi", acesso, typeof(LoginResult));
            if (result is LoginResult)
            {
                LoginResult dados = (LoginResult)result;
                Console.WriteLine("Sessão: " + dados.session);
                Console.WriteLine("Erro:" + dados.error);
                if (dados.session == null)
                    Assert.Inconclusive("Login invalido");
            }
            else
            {
                Assert.Fail((string)result);
            }
        }

        [TestMethod]
        public void AcionaRele_Json()
        {
            LoginRequest acesso = new LoginRequest();
            acesso.login = "admin";
            acesso.password = "admin";

            object result1 = WebJson.JsonCommand("http://192.168.0.200/login.fcgi", acesso, typeof(LoginResult));
            if (result1 is LoginResult)
            {
                LoginResult dados = (LoginResult)result1;
                Console.WriteLine("Sessão: " + dados.session);
                if (dados.session != null)
                {
                    ActionsRequest ar = new ActionsRequest();
                    ar.actions = new ActionItem[] { new ActionItem() { action = "door", parameters = "door=1" } };
                    // Não retorna saida
                    WebJson.JsonCommand("http://192.168.0.200/execute_actions.fcgi?session=" + dados.session, ar, null);
                }
                else
                    Assert.Inconclusive("Login invalido");
            }
            else
            {
                Assert.Fail((string)result1);
            }
        }
    }
}