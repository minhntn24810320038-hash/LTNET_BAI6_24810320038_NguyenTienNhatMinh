using System;
using System.Drawing;
using System.Windows.Forms;
using LTNET_BAI6.BUS;

namespace LTNET_BAI6.GUI
{
    public partial class frmSalesReport : Form
    {
        private SalesBUS salesBUS = new SalesBUS();

        private DateTimePicker dtpTuNgay = null!;
        private DateTimePicker dtpDenNgay = null!;
        private Button btnThongKe = null!;
        private Label lblTongDoanhThu = null!;
        private DataGridView dgvTop5 = null!;

        public frmSalesReport()
        {
            InitializeComponentCustom();
        }

        private void InitializeComponentCustom()
        {
            this.Size = new Size(800, 550);
            this.Text = "Báo cáo Doanh thu & Top 5 Bán chạy";

            Label lblTu = new Label { Text = "Từ ngày:", Location = new Point(20, 20), AutoSize = true };
            dtpTuNgay = new DateTimePicker { Location = new Point(80, 16), Format = DateTimePickerFormat.Short };

            Label lblDen = new Label { Text = "Đến ngày:", Location = new Point(220, 20), AutoSize = true };
            dtpDenNgay = new DateTimePicker { Location = new Point(290, 16), Format = DateTimePickerFormat.Short };

            btnThongKe = new Button { Text = "Thống kê", Location = new Point(430, 15) };
            btnThongKe.Click += btnThongKe_Click;

            Label lblLabelDoanhThu = new Label { Text = "Tổng doanh thu:", Location = new Point(20, 60), AutoSize = true, Font = new Font("Arial", 11, FontStyle.Bold) };
            lblTongDoanhThu = new Label { Text = "0 VNĐ", Location = new Point(150, 60), AutoSize = true, Font = new Font("Arial", 11, FontStyle.Bold), ForeColor = Color.Blue };

            Label lblTop5 = new Label { Text = "Top 5 Sản phẩm bán chạy nhất:", Location = new Point(20, 100), AutoSize = true, Font = new Font("Arial", 10, FontStyle.Bold) };
            dgvTop5 = new DataGridView { Location = new Point(20, 130), Size = new Size(740, 350) };

            this.Controls.Add(lblTu);
            this.Controls.Add(dtpTuNgay);
            this.Controls.Add(lblDen);
            this.Controls.Add(dtpDenNgay);
            this.Controls.Add(btnThongKe);
            this.Controls.Add(lblLabelDoanhThu);
            this.Controls.Add(lblTongDoanhThu);
            this.Controls.Add(lblTop5);
            this.Controls.Add(dgvTop5);
        }

        private void btnThongKe_Click(object? sender, EventArgs e)
        {
            decimal tongDoanhThu = salesBUS.TinhDoanhThu(dtpTuNgay.Value, dtpDenNgay.Value);
            lblTongDoanhThu.Text = tongDoanhThu.ToString("N0") + " VNĐ";
            dgvTop5.DataSource = salesBUS.GetTop5SanPham();
        }
    }
}