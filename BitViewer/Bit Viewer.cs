﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace BitViewer
{
    public partial class Form1 : Form
    {
        BitArray gBits = null;
        uint BASIC_BIT_SIZE = 10;
        uint BASIC_BORDER_SIZE = 2;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void LoadBits_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                gBits = new BitArray(File.ReadAllBytes(openFileDialog1.FileName));
                RefreshBMP();
            }

        }

        private void RefreshBMP()
        {
            if ((gBits == null) || (readFileOffset.Value >= gBits.Length))
            {
                // nothing to show
                ImagePanel.BackgroundImage = new Bitmap(1, 1);
                return;
            }

            // set cursor to waiting
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();

            uint bitSizeInPixels = BASIC_BIT_SIZE * (uint)bitSize.Value;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // configure the scroll bars
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            uint visibleBitsPerLine = (uint)ImagePanel.Width / (BASIC_BORDER_SIZE + bitSizeInPixels);
            uint visibleNumLines = (uint)ImagePanel.Height / (BASIC_BORDER_SIZE + BASIC_BIT_SIZE * (uint)bitSize.Value);

            uint bitsPerLine = (uint)FrameSize1.Value * (uint)FrameSize2.Value;
            uint numLines = ((uint)gBits.Length - (uint)readFileOffset.Value + bitsPerLine - 1) / bitsPerLine;

            // the maximum should be the number of bits we're not seeing
            if (bitsPerLine > visibleBitsPerLine)
            {
                hScrollBar1.Maximum = (int)((uint)FrameSize1.Value * (uint)FrameSize2.Value - visibleBitsPerLine);
                //hScrollBar1.Value = 0;
                hScrollBar1.Visible = true;
            }
            else
            {
                hScrollBar1.Visible = false;
            }

            if (numLines > visibleNumLines)
            {
                vScrollBar1.Maximum = (int)(numLines - visibleNumLines);
                //vScrollBar1.Value = 0;
                vScrollBar1.Visible = true;
            }
            else
            {
                vScrollBar1.Visible = false;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Done with the scroll bars
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // update numLines to include the scrolled lines
            numLines -= (uint)vScrollBar1.Value;

            Bitmap bitsBitmap = new Bitmap(ImagePanel.Width, ImagePanel.Height);
            
            using (Graphics g = Graphics.FromImage(bitsBitmap))
            using (SolidBrush blueBrush = new SolidBrush(Color.Blue))
            using (SolidBrush whiteBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(whiteBrush, 0, 0, ImagePanel.Width, ImagePanel.Height);

                IEnumerator iterator = gBits.GetEnumerator();
                iterator.MoveNext(); // now points to the first element

                // skip bits as needed
                for (int i = 0; i < (readFileOffset.Value + bitsPerLine * vScrollBar1.Value); ++i)
                {
                    iterator.MoveNext();
                }

                // read all the complete lines (except the last, possibly incomplete, line)
                for (int y = 0; y < (numLines - 1); ++y)
                {
                    for (int x = 0; x < bitsPerLine; ++x)
                    {
                        if ((bool)iterator.Current)
                        {
                            // draw a pixel
                            g.FillRectangle(blueBrush,
                                x * (bitSizeInPixels + BASIC_BORDER_SIZE),
                                y * (bitSizeInPixels + BASIC_BORDER_SIZE),
                                bitSizeInPixels,
                                bitSizeInPixels);
                        }

                        iterator.MoveNext();
                    }
                }

                // read the last, possibly incomplete, line
                for (int x = 0; x < (gBits.Length % bitsPerLine); ++x)
                {
                    if ((bool)iterator.Current)
                    {
                        // draw a pixel
                        g.FillRectangle(blueBrush,
                            x * (bitSizeInPixels + BASIC_BORDER_SIZE),
                            (numLines - 1) * (bitSizeInPixels + BASIC_BORDER_SIZE),
                            bitSizeInPixels,
                            bitSizeInPixels);
                    }

                    if (!iterator.MoveNext())
                    {
                        break;
                    }
                }
            }

            // set cursor to normal
            Cursor.Current = Cursors.Default;
            Application.DoEvents();

            // display image
            // ImagePanel.Height = (int)(numLines * bitSizeInPixels);
            // ImagePanel.Width = (int)(bitSizeInPixels * bitsPerLine);
            ImagePanel.BackgroundImage = bitsBitmap;
            
        }

        private void FrameSize1_ValueChanged(object sender, EventArgs e)
        {
            RefreshBMP();
        }

        private void FrameSize2_ValueChanged(object sender, EventArgs e)
        {
            RefreshBMP();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            RefreshBMP();
        }

        private void btnHeyLena_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                // Stream st = new MemoryStream(File.ReadAllBytes(openFileDialog1.FileName));
                // BitsPicture.Image = new Bitmap(st);
            }
        }

        private void numericUpDown1_ValueChanged_1(object sender, EventArgs e)
        {
            RefreshBMP();
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            RefreshBMP();
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (vScrollBar1.Value != e.NewValue)
            {
                vScrollBar1.Value = e.NewValue;
                RefreshBMP();
            }
            
        }

    }
}
