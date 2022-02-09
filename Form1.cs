using Microsoft.Win32;
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
        public Form1()
        {
            InitializeComponent();
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
            using(var addForm = new AddForm())
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
    }
}
