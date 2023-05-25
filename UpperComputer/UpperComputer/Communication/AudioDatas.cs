using MathNet.Numerics.Data.Matlab;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using System;
using System.Numerics;

namespace UpperComputer.Communication
{
    public partial class AudioDatas
    {
        private Matrix<Complex> datas;
        private int p;

        public int Length => datas.ColumnCount;
        public readonly int Element;

        private Complex[][] output;

        public AudioDatas(int length, int element)
        {
            Element = element;
            datas = new DenseMatrix(Element, length);
            output = datas.ToRowArrays();
            p = 0;

            filters = new MathNet.Filtering.FIR.OnlineFirFilter[element];
            for (int i = 0; i < element; i++)
            {
                filters[i] = new(BDF_1600_1800_2200_2400);
            }
        }

        public void Add(double[] data)
        {
            for (int i = 0; i < datas.RowCount; i++)
            {
                datas[i, p] = filters[i].ProcessSample(data[i]);
                //datas[i, p] = data[i];
            }
            p++;

            if (p == datas.ColumnCount)
            {
                MatlabWriter.Store("D:\\record.mat",
                        new MatlabMatrix[] { MatlabWriter.Pack(datas, "X") });
                output = datas.ToRowArrays();
                p = 0;
                Console.WriteLine("done");
            }
        }

        public Complex[][] GetData()
            => output;
    }

    /*
    public class AudioDatas
    {
        private readonly double[][] dataA;
        private readonly double[][] dataB;
        private int p;

        public int Length => dataA.Length;
        public readonly int Element;

        public bool IsWritingA { get; private set; }

        public AudioDatas(int length, int element)
        {
            Element = element;
            dataA = new double[length][];
            dataB = new double[length][];
            double[] defaultValue = new double[element];
            Array.Fill(dataA, defaultValue);
            Array.Fill(dataB, defaultValue);
            p = 0;
            IsWritingA = true;
        }

        public void Add(double[] data)
        {
            if (IsWritingA)
            {
                lock (dataA)
                {
                    if (p != 0)
                    {
                        ArrayPool<double>.Shared.Return(dataA[p]);
                    }
                    dataA[p] = data;
                    p++;
                    if (p == Length)
                    {
                        p = 0;
                        IsWritingA = false;
                    }
                }
            }
            else
            {
                lock (dataB)
                {
                    if (p != 0)
                    {
                        ArrayPool<double>.Shared.Return(dataB[p]);
                    }
                    dataB[p] = data;
                    p++;
                    if (p == Length)
                    {
                        p = 0;
                        IsWritingA = true;
                    }
                }
            }
        }

        public Complex[][] GetData()
        {
            if (IsWritingA)
            {
                lock (dataB)
                {
                    Complex[][] datas = new Complex[Element][];
                    for (int e = 0; e < Element; e++)
                    {
                        Complex[] data = new Complex[Length];
                        for (int serial = 0; serial < Length; serial++)
                        {
                            data[serial] = dataB[serial][e];
                        }
                        datas[e] = data;
                    }
                    return datas;
                }
            }
            else
            {
                lock (dataA)
                {
                    Complex[][] datas = new Complex[Element][];
                    for (int e = 0; e < Element; e++)
                    {
                        Complex[] data = new Complex[Length];
                        for (int serial = 0; serial < Length; serial++)
                        {
                            data[serial] = dataA[serial][e];
                        }
                        datas[e] = data;
                    }
                    return datas;
                }
            }
        }
    }
    */

    /*
                save = new DenseMatrix(Element, 0x100);
                saveIndex = 0;
            }

    #warning 临时记录数据
            private Matrix<double> save;
            private int saveIndex;
            private void Save(double[] datas)
            {
                if (saveIndex == save.ColumnCount)
                {
                    MatlabWriter.Store("D:\\record.mat",
                        new MatlabMatrix[] { MatlabWriter.Pack(save, "X") });
                    Console.WriteLine("done");
                    saveIndex = 0;
                }
                else
                {
                    for (int i = 0; i < save.RowCount; i++)
                    {
                        save[i, saveIndex] = datas[i];
                    }
                    saveIndex++;
                }
            }
    */
}
