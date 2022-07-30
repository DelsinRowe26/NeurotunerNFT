using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleNeurotuner
{
    public class TembroClass
    {
        private static int[] minfreq = new int[48000];
        private static int[] maxfreq = new int[48000];
        private static int[] coef = new int[48000];
        public static int[] kt = new int[48000]; 
        private static StreamReader fileName = new StreamReader("Wide_voiceTurbo.txt", System.Text.Encoding.Default);
        private static int Nlines = File.ReadAllLines("Wide_voiceTurbo.txt").Length;
        private static string[] txt = fileName.ReadToEnd().Split(new char[] { ' ', '.' }, StringSplitOptions.None);

        public static void Tembro(int sampleRate)
        {
            for (int k = 0; k < 48000; k++)
            {
                kt[k] = 1;
            }

            for (int p = 0; p < Nlines; p++)
            {
                for (int q = 0, r = 1, e = 2; q < Nlines && r < Nlines && e < Nlines; q += 3, r += 3, e += 3)
                {
                    minfreq[p] = int.Parse(txt[q]);
                    maxfreq[p] = int.Parse(txt[r]);
                    coef[p] = int.Parse(txt[e]);
                }
            }

            for (int t = 0; t < Nlines; t++)
            {
                for (int k = minfreq[t]; k < maxfreq[t]; k++)
                {
                    kt[k] *= coef[t];
                    kt[(int)sampleRate - k] *= coef[t];
                }
            }
        }
    }
}
