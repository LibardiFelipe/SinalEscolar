using Microsoft.Win32;
using SinalEscolar.Classes;
using SinalEscolar.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        private readonly string _dayOfWeek = DateTime.Now.DayOfWeek.ToString();

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

            _alarms.Add(alarm);
            _addForm.Close();

            // Atualiza a lista com os alarmes
            UpdateListView();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Seta o programa pra iniciar com o windows
            StartWithWindows();

            // TODO: Carregar os alarmes salvos de um arquivo

            // Atualiza a lista com os alarmes
            UpdateListView();
        }

        private void UpdateListView()
        {
            listView1.Items.Clear();

            foreach(var item in _alarms)
            {
                string played = item.bPlayed ? "Tocado" : "Em espera";
                string[] row = { item.Time, ToPtBr(item.Day), ToSafeFileName(item.Song), ToTime(item.IntervalInSeconds), $"{played}"};
                var li = new ListViewItem(row);
                listView1.Items.Add(li);
            }
        }

        private string ToTime(int seconds)
        {
            if (seconds == 0)
                return "Até acabar";

            return $"{seconds} segundos";
        }

        private string ToSafeFileName(string path)
        {
            var fileName = Path.GetFileName(path);
            // Remove o . + a extensão do nome do arquivo
            // (suponhando que ele seja .xxx que é o que todos devem ser né...)
            fileName = fileName.Remove(fileName.Length - 4, 4);
            return fileName;
        }

        private string ToPtBr(string day)
        {
            switch (day)
            {
                case "Daily":
                    return "Diário";
                case "Monday":
                    return "Segunda-feira";
                case "Tuesday":
                    return "Terça-feira";
                case "Wednesday":
                    return "Quarta-feira";
                case "Thursday":
                    return "Quinta-feira";
                case "Friday":
                    return "Sexta-feira";
                case "Saturday":
                    return "Sábado";
                case "Sunday":
                    return "Domingo";
                default:
                    return "BAD_DATE";
            }
        }

        public void StartWithWindows()
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
                var hour = string.Format("{0:00}", date.Hour);
                var minutes = string.Format("{0:00}", date.Minute);
                if (_alarms[i].Time == $"{hour}:{minutes}" &&
                    (_alarms[i].Day == _dayOfWeek || _alarms[i].Day == "Daily"))
                {
                    // Verifica se o alarme ainda NÃO foi tocado e o toca
                    if (!_lastAlarmIds.Contains(_alarms[i].Id))
                    {
                        PlayAlarm(_alarms[i]);
                        _alarms[i].bPlayed = true;

                        // Atualiza a lista com os alarmes
                        UpdateListView();
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
