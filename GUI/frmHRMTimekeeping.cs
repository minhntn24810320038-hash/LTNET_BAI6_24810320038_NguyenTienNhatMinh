using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using LTNET_BAI6.BUS;

namespace LTNET_BAI6.GUI
{
    public partial class frmHRMTimekeeping : Form
    {
        private HRMBUS hrmBUS = new HRMBUS();
        private DataTable? dtChamCong;

        private TextBox txtThangNam = null!;
        private Button btnTaiDuLieu = null!;
        private Button btnLuuChamCong = null!;
        private Button btnChotLuong = null!;
        private DataGridView dgvChamCong = null!;

        public frmHRMTimekeeping()
        {
            InitializeComponentCustom();
        }

        private void InitializeComponentCustom()
        {
            this.Size = new Size(900, 550);
            this.Text = "Quản lý Chấm công Ngắt kết nối & Chốt lương";

            Label lblThang = new Label { Text = "Tháng/Năm (mm/yyyy):", Location = new Point(20, 20), AutoSize = true };
            txtThangNam = new TextBox { Location = new Point(170, 17), Width = 100, Text = "11/2026" };

            btnTaiDuLieu = new Button { Text = "Tải dữ liệu", Location = new Point(290, 15) };
            btnTaiDuLieu.Click += btnTaiDuLieu_Click;

            btnLuuChamCong = new Button { Text = "Lưu chấm công (Disconnected)", Location = new Point(380, 15), Width = 200 };
            btnLuuChamCong.Click += btnLuuChamCong_Click;

            btnChotLuong = new Button { Text = "Chốt lương tháng (SqlTransaction)", Location = new Point(590, 15), Width = 230 };
            btnChotLuong.Click += btnChotLuong_Click;

            dgvChamCong = new DataGridView { Location = new Point(20, 60), Size = new Size(840, 420) };

            this.Controls.Add(lblThang);
            this.Controls.Add(txtThangNam);
            this.Controls.Add(btnTaiDuLieu);
            this.Controls.Add(btnLuuChamCong);
            this.Controls.Add(btnChotLuong);
            this.Controls.Add(dgvChamCong);
        }

        private void btnTaiDuLieu_Click(object? sender, EventArgs e)
        {
            string thangNam = txtThangNam.Text.Trim();
            dtChamCong = hrmBUS.GetBangChamCong(thangNam);
            dgvChamCong.DataSource = dtChamCong;
        }

        private void btnLuuChamCong_Click(object? sender, EventArgs e)
        {
            if (dtChamCong != null)
            {
                bool thanhCong = hrmBUS.LuuBangChamCong(dtChamCong);
                if (thanhCong)
                {
                    MessageBox.Show("Đã lưu bảng chấm công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnChotLuong_Click(object? sender, EventArgs e)
        {
            string thangNam = txtThangNam.Text.Trim();
            string loi;
            bool thanhCong = hrmBUS.ChotLuongThang(thangNam, out loi);

            if (thanhCong)
            {
                MessageBox.Show($"Đã chốt lương thành công cho tháng {thangNam}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Chốt lương thất bại: " + loi, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}