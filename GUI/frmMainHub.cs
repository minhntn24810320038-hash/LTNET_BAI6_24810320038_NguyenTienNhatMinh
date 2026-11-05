using System;
using System.Drawing;
using System.Windows.Forms;

namespace LTNET_BAI6.GUI
{
    public partial class frmMainHub : Form
    {
        private MenuStrip menuStrip = null!;
        private ToolStripMenuItem menuBanHang = null!;
        private ToolStripMenuItem menuBaoCao = null!;
        private ToolStripMenuItem menuChamCong = null!;

        public frmMainHub()
        {
            InitializeComponentCustom();
        }

        private void InitializeComponentCustom()
        {
            this.Size = new Size(1200, 750);
            this.Text = "Hệ thống Quản lý Bán hàng & Nhân sự";
            this.IsMdiContainer = true;

            menuStrip = new MenuStrip();
            menuBanHang = new ToolStripMenuItem("Bán hàng (POS)");
            menuBanHang.Click += menuBanHang_Click;

            menuBaoCao = new ToolStripMenuItem("Báo cáo doanh thu");
            menuBaoCao.Click += menuBaoCao_Click;

            menuChamCong = new ToolStripMenuItem("Chấm công & Tính lương");
            menuChamCong.Click += menuChamCong_Click;

            menuStrip.Items.Add(menuBanHang);
            menuStrip.Items.Add(menuBaoCao);
            menuStrip.Items.Add(menuChamCong);

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void menuBanHang_Click(object? sender, EventArgs e)
        {
            frmSalesPOS frm = new frmSalesPOS();
            frm.MdiParent = this;
            frm.Show();
        }

        private void menuBaoCao_Click(object? sender, EventArgs e)
        {
            frmSalesReport frm = new frmSalesReport();
            frm.MdiParent = this;
            frm.Show();
        }

        private void menuChamCong_Click(object? sender, EventArgs e)
        {
            frmHRMTimekeeping frm = new frmHRMTimekeeping();
            frm.MdiParent = this;
            frm.Show();
        }
    }
}