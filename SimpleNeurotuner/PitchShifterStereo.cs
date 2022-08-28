using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace SimpleNeurotuner
{
    public class PitchShifterStereo
    {
        #region Private Static Members
        private static int MAX_FRAME_LENGTH = 48000;
        private static float[] gInFIFO = new float[MAX_FRAME_LENGTH];
        private static float[] gOutFIFO = new float[MAX_FRAME_LENGTH];
        private static float[] gFFTworksp = new float[MAX_FRAME_LENGTH * 2];
        private static float[] gFFTworksp2 = new float[MAX_FRAME_LENGTH * 2];
        private static float[] gLastPhase = new float[MAX_FRAME_LENGTH / 2 + 1];
        private static float[] gSumPhase = new float[MAX_FRAME_LENGTH / 2 + 1];
        private static float[] gOutputAccum = new float[MAX_FRAME_LENGTH * 2];
        private static float[] gAnaFreq = new float[MAX_FRAME_LENGTH];
        private static float[] gAnaMagn = new float[MAX_FRAME_LENGTH];
        private static float[] gSynFreq = new float[MAX_FRAME_LENGTH];
        private static float[] gSynMagn = new float[MAX_FRAME_LENGTH];
        private static float ToneStep = (float)Math.Pow(2, 1.0 / 12);
        private static long gRover, gInit;
        #endregion

        #region Public Static Methods
        public static void PitchShift(float pitchShift, int sampleCount, float sampleRate, float[] indata)
        {
            PitchShift(pitchShift, 0, sampleCount, 4096, 4, sampleRate, indata);
        }

        public static void PitchShift(float pitchShift, int offset, int sampleCount, int fftFrameSize,
            int osamp, float sampleRate, float[] indata)
        {
            double magn, phase, tmp, window, real, imag;
            double freqPerBin, expct;
            int i, k, qpd, index, inFifoLatency, stepSize, fftFrameSize2;

            float[] outdata = indata;

            fftFrameSize2 = fftFrameSize / 2;
            stepSize = fftFrameSize / osamp;
            freqPerBin = sampleRate / (double)fftFrameSize;
            expct = 2.0 * Math.PI * stepSize / fftFrameSize;
            inFifoLatency = fftFrameSize - stepSize;
            if(gRover == 0) gRover = inFifoLatency;

            for(i = offset; i < sampleCount; i++)
            {
                gInFIFO[gRover] = indata[i];
                outdata[i] = gOutFIFO[gRover - inFifoLatency];
                gRover++;

                if(gRover >= fftFrameSize)
                {
                    gRover = inFifoLatency;

                    for(k = 0; k < fftFrameSize; k++)
                    {
                        window = -.5 * Math.Cos(2.0 * Math.PI * (double)k / (double)fftFrameSize) + .5;
                        gFFTworksp[2 * k] = (float)(gInFIFO[k] * window);
                        gFFTworksp[2 * k + 1] = 0.0f;
                    }

                    for(k = fftFrameSize; k < sampleRate; k++)
                    {
                        gFFTworksp[k * 2] = 0.0f;
                        gFFTworksp[k * 2 + 1] = 0.0f;
                    }

                    ShortTimeFourierTransform(gFFTworksp, fftFrameSize, -1);

                    for(k = 0; k < fftFrameSize2; k++)
                    {
                        real = gFFTworksp[k * 2];
                        imag = gFFTworksp[k * 2 + 1];

                        magn = Math.Sqrt(real * real + imag * imag);
                        phase = Math.Atan2(imag, real);

                        tmp = phase - gLastPhase[k];
                        gLastPhase[k] = (float)phase;

                        tmp -= k * expct;

                        qpd = (int)(tmp / Math.PI);
                        if (qpd >= 0) qpd += qpd & 1;
                        else qpd -= qpd & 1;
                        tmp -= Math.PI * (double)qpd;

                        tmp = osamp * tmp / (2.0 * Math.PI);

                        tmp = (double)k * freqPerBin + tmp * freqPerBin;

                        gAnaMagn[k] = (float)magn;
                        gAnaFreq[k] = (float)tmp;
                    }

                    for (k = 0; k < fftFrameSize; k++)
                    {
                        //gFFTworksp[k * 2] *= TembroClass.kt[k];
                        //gFFTworksp[k * 2 + 1] *= TembroClass.kt[k];
                    }


                    for(int zero = 0; zero < fftFrameSize; zero++)
                    {
                        gSynMagn[zero] = 0;
                        gSynFreq[zero] = 0;
                    }

                    for(k = 0; k < fftFrameSize2; k++)
                    {
                        index = (int)(k * pitchShift);
                        if(index <= fftFrameSize2)
                        {
                            gSynMagn[index] += gAnaMagn[k];
                            gSynFreq[index] = gAnaFreq[k] * pitchShift;
                        }
                    }


                    for(k = 0; k <= fftFrameSize2; k++)
                    {
                        magn = gSynMagn[k];
                        tmp = gSynFreq[k];

                        tmp -= (double)k * freqPerBin;

                        tmp /= freqPerBin;

                        tmp = 2.0 * Math.PI * tmp / osamp;

                        tmp += (double)k * expct;


                        gSumPhase[k] += (float)tmp;
                        phase = gSumPhase[k];

                        gFFTworksp[k * 2] = (float)(magn * Math.Cos(phase));
                        gFFTworksp[k * 2 + 1] = (float)(magn * Math.Sin(phase));
                    }

                    for(k = fftFrameSize + 2; k < fftFrameSize; k++) { gFFTworksp[k] = 0.0f; }

                    ShortTimeFourierTransform(gFFTworksp, fftFrameSize, 1);

                    for (k = 0; k < fftFrameSize; k++)
                    {
                        window = -.5 * Math.Cos(2.0 * Math.PI * (double)k / (double)fftFrameSize) + .5;
                        gOutputAccum[k] += (float)(2.0 * window * gFFTworksp[2 * k] / (fftFrameSize2 * osamp));
                    }

                    for(k = 0; k < stepSize; k++) gOutFIFO[k] = gOutputAccum[k];

                    for(k = 0; k < fftFrameSize; k++)
                    {
                        gOutputAccum[k] = gOutputAccum[k + stepSize];
                    }

                    for (k = 0; k < inFifoLatency; k++) gInFIFO[k] = gInFIFO[k + stepSize];
                }
            }
        }
        #endregion

        #region Private Static Methods
        public static void ShortTimeFourierTransform(float[] fftBuffer, long fftFrameSize, long sign)
        {
            float wr, wi, arg, temp;
            float tr, ti, ur, ui;
            long i, bitm, j, le, le2, k;

            for(i = 2; i < 2 * fftFrameSize - 2; i += 2)
            {
                for(bitm = 2, j = 0; bitm < 2 * fftFrameSize; bitm <<= 1)
                {
                    if ((i & bitm) != 0) j++;
                    j <<= 1;
                }
                if (i < j)
                {
                    temp = fftBuffer[i];
                    fftBuffer[i] = fftBuffer[j];
                    fftBuffer[j] = temp;
                    temp = fftBuffer[i + 1];
                    fftBuffer[i + 1] = fftBuffer[j + 1];
                    fftBuffer[j + 1] = temp;
                }
            }
            long max = (long)(Math.Log(fftFrameSize) / Math.Log(2.0) + .5);
            for (k = 0, le = 2; k < max; k++)
            {
                le <<= 1;
                le2 = le >> 1;
                ur = 1.0f;
                ui = 0.0f;
                arg = (float)Math.PI / (le2 >> 1);
                wr = (float)(Math.Cos(arg));
                wi = (float)(sign * Math.Sin(arg));
                for(j = 0; j < le2; j += 2)
                {
                    for(i = j; i < 2 * fftFrameSize; i += le)
                    {
                        tr = fftBuffer[i + le2] * ur - fftBuffer[i + le2 + 1] * ui;
                        ti = fftBuffer[i + le2] * ui + fftBuffer[i + le2 + 1] * ur;
                        fftBuffer[i + le2] = fftBuffer[i] - tr;
                        fftBuffer[i + le2 + 1] = fftBuffer[i + 1] - ti;
                        fftBuffer[i] += tr;
                        fftBuffer[i + 1] += ti;
                    }
                    tr = ur * wr - ui * wi;
                    ui = ur * wi + ui * wr;
                    ur = tr;
                }
            }
        }
        #endregion
    }
}
