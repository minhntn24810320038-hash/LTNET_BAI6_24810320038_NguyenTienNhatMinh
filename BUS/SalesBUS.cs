using System;
using System.Collections.Generic;
using System.Data;
using LTNET_BAI6.DAL;
using LTNET_BAI6.DTO;

namespace LTNET_BAI6.BUS
{
    public class SalesBUS
    {
        private SalesDAL salesDAL = new SalesDAL();

        public DataTable TimKiemSanPham(string tuKhoa)
        {
            return salesDAL.TimKiemSanPham(tuKhoa);
        }

        public bool ThanhToan(int maNV, int? maKH, decimal tongTien, List<ChiTietHoaDonDTO> dsChiTiet, out string loi)
        {
            if (dsChiTiet == null || dsChiTiet.Count == 0)
            {
                loi = "Giỏ hàng rỗng, không thể thanh toán.";
                return false;
            }
            return salesDAL.ThanhToanHoaDon(maNV, maKH, tongTien, dsChiTiet, out loi);
        }

        public decimal TinhDoanhThu(DateTime tuNgay, DateTime denNgay)
        {
            return salesDAL.TinhTongDoanhThu(tuNgay, denNgay);
        }

        public DataTable GetTop5SanPham()
        {
            return salesDAL.GetTop5SanPhamBanChay();
        }
    }
}