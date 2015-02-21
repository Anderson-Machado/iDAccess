using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using Windows.Web.Http;

namespace App2
{
    class WebJson
    {
        // http://stackoverflow.com/questions/14577346/converting-ordinary-http-post-web-request-with-async-and-await
        //public static class WebRequestAsyncExtensions
        //{
        //    public static Task<Stream> xGetRequestStreamAsync(this WebRequest request)
        //    {
        //        return Task.Factory.FromAsync<Stream>(
        //            request.BeginGetRequestStream, request.EndGetRequestStream, null);
        //    }

        //    public static Task<WebResponse> xGetResponseAsync(this WebRequest request)
        //    {
        //        return Task.Factory.FromAsync<WebResponse>(
        //            request.BeginGetResponse, request.EndGetResponse, null);
        //    }
        //}
        // http://stackoverflow.com/questions/566437/http-post-returns-the-error-417-expectation-failed-c

        public static LoginResult lr;

        public static object JsonCommand(string cURL, object objRequest, Type tpResult)
        {
            try
            {
                //using (HttpClient client = new HttpClient())
                //{
                //    client.MaxResponseContentBufferSize = 100000;

                //    HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, pUrl);
                //    message.Headers.ExpectContinue = false;

                //    //message.Content = new StringContent(pParameters);
                //    //message.Content.Headers.ContentLength = pParameters.Length;

                //    response = await client.SendAsync(message);
                //}

                Type tpRequest = objRequest.GetType();
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(tpRequest);
                MemoryStream ms = new MemoryStream();
                //serializer.WriteObject(send, objRequest);
                serializer.WriteObject(ms, objRequest);
                byte[] bt = ms.ToArray();
                String cmd = System.Text.UTF8Encoding.UTF8.GetString(bt, 0, bt.Length);

                // https://social.msdn.microsoft.com/Forums/windowsapps/en-US/ee685fb9-d2aa-44c0-bd17-ea615e838aa6/httpclient-only-works-when-using-httprequestmessage-explicitly?forum=winappswithcsharp
                // http://stackoverflow.com/questions/10304863/how-to-use-system-net-httpclient-to-post-a-complex-type

                //Mutex mut = new Mutex();
                SemaphoreSlim semaphore = new SemaphoreSlim(1);

                string dataout = null;
                var client = new HttpClient();
                client.DefaultRequestHeaders.ExpectContinue = false;
                //HttpContent content = new StringContent(cmd); // how do I construct the Widget to post?
                HttpContent content = new StringContent(cmd, Encoding.UTF8, "application/json");
                //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                client.PostAsync(cURL, content).ContinueWith(
                (postTask) =>
                {
                    postTask.Result.EnsureSuccessStatusCode();
                    if (tpResult == typeof(LoginResult))
                    {
                        postTask.Result.Content.ReadAsStringAsync().ContinueWith(
                        (readTask) =>
                        {
                            dataout = readTask.Result;

                            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(tpResult);
                            ms = new MemoryStream();
                            bt = System.Text.UTF8Encoding.UTF8.GetBytes(dataout);
                            ms.Write(bt, 0, bt.Length);
                            ms.Position = 0;
                            lr = (LoginResult)deserializer.ReadObject(ms);

                            //semaphore.Release();
                            //mut.ReleaseMutex();
                        });
                    }
                });

                //mut.WaitOne();
                //semaphore.Wait(5000);


                //var request = WebRequest.Create(cURL);
                //request.ContentType = "application/json";
                //request.Method = "POST";
                //request.Headers.ExpectContinue = false;

                //using (Stream send = request.GetRequestStream()) {
                //var postStream = request.GetRequestStreamAsync();

                //// faz outras coisas enquanto aguarda
                //Type tpRequest = objRequest.GetType();
                //DataContractJsonSerializer serializer = new DataContractJsonSerializer(tpRequest);

                ////serializer.WriteObject(send, objRequest);
                //serializer.WriteObject(postStream.Result, objRequest);

                //using(WebResponse response = request.GetResponse()) {
                //var response = request.GetResponseAsync();

                //if (tpResult != null)
                //{
                //var reader = new StreamReader(response.Result.GetResponseStream());
                //string responseString = reader.ReadToEnd();
                //    Credentails = JsonConvert.DeserializeObject<Credentials>(responseString);
                //    if (Credentails != null && string.IsNullOrEmpty(Credentails.Err))
                //        CredentialsCallback(Credentails);
                //    else
                //    {
                //        if (Credentails != null)
                //            ErrorCallback(new Exception(string.Format("Error Code : {0}", StorageCredentails.Err)));
                //    }
                //}

                //DataContractJsonSerializer deserializer = new DataContractJsonSerializer(tpResult);
                //return deserializer.ReadObject(response.GetResponseStream());
                //return deserializer.ReadObject(response.Result.GetResponseStream());
                //}
                //else
                return null;

            }
            catch (Exception e)
            {
                return "Erro Geral: " + e.Message;
            }
        }
    }
}
