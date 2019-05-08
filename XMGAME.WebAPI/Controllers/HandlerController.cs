using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using XMGAME.BLL;
using XMGAME.WebAPI.Models;
using XMGAME.Model;
using System.Configuration;

namespace XMGAME.WebAPI.Controllers
{
    public class HandlerController : ApiController
    {


        public UserBLL userBLL = new UserBLL();

        [Route("test")]
        public string test() {

            return "test";
        }

        /// <summary>
        /// 登入方法
        /// </summary>
        /// <param name="parsItem"></param>
         [HttpPost]
        [Route("login")]
        public result_base Login( ParsItem parsItem) {
           
            result_base objResult = new result_base();
           List<string> objParam=parsItem.paras;
           User user =userBLL.Login(objParam[1],objParam[0]);
           if (user == null) {
              bool isSucess=userBLL.Register(new User() {
                    AccountName = objParam[1],
                    UserPassWord = objParam[0]
                });
              user = userBLL.Login(objParam[1],objParam[0]);

           }
            if (user.Token != "") {
                objResult.errorCode = "200";
                objResult.result = string.Format("{0}?tk={1}&mb={2}&cl={3}", ConfigurationManager.AppSettings["LoginUrl"], user.Token, objParam[2], "zh");
            }
            return objResult;
        }




    }
}