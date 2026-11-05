using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using LTNET_BAI6.BUS;
using LTNET_BAI6.DTO;

namespace LTNET_BAI6.GUI
{
    public partial class frmSalesPOS : Form
    {
        private SalesBUS salesBUS = new SalesBUS();
        private List<ChiTietHoaDonDTO> gioHang = new List<ChiTietHoaDonDTO>();
        private decimal tongTien = 0;

        private DataGridView dgvSanPham = null!;
        private DataGridView dgvGioHang = null!;
        private TextBox txtTimKiem = null!;
        private TextBox txtMaKH = null!;
        private NumericUpDown nudSoLuong = null!;
        private Label lblTongTien = null!;
        private Label lblDiemTichLuy = null!;
        private Button btnTimKiem = null!;
        private Button btnThemGioHang = null!;
        private Button btnThanhToan = null!;

        public frmSalesPOS()
        {
            InitializeComponentCustom();
        }

        private void InitializeComponentCustom()
        {
            this.Size = new Size(1000, 600);
            this.Text = "Màn hình Bán hàng & Lập Hóa đơn";

            txtTimKiem = new TextBox { Location = new Point(20, 20), Width = 200 };
            btnTimKiem = new Button { Text = "Tìm kiếm", Location = new Point(230, 18) };
            btnTimKiem.Click += btnTimKiem_Click;

            dgvSanPham = new DataGridView { Location = new Point(20, 60), Size = new Size(450, 400) };
            nudSoLuong = new NumericUpDown { Location = new Point(20, 480), Minimum = 1, Value = 1 };
            btnThemGioHang = new Button { Text = "Thêm vào giỏ", Location = new Point(150, 478) };
            btnThemGioHang.Click += btnThemGioHang_Click;

            dgvGioHang = new DataGridView { Location = new Point(500, 60), Size = new Size(450, 350) };
            
            Label lblKhach = new Label { Text = "Mã KH:", Location = new Point(500, 430), AutoSize = true };
            txtMaKH = new TextBox { Location = new Point(560, 427), Width = 100 };

            lblTongTien = new Label { Text = "0 VNĐ", Location = new Point(500, 470), AutoSize = true, Font = new Font("Arial", 12, FontStyle.Bold) };
            lblDiemTichLuy = new Label { Text = "0 Điểm", Location = new Point(700, 470), AutoSize = true, Font = new Font("Arial", 12, FontStyle.Bold) };

            btnThanhToan = new Button { Text = "Thanh toán (SqlTransaction)", Location = new Point(500, 510), Width = 200, Height = 35 };
            btnThanhToan.Click += btnThanhToan_Click;

            this.Controls.Add(txtTimKiem);
            this.Controls.Add(btnTimKiem);
            this.Controls.Add(dgvSanPham);
            this.Controls.Add(nudSoLuong);
            this.Controls.Add(btnThemGioHang);
            this.Controls.Add(dgvGioHang);
            this.Controls.Add(lblKhach);
            this.Controls.Add(txtMaKH);
            this.Controls.Add(lblTongTien);
            this.Controls.Add(lblDiemTichLuy);
            this.Controls.Add(btnThanhToan);
        }

        private void btnTimKiem_Click(object? sender, EventArgs e)
        {
            dgvSanPham.DataSource = salesBUS.TimKiemSanPham(txtTimKiem.Text.Trim());
        }

        private void btnThemGioHang_Click(object? sender, EventArgs e)
        {
            if (dgvSanPham.CurrentRow == null) return;

            string? maSP = dgvSanPham.CurrentRow.Cells["MaSP"].Value?.ToString();
            if (string.IsNullOrEmpty(maSP)) return;

            decimal donGia = Convert.ToDecimal(dgvSanPham.CurrentRow.Cells["GiaBan"].Value);
            int soLuong = (int)nudSoLuong.Value;

            gioHang.Add(new ChiTietHoaDonDTO { MaSP = maSP, SoLuong = soLuong, DonGia = donGia });
            CapNhatGioHangUI();
        }

        private void CapNhatGioHangUI()
        {
            dgvGioHang.DataSource = null;
            dgvGioHang.DataSource = gioHang;
            tongTien = 0;
            foreach (var item in gioHang)
            {
                tongTien += item.ThanhTien;
            }
            lblTongTien.Text = tongTien.ToString("N0") + " VNĐ";
            lblDiemTichLuy.Text = ((int)(tongTien / 100000)).ToString() + " Điểm";
        }

        private void btnThanhToan_Click(object? sender, EventArgs e)
        {
            int maNV = 1;
            int? maKH = string.IsNullOrEmpty(txtMaKH.Text) ? (int?)null : int.Parse(txtMaKH.Text);

            string loi;
            bool thanhCong = salesBUS.ThanhToan(maNV, maKH, tongTien, gioHang, out loi);

            if (thanhCong)
            {
                MessageBox.Show("Thanh toán thành công và đã cộng điểm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                gioHang.Clear();
                CapNhatGioHangUI();
            }
            else
            {
                MessageBox.Show("Lỗi thanh toán: " + loi, "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}