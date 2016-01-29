<ChineseChat>项目,总共两个前台页面和其它的后台页面,前台页面包括主页和文章详情页
NAudio为Mp3播放长度类库

自己的帐号体系与云信帐号体系的对应(简称云信帐号,应用帐号[表Customer]);

(云信帐号要求)
参数	类型	必须	说明
accid	String	是	    云信ID，最大长度32字节，必须保证一个APP内唯一（只允许字母、数字、半角下划线_、@、半角点以及半角-组成，不区分大小写，会统一小写处理）
name	String	否	    云信ID昵称，最大长度64字节，用来PUSH推送时显示的昵称
props	String	否	    json属性，第三方可选填，最大长度1024字节
icon	String	否	    云信ID头像URL，第三方可选填，最大长度1024
token	String	否	    云信ID可以指定登录token值，最大长度128字节，并更新，如果未指定，会自动生成token，并在创建成功后返回

云信帐号.accid=应用帐号.accid
云信帐号.name =应用帐号.NickName
云信帐号.props=应用帐号.props
云信帐号.icon =应用帐号.icon
云信帐号.token=md5(应用帐号.accid+AppKey)








状态码:为了避免与云信的状态码冲突,本地API的状态码从20000算起,以下为各状态码含义
20000 帐号已经存在

