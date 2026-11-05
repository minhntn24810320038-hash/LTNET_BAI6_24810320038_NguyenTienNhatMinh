using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using LTNET_BAI6.DTO;

namespace LTNET_BAI6.DAL
{
    public class SalesDAL
    {
        public DataTable TimKiemSanPham(string tuKhoa)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseHelper.GetSalesConnection())
            {
                string sql = "SELECT * FROM SanPham WHERE TenSP LIKE @TuKhoa OR MaSP LIKE @TuKhoa";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@TuKhoa", "%" + tuKhoa + "%");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool ThanhToanHoaDon(int maNV, int? maKH, decimal tongTien, List<ChiTietHoaDonDTO> dsChiTiet, out string loi)
        {
            loi = string.Empty;
            using (SqlConnection conn = DatabaseHelper.GetSalesConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    string sqlHD = @"INSERT INTO HoaDon (MaNV, MaKH, TongTien) 
                                     VALUES (@MaNV, @MaKH, @TongTien); 
                                     SELECT SCOPE_IDENTITY();";
                    SqlCommand cmdHD = new SqlCommand(sqlHD, conn, trans);
                    cmdHD.Parameters.AddWithValue("@MaNV", maNV);
                    cmdHD.Parameters.AddWithValue("@MaKH", (object)maKH ?? DBNull.Value);
                    cmdHD.Parameters.AddWithValue("@TongTien", tongTien);
                    int maHD = Convert.ToInt32(cmdHD.ExecuteScalar());

                    foreach (var item in dsChiTiet)
                    {
                        string sqlCheck = "SELECT SoLuongTon FROM SanPham WHERE MaSP = @MaSP";
                        SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn, trans);
                        cmdCheck.Parameters.AddWithValue("@MaSP", item.MaSP);
                        int tonKho = Convert.ToInt32(cmdCheck.ExecuteScalar());

                        if (tonKho < item.SoLuong)
                        {
                            trans.Rollback();
                            loi = $"Sản phẩm {item.MaSP} không đủ tồn kho (Tồn: {tonKho}, Bán: {item.SoLuong})";
                            return false;
                        }

                        string sqlCT = @"INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia) 
                                         VALUES (@MaHD, @MaSP, @SoLuong, @DonGia)";
                        SqlCommand cmdCT = new SqlCommand(sqlCT, conn, trans);
                        cmdCT.Parameters.AddWithValue("@MaHD", maHD);
                        cmdCT.Parameters.AddWithValue("@MaSP", item.MaSP);
                        cmdCT.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                        cmdCT.Parameters.AddWithValue("@DonGia", item.DonGia);
                        cmdCT.ExecuteNonQuery();

                        string sqlUpdateStock = @"UPDATE SanPham 
                                                 SET SoLuongTon = SoLuongTon - @SoLuong 
                                                 WHERE MaSP = @MaSP";
                        SqlCommand cmdStock = new SqlCommand(sqlUpdateStock, conn, trans);
                        cmdStock.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                        cmdStock.Parameters.AddWithValue("@MaSP", item.MaSP);
                        cmdStock.ExecuteNonQuery();
                    }

                    if (maKH.HasValue)
                    {
                        int diemCong = (int)(tongTien / 100000);
                        if (diemCong > 0)
                        {
                            string sqlPoint = @"UPDATE KhachHang 
                                                SET DiemTichLuy = DiemTichLuy + @Diem 
                                                WHERE MaKH = @MaKH";
                            SqlCommand cmdPoint = new SqlCommand(sqlPoint, conn, trans);
                            cmdPoint.Parameters.AddWithValue("@Diem", diemCong);
                            cmdPoint.Parameters.AddWithValue("@MaKH", maKH.Value);
                            cmdPoint.ExecuteNonQuery();
                        }
                    }

                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    loi = ex.Message;
                    return false;
                }
            }
        }

        public decimal TinhTongDoanhThu(DateTime tuNgay, DateTime denNgay)
        {
            using (SqlConnection conn = DatabaseHelper.GetSalesConnection())
            {
                string sql = "SELECT ISNULL(SUM(TongTien), 0) FROM HoaDon WHERE NgayLap BETWEEN @TuNgay AND @DenNgay";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                conn.Open();
                object result = cmd.ExecuteScalar();
                return Convert.ToDecimal(result);
            }
        }

        public DataTable GetTop5SanPhamBanChay()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseHelper.GetSalesConnection())
            {
                string sql = @"SELECT TOP 5 sp.MaSP, sp.TenSP, SUM(ct.SoLuong) AS TongDaBan
                               FROM ChiTietHoaDon ct
                               JOIN SanPham sp ON ct.MaSP = sp.MaSP
                               GROUP BY sp.MaSP, sp.TenSP
                               ORDER BY TongDaBan DESC";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();
            }
            return dt;
        }
    }
}