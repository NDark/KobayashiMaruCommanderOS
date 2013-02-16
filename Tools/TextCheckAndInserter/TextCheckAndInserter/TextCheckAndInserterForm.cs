/*
 * @file TextCheckAndInserterForm.cs
 * 
 * @date 20130213 by NDark . add unicode encoding at CheckFile()
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TextCheckAndInserter
{
    public partial class TextCheckAndInserterForm : Form
    {
        private string m_TargetText = "" ;

        public TextCheckAndInserterForm()
        {
            InitializeComponent();
        }

        private void bTargetTextFilepath_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                tBoxTargetTextFilepath.Text = openFileDialog1.FileName;
                LoadTargetText(tBoxTargetTextFilepath.Text);
            }
        }

        private void tBoxTargetTextFilepath_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void tBoxTargetTextFilepath_DragDrop(object sender, DragEventArgs e)
        {
            // Ensure that the list item index is contained in the data.
            Array filelist = (Array)e.Data.GetData(DataFormats.FileDrop);
            foreach (string str in filelist)
            {
                tBoxTargetTextFilepath.Text = str;
                LoadTargetText(tBoxTargetTextFilepath.Text);
            }
        }

        private void LBoxExtention_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (null == LBoxExtention.SelectedItem)
                return;
            LBoxExtention.Items.Remove(LBoxExtention.SelectedItem);
        }

        private void tBoxExtenstion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (13 == e.KeyChar)
            {
                string addStr = tBoxExtenstion.Text;
                if (tBoxExtenstion.Text.Length == 0)
                    return;

                if (-1 == addStr.IndexOf("."))
                {
                    addStr = "." + addStr;
                }
                addStr = addStr.ToLower();
                LBoxExtention.Items.Add(addStr);
                tBoxExtenstion.Clear();
            }
        }

        private void TextCheckAndInserterForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ( 27 == e.KeyChar)
            {
                Application.Exit();
            }
        }

        private void TextCheckAndInserterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult.Yes != MessageBox.Show("請問是否確認關閉程式", "警告", MessageBoxButtons.YesNo))
            {
                e.Cancel = true;
            }
        }

        private void TextCheckAndInserterForm_Load(object sender, EventArgs e)
        {
            LBoxExtention.Items.Clear();
            LBoxExtention.Items.Add(".cs");
            tBoxCompareLine.Text = "3";
            tBoxInserLine.Text = "0" ;

            folderBrowserDialog1.SelectedPath = Environment.CurrentDirectory;
            openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
        }

        private void LoadTargetText( string _filepath )
        {
            StreamReader SR = new StreamReader(_filepath);
            if (null == SR)
                return;
            string content = SR.ReadToEnd();
            m_TargetText = content;
            if( -1 == content.IndexOf( "\r\n" ) )
                content = content.Replace("\n", Environment.NewLine);
            tBoxTargetText.Text = content;
            SR.Close();
        }

        private void bDirectory_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == this.folderBrowserDialog1.ShowDialog())
            {
                this.tBoxSetDirectory.Text = folderBrowserDialog1.SelectedPath;                
            }
        }

        private void bExcute_Click(object sender, EventArgs e)
        {
            if( 0 != tBoxTargetText.Text.Length &&
                0 != tBoxSetDirectory.Text.Length)
                Excute();
        }

        private void Excute()
        {
            string insertText = m_TargetText;
            string[] targetTextVec = insertText.Split('\n');
            foreach (string str in targetTextVec)
            {
                str.Trim();
            }

            Queue<string> directories = new Queue<string>();
            directories.Enqueue( tBoxSetDirectory.Text );
            tBoxStatus.Text += ("add directory" + tBoxSetDirectory.Text + Environment.NewLine);
            while (directories.Count > 0)
            {
                string directory = directories.Dequeue();
                if (false == Directory.Exists(directory))
                    continue;

                DirectoryInfo DI = new DirectoryInfo(directory);
                FileInfo[] FIs = DI.GetFiles();
                foreach (FileInfo fi in FIs)
                {
                    if( -1 != LBoxExtention.Items.IndexOf( fi.Extension ) )
                        CheckFile(targetTextVec, insertText , fi.FullName);
                }
                DirectoryInfo[] DIs = DI.GetDirectories();
                foreach( DirectoryInfo di in DIs )
                {
                    tBoxStatus.Text += ("add directory" + di.FullName + Environment.NewLine);
                    directories.Enqueue(di.FullName);
                }
            }
        }

        private void CheckFile(string[] _targetTextVec, string _insertWholeStr ,string _testFilepath)
        {
            tBoxStatus.Text += (_testFilepath + Environment.NewLine);

            StreamReader SR = new StreamReader(_testFilepath);
            string wrtieFilepath = _testFilepath + ".tmp" ;
            if (null == SR )
                return;

            string CheckLineStr = tBoxCompareLine.Text;
            int CheckLineNum = 0;
            int.TryParse(CheckLineStr, out CheckLineNum);

            string InsertLineStr = tBoxInserLine.Text;
            int InsertLineNum = 0;
            int.TryParse(InsertLineStr, out InsertLineNum);

            int CheckLineSize = CheckLineNum;

            if (0 == CheckLineNum)
                CheckLineSize = _targetTextVec.Length;

            bool found = true;            

            string StrRead = SR.ReadToEnd();
            for (int i = 0; i < CheckLineSize; ++i)
            {
                if (-1 == StrRead.IndexOf(_targetTextVec[i]))
                {
                    found = false;
                    break;
                }
            }

            SR.Close();

            bool valid = false;
            if (false == found)
            {
                // find index
                int tmpIndex = 0 ;
                int insertIndex = 0;
                for (int i = 0; i < InsertLineNum; ++i)
                {
                    tmpIndex = StrRead.IndexOf('\n', tmpIndex ) + 1 ;
                }

                if (-1 != tmpIndex)
                {
                    StreamWriter SW = new StreamWriter(wrtieFilepath , false  , Encoding.Unicode);
                    if ( null != SW)
                    {
                        string StrWrite = StrRead.Insert(insertIndex, _insertWholeStr);
                        SW.Write(StrWrite);
                        SW.Close();
                        valid = true;
                    }
                }
            }

            

            if (true == valid)
            {
                tBoxStatus.Text += ("ReplaceCompleted" + Environment.NewLine);
                File.Delete(_testFilepath);
                File.Copy(wrtieFilepath, _testFilepath);
                File.Delete(wrtieFilepath);
            }
        }
    }
}
