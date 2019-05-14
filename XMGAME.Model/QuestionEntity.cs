using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Model
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-9
    /// 修改时间：
    /// 功能：
    /// </summary>
    public class QuestionEntity
    {
        public int ID { get; set; }
        public string Topic { get; set; }

        public string Answer { get; set; }

        public int Genre { get; set; }

        public int Score { get; set; }

        [NotMapped]
        public string GenreName { get; set; }

    }
}
