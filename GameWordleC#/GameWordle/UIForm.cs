using System;
using Eto.Forms;
using Eto.Drawing;

namespace WordleGame
{
    public class MainForm : Form
    {
        private WordleGameLogic gameLogic = new WordleGameLogic();

        // UI 控件
        private TextArea fileContentArea;
        private TextBox guessInput;
        private Label resultLabel;
        private Label feedbackLabel;
        private Button submitButton;
        private Button newGameButton;
        private Button showAnswerButton;

        public MainForm()
        {
            Title = "Wordle Game";
            ClientSize = new Size(700, 500);

            InitializeComponents();
            InitializeGame();
        }

        // 初始化所有UI组件和布局
        private void InitializeComponents()
        {
            // ======================
            // 1. 初始化各个UI控件
            // ======================

            // 左侧文件内容显示区域（只读、不自动换行）
            fileContentArea = new TextArea
            {
                ReadOnly = true,    // 设置为只读模式
                Wrap = false        // 禁用自动换行
            };

            // 猜测输入文本框（宽度180px，带占位文字）
            guessInput = new TextBox
            {
                PlaceholderText = "Enter here...",  // 输入提示文字
                Width = 180                         // 固定宽度
            };

            // 三个功能按钮
            submitButton = new Button { Text = "Submit Guess" };    // 提交猜测
            newGameButton = new Button { Text = "New Game" };       // 开始新游戏
            showAnswerButton = new Button { Text = "Show Answer" };  // 显示答案

            // 结果显示标签（较大字体，黑色文字）
            resultLabel = new Label
            {
                Text = "",                              // 初始为空
                Font = Fonts.Sans(13),                  // 13pt字体
                TextColor = Colors.Red  ,               // 黑色文字
                VerticalAlignment = VerticalAlignment.Center  // 垂直居中
            };

            // 反馈信息标签（较小字体，黄绿色文字）
            feedbackLabel = new Label
            {
                Text = "",                              // 初始为空
                Font = Fonts.Sans(12),                  // 12pt字体
                TextColor = Colors.YellowGreen          // 黄绿色文字
            };

            // ============================
            // 2. 构建游戏规则说明及反馈区域
            // ============================

            var rulesContainer = new StackLayout
            {
                Spacing = 6  // 子控件间距5px
            };

            // 按顺序添加规则说明项
            rulesContainer.Items.Add(new Label { Text = "1. Guess the 5-letter hidden word." });
            rulesContainer.Items.Add(new Label { Text = "2. \"G\": Correct character in correct spot." });
            rulesContainer.Items.Add(new Label { Text = "3. \"Y\": Correct character in wrong spot." });
            rulesContainer.Items.Add(new Label { Text = "4. \"B\": Incorrect character." });
            rulesContainer.Items.Add(new Label { Text = "5. Each target character matches at most one guess character." });
            rulesContainer.Items.Add(feedbackLabel);  // 第一项为动态反馈信息

            // ======================
            // 3. 构建右侧交互面板
            // ======================

            var rightPanel = new StackLayout
            {
                Orientation = Orientation.Vertical,  // 垂直排列
                Spacing = 10,                        // 子控件间距10px
                Padding = new Padding(10),            // 内边距10px
                Items =
                {
                    // 3.1 游戏规则区域（分组框）
                    new GroupBox
                    {
                        Text = "Game Rules",  // 分组标题
                        Font = Fonts.Sans(14),
                        Width = 390,
                        Content = rulesContainer,  // 使用前面构建的规则容器
                    },

                    new GroupBox
                    {
                        Text = "Play Zone",  // 分组标题
                        Font = Fonts.Sans(14),
                        Width = 390,
                        Content = new TableLayout // 使用表格布局确保对齐
                        {
                            Spacing = new Size(5, 5),  // 单元格间距5px
                            Rows =
                            {
                                // 第一行：输入提示标签
                                new TableRow(new Label { Text = "Enter your 5-letter guess:" }),
                        
                                // 第二行：输入框
                                new TableRow(guessInput),
                        
                                // 第三行：三个功能按钮
                                new TableRow(submitButton, newGameButton, showAnswerButton),
                        
                                // 第四行：结果显示
                                new TableRow(resultLabel)
                            }
                        }
                    },
                }
            };

            // ======================
            // 4. 构建主窗口布局
            // ======================

            Content = new TableLayout
            {
                Spacing = new Size(3, 9),      // 整体水平间距3px，垂直间距7px
                Padding = new Padding(0),      // 无内边距
                Rows =
                {
                    new TableRow(
                        // 4.1 左侧面板（文件内容区）
                        new TableCell(new Scrollable  // 可滚动容器
                        {
                            Content = fileContentArea,  // 包含文本区域
                            Border = BorderType.Line,   // 显示边框
                            Width = 280                // 固定宽度300px
                        }),
                
                        // 4.2 右侧面板（游戏交互区）
                        new TableCell(rightPanel)  // 使用前面构建的右侧面板
                    )
                }
            };

            // ======================
            // 5. 绑定事件处理器
            // ======================

            // 提交按钮点击事件
            submitButton.Click += OnSubmitGuess;

            // 新游戏按钮点击事件
            newGameButton.Click += OnNewGame;

            // 显示答案按钮点击事件
            showAnswerButton.Click += OnShowAnswer;

            // 输入框回车键事件（支持按Enter提交）
            guessInput.KeyUp += (sender, e) =>
            {
                if (e.Key == Keys.Enter)
                {
                    OnSubmitGuess(sender, e);
                }
            };
        }

        private void InitializeGame()
        {
            try
            {
                gameLogic = new WordleGameLogic();
                gameLogic.LoadWords("words.txt");
                fileContentArea.Text = string.Join(Environment.NewLine, gameLogic.WordList);
                StartNewGame();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxType.Error);
                Close();
            }
        }

        private void StartNewGame()
        {
            gameLogic.StartNewGame();
            resultLabel.Text = "";
            feedbackLabel.Text = "New game started! Make your first guess.";
            guessInput.Text = "";
            guessInput.Focus();
            submitButton.Enabled = true;
        }

        private void OnSubmitGuess(object sender, EventArgs e)
        {
            try
            {
                string guess = guessInput.Text.Trim().ToUpper();
                if (guess.Length != 5)
                {
                    feedbackLabel.Text = "Please enter exactly 5 letters";
                    return;
                }

                string result = gameLogic.EvaluateGuess(guess);
                resultLabel.Text = result;

                if (result == "GGGGG")
                {
                    feedbackLabel.Text = $"Congratulations! You guessed the word: {gameLogic.CurrentSecret}";
                    submitButton.Enabled = false;
                }
                else
                {
                    feedbackLabel.Text = "Try again!";
                    //guessInput.Text = "";
                    guessInput.Focus();
                }
            }
            catch (Exception ex)
            {
                feedbackLabel.Text = ex.Message;
            }
        }

        private void OnNewGame(object sender, EventArgs e)
        {
            StartNewGame();
        }

        private void OnShowAnswer(object sender, EventArgs e)
        {
            feedbackLabel.Text = $"The secret word was: {gameLogic.CurrentSecret}";
            submitButton.Enabled = false;
        }

        [STAThread]
        public static void Main()
        {
            new Application().Run(new MainForm());
        }
    }
}

