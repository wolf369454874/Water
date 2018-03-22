using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Water.Util;
using Water.Dal;

namespace Water.Bussiness
{
    class UserBussiness
    {
        [Water.Bussiness.Aop.AopMethodConfig(ReturnExceptionDefaultValue = true, ExceptionDefaultValue = new object[] { Status.Error, "Error_DataAccessError" })]
        public StatusResult GetUserList()
        {
            StatusResult result = new StatusResult();
            Water.Dal.UserDao userDao = new UserDao();

            var userList = Water.Util.ModelConvertHelper<Water.Bussiness.UserModel>.ConvertToModel(userDao.getUserList());
            return new StatusResult() { UserData = userList };
        }

    }
}
