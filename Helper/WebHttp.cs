using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class WebHttp
    {
        public CookieContainer Cookie = new CookieContainer();

        #region 同步通过POST方式发送数据
        /// <summary>
        /// 通过Post提交数据
        /// </summary>
        /// <param name="Url">接口地址</param>
        /// <param name="postDataStr">欲提交的Json数据</param>
        /// <param name="receStr">服务器返回Json</param>
        /// <returns></returns>
        public bool SendDataByPost(string Url, string postDataStr, out string receStr)
        {
            bool result = false;
            bool CheckUri = false;
            receStr = "";

            HttpWebRequest request = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(Url);
                //延时相应时间
                request.Timeout = 5000;
                request.KeepAlive = false;
                if (Cookie.Count == 0)
                {
                    request.CookieContainer = new CookieContainer();
                    Cookie = request.CookieContainer;
                }
                else
                {
                    request.CookieContainer = Cookie;
                }
                CheckUri = true;
            }
            catch (Exception e)
            {
                result = false;
                receStr = e.Message;
            }
            HttpWebResponse response = null;
            try
            {
                if (CheckUri)
                {
                    byte[] buff = Encoding.UTF8.GetBytes(postDataStr);
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.ContentLength = buff.Length;
                    using (Stream myRequestStream = request.GetRequestStream())
                    {
                        myRequestStream.Write(buff, 0, buff.Length);
                    }
                    response = (HttpWebResponse)request.GetResponse();
                    using (Stream myResponseStream = response.GetResponseStream())
                    {
                        using (StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8")))
                        {
                            receStr = myStreamReader.ReadToEnd();
                            result = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                receStr = e.Message;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
                response = null;
                request = null;
            }
            return result;
        }
        #endregion

        #region 旧方法有证书验证 SSL 修改日期2023年8月22日 建议使用这个符合安全规范

        public bool SendDataByPost1(string Url, string postDataStr, out string receStr)
        {
            bool result = false;
            bool CheckUri = false;
            receStr = "";
            HttpWebRequest request = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(Url);
                //延时相应时间
                request.Timeout = 5000;
                request.KeepAlive = false;
                if (Cookie.Count == 0)
                {
                    request.CookieContainer = new CookieContainer();
                    Cookie = request.CookieContainer;
                }
                else
                {
                    request.CookieContainer = Cookie;
                }
                CheckUri = true;
            }
            catch (Exception e)
            {
                result = false;
                receStr = e.Message;
            }
            HttpWebResponse response = null;
            try
            {
                if (CheckUri)
                {
                    byte[] buff = Encoding.UTF8.GetBytes(postDataStr);
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.ContentLength = buff.Length;
                    request.ServicePoint.Expect100Continue = false;   //加了这一行代码 就OK了
                    using (Stream myRequestStream = request.GetRequestStream())
                    {
                        myRequestStream.Write(buff, 0, buff.Length);
                    }
                    response = (HttpWebResponse)request.GetResponse();
                    using (Stream myResponseStream = response.GetResponseStream())
                    {
                        using (StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8")))
                        {
                            receStr = myStreamReader.ReadToEnd();
                            result = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                receStr = e.Message;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
                response = null;
                request = null;
            }
            return result;
        }
        #endregion

        #region 同步通过GET方式发送数据
        /// <summary>
        /// 通过GET方式发送数据
        /// </summary>
        /// <param name="Url">url</param>
        /// <param name="postDataStr">GET数据</param>
        /// <returns></returns>
        public bool SendDataByGET(string Url, string postDataStr, out string receStr)
        {
            bool result = false;
            bool CheckUri = false;
            receStr = "";
            HttpWebRequest request = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
                request.Timeout = 3000;
                request.KeepAlive = false;
                if (Cookie.Count == 0)
                {
                    request.CookieContainer = new CookieContainer();
                    Cookie = request.CookieContainer;
                }
                else
                {
                    request.CookieContainer = Cookie;
                }
                CheckUri = true;
            }
            catch (Exception e)
            {
                result = false;
                receStr = e.Message;
            }
            HttpWebResponse response = null;
            try
            {
                if (CheckUri)
                {
                    request.Method = "GET";
                    request.ContentType = "text/html;charset=UTF-8";
                    response = (HttpWebResponse)request.GetResponse();
                    using (Stream myResponseStream = response.GetResponseStream())
                    {
                        using (StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8")))
                        {
                            receStr = myStreamReader.ReadToEnd();
                            result = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                receStr = e.Message;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
                response = null;
                request = null;
            }
            return result;
        }
        #endregion 
    }
}
