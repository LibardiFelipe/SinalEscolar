using SinalEscolar.Classes;
using SinalEscolar.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SinalEscolar
{
    public partial class AddForm : Form
    {
        private string _selectedMusic;
        private bool _testingMusic;
        private Form1 _form1;

        public AddForm(Form1 form1)
        {
            InitializeComponent();

            _form1 = form1;
            comboBox1.SelectedIndex = 0;
        }

        private void AddForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Arquivos de Áudio | *.wav; *.wmv; *.mp3; *.WAV; *.WMV; *.MP3;";
                ofd.FileName = "";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _selectedMusic = ofd.FileName;
                    label4.Text = ofd.SafeFileName;

                    if (_testingMusic)
                        button2_Click(this, e);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedMusic))
                return;

            if (_testingMusic)
            {
                _testingMusic = false;
                timer1.Stop();
                MediaPlayer.StopPlaying();
            }
            else
            {
                // Caso não esteja testando música nenhuma
                if (File.Exists(_selectedMusic))
                {
                    _testingMusic = true;
                    timer1.Start();
                    MediaPlayer.Play(_selectedMusic);
                }
            }

            button2.Text = _testingMusic ? "parar" : "testar";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var p = Process.GetProcessesByName("wmplayer");
            if (p.Length <= 0)
            {
                if (_testingMusic)
                {
                    timer1.Stop();
                    button2_Click(this, e);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedMusic) || !File.Exists(_selectedMusic))
                return;

            var alarm = new Alarm()
            {
                Day = GetDayById(comboBox1.SelectedIndex),
                Time = $"{string.Format("{0:00}", numericUpDown1.Value)}:{string.Format("{0:00}", numericUpDown2.Value)}",
                Song = _selectedMusic,
                IntervalInSeconds = Convert.ToInt32(numericUpDown3.Value)
            };

            _form1.AddAlarm(alarm);
        }

        private static string GetDayById(int id)
        {
            switch (id)
            {
                case 0: return "Monday";
                case 1: return "Tuesday";
                case 2: return "Wednesday";
                case 3: return "Thursday";
                case 4: return "Friday";
                case 5: return "Saturday";
                case 6: return "Sunday";
                default: throw new IndexOutOfRangeException();
            }
        }
    }
}
