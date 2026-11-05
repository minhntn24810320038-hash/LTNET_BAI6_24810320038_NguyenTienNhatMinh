using System;
using System.Data;
using Microsoft.Data.SqlClient;
using LTNET_BAI6.DTO;

namespace LTNET_BAI6.DAL
{
    public class HRMDAL
    {
        private SqlDataAdapter adapter;

        public DataTable GetBangChamCong(string thangNam)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseHelper.GetHRMConnection())
            {
                string sql = @"SELECT c.MaCong, n.MaNV, n.HoTen, 
                                      ISNULL(c.ThangNam, @ThangNam) AS ThangNam, 
                                      ISNULL(c.SoNgayCong, 0) AS SoNgayCong, 
                                      ISNULL(c.SoGioOT, 0) AS SoGioOT
                               FROM NhanVien n
                               LEFT JOIN BangChamCong c ON n.MaNV = c.MaNV AND c.ThangNam = @ThangNam";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ThangNam", thangNam);
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }

        public bool LuuBangChamCong(DataTable dtChamCong)
        {
            using (SqlConnection conn = DatabaseHelper.GetHRMConnection())
            {
                string selectQuery = "SELECT MaCong, MaNV, ThangNam, SoNgayCong, SoGioOT FROM BangChamCong";
                adapter = new SqlDataAdapter(selectQuery, conn);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                conn.Open();
                adapter.Update(dtChamCong);
                return true;
            }
        }

        public bool ChotLuongThang(string thangNam, out string loi)
        {
            loi = string.Empty;
            using (SqlConnection conn = DatabaseHelper.GetHRMConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    string sqlGetCC = @"SELECT c.MaNV, n.LuongCoBan, n.HeSoLuong, c.SoNgayCong, c.SoGioOT 
                                        FROM BangChamCong c
                                        JOIN NhanVien n ON c.MaNV = n.MaNV
                                        WHERE c.ThangNam = @ThangNam";
                    SqlCommand cmdCC = new SqlCommand(sqlGetCC, conn, trans);
                    cmdCC.Parameters.AddWithValue("@ThangNam", thangNam);
                    SqlDataReader reader = cmdCC.ExecuteReader();

                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    reader.Close();

                    foreach (DataRow row in dt.Rows)
                    {
                        int maNV = Convert.ToInt32(row["MaNV"]);
                        decimal luongCB = Convert.ToDecimal(row["LuongCoBan"]);
                        double heSo = Convert.ToDouble(row["HeSoLuong"]);
                        double ngayCong = Convert.ToDouble(row["SoNgayCong"]);
                        double gioOT = Convert.ToDouble(row["SoGioOT"]);

                        decimal thucLinh = (decimal)((double)luongCB * heSo / 22.0) * (decimal)ngayCong + (decimal)(gioOT * 50000.0);

                        string sqlInsertLuong = @"IF EXISTS (SELECT 1 FROM BangLuong WHERE MaNV = @MaNV AND ThangNam = @ThangNam)
                                                    UPDATE BangLuong SET ThucLinh = @ThucLinh WHERE MaNV = @MaNV AND ThangNam = @ThangNam
                                                  ELSE
                                                    INSERT INTO BangLuong (MaNV, ThangNam, ThucLinh) VALUES (@MaNV, @ThangNam, @ThucLinh)";
                        SqlCommand cmdL = new SqlCommand(sqlInsertLuong, conn, trans);
                        cmdL.Parameters.AddWithValue("@MaNV", maNV);
                        cmdL.Parameters.AddWithValue("@ThangNam", thangNam);
                        cmdL.Parameters.AddWithValue("@ThucLinh", thucLinh);
                        cmdL.ExecuteNonQuery();
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
    }
}