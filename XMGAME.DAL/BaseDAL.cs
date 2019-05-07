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

namespace XMGAME.DAL
{
    public class BaseDAL<T> : IBaseDAL<T> where T : class, new()
    {
    
        private DbContext dbContext = new MyDbContext();
        public bool Delete(T entity)
        {
            dbContext.Set<T>().Remove(entity);
            return dbContext.SaveChanges()>0?true:false;
          
        }

        public IQueryable<T> GetAll()
        {
          
          return  dbContext.Set<T>();
        }


        public IQueryable<T> GetByWhere(T entity,Dictionary<string,string> fieldRelation,string relation="")
        {
            return dbContext.Set<T>().Where(DynamicQueryBulid(entity,fieldRelation,relation));
        }

        public bool Insert(T entity)
        {
            dbContext.Set<T>().Add(entity);
            return dbContext.SaveChanges()>0?true:false;
        }

        public bool Update(T entity)
        {
            T updateEntity = DynamicUpdate(entity);
            dbContext.Set<T>().Attach(updateEntity);
            dbContext.Entry<T>(updateEntity).State=EntityState.Modified;
            return dbContext.SaveChanges() > 0 ? true : false;           
        }

        public T GetEntityByID(int id)
        {
            return dbContext.Set<T>().Find(id);
        }

    

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

     
    }
}
