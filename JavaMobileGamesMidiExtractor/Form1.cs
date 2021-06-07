using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JavaMobileGamesMidiExtractor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //int i = MidiExtractor.ExtractFromFile(@"D:\Christoph\Dokumente\DigitalChoclate\3dbrickbre_pd6of3ti\r2", @"D:\Christoph\Dokumente\DigitalChoclate\3dbrickbre_pd6of3ti");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            button2.Enabled = false;
            listBox1.Items.Clear();
            Thread ex = new Thread(ExtractThread);
            ex.Start();
        }
        public void ExtractThread()
        {
            string tmp = Path.GetTempPath() + "\\xme" + Guid.NewGuid().ToString();
            if (!Directory.Exists(tmp))
            {
                Directory.CreateDirectory(tmp);
            }
            foreach (string jar in Directory.GetFiles(textBox1.Text, "*.jar", SearchOption.AllDirectories))
            {
                try
                {
                    string jtemp = tmp + "\\" + Path.GetFileNameWithoutExtension(jar);
                    string jarName = Path.GetFileNameWithoutExtension(jar).Replace('_', ' ');
                    if (jarName.Contains("-")) jarName = jarName.Replace(jarName.Split('-').Last(), "");
                    string output = textBox2.Text + "\\" + jarName.Replace('-', ' ').Trim();
                    if (!Directory.Exists(jtemp)) Directory.CreateDirectory(jtemp);
                    if (!Directory.Exists(output)) Directory.CreateDirectory(output);
                    ZipFile.ExtractToDirectory(jar, jtemp);
                    int found = 0;
                    foreach (string data in Directory.GetFiles(jtemp, "*", SearchOption.AllDirectories))
                    {
                        found += MidiExtractor.ExtractFromFile(data, output);
                    }
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        listBox1.Items.Add("JAR: " + Path.GetFileName(jar) + " -> MIDI FILES FOUND: " + found.ToString());
                    });
                    if (found == 0) Directory.Delete(output);
                }
                catch(Exception ex)
                {
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        listBox1.Items.Add("JAR: " + Path.GetFileName(jar) + " -> ERROR: " + ex.Message);
                    });
                }
            }
            this.BeginInvoke((MethodInvoker)delegate
            {
                listBox1.Items.Add("DELETE TMP FILES!");
            });
            Directory.Delete(tmp, true);
            this.BeginInvoke((MethodInvoker)delegate
            {
                listBox1.Items.Add("COMPLETE!");
                button3.Enabled = true;
                button2.Enabled = true;
            });
        }
    }
}
