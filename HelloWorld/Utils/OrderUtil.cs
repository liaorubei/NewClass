using Com.Alipay;
using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace StudyOnline.Utils
{
    public class OrderUtil
    {
        public static String UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return HttpUtility.UrlEncode(str, Encoding.UTF8);

            //  return (sb.ToString());
        }

        //create the order info. 创建订单信息
        public static String getOrderInfo(Orders order)
        {
            String notify_url = "http://voc2015.azurewebsites.net/Api/Payment/AlipayNotify";//http://voc2015.azurewebsites.net/notify_url.aspx

            // 签约合作者身份ID,即商户PID
            String orderInfo = "partner=" + "\"" + Config.Partner + "\"";

            // 签约卖家支付宝账号,即商户收款账号
            orderInfo += "&seller_id=" + "\"" + Config.SellerId + "\"";

            // 商户网站唯一订单号
            orderInfo += "&out_trade_no=" + "\"" + order.Id + "\"";

            // 商品名称
            orderInfo += "&subject=" + "\"" + order.Main + "\"";

            // 商品详情
            orderInfo += "&body=" + "\"" + order.Body + "\"";

            // 商品金额
            orderInfo += "&total_fee=" + "\"" + order.Amount + "\"";

            // 服务器异步通知页面路径
            orderInfo += "&notify_url=" + "\"" + notify_url + "\"";

            // 服务接口名称， 固定值
            orderInfo += "&service=\"mobile.securitypay.pay\"";

            // 支付类型， 固定值
            orderInfo += "&payment_type=\"1\"";

            // 参数编码， 固定值
            orderInfo += "&_input_charset=\"utf-8\"";

            // 设置未付款交易的超时时间
            // 默认30分钟，一旦超时，该笔交易就会自动被关闭。
            // 取值范围：1m～15d。
            // m-分钟，h-小时，d-天，1c-当天（无论交易何时创建，都在0点关闭）。
            // 该参数数值不接受小数点，如1.5h，可转换为90m。
            orderInfo += "&it_b_pay=\"30m\"";

            // extern_token为经过快登授权获取到的alipay_open_id,带上此参数用户将使用授权的账户进行支付
            // orderInfo += "&extern_token=" + "\"" + extern_token + "\"";

            // 支付宝处理完请求后，当前页面跳转到商户指定页面的路径，可空
            orderInfo += "&return_url=\"m.alipay.com\"";

            // 调用银行卡支付，需配置此参数，参与签名， 固定值 （需要签约《无线银行卡快捷支付》才能使用）
            // orderInfo += "&paymethod=\"expressGateway\"";

            return orderInfo;
        }
    }
}