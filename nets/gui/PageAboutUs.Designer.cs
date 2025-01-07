using System.Windows.Forms;

namespace nets.gui
{
    partial class PageAboutUs : Panel
    {
        #region Window Form Designer Generated codes

        private System.Windows.Forms.Label lb_BriefIntro;
        private System.Windows.Forms.Label lb_Title;
        private System.Windows.Forms.Label lb_VersionInfo;
        private System.Windows.Forms.Label lb_TeamMember;
        private System.Windows.Forms.LinkLabel llb_NETDownload;
        private System.Windows.Forms.LinkLabel llb_GoogleLink;
        private System.Windows.Forms.PictureBox pcb_TeamPicture;
        private Label lb_EmailImage;

        private void InitializeComponent()
        {
            this.lb_BriefIntro = new System.Windows.Forms.Label();
            this.lb_Title = new System.Windows.Forms.Label();
            this.lb_VersionInfo = new System.Windows.Forms.Label();
            this.lb_TeamMember = new System.Windows.Forms.Label();
            this.llb_NETDownload = new System.Windows.Forms.LinkLabel();
            this.llb_GoogleLink = new System.Windows.Forms.LinkLabel();
            this.pcb_TeamPicture = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pcb_TeamPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // lb_Title
            // 
            this.lb_Title.AutoSize = true;
            this.lb_Title.Font = new System.Drawing.Font("Cambria", 15F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_Title.Location = new System.Drawing.Point(122, 25);
            this.lb_Title.Name = "lb_Title";
            this.lb_Title.Size = new System.Drawing.Size(306, 27);
            this.lb_Title.TabIndex = 1;
            this.lb_Title.Text = "NETS - Nothing Else To Sync";
            // 
            // lb_VersionInfo
            // 
            this.lb_VersionInfo.AutoSize = true;
            this.lb_VersionInfo.Location = new System.Drawing.Point(124, 63);
            this.lb_VersionInfo.Name = "lb_VersionInfo";
            this.lb_VersionInfo.Size = new System.Drawing.Size(130, 39);
            this.lb_VersionInfo.TabIndex = 2;
            this.lb_VersionInfo.Text = "Version 2.0 \r\nCopyright(R) 2010 frldrs.exe";
            // 
            // lb_TeamMember
            // 
            this.lb_TeamMember.AutoSize = true;
            this.lb_TeamMember.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_TeamMember.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lb_TeamMember.Location = new System.Drawing.Point(362, 198);
            this.lb_TeamMember.Name = "lb_TeamMember";
            this.lb_TeamMember.Size = new System.Drawing.Size(157, 112);
            this.lb_TeamMember.TabIndex = 3;
            this.lb_TeamMember.Text = "Developed by:\r\n     Dang Thu Giang\r\n     Hoang Nguyen Nhat Tao\r\n     Nguyen Hoang Hai\r\n     Nguyen Thi Yen Duong\r\n     Tran Binh Nguyen\r\n     Vu An Hoa";
            // 
            // llb_NETDownload
            // 
            this.llb_NETDownload.AutoSize = true;
            this.llb_NETDownload.LinkArea = new System.Windows.Forms.LinkArea(0, 30);
            this.llb_NETDownload.Location = new System.Drawing.Point(553, 98);
            this.llb_NETDownload.Name = "llb_NETDownload";
            this.llb_NETDownload.Size = new System.Drawing.Size(150, 15);
            this.llb_NETDownload.LinkBehavior = LinkBehavior.HoverUnderline;
            this.llb_NETDownload.TabIndex = 4;
            this.llb_NETDownload.TabStop = true;
            this.llb_NETDownload.Text = "(Download)";
            this.llb_NETDownload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llb_NETDownload_LinkClicked);
            // 
            // llb_GoogleLink
            // 
            this.llb_GoogleLink.AutoSize = true;
            this.llb_GoogleLink.LinkArea = new System.Windows.Forms.LinkArea(0, 30);
            this.llb_GoogleLink.Location = new System.Drawing.Point(124, 91);
            this.llb_GoogleLink.Name = "llb_GoogleLink";
            this.llb_GoogleLink.Size = new System.Drawing.Size(150, 15);
            this.llb_GoogleLink.LinkBehavior = LinkBehavior.HoverUnderline;
            this.llb_GoogleLink.TabIndex = 5;
            this.llb_GoogleLink.TabStop = true;
            this.llb_GoogleLink.Text = "Google Code Project Homepage";
            this.llb_GoogleLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llb_GoogleHost_LinkClicked);
            // 
            // pcb_ContextMenuPicture
            // 
            this.pcb_TeamPicture.Image = global::nets.Properties.Resources.team;
            this.pcb_TeamPicture.Location = new System.Drawing.Point(124, 112);
            this.pcb_TeamPicture.Name = "pcb_ContextMenuPicture";
            this.pcb_TeamPicture.Size = new System.Drawing.Size(232, 190);
            this.pcb_TeamPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pcb_TeamPicture.TabIndex = 6;
            this.pcb_TeamPicture.TabStop = false;
            // 
            // lb_BriefIntro
            // 
            this.lb_BriefIntro.AutoSize = true;
            this.lb_BriefIntro.Font = new System.Drawing.Font("Cambria", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_BriefIntro.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lb_BriefIntro.Location = new System.Drawing.Point(362, 63);
            this.lb_BriefIntro.Name = "lb_BriefIntro";
            this.lb_BriefIntro.Size = new System.Drawing.Size(238, 64);
            this.lb_BriefIntro.TabIndex = 0;
            this.lb_BriefIntro.Text = "Free and Opensource\r\nWindows XP and above\r\nRequires .NET Framework 4.8.1";
            // 
            // PageAboutUs
            // 
            this.Controls.Add(this.lb_Title);
            this.Controls.Add(this.lb_VersionInfo);
            this.Controls.Add(this.lb_BriefIntro);
            this.Controls.Add(this.lb_TeamMember);
            this.Controls.Add(this.llb_GoogleLink);
            this.Controls.Add(this.llb_NETDownload);
            this.Controls.Add(this.pcb_TeamPicture);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
