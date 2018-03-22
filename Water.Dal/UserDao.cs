using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Water.Dal
{
    public class UserDao
    {
        public DataTable getUserList() {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id",typeof(int));
            dt.Columns.Add("UserSn", typeof(string));
            dt.Columns.Add("UserName", typeof(string));
            DataRow dr = dt.NewRow();
            dr[0] = 1;
            dr[1] = "10010001";
            dr[2] = "张文";
            dt.Rows.Add(dr);
            return dt;
        }
    }
}
