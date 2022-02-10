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

        private AddForm _addForm;

        public Form1()
        {
            InitializeComponent();
        }

        public void AddAlarm(Alarm alarm)
        {
            foreach (var item in _alarms)
            {
                if ((item.Day == alarm.Day && item.Time == alarm.Time) ||
                    (item.Day == "Daily" && item.Time == alarm.Time))
                {
                    Dialog.Show("Já existe um alarme com esse dia e horário cadastrado!", false, EMessageCode.Exclamation);
                    return;
                }
            }

            this._alarms.Add(alarm);
            _addForm.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Seta o programa pra iniciar com o windows
            StartWithWindows();

            // TODO: Carregar os alarmes salvos de um arquivo

            // TODO: Atualizar a lista exibindo o status dos alarmes
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
                    var result = Dialog.Show("Deseja iniciar o programa automaticamente ao ligar o computador?", true);
                    if (result == DialogResult.Yes)
                        key.SetValue(keyName, $"\"{Application.ExecutablePath}\"");
                }

                key.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _addForm = new AddForm(this);
            _addForm.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = Dialog.Show("Caso o programa seja fechado, o sinal não tocará mais.\nDeseja continuar?", true);
            if (result != DialogResult.Yes)
                e.Cancel = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Loopa por todos os alarmes e verifica qual que deve tocar
            for (int i = 0; i < _alarms.Count; i++)
            {
                // Se a hora do alarme bater com a hora atual
                // E o dia do alarme for "diário" ou também bater com
                // o dia atual...
                var date = DateTime.Now;
                if (_alarms[i].Time == $"{date.Hour}:{date.Minute}" &&
                    (_alarms[i].Day == _dayOfWeek || _alarms[i].Day == "Daily"))
                {
                    // Verifica se o alarme ainda NÃO foi tocado e o toca
                    if (!_lastAlarmIds.Contains(_alarms[i].Id))
                    {
                        PlayAlarm(_alarms[i]);
                        _alarms[i].bPlayed = true;

                        // TODO: Atualizar a lista exibindo o status dos alarmes
                    }
                }
            }
        }

        private void PlayAlarm(Alarm alarm)
        {
            _lastAlarmIds.Add(alarm.Id);
            MediaPlayer.Play(alarm.Song);

            // Chama o timer pra parar de tocar depois de X segundos
            if (alarm.IntervalInSeconds > 0)
            {
                stopSongTimer.Interval = alarm.IntervalInSeconds * 1000;
                stopSongTimer.Start();
            }
        }

        private void stopSongTimer_Tick(object sender, EventArgs e)
        {
            stopSongTimer.Stop();
            MediaPlayer.StopPlaying();
        }
    }
}
