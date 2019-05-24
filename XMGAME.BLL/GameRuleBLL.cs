using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.DAL;
using XMGAME.IDAL;
using XMGAME.Model;

namespace XMGAME.BLL
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-21
    /// 修改时间：
    /// 功能：游戏规则逻辑处理类
    /// </summary>
    public class GameRuleBLL
    {

        private IGameRuleDAL mobjGameRuleDAL = new GameRuleDAL();

        /// <summary>
        /// 添加条记录
        /// </summary>
        /// <param name="aAddGameRule">游戏规则实体类</param>
        /// <returns></returns>
        public bool InsertGameRule(GameRuleEntity aAddGameRule) {

            return mobjGameRuleDAL.Insert(aAddGameRule);

        }

        /// <summary>
        /// 修改一条记录
        /// </summary>
        /// <param name="aEditGameRule">游戏规则实体类</param>
        /// <returns></returns>
        public bool UpdateGameRule(GameRuleEntity aEditGameRule) {
            return mobjGameRuleDAL.Update(aEditGameRule);
        }

        /// <summary>
        ///删除一条记录
        /// </summary>
        /// <param name="aintId">游戏规则ID</param>
        /// <returns></returns>
        public bool DeleteGameRule(int aintId) {
            GameRuleEntity objDeleteGmaeRule = mobjGameRuleDAL.GetEntityByID(aintId);
            return mobjGameRuleDAL.Delete(objDeleteGmaeRule);
        }

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public IQueryable<object> GetALLByFill()
        {
            return mobjGameRuleDAL.GetAllByFill();
        }

        public GameRuleEntity GetByID(int id) {
            return mobjGameRuleDAL.GetEntityByID(id);
        }

        public GameRuleEntity GetByGameID(int gameID) {

            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("GameID", "==");
            GameRuleEntity gameRuleEntity = new GameRuleEntity() {
                GameID = gameID
             };

          return  mobjGameRuleDAL.GetByWhere(gameRuleEntity,pairs,"").FirstOrDefault();

        }
    }
}
