using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Comm;
using XMGAME.DAL;
using XMGAME.IDAL;
using XMGAME.Model;

namespace XMGAME.BLL
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-5
    /// 修改时间：
    /// 功能：游戏记录逻辑处理类
    /// </summary>
    public class RecordBLL
    {
        #region 私有变量
        /// <summary>
        /// 游戏记录数据访问对象
        /// </summary>
        private IRecordDAL recordDAL = new RecordDAL();

        /// <summary>
        /// 用户信息数据访问对象
        /// </summary>
        private IUserDAL userDAL = new UserDAL();

        #endregion


        #region CRUD

        /// <summary>
        /// 根据用户令牌得到记录
        /// </summary>
        /// <param name="accountName">用户令牌</param>
        /// <returns></returns>
     //   [Redis("recordInfo",ArgumentName ="accountName")]
        public IQueryable<Record> GetRecordInfo(string accountName) 
        {
            Record record = new Record();
            record.AccountName = userDAL.GetUserByToken(accountName).AccountName;
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("AccountName", "==");         
            return recordDAL.GetByWhere(record, pairs, "").OrderByDescending(t=>t.ID);
       
        }



        /// <summary>
        /// 添加一条记录
        /// 错误状态码：108 ：添加失败
        /// </summary>
        /// <param name="record">记录信息实体类</param>
        /// <returns></returns>
        
     //   [Redis("recordInfo",ArgumentName ="accountName",IsDelete =true)]
        [ErroAttribute(Rule = new object[] { 108, null })] 
        public Record AddRecord(Record record) {
            string strToken = record.AccountName;
            record.AccountName = userDAL.GetUserByToken(record.AccountName).AccountName;
            bool isSuccess = recordDAL.Insert(record);
            if (isSuccess)
            {

                return GetRecordByUserAndRoom(strToken, record.RoomID);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 添加一条记录不返回添加的信息（主要是ID）
        /// </summary>
        /// <param name="record">记录信息</param>
        /// <returns></returns>

      //  [Redis("recordInfo", ArgumentName = "accountName", IsDelete = true)]
        public bool AddRecordNotReturn(Record record) {
            string strToken = record.AccountName;
            record.AccountName = userDAL.GetUserByToken(record.AccountName).AccountName;
            bool isSuccess = recordDAL.Insert(record);
            User userEdit = userDAL.GetUserByToken(strToken);
            User user = new User()
            {
                ID = userEdit.ID,
                AccountName = userEdit.AccountName,
                Integral = record.Integral
            };
            userDAL.Update(user);
            return isSuccess;
        }

        /// <summary>
        /// 修改一条记录
        /// 错误状态码： 109 ：修改失败
        /// </summary>
        /// <param name="record">记录信息</param>
        /// <returns></returns>
        [ErroAttribute(Rule = new object[] { 109,false })]
        public bool UpdateRecord(Record record) {
            User userEdit= userDAL.GetUserByToken(record.AccountName);
            User user = new User() {
                ID=userEdit.ID,
                AccountName= userEdit.AccountName,
                Integral=record.Integral
            };
            userDAL.Update(user);
            record.AccountName = userEdit.AccountName;
            return recordDAL.Update(record);
        }


        /// <summary>
        /// 根据房间ID得到记录
        /// </summary>
        /// <param name="roomID">房间ID</param>
        /// <returns></returns>
    //    [Redis("recordInfo", ArgumentName = "accountName", IsDelete = true)]
        public IQueryable<Record> GetRecords(string roomID) {
            Record record = new Record();
            record.RoomID = roomID;
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("RoomID", "==");
            IQueryable<Record> records= recordDAL.GetByWhere(record, pairs,"").OrderByDescending(t=>t.Integral);
            return records;
        }

        /// <summary>
        /// 根据用户令牌和房间ID得到记录信息
        /// </summary>
        /// <param name="accountName">用户令牌</param>
        /// <param name="roomID">房间ID</param>
        /// <returns></returns>
        public Record GetRecordByUserAndRoom(string accountName, string roomID) {

            Record record = new Record();
            record.AccountName = userDAL.GetUserByToken(accountName).AccountName;
            record.RoomID = roomID;
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("AccountName", "==");
            pairs.Add("RoomID","==");
            return recordDAL.GetByWhere(record, pairs," and ").FirstOrDefault();
        
        }

        /// <summary>
        /// 根据用户令牌得到用户玩过的游戏
        /// </summary>
        /// <param name="accountName">用户令牌</param>
        /// <returns></returns>
     
        public IQueryable<object> GetGameByAccountRecord(string accountName) {
              string userName= userDAL.GetUserByToken(accountName).AccountName;
            return recordDAL.GetGameIDsByAccountRecord(userName);
        }


        /// <summary>
        /// 根据房间ID 查询用户得分与正确答题数
        /// </summary>
        /// <param name="roomID">房间ID</param>
        /// <returns></returns>
        public IQueryable<object> GetRecordRightCount(string roomID) {       
           IQueryable<object> rightCount= recordDAL.GetRightCount(roomID);
            return rightCount;
        }

        #endregion
    }
}
