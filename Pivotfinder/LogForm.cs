using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Pivotfinder
{
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
        }

        public void AddMessagesToListBox(List<string> messages)
        {
            foreach (string message in messages)
            {
                listBoxLog.Items.Add(message);
            }
        }

        private void buttonLogExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listBoxLog_KeyDown(object sender, KeyEventArgs e)
        {
            // Überprüfen, ob Strg + C gedrückt wurde
            if (e.Control && e.KeyCode == Keys.C)
            {
                // Kopiere den ausgewählten Text der ListBox in die Zwischenablage
                if (listBoxLog.SelectedItem != null)
                {
                    Clipboard.SetText(listBoxLog.SelectedItem.ToString());
                }
            }
        }
    }

}
