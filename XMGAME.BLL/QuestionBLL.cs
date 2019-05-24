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
    /// 创建时间:2019-5-10
    /// 修改时间：
    /// 功能：题目逻辑类
    /// </summary>
    public class QuestionBLL
    {
        #region 私有变量
        /// <summary>
        /// 题目信息的数据访问对象
        /// </summary>
        private IQuestionDAL questionDAL = new QuestionDAL();


        /// <summary>
        /// 题目类型的数据访问对象
        /// </summary>
        private IGenreDAL genreDAL = new GenreDAL();

        /// <summary>
        /// 游戏记录的数据访问对象
        /// </summary>
        private RecordBLL recordBLL = new RecordBLL();

        #endregion

        #region CRUD
        /// <summary>
        /// 添加题目
        /// </summary>
        /// <param name="question">题目信息实体类</param>
        /// <returns></returns>
        public bool AddQuestion(QuestionEntity question) {

            return questionDAL.Insert(question);
        }

        /// <summary>
        /// 修改题目
        /// </summary>
        /// <param name="question">题目信息</param>
        /// <returns></returns>
        public bool EditQuestion(QuestionEntity question) {
            return questionDAL.Update(question);
        }

        /// <summary>
        /// 删除题目
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <returns></returns>
        public bool DeleteQuestion(int id) {
            QuestionEntity question =GetQuestion(id);
            return  questionDAL.Delete(question);
        }

        /// <summary>
        /// 得到一场比赛的题目（现在是随机5题）
        /// 错误状态码：106 ：题库题目为空
        /// </summary>
        /// <returns></returns>
        [ErroAttribute(Rule = new object[] { "ch106", null })]
        public IQueryable<QuestionEntity> GetQuestions() {
           
              int max= GetAll().Count();
              int [] ids= getRadomNumber(1,max,5);
            return AssembleQuestions(questionDAL.GetByIDs(ids));
        }

        /// <summary>
        /// 得到所有题目
        /// 错误状态码：106 题库题目为空
        /// </summary>
        /// <returns></returns>
        [ErroAttribute(Rule = new object[] { "ch106", null })]
        public IQueryable<QuestionEntity> GetAll() {
            return AssembleQuestions(questionDAL.GetAll());
        }

        /// <summary>
        /// 根据ID 得到题目
        /// 错误状态码：107 ：没有该题目
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <returns></returns>
        [ErroAttribute(Rule = new object[] { "ch107", null })]
        public QuestionEntity GetQuestion(int id) {
            QuestionEntity question = new QuestionEntity();
            question.ID = id;
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("ID","==");
            return AssembleQuestion(questionDAL.GetByWhere(question,pairs,"").FirstOrDefault());
        }

        /// <summary>
        /// 给题目信息填充类型字段内容
        /// </summary>
        /// <param name="questionEntity">题目信息</param>
        /// <returns></returns>
        private QuestionEntity AssembleQuestion(QuestionEntity questionEntity) {
            GenreEntity genreEntities = genreDAL.GetEntityByID(questionEntity.Genre);
            questionEntity.GenreName = genreEntities.GenreName;
            return questionEntity;
        }

        /// <summary>
        /// 给题目信息填充类型字段内容(集合情况)
        /// </summary>
        /// <param name="questionEntity">题目信息</param>
        /// <returns></returns>
        private IQueryable<QuestionEntity> AssembleQuestions(IQueryable<QuestionEntity> questionEntitys)
        {
            IQueryable<GenreEntity> genres = genreDAL.GetAll();
            Dictionary<int,GenreEntity> pairs= genres.ToDictionary(s=>s.ID);
            foreach (var item in questionEntitys)
            {
                if (pairs.ContainsKey(item.Genre))
                    item.GenreName = pairs[item.Genre].GenreName;
            }
            return questionEntitys;
        }

        /// <summary>
        /// 得到不重复随机数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="count">总数</param>
        /// <returns></returns>
        private int[] getRadomNumber(int min,int max,int count) {
            Random random = new Random();
            List<int> numbers = new List<int>(count);
            while (numbers.Count()<count)
            {
                int number = random.Next(min,max+1);
                if (!numbers.Contains(number)) {
                    numbers.Add(number);
                }
            }
            return numbers.ToArray();
        }

        #endregion
    }
}
