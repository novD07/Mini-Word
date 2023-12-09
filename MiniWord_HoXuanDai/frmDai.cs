using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace MiniWord_HoXuanDai
{
    public partial class frmDai : Form
    {
        private string currentFilePath;
        public frmDai()
        {
            InitializeComponent();
            InitializeFontFamily();
            InitializeFontSize();                  
            SetDefaultFont();
       
        }
        private bool isBold = false;
        private bool isItalic = false;
        private bool isUnderline = false;
        private Font currentTextFont;
        private Color currentTextColor;
        private bool isColorDialogOpen = false;
        private List<Range> highlightedRanges = new List<Range>();
        private void frmLoad(object sender, EventArgs e)
        {
            tbFont.Text = "Times New Roman";
            tbSize.Text = "12";
        }
        private void rtbContent_SelectionChanged(object sender, EventArgs e)
        {
            if (rtbContent.SelectionFont != null)
            {
                tbFont.SelectedItem = rtbContent.SelectionFont.FontFamily.Name;
                tbSize.SelectedItem = rtbContent.SelectionFont.Size.ToString();
            }
            bool isBold = rtbContent.SelectionFont?.Bold ?? false;
            btnBold.BackColor = isBold ? Color.LightSkyBlue : SystemColors.Control;

            bool isItalic = rtbContent.SelectionFont?.Italic ?? false;
            btnItalic.BackColor = isItalic ? Color.LightSkyBlue : SystemColors.Control;

            bool isUnderline = rtbContent.SelectionFont?.Underline ?? false;
            btnUnder.BackColor = isUnderline ? Color.LightSkyBlue : SystemColors.Control;


        }
        private void btnBold_Click(object sender, EventArgs e)
        {
            ToggleFontStyle(FontStyle.Bold);
        }
        private void btnItalic_Click(object sender, EventArgs e)
        {

            ToggleFontStyle(FontStyle.Italic);
        }
        private void btnUnder_Click(object sender, EventArgs e)
        {
            ToggleFontStyle(FontStyle.Underline);
        }
        private void ToggleFontStyle(FontStyle style)
        {
            if (rtbContent.SelectionFont != null)
            {
                FontStyle currentStyle = rtbContent.SelectionFont.Style;
                FontStyle newStyle = currentStyle ^ style;

                // If currentTextFont is null, use rtbContent.SelectionFont as the base font
                System.Drawing.Font newFont = currentTextFont != null
                    ? new System.Drawing.Font(currentTextFont, newStyle)
                    : new System.Drawing.Font(rtbContent.SelectionFont.FontFamily, rtbContent.SelectionFont.Size, newStyle);
                // Giữ lại thuộc tính Bold nếu nó đã được thiết lập từ trước
                if (currentStyle.HasFlag(FontStyle.Bold))
                {
                    newFont = new System.Drawing.Font(newFont, newFont.Style | FontStyle.Bold);
                }
                currentTextFont = newFont;
                rtbContent.SelectionFont = newFont;
                rtbContent.SelectionColor = currentTextColor;
            }
        }
        private void btnCopy_Click(object sender, EventArgs e)
        {
            rtbContent.Copy();
        }
        private void btnPaste_Click(object sender, EventArgs e)
        {
            rtbContent.Paste();
        }
        private void btnCut_Click(object sender, EventArgs e)
        {
            rtbContent.Cut();
        }
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            rtbContent.SelectAll();
        }
        private void SetDefaultFont()
        {
            // Set the default font to Times New Roman, size 12
            rtbContent.Font = new Font("Times New Roman", 12);

        }
        private void InitializeFontFamily()
        {
            foreach (FontFamily fontFamily in FontFamily.Families)
            {
                tbFont.Items.Add(fontFamily.Name);
            }
            tbFont.SelectedItem = rtbContent.Font.FontFamily.Name;
        }
        private void InitializeFontSize()
        {
            // Populate the font size dropdown with common font sizes
            int[] commonFontSizes = { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 28, 36, 48, 72 };
            foreach (int fontSize in commonFontSizes)
            {
                tbSize.Items.Add(fontSize.ToString());
            }

            // Set the default font size
            tbSize.SelectedItem = rtbContent.Font.Size.ToString();
        }
        private void tbFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Thay đổi font cho văn bản được chọn
            if (tbFont.SelectedItem != null && rtbContent.SelectionFont != null)
            {
                rtbContent.SelectionFont = new Font(tbFont.SelectedItem.ToString(), rtbContent.SelectionFont.Size, rtbContent.SelectionFont.Style);
            }
        }
        private void tbSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Change the font size of the selected text
            if (tbSize.SelectedItem != null)
            {
                float newSize;
                if (float.TryParse(tbSize.SelectedItem.ToString(), out newSize))
                {
                    rtbContent.SelectionFont = new Font(rtbContent.SelectionFont.FontFamily, newSize, rtbContent.SelectionFont.Style);
                }
            }
        }
        private void btnFont_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            fontDialog.Font = rtbContent.SelectionFont ?? new Font("Times New Roman", 12);
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                tbFont.Text = fontDialog.Font.FontFamily.Name;
                tbSize.Text = fontDialog.Font.Size.ToString();
                rtbContent.SelectionFont = fontDialog.Font;
            }           
        }
        private void btnTextColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();        
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                rtbContent.SelectionColor = colorDialog.Color;
            }          
        }
        private void btnTextBackColor_Click(object sender, EventArgs e)
        {
            // Khởi tạo ColorDialog
            using (ColorDialog colorDialog = new ColorDialog())
            {
                // Mở ColorDialog
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    // Áp dụng màu nền đã chọn cho văn bản đang được chọn trong RichTextBox
                    rtbContent.SelectionBackColor = colorDialog.Color;
                }
            }
        }
        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            ChangeFontSize(1.2f); // Tăng kích thước font lên 20%
        }
        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            ChangeFontSize(0.8f); // Giảm kích thước font xuống 20%
        }
        private void ChangeFontSize(float scaleFactor)
        {
            // Lặp qua từng đoạn văn bản trong rtbContent
            for (int i = 0; i < rtbContent.Text.Length; i++)
            {
                // Di chuyển SelectionStart đến vị trí của đoạn văn bản hiện tại
                rtbContent.SelectionStart = i;
                rtbContent.SelectionLength = 1;

                // Lấy kích thước font của đoạn văn bản hiện tại và thay đổi nó
                float newSize = rtbContent.SelectionFont.Size * scaleFactor;

                // Kiểm tra giới hạn kích thước font để tránh giảm quá mức
                if (newSize > 1 && newSize < 100)
                {
                    rtbContent.SelectionFont = new Font(
                        rtbContent.SelectionFont.FontFamily,
                        newSize,
                        rtbContent.SelectionFont.Style
                    );
                }
            }
        }
        private void btnLeftAlign_Click(object sender, EventArgs e)
        {
            rtbContent.SelectionAlignment = HorizontalAlignment.Left;
        }
        private void btnCenterAlign_Click(object sender, EventArgs e)
        {
            rtbContent.SelectionAlignment = HorizontalAlignment.Center;
        }
        private void btnRightAlign_Click(object sender, EventArgs e)
        {
            rtbContent.SelectionAlignment = HorizontalAlignment.Right;
        }
        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (rtbContent.CanUndo)
            {
                rtbContent.Undo();
            }
        }
        private void btnRedo_Click(object sender, EventArgs e)
        {
            if (rtbContent.CanRedo)
            {
                rtbContent.Redo();
            }
        }
        private void btnImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files|*.*";
                openFileDialog.Title = "Select an Image";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string imagePath = openFileDialog.FileName;

                    // Tạo một đối tượng hình ảnh từ tệp được chọn
                    Image image = Image.FromFile(imagePath);

                    // Chèn hình ảnh vào RichTextBox
                    Clipboard.SetImage(image);
                    rtbContent.Paste();
                }
            }
        }
        private async void btnFind_Click(object sender, EventArgs e)
        {
            // Lấy văn bản cần tìm kiếm và đặt màu nền
            string searchText = tstbFind.Text;
            Color highlightColor = Color.Yellow;
            Color defaultBackColor = rtbContent.BackColor;

            // Đặt SelectionStart về đầu để bắt đầu tìm kiếm từ đầu
            rtbContent.SelectionStart = 0;
            rtbContent.SelectionLength = 0;

            // Xóa các Range trước đó nếu có
            foreach (var range in highlightedRanges)
            {
                rtbContent.Select(range.Start, range.Length);
                rtbContent.SelectionBackColor = defaultBackColor;
            }
            highlightedRanges.Clear();

            // Duyệt qua văn bản và tìm kiếm
            int index = rtbContent.Text.IndexOf(searchText);
            int count = 0;
            while (index != -1)
            {
                // Tìm thấy, đặt màu nền và lưu thông tin Range
                rtbContent.SelectionStart = index;
                rtbContent.SelectionLength = searchText.Length;
                rtbContent.SelectionBackColor = highlightColor;
                highlightedRanges.Add(new Range(index, searchText.Length));

                // Di chuyển SelectionStart đến vị trí tiếp theo
                rtbContent.SelectionLength = 0;
                index = rtbContent.Text.IndexOf(searchText, index + 1);
                count++;
            }
            if (count == 0)
            {
                MessageBox.Show("Không tìm thấy chữ cần tìm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            // Chờ 3 giây trước khi đặt màu nền về mặc định
            await Task.Delay(3000);

            // Đặt màu nền về mặc định cho tất cả các Range
            foreach (var range in highlightedRanges)
            {
                rtbContent.Select(range.Start, range.Length);
                rtbContent.SelectionBackColor = defaultBackColor;
            }
            highlightedRanges.Clear();
        }
        public class Range
        {
            public int Start { get; }
            public int Length { get; }

            public Range(int start, int length)
            {
                Start = start;
                Length = length;
            }
        }
        private async void tsBtnReplace_Click(object sender, EventArgs e)
        {
            // Lấy văn bản cần thay thế và văn bản thay thế
            string searchText = tstbTextRP.Text;
            string replaceText = tstbTextRPed.Text;
            Color highlightColor = Color.Yellow;

            // Đặt SelectionStart về đầu để bắt đầu tìm kiếm từ đầu
            int startIndex = 0;

            // Xóa màu nền cũ nếu có
            rtbContent.SelectAll();
            rtbContent.SelectionBackColor = rtbContent.BackColor;

            // Duyệt qua văn bản và thay thế
            int index = rtbContent.Text.IndexOf(searchText, startIndex);
            int count = 0;
            while (index != -1)
            {
                // Tìm thấy, đặt màu nền và di chuyển SelectionStart đến vị trí tiếp theo
                rtbContent.SelectionStart = index;
                rtbContent.SelectionLength = searchText.Length;
                rtbContent.SelectionBackColor = highlightColor;

                // Thay thế văn bản
                rtbContent.SelectedText = replaceText;

                // Di chuyển SelectionStart đến vị trí tiếp theo
                startIndex = index + replaceText.Length;
                rtbContent.SelectionStart = startIndex;

                // Tìm kiếm vị trí tiếp theo
                index = rtbContent.Text.IndexOf(searchText, startIndex);
                count++;
            }
            if (count == 0)
            {
                MessageBox.Show("Không tìm thấy chữ cần tìm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            // Chờ 3 giây trước khi đặt màu nền về mặc định
            await Task.Delay(3000);

            // Đặt màu nền về mặc định cho toàn bộ văn bản
            rtbContent.SelectAll();
            rtbContent.SelectionBackColor = rtbContent.BackColor;
        }
        private void btnEmoji_Click(object sender, EventArgs e)
        {
            // Tạo ContextMenuStrip
            ContextMenuStrip emojiMenu = new ContextMenuStrip();

            // Thêm các emoji vào danh sách
            string[] emojis = { "😊", "😍", "👍", "❤️", "🎉", "😄", "😝" };

            foreach (string emoji in emojis)
            {
                ToolStripMenuItem emojiItem = new ToolStripMenuItem(emoji);
                emojiItem.Click += EmojiItemClick;
                emojiMenu.Items.Add(emojiItem);
            }

            // Hiển thị ContextMenuStrip tại vị trí của toolStripDropDownButton2
            emojiMenu.Show(btnEmoji.Owner, new Point(btnEmoji.Bounds.Right, btnEmoji.Bounds.Bottom));

        }

        private void EmojiItemClick(object sender, EventArgs e)
        {
            // Xử lý sự kiện khi emoji được chọn
            if (sender is ToolStripMenuItem emojiItem)
            {
                // Chèn emoji vào vị trí hiện tại của con trỏ trong RichTextBox
                rtbContent.SelectedText = emojiItem.Text;
            }
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            // Xử lý sự kiện khi nhấn nút New
            // Kiểm tra xem có thay đổi chưa trước khi tạo tài liệu mới
            if (IsDocumentModified())
            {
                DialogResult result = MessageBox.Show("Do you want to save changes?", "Save Changes", MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.Yes)
                {
                    SaveFile();
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }

            // Reset các giá trị và tạo tài liệu mới
            rtbContent.Clear();
            currentFilePath = null;
            EnableAllComponents();
        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            // Xử lý sự kiện khi nhấn nút Open
            // Kiểm tra xem có thay đổi chưa trước khi mở tệp mới
            if (IsDocumentModified())
            {
                DialogResult result = MessageBox.Show("Do you want to save changes?", "Save Changes", MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.Yes)
                {
                    SaveFile();
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }

            // Mở OpenFileDialog để chọn tệp tin
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text Files|*.txt|All Files|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Đọc nội dung tệp và hiển thị trong RichTextBox
                    rtbContent.Text = File.ReadAllText(openFileDialog.FileName);
                    currentFilePath = openFileDialog.FileName;
                }
            }
            EnableAllComponents();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                // Nếu chưa có đường dẫn tệp, sử dụng Save As
                btnSaveAs_Click(sender, e);
            }
            else
            {
                // Kiểm tra xem file đã tồn tại chưa
                if (File.Exists(currentFilePath))
                {
                    // Lưu nội dung vào tệp tin hiện tại
                    File.WriteAllText(currentFilePath, rtbContent.Text);
                }
                else
                {
                    // Nếu file không tồn tại, sử dụng Save As
                    btnSaveAs_Click(sender, e);
                }
            }

        }
        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            // Xử lý sự kiện khi nhấn nút Save As
            // Mở SaveFileDialog để chọn đường dẫn mới
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text Files|*.txt|All Files|*.*";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Lưu nội dung vào tệp tin mới và cập nhật đường dẫn
                    File.WriteAllText(saveFileDialog.FileName, rtbContent.Text);
                    currentFilePath = saveFileDialog.FileName;
                }
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có thay đổi chưa trước khi đóng tài liệu
            if (IsDocumentModified())
            {
                DialogResult result = MessageBox.Show("Do you want to save changes?", "Save Changes", MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.Yes)
                {
                    SaveFile();
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }
            // Disable tất cả các thành phần trong form
            DisableAllComponents();
            // Enable btnNew
            btnNew.Enabled = true;
        }
        private void DisableAllComponents()
        {
            msTop.Enabled = false;
            btnSave.Enabled = false;
            rtbContent.Enabled = false;
            btnCopy.Enabled = false;
            btnPaste.Enabled = false;
            btnCut.Enabled = false;
            btnUndo.Enabled = false;
            btnRedo.Enabled = false;
            btnFont.Enabled = false;
            tbFont.Enabled = false;
            tbSize.Enabled = false;
            btnTextColor.Enabled = false;
            btnBgTextColor.Enabled = false;
            btnZoomin.Enabled = false;
            btnZoomout.Enabled = false;
            btnLeftAlign.Enabled = false;
            btnCenterAlign.Enabled = false;
            btnRightAlign.Enabled = false;
            btnImage.Enabled = false;
            btnFind.Enabled = false;
            tstbFind.Enabled = false;
            tsBtnReplace.Enabled = false;
            tstbTextRP.Enabled = false;
            tstbTextRPed.Enabled = false;
            tsBtnReplace.Enabled = false;
            btnBold.Enabled = false;
            btnItalic.Enabled = false;
            btnUnder.Enabled = false;
        }
        private void EnableAllComponents()
        {
            msTop.Enabled = true;
            btnSave.Enabled = true;
            rtbContent.Enabled = true;
            btnCopy.Enabled = true;
            btnPaste.Enabled = true;
            btnCut.Enabled = true;
            btnUndo.Enabled = true;
            btnRedo.Enabled = true;
            btnFont.Enabled = true;
            tbFont.Enabled = true;
            tbSize.Enabled = true;
            btnTextColor.Enabled = true;
            btnBgTextColor.Enabled = true;
            btnZoomin.Enabled = true;
            btnZoomout.Enabled = true;
            btnLeftAlign.Enabled = true;
            btnCenterAlign.Enabled = true;
            btnRightAlign.Enabled = true;
            btnImage.Enabled = true;
            btnFind.Enabled = true;
            tstbFind.Enabled = true;
            tsBtnReplace.Enabled = true;
            tstbTextRP.Enabled = true;
            tstbTextRPed.Enabled = true;
            tsBtnReplace.Enabled = true;
            btnBold.Enabled = true;
            btnItalic.Enabled = true;
            btnUnder.Enabled = true;
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            // Xử lý sự kiện khi nhấn nút Exit
            // Kiểm tra xem có thay đổi chưa trước khi thoát ứng dụng
            if (IsDocumentModified())
            {
                DialogResult result = MessageBox.Show("Do you want to save changes?", "Save Changes", MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.Yes)
                {
                    SaveFile();
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }
            // Thoát ứng dụng
            Application.Exit();
        }
        private void SaveFile()
        {
            // Lưu nội dung vào tệp tin hiện tại
            File.WriteAllText(currentFilePath, rtbContent.Text);
        }
        private bool IsDocumentModified()
        {
            // Kiểm tra xem có thay đổi trong văn bản không
            return !string.IsNullOrEmpty(currentFilePath) && rtbContent.Text != File.ReadAllText(currentFilePath);
        }
        
    }
}
