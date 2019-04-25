using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Model;


namespace XMGAME.IDAL
{
    public interface IUserDAL:IBaseDAL<User>
    {
       
        bool UpdateOrAddToken(User user);

        User GetUserByToken(string token);

    }
}
