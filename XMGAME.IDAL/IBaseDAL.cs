using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.IDAL
{
   public interface IBaseDAL<T>
   {

        #region CRUD
        /// <summary>
        /// 增加一条记录
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        bool Insert(T entity);

        /// <summary>
        /// 增加多条记录
        /// </summary>
        /// <param name="entitys">实体集合</param>
        /// <returns></returns>
        bool InsertMore(List<T> entitys);

        /// <summary>
        /// 修改一条记录
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        bool Update(T entity);

        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        bool Delete(T entity);

        /// <summary>
        /// 得到所有记录
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// 根据ID 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetEntityByID(int id);

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        IQueryable<T> GetByWhere(T entity, Dictionary<string, string> fieldRelation, string relation, bool isPage = false, int pageNum = 1, int pageSize = 10);

        /// <summary>
        /// 使用sql语句查询
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        IQueryable<T> QueryBySql<T>(string sql,object[] param);

        #endregion

    }
}
