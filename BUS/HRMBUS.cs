using System.Data;
using LTNET_BAI6.DAL;

namespace LTNET_BAI6.BUS
{
    public class HRMBUS
    {
        private HRMDAL hrmDAL = new HRMDAL();

        public DataTable GetBangChamCong(string thangNam)
        {
            return hrmDAL.GetBangChamCong(thangNam);
        }

        public bool LuuBangChamCong(DataTable dtChamCong)
        {
            return hrmDAL.LuuBangChamCong(dtChamCong);
        }

        public bool ChotLuongThang(string thangNam, out string loi)
        {
            return hrmDAL.ChotLuongThang(thangNam, out loi);
        }
    }
}