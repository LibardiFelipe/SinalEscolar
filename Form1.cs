using Microsoft.Win32;
using SinalEscolar.Classes;
using SinalEscolar.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SinalEscolar
{
    public partial class Form1 : Form
    {
        // Var em que ficará o id do último alarme tocado
        private List<string> _lastAlarmIds = new List<string>();

        private List<Alarm> _alarms = new List<Alarm>();
        private string _dayOfWeek = DateTime.Now.DayOfWeek.ToString();

        public Form1()
        {
            InitializeComponent();
        }

        public void AddAlarm(Alarm alarm)
        {
            this._alarms.Add(alarm);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Seta o programa pra iniciar com o windows
            StartWithWindows();
        }

        public static void StartWithWindows()
        {
            var subKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            var keyName = "Sinal Escolar";

            var key = Registry.CurrentUser.OpenSubKey(subKeyPath, true);

            if (key != null)
            {
                object o = key.GetValue(keyName);
                if (o != null)
                {
                    // Atualiza o caminho do programa caso ele
                    // seja diferente do que foi salvo no registro.
                    if (o.ToString() != $"\"{Application.StartupPath}\"")
                        key.SetValue(keyName, $"\"{Application.ExecutablePath}\"");
                }
                else
                {
                    // Cria o objeto na chave caso ele não exista.
                    var result = MessageBox.Show("Deseja iniciar o programa automaticamente ao ligar o computador?", "Sinal Escolar", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                        key.SetValue(keyName, $"\"{Application.ExecutablePath}\"");
                }

                key.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using(var addForm = new AddForm(this))
            {
                addForm.ShowDialog();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show("Caso o programa seja fechado, o sinal não tocará mais.\nDeseja continuar?", "Sinal Escolar", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes)
                e.Cancel = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var item in _alarms)
            {
                var date = DateTime.Now;
                MessageBox.Show($"id: {item.Id}\n" +
                    $"dia {item.Day}\n" +
                    $"time: {item.Time}\n" +
                    $"song: {item.Song}\n" +
                    $"today: {date.DayOfWeek}, {date.Hour}:{date.Minute}");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Loopa por todos os alarmes e verifica qual que deve tocar
            foreach(var alarm in _alarms)
            {
                // Se o dia do alarme tocar foi igual ao dia de hoje,
                // e a hora do alarme for igual a hor atual...
                var date = DateTime.Now;
                if (alarm.Day == _dayOfWeek && alarm.Time == $"{date.Hour}:{date.Minute}")
                {
                    // Verifica se o alarme ainda NÃO foi tocado e o toca
                    if (!_lastAlarmIds.Contains(alarm.Id))
                    {
                        _lastAlarmIds.Add(alarm.Id);
                        PlayAlarm(alarm.Song, alarm.IntervalInSeconds);
                    }
                }
            }

        }

        private void PlayAlarm(string songPath, int segs)
        {
            MediaPlayer.Play(songPath);

            // Chama o timer pra parar de tocar depois de X segundos
            stopSongTimer.Interval = segs * 1000;
            stopSongTimer.Start();
        }

        private void stopSongTimer_Tick(object sender, EventArgs e)
        {
            stopSongTimer.Stop();
            MediaPlayer.StopPlaying();
        }
    }
}
