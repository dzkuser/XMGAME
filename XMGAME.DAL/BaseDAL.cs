using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Comm;
using XMGAME.DATA;
using XMGAME.IDAL;
using XMGAME.Model;

namespace XMGAME.DAL
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-9
    /// 修改时间:
    /// 功能：增删查改父类
    /// </summary>
    public class BaseDAL<T> : IBaseDAL<T> where T : class, new()
    {

        #region 数据库上下文对象
        /// <summary>
        /// 数据库上下文对象
        /// </summary>
        private DbContext dbContext = new MyDbContext();

        #endregion

        #region CRUD
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public bool Delete(T entity)
        {
            dbContext.Set<T>().Remove(entity);
            return dbContext.SaveChanges()>0?true:false;
          
        }

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetAll()
        {
          
          return  dbContext.Set<T>();
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="entity">实体类信息</param>
        /// <param name="fieldRelation">字段规则 比如： ID = 或 ID > </param>
        /// <param name="relation">连接条件的运算符 不如 ： && ，|| </param>
        /// <returns></returns>
        public IQueryable<T> GetByWhere(T entity,Dictionary<string,string> fieldRelation,string relation="")
        {
            return dbContext.Set<T>().Where(DynamicQueryBulid(entity,fieldRelation,relation));
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="entity">实体类信息</param>
        /// <returns></returns>
        public bool Insert(T entity)
        {                     
            dbContext.Set<T>().Add(entity);
            return dbContext.SaveChanges()>0?true:false;
           
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity">实体类信息</param>
        /// <returns></returns>
        public bool Update(T entity)
        {
            T updateEntity = DynamicUpdate(entity);
            dbContext.Set<T>().Attach(updateEntity);
            dbContext.Entry<T>(updateEntity).State=EntityState.Modified;
            return dbContext.SaveChanges() > 0 ? true : false;           
        }

        /// <summary>
        /// 根据ID查询信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetEntityByID(int id)
        {
            return dbContext.Set<T>().Find(id);
        }

        /// <summary>
        /// 动态更新
        /// 把空的字段补上相对应的值
        /// </summary>
        /// <param name="entity">实体类信息</param>
        /// <returns></returns>
        private T DynamicUpdate(T entity)
        {
            Dictionary<string, object> field = GetProperty(entity);
            T re = dbContext.Set<T>().Find(field["ID"]);
            Type type = re.GetType();
            Dictionary<string, PropertyInfo> pairs= type.GetProperties().ToDictionary(t=>t.Name);
            foreach (var item in field)
            {

                if (item.Key == "Integral")
                {

                    pairs[item.Key].SetValue(re,(int)field[item.Key]+(int)pairs[item.Key].GetValue(re));
                }
                else {
                    if (pairs.ContainsKey(item.Key))
                        pairs[item.Key].SetValue(re,item.Value);
                }
               
            }
            return re;
        }

        /// <summary>
        /// 得到实体类属性
        /// </summary>
        /// <param name="entity">实体类信息</param>
        /// <returns></returns>
        private Dictionary<string, object> GetProperty(T entity) {          
            Type type= entity.GetType();
            PropertyInfo[] fieldInfo = type.GetProperties();
            Dictionary<string, object> fields = new Dictionary<string, object>();
            foreach (var item in fieldInfo)
            {
               object value= item.GetValue(entity);
                if (value != null) {
                    fields.Add(item.Name,value);
                }
            }
            return fields;
        }

        /// <summary>
        ///动态查询
        /// </summary>
        /// <param name="entity">实体类信息</param>
        /// <param name="fieldRelation">字段规则</param>
        /// <param name="relation">连接条件运算符</param>
        /// <returns></returns>
        private Expression<Func<T, bool>> DynamicQueryBulid(T entity, Dictionary<string, string> fieldRelation, string relation) {

            StringBuilder str = new StringBuilder();
            Dictionary<string, object> fields = GetProperty(entity);
            int i = 0;
            foreach (var item in fields.Keys)
            {
                if (fieldRelation.ContainsKey(item)) {
                    str.Append(item).Append(fieldRelation[item] + $"@{i}").Append(" ").Append(relation);                  
                }
                i++;
            }
            var lambdaStr = str.ToString();
            lambdaStr = lambdaStr.Substring(0, lambdaStr.Length - relation.Length);

            return DynamicExpressionParser.ParseLambda<T, bool>(new ParsingConfig(), false, lambdaStr, fields.Values.ToArray());


        }
        #endregion

    }
}
