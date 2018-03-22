using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Water.Bussiness
{
    public class UserModel
    {
        private int _id;//用户ID
        private string _userSn;//用户编号
        private string _userName;//用户名称


        public int Id {
            get { return _id; }
            set { _id = value; }
        }

        public string UserSn
        {
            get { return _userSn; }
            set { _userSn = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
    }
}
