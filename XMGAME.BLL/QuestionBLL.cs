﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.DAL;
using XMGAME.IDAL;
using XMGAME.Model;

namespace XMGAME.BLL
{
    public class QuestionBLL
    {
        private IQuestionDAL questionDAL = new QuestionDAL();

        private IGenreDAL genreDAL = new GenreDAL();

        private RecordBLL recordBLL = new RecordBLL();

        public bool AddQuestion(QuestionEntity question) {

            return questionDAL.Insert(question);
        }

        public bool EditQuestion(QuestionEntity question) {
            return questionDAL.Update(question);
        }

        public bool DeleteQuestion(int id) {
            QuestionEntity question =GetQuestion(id);
            return  questionDAL.Delete(question);
        }

        public IQueryable<QuestionEntity> GetQuestions() {
            return AssembleQuestions(questionDAL.GetAll());
        }

        public QuestionEntity GetQuestion(int id) {
            QuestionEntity question = new QuestionEntity();
            question.ID = id;
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("ID","==");
            return AssembleQuestion(questionDAL.GetByWhere(question,pairs,"").FirstOrDefault());
        }

        private QuestionEntity AssembleQuestion(QuestionEntity questionEntity) {
            GenreEntity genreEntities = genreDAL.GetEntityByID(questionEntity.Genre);
            questionEntity.GenreName = genreEntities.GenreName;
            return questionEntity;
        }

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


        public QuestionEntity IsRight(QuestionEntity question)
        {
            return question;
        }

    }
}