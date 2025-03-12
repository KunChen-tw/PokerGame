using System;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Poker
{
    public partial class frmPoker : Form
    {
        Random rand = new Random();
        private PictureBox[] pic = new PictureBox[5];

        /// <summary>
        /// 記錄所有撲克牌的編號
        /// </summary>
        private int[] allPoker = new int[52];

        /// <summary>
        /// 記錄玩家的五張牌
        /// </summary>
        private int[] playerPoker = new int[5];


        public frmPoker()
        {
            InitializeComponent();

            // 動態產生五張牌
            InitializePoker();
        }

        #region 自訂函式
        private void InitializePoker()
        {
            for (int i = 0; i < 5; i++)
            {
                pic[i] = new PictureBox();
                pic[i].Image = this.GetImage("back");
                pic[i].SizeMode = PictureBoxSizeMode.AutoSize;
                pic[i].Name = $"pic{i}";
                pic[i].Top = 30;
                pic[i].Left = 20 + (pic[i].Width + 10) * i;
                pic[i].Enabled = false;
                pic[i].Tag = "back";

                pic[i].Click += new EventHandler(pic_Click);
                this.grpPoker.Controls.Add(pic[i]);
            }

            // 將桌面上的五張牌設為背面 (-1)
            for (int i = 0; i < playerPoker.Length; i++)
            {
                playerPoker[i] = -1;
            }
        }


        /// <summary>
        /// 從資源檔取得圖片
        /// </summary>
        /// <param name="name">圖片名稱，格式為 "pic" + 數字</param>
        /// <returns></returns>
        private Image GetImage(string name)
        {
            ResourceManager rm = Poker.Properties.Resources.ResourceManager;
            Image img = rm.GetObject(name) as Image;
            if (img == null)
            {
                MessageBox.Show($"找不到 {name} 圖片檔", "資源檔讀取錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return img;
        }

        /// <summary>
        /// 洗牌52張牌
        /// </summary>
        private void Shuffle()
        {
            int temp;
            int r;
            //int n = allPoker.Length;
            int n = 500;
            for (int i = 0; i < n; i++)
            {
                // 產生隨機位置 0~51
                r = rand.Next(allPoker.Length);
                temp = allPoker[r];
                //allPoker[r] = allPoker[0];
                //allPoker[0] = temp;
                allPoker[r] = allPoker[i % 52];
                allPoker[i % 52] = temp;
            }

        }
        #endregion


        #region 控制項事件
        /// <summary>
        /// 發牌按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnDealCard_Click(object sender, EventArgs e)
        {

            // 將結果清空
            lblResult.Text = "";

            // 將桌上的五張牌設為背面
            for (int i = 0; i < pic.Length; i++)
            {
                pic[i].Image = this.GetImage("back");
            }

            // 暫停500ms
            await Task.Delay(500);


            // 產生新的撲克牌
            for (int i = 0; i < allPoker.Length; i++)
            {
                allPoker[i] = i;
            }

            // 洗牌
            Shuffle();

            // 發牌 (前五張)
            for (int i = 0; i < 5; i++)
            {
                // 取得撲克牌圖片編號
                int idx = allPoker[i];
                // 將撲克牌圖片顯示在 PictureBox 控制項上
                pic[i].Image = this.GetImage($"pic{idx + 1}");
                // 記錄玩家的五張牌
                playerPoker[i] = idx;
            }

            for (int i = 0; i < pic.Length; i++)
            {
                pic[i].Enabled = true;
                pic[i].Tag = "front";
            }

            // 將判斷牌型按鈕啟用
            btnCheck.Enabled = true;
            // 將發牌按鈕停用
            btnDealCard.Enabled = false;
        }

        /// <summary>
        /// 桌上撲克牌點擊事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pic_Click(object sender, EventArgs e)
        {

            PictureBox picbox = sender as PictureBox;

            // 取得 PictureBox 控制項的編號
            int idx = int.Parse(picbox.Name.Replace("pic", ""));

            // 如果是正面, 則翻成背面
            if (picbox.Tag.ToString() == "front")
            {
                picbox.Image = this.GetImage("back");
                picbox.Tag = "back";
            }
            else // 如果是背面, 則翻成正面，並顯示原來的撲克牌
            {
                picbox.Image = this.GetImage($"pic{playerPoker[idx] + 1}");
                picbox.Tag = "front";
            }

            // 將判斷牌型按鈕啟用
            btnCheck.Enabled = true;
            // 將換牌按鈕啟用
            btnChangeCard.Enabled = false;

            // 只要有一張牌是背面，則判斷牌型按鈕停用
            for (int i = 0; i < pic.Length; i++)
            {
                if (pic[i].Tag.ToString() == "back")
                {
                    // 將判斷牌型按鈕停用
                    btnCheck.Enabled = false;

                    // 如果有一張牌是背面，則換牌按鈕啟用
                    btnChangeCard.Enabled = true;
                    break;
                }
            }

        }
        #endregion

        /// <summary>
        /// 換牌按鈕事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChangeCard_Click(object sender, EventArgs e)
        {
            int cardindex = 5;
            for (int i = 0; i < pic.Length; i++)
            {
                // 如果是背面，則換一張新的牌
                if (pic[i].Tag.ToString() == "back")
                {
                    int idx = allPoker[cardindex];
                    pic[i].Image = this.GetImage($"pic{idx + 1}");
                    pic[i].Tag = "front";
                    playerPoker[i] = idx;
                    cardindex++;
                }
            }

            // 因為只能換一次牌，所以將桌上的牌設成不能點擊 (不能再換牌)
            for (int i = 0; i < pic.Length; i++)
            {
                pic[i].Enabled = false;
            }

            // 將換牌按鈕停用
            btnChangeCard.Enabled = false;
            // 將判斷牌型按鈕啟用
            btnCheck.Enabled = true;

        }


        /// <summary>
        /// 判斷牌型按鈕事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheck_Click(object sender, EventArgs e)
        {
            // 記錄五張撲克牌要顯示的花色
            string[] colorList = { "梅花", "方塊", "愛心", "黑桃" };

            // 記錄五張撲克牌要顯示的點數
            string[] pointList = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

            // 計錄目前五張撲克牌的花色和點數的陣列
            int[] pokerColor = new int[5];
            int[] pokerPoint = new int[5];

            // 將每張牌的顏色和點數分別存入 pokerColor 和 pokerPoint 陣列
            for (int i = 0; i < 5; i++)
            {
                // 計算花色
                pokerColor[i] = playerPoker[i] % 4;
                // 計算點數
                pokerPoint[i] = playerPoker[i] / 4;
            }

            int[] colorCount = new int[4];
            int[] pointCount = new int[13];

            // 計算花色和點數的數量
            for (int i = 0; i < 5; i++)
            {
                colorCount[pokerColor[i]]++;
                pointCount[pokerPoint[i]]++;
            }

            
            Array.Sort(colorCount, colorList); // 依照花色數量排序 (由小到大)
            Array.Reverse(colorCount);         // 花色數量由大到小
            Array.Reverse(colorList);          // 花色名稱由大到小
            Array.Sort(pointCount, pointList); // 依照點數數量排序 (由小到大)
            Array.Reverse(pointCount);         // 點數數量由大到小
            Array.Reverse(pointList);          // 點數名稱由大到小


            // 判斷牌型

            // 判斷是否為同花
            bool isFlush = (colorCount[0] == 5);
            // 判斷是否為五張單張
            bool isSingle = (pointCount[0] == 1 && pointCount[1] == 1 && pointCount[2] == 1 && pointCount[3] == 1 && pointCount[4] == 1);
            // 判斷是否為差四
            bool isDiffFout = (pokerPoint.Max() - pokerPoint.Min() == 4);
            // 判斷是否為大順
            bool isRoyal = pokerPoint.Contains(0) && pokerPoint.Contains(9) && pokerPoint.Contains(10) && pokerPoint.Contains(11) && pokerPoint.Contains(12);
            // 判斷是否為同花大順
            bool isRoyalisFlush = isFlush && isRoyal;
            // 判斷是否為同花順
            bool isStraightFlush = isFlush && isSingle && isDiffFout;
            // 判斷是否為順子
            bool isStraight = isSingle && (isDiffFout || isRoyal);
            // 判斷是否為鐵支
            bool isFourOfAKind = (pointCount[0] == 4);
            // 判斷是否為葫蘆
            bool isFullHouse = (pointCount[0] == 3 && pointCount[1] == 2);
            // 判斷是否為三條
            bool isThreeOfAKind = (pointCount[0] == 3);
            // 判斷是否為兩對
            bool isTwoPair = (pointCount[0] == 2 && pointCount[1] == 2);
            // 判斷是否為一對
            bool isOnePair = (pointCount[0] == 2);

            // 記錄結果
            string result;

            // 判斷牌型
            if (isRoyalisFlush)
            {
                result = $"{colorList[0]} 同花大順";
            }
            else if (isStraightFlush)
            {
                result = $"{colorList[0]} 同花順";
            }
            else if (isStraight)
            {
                result = "順子";
            }
            else if (isFourOfAKind)
            {
                result = $"{pointList[0]} 鐵支";
            }
            else if (isFullHouse)
            {
                result = $"{pointList[0]}三張{pointList[1]}兩張 葫蘆";
            }
            else if (isFlush)
            {
                result = $"{colorList[0]} 同花";
            }
            else if (isThreeOfAKind)
            {
                result = $"{pointList[0]} 三條";
            }
            else if (isTwoPair)
            {
                result = $"{pointList[0]}{pointList[1]} 兩對";
            }
            else if (isOnePair)
            {
                result = $"{pointList[0]} 一對";
            }
            else
            {
                result = "雜牌";
            }
            lblResult.Text = result;

            btnDealCard.Enabled = true;
            btnCheck.Enabled = false;

        }
    }
}
