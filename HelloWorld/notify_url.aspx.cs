using Com.Alipay;
using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace StudyOnline
{


    /// <summary>
    /// 功能：服务器异步通知页面
    /// 版本：3.3
    /// 日期：2012-07-10
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
    /// 
    /// ///////////////////页面功能说明///////////////////
    /// 创建该页面文件时，请留心该页面文件中无任何HTML代码及空格。
    /// 该页面不能在本机电脑测试，请到服务器上做测试。请确保外部可以访问该页面。
    /// 该页面调试工具请使用写文本函数logResult。
    /// 如果没有收到该页面返回的 success 信息，支付宝会在24小时内按一定的时间策略重发通知
    /// </summary>
    public partial class notify_url : System.Web.UI.Page
    {
        private readonly log4net.ILog logi = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private StudyOnlineEntities entities = new StudyOnlineEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
            SortedDictionary<string, string> sPara = GetRequestPost();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);

                //打印日志
                logi.Debug("VerifyResult=" + verifyResult + "  sPara:" + String.Join(",", sPara.Select(o => String.Format("[{0},{1}]", o.Key, o.Value))));

                if (verifyResult)//验证成功
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码


                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中服务器异步通知参数列表

                    //商户订单号                
                    string out_trade_no = Request.Form["out_trade_no"];

                    //支付宝交易号                
                    string trade_no = Request.Form["trade_no"];

                    //交易状态
                    string trade_status = Request.Form["trade_status"];

                    #region 业务处理,liaorubei添加
                    Orders order = entities.Orders.Find(out_trade_no);
                    if (order == null)
                    {
                        logi.Debug("order is null");
                        Response.Write("订单不存在");
                    }
                    order.TradeNo = trade_no;
                    order.TradeStatus = trade_status;
                    entities.SaveChanges();
                    #endregion


                    if (Request.Form["trade_status"] == "TRADE_FINISHED")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序

                        //注意：
                        //退款日期超过可退款期限后（如三个月可退款），支付宝系统发送该交易状态通知
                        //请务必判断请求时的total_fee、seller_id与通知时获取的total_fee、seller_id为一致的
                        logi.Debug("success");
                        Response.Write("success");  //请不要修改或删除
                        return;
                    }
                    else if (Request.Form["trade_status"] == "TRADE_SUCCESS")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序

                        //注意：
                        //付款完成后，支付宝系统发送该交易状态通知
                        //请务必判断请求时的total_fee、seller_id与通知时获取的total_fee、seller_id为一致的
                        logi.Debug("success");
                        Response.Write("success");  //请不要修改或删除
                        return;
                    }
                    else if (Request.Form["trade_status"] == "WAIT_BUYER_PAY")
                    {
                        logi.Debug("success");
                        Response.Write("success");  //请不要修改或删除
                        return;
                    }

                    //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——
                    logi.Debug("fail");
                    Response.Write("fail");
                    return;

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                else//验证失败
                {
                    logi.Debug("fail");
                    Response.Write("fail");
                    return;
                }
            }
            else
            {
                logi.Debug("parames is null");
                Response.Write("无通知参数");
                return;
            }
        }

        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }

            return sArray;
        }
    }
}