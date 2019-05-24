using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        /// <summary>
        /// 答题记录数据访问对象
        /// </summary>
        private IRecordQuestionDAL recordQuestionDAL = new RecordQuestionDAL();

        /// <summary>
        /// 活动信息逻辑处理类
        /// </summary>
        private ActityBLL mobjActityBLL = new ActityBLL();

        /// <summary>
        /// 活动参加记录处理类
        /// </summary>
        private ActToRecordBLL mobjActTRBLL = new ActToRecordBLL();

        /// <summary>
        /// 奖励记录信息逻辑处理类
        /// </summary>
        private ActRecordBLL mobjActRecord = new ActRecordBLL();

        /// <summary>
        /// 用户信息逻辑处理类
        /// </summary>
        private UserBLL mobjUserBLL = new UserBLL();
        #endregion


        #region CRUD


        /// <summary>
        /// 查询所有记录
        /// </summary>
        /// <returns></returns>
        public IQueryable<Record> GetALL() {
            return recordDAL.GetAll();
        }


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
            return recordDAL.GetByWhere(record, pairs, "").OrderByDescending(t => t.ID);

        }



        /// <summary>
        /// 添加一条记录
        /// 错误状态码：108 ：添加失败
        /// </summary>
        /// <param name="record">记录信息实体类</param>
        /// <returns></returns>
        //   [Redis("recordInfo",ArgumentName ="accountName",IsDelete =true)]
        [ErroAttribute(Rule = new object[] { "ch108", null })]
        public Record AddRecord(Record record)
        {
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
        /// 把记录信息添加到Redis
        /// </summary>
        /// <param name="record">记录信息实体</param>
        public void AddRecordToRedis(Record record)
        {
            string strKey = record.AccountName + "::GameRecord";
            RedisHelper.SetData<Record>(strKey, record);
        }

        /// <summary>
        /// 添加一条记录不返回添加的信息（主要是ID）
        /// </summary>
        /// <param name="record">记录信息</param>
        /// <returns></returns>
        //  [Redis("recordInfo", ArgumentName = "accountName", IsDelete = true)]
        public bool AddRecordNotReturn(Record record)
        {
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
            if(record.Integral>0)
            PutActivity(userEdit.AccountName,record.Integral,record.GameID.Value);

            return isSuccess;
        }

        /// <summary>
        /// 修改一条记录
        /// 错误状态码： 109 ：修改失败
        /// </summary>
        /// <param name="record">记录信息</param>
        /// <returns></returns>
        [ErroAttribute(Rule = new object[] { "ch109", false })]
        public bool UpdateRecord(Record record)
        {
            User userEdit = userDAL.GetUserByToken(record.AccountName);
            User user = new User()
            {
                ID = userEdit.ID,
                AccountName = userEdit.AccountName,
                Integral = record.Integral
            };
            userDAL.Update(user);
            record.AccountName = userEdit.AccountName;
            PutActivity(userEdit.AccountName, record.Integral,record.GameID.Value);
            return recordDAL.Update(record);
        }

        /// <summary>
        /// 修改游戏记录（新增）
        /// 从redis中拿出游戏记录并计算分数然后插入
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public bool UpdateRecordToRedis(Record record)
        {
            string strKey = record.AccountName + "::GameRecord";
            string strRQKey = record.AccountName + "::RecordQuestion";
            string strSumKey = record.AccountName + "::RecordQuestionSum";
            Record objRedisData = RedisHelper.GetDataProtogenesis<Record>(strKey);
            List<RecordQuestion> recordQuestions = RedisHelper.GetDateByList<RecordQuestion>(strRQKey);
            int intGoalSum = RedisHelper.GetData<int>(strSumKey);
            string strToken = objRedisData.AccountName;
            objRedisData.Integral = intGoalSum;
            objRedisData.AccountName = userDAL.GetUserByToken(objRedisData.AccountName).AccountName;
            bool isSuccess = recordDAL.Insert(objRedisData);
            if (isSuccess)
            {
                Record recordNew = GetRecordByUserAndRoom(strToken, objRedisData.RoomID);
                User userEdit = userDAL.GetUserByToken(record.AccountName);
                User user = new User()
                {
                    ID = userEdit.ID,
                    AccountName = userEdit.AccountName,
                    Integral = recordNew.Integral
                };
                userDAL.Update(user);
                GiveIDToRecordQuestion(ref recordQuestions, objRedisData);
                RedisHelper.DeleteKey(strKey);
                RedisHelper.DeleteKey(strRQKey);
                RedisHelper.DeleteKey(strSumKey);
                PutActivity(userEdit.AccountName, recordNew.Integral, recordNew.GameID.Value);
                return true;

            }
            else
            {
                return false;
            }
        }


  
        /// <summary>
        /// 新线程：
        /// 判断用户是否达到活动需求并给与相应积分
        /// </summary>
        /// <param name="account">用户名</param>
        /// <param name="integral">积分</param>
        /// <param name="intGameID">游戏ID</param>
        private void PutActivity(string account,int integral,int intGameID)
        {
            Task.Factory.StartNew(() => {
               
                ExcetuActity(account,integral,intGameID);
            });        
        }

        private void InsertDb(string account,int integral,int intGameID)
        {
            Dictionary<int, ActivityEntity> aobjActivities = mobjActityBLL.GetByTime().ToDictionary(t => t.ID);
            List<ActToRecordEntity> aobjActTr = mobjActTRBLL.GetByAccount(account);
            List<ActToRecordEntity> objLists = new List<ActToRecordEntity>();
            foreach (var item in aobjActTr)
            {
                if (item.GameID!=0&&intGameID != item.GameID) {
                    continue;
                }
                ActToRecordEntity objActTo = item;
      
                int intAward = 0;
                if (objActTo.ID==null||objActTo.ID.Value==0)
                {           
                    objActTo.AccountName = account;
                    objActTo.PresentNow = integral;
                    objActTo.PresentCount = 0;
                    insertAct(ref objActTo,ref intAward,integral, aobjActivities[objActTo.ActID.Value].Award);                               
                    objLists.Add(objActTo);
                    
                }
                else {
                    UpdateAct(ref objActTo,integral);           
                    intAward = Math.Max(objActTo.PresentCount.Value -item.PresentCount.Value,1)* aobjActivities[objActTo.ActID.Value].Award;   
              
                }
                //if (intAward > 0) {
                //    InsertActRecord(intAward,item.ActID.Value,account);
                //}
                
                
            }
            mobjActTRBLL.InsertMore(objLists);
        }

        /// <summary>
        ///  判断用户是否达到活动需求并给与相应积分的具体操作
        /// </summary>
        /// <param name="account">用户名</param>
        /// <param name="integral">积分</param>
        /// <param name="intGameID">游戏ID</param>
        private void ExcetuActity(string account, int integral, int intGameID) {
            Dictionary<int, ActivityEntity> aobjActivities = mobjActityBLL.GetByTime().ToDictionary(t => t.ID);
            string redisKey = account + ".ActToRecord::" + intGameID;
            Dictionary<int, ActToRecordEntity> objValue = new Dictionary<int, ActToRecordEntity>();
            if (RedisHelper.ContainsKey(redisKey))
            {
                objValue = RedisHelper.GetDataProtogenesis<Dictionary<int, ActToRecordEntity>>(redisKey);
                RedisHelper.DeleteKey(redisKey);
            }

            foreach (var item in aobjActivities)
            {
                if (item.Value.GameID != 0 && intGameID != item.Value.GameID) {
                    continue;
                }
                    int intAward = 0;
                ActToRecordEntity objActTo = null;
                if (objValue.ContainsKey(item.Key))
                {
                    objActTo = objValue[item.Key];
                    if (objActTo.PresentCount == objActTo.Total)
                    {
                        objValue.Remove(item.Key);
                    }
                    else {
                        UpdateAct(ref objActTo, integral);
                        intAward = Math.Max(objActTo.PresentCount.Value - objValue[item.Key].PresentCount.Value, 1) * item.Value.Award;
                        objValue[item.Key] = objActTo;
                    }                                    
                }
                else {
                    objActTo = new ActToRecordEntity();
                    objActTo.AccountName = account;
                    objActTo.ActID = item.Value.ID;
                    objActTo.GameID = item.Value.GameID;
                    objActTo.PresentCount = 0;
                    objActTo.PresentNow = 0;
                    objActTo.Total = item.Value.Total;
                    objActTo.ActTarget = item.Value.AcRule;
                    insertAct(ref objActTo,ref intAward,integral,item.Value.Award);
                    objValue.Add(objActTo.ActID.Value,objActTo);
                }

                if (intAward > 0) {
                    InsertActRecord(intAward,item.Key,account,item.Value.Award);
                }
            }

            RedisHelper.SetData<Dictionary<int, ActToRecordEntity>>(redisKey,objValue,-1);
            


         
        }

        /// <summary>
        /// 用户参加新活动的操作
        /// </summary>
        /// <param name="objActTo">用户参加活动记录实体</param>
        /// <param name="intAward">得到总奖励</param>
        /// <param name="integral">积分</param>
        /// <param name="award">达到要求的奖励</param>
        private void insertAct(ref ActToRecordEntity objActTo,ref int intAward,int integral,int award) {
            if (integral > objActTo.ActTarget)
            {
                int divisor = integral / objActTo.ActTarget.Value == 0 ? 1 : integral / objActTo.ActTarget.Value;
                int remainder = integral % objActTo.ActTarget.Value;
                objActTo.PresentNow = remainder;
                if (objActTo.Total == 0)
                {
                    objActTo.PresentCount = divisor > objActTo.Total ? divisor : objActTo.Total;
                }
                else
                {
                    objActTo.PresentCount = divisor > objActTo.Total ? objActTo.Total : divisor;
                }

                intAward = objActTo.PresentCount.Value * award;
            }

        }

        /// <summary>
        /// 用户已经有参加活动记录的操作
        /// </summary>
        /// <param name="objActTo">用户参加活动记录实体</param>
        /// <param name="integral">积分</param>
        private void UpdateAct(ref ActToRecordEntity objActTo, int integral) {
            int intSum = integral + objActTo.PresentNow.Value;
            int divisor = intSum / objActTo.ActTarget.Value == 0 ? 1 : intSum / objActTo.ActTarget.Value;
            int remainder = intSum % objActTo.ActTarget.Value;
            objActTo.PresentNow = remainder;
            if (objActTo.Total == 0)
            {
                int intCount = divisor + objActTo.PresentCount.Value;
                objActTo.PresentCount = intCount > objActTo.Total ? intCount : objActTo.Total;

            }
            else
            {
                objActTo.PresentCount = divisor + objActTo.PresentCount.Value > objActTo.Total ? objActTo.Total : divisor + objActTo.PresentCount.Value;
            }
         
        }

        /// <summary>
        /// 达到奖励条件插入领奖记录
        /// </summary>
        /// <param name="aintAward">得到总奖励</param>
        /// <param name="aintActID">活动ID</param>
        /// <param name="astrAccount">用户名</param>
        /// <param name="aintActAward">达到条件的奖励</param>
        private void InsertActRecord(int aintAward,int aintActID,string astrAccount,int aintActAward) {
            List<ActRecordEntity> objActRecords = new List<ActRecordEntity>();
            for (int i = 0; i < aintActAward/aintActAward; i++)
            {
                ActRecordEntity actRecordEntity = new ActRecordEntity()
                {
                    Award = aintActAward,
                    ActID = aintActID
                };
                objActRecords.Add(actRecordEntity);
            }
            mobjActRecord.InsertMore(objActRecords);
            User user = mobjUserBLL.GetUserByAccountName(astrAccount);
            user.Integral = aintAward;
            userDAL.Update(user);
        }

        /// <summary>
        /// 把记录编号赋值给答题记录
        /// </summary>
        /// <param name="recordQuestions">答题记录信息集合</param>
        /// <param name="record">游戏记录</param>
        private void GiveIDToRecordQuestion(ref List<RecordQuestion> recordQuestions, Record record)
        {
            foreach (var item in recordQuestions)
            {
                item.RecordID = record.ID;
            }

            recordQuestionDAL.InsertMore(recordQuestions);

        }

        /// <summary>
        /// 根据房间ID得到记录
        /// </summary>
        /// <param name="roomID">房间ID</param>
        /// <returns></returns>
        //    [Redis("recordInfo", ArgumentName = "accountName", IsDelete = true)]
        public IQueryable<Record> GetRecords(string roomID)
        {
            Record record = new Record();
            record.RoomID = roomID;
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("RoomID", "==");
            IQueryable<Record> records = recordDAL.GetByWhere(record, pairs, "").OrderByDescending(t => t.Integral);
            return records;
        }

        /// <summary>
        /// 根据用户令牌和房间ID得到记录信息
        /// </summary>
        /// <param name="accountName">用户令牌</param>
        /// <param name="roomID">房间ID</param>
        /// <returns></returns>
        public Record GetRecordByUserAndRoom(string accountName, string roomID)
        {

            Record record = new Record();
            record.AccountName = userDAL.GetUserByToken(accountName).AccountName;
            record.RoomID = roomID;
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("AccountName", "==");
            pairs.Add("RoomID", "==");
            return recordDAL.GetByWhere(record, pairs, " and ").FirstOrDefault();

        }

        /// <summary>
        /// 根据用户令牌得到用户玩过的游戏
        /// </summary>
        /// <param name="accountName">用户令牌</param>
        /// <returns></returns>

        public IQueryable<object> GetGameByAccountRecord(string accountName)
        {
            string userName = userDAL.GetUserByToken(accountName).AccountName;
            return recordDAL.GetGameIDsByAccountRecord(userName);
        }

        /// <summary>
        /// 根据用户名得到用户玩过的游戏
        /// </summary>
        /// <param name="accountName">用户令牌</param>
        /// <returns></returns>

        public IQueryable<object> GetGameByAccountNameRecord(string accountName)
        {
            return recordDAL.GetGameIDsByAccountRecord(accountName);
        }

        /// <summary>
        /// 根据房间ID 查询用户得分与正确答题数
        /// </summary>
        /// <param name="roomID">房间ID</param>
        /// <returns></returns>
        public IQueryable<object> GetRecordRightCount(string roomID)
        {
            IQueryable<object> rightCount = recordDAL.GetRightCount(roomID);
            return rightCount;
        }

        public IQueryable<Record> GetRecordBySql(string sql, object[] param)
        {
            return recordDAL.QueryBySql<Record>(sql, param);
        }

        public IQueryable<object> GetRecordCollect(string account, DateTime? createTime, DateTime? endTime)
        {


            return recordDAL.GetRecordCollect(account, createTime, endTime);


        }

        public IQueryable<object> GetRecordCollectByAgency(string agent, DateTime? createTime, DateTime? endTime, int gameID) {
            return recordDAL.GetRecordCollectByAgency(agent,createTime,endTime,gameID);
        }

       /// <summary>
       /// 根据ID删除记录
       /// </summary>
       /// <param name="id">记录编号</param>
       /// <returns></returns>
        public bool DeleteRecord(int id) {
            Record objDelete = recordDAL.GetEntityByID(id);        
            bool isSucess= recordDAL.Delete(objDelete);
            if (isSucess)
            {
                User objEditUser = mobjUserBLL.GetUserByAccountName(objDelete.AccountName);
                objEditUser.Integral = objDelete.Integral;
                userDAL.Update(objEditUser);
            }
            return isSucess;
        }
        #endregion
    }
}
