using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpringPendulum
{
    class SpringPendulumObject
    {
        bool showSpringLen = true;


        public bool ShowSpringLen
        {
            get { return showSpringLen; }
            set { showSpringLen = value; }
        }
        bool showVetricalAngle = false;

        public bool ShowVetricalAngle
        {
            get { return showVetricalAngle; }
            set { showVetricalAngle = value; }
        }
        bool showAngleSpeed = false;

        public bool ShowAngleSpeed
        {
            get { return showAngleSpeed; }
            set { showAngleSpeed = value; }
        }
        bool showSpringSpeed = true;

        public bool ShowSpringSpeed
        {
            get { return showSpringSpeed; }
            set { showSpringSpeed = value; }
        }
        double springLenght = 2;

        public double SpringLenght
        {
            get { return springLenght; }
            set { springLenght = value; }
        }
        double springHooke = 10;

        public double SpringHooke
        {
            get { return springHooke; }
            set { springHooke = value; }
        }
        double gravityConstant = 10;

        public double GravityConstant
        {
            get { return gravityConstant; }
            set { gravityConstant = value; }
        }
        double initAngle = 1;

        public double InitAngle
        {
            get { return initAngle; }
            set { initAngle = value; }
        }
        double initVelocity = 0;

        public double InitVelocity
        {
            get { return initVelocity; }
            set { initVelocity = value; }
        }
        double objectMass = 0.05;

        public double ObjectMass
        {
            get { return objectMass; }
            set { objectMass = value; }
        }

        const int N = 4;
        double h = 0.001;
        int stepNum = 1;
        double[] innerPara = null;
        int stepCount = 10000;
        int stepIndex = 0;
        double[][] historyBuffer = null;

        public double[][] HistoryBuffer
        {
            get { return historyBuffer; }
            set { historyBuffer = value; }
        }

        public SpringPendulumObject()
        {
            innerPara = new double[N];
            historyBuffer=  new double[N][];
            for (int i = 0; i < historyBuffer.Length;i ++ )
            {
                historyBuffer[i] = new double[stepCount];
            }
            Reset();
        }
        public double Y
        {
            get
            {
                
                return innerPara[2] * 100 * Math.Cos(innerPara[0]);
            }
        }
        public double X
        {
            get
            {
                return innerPara[2] * 100 * Math.Sin(innerPara[0]);
            }
        }
        public void Reset()
        {
            for (int i = 0; i < historyBuffer.Length;i ++ )
            {
                for (int k = 0; k < historyBuffer[i].Length; k ++ )
                {
                    historyBuffer[i][k] = 0;
                }
            }
            stepNum = 1;
            stepIndex = 0;
            innerPara[0] = initAngle;
            innerPara[1] = initVelocity;
            innerPara[2] = (springLenght * springHooke + objectMass * gravityConstant * Math.Cos(initAngle)) / (springHooke - objectMass * innerPara[1] * innerPara[1]);
            innerPara[3] = 0;
        }
        public void Step(int n)
        {
            for (int i = 0; i < n;i ++ )
            {
                RK4Compute(stepNum++, ref innerPara);
            }
        }
        private void RK4Compute(int i,ref double[] y)
        {
            double[] k1 = new double[N];
            double[] k2 = new double[N];
            double[] k3 = new double[N];
            double[] k4 = new double[N];
            //double[] y = new double[N];
            double[] t1 = new double[N];
            double[] t2 = new double[N];
            double[] t3 = new double[N];
           
            double x = (i - 1) * h;

            for (int k = 0; k < N; k++)
            {
                k1[k] = h * div(x, y, k);
                t1[k] = y[k] + 0.5 * k1[k];
            }
            for (int k = 0; k < N; k++)
            {
                k2[k] = h * div(x + 0.5 * h, t1, k);
                t2[k] = y[k] + 0.5 * k2[k];
            }
            for (int k = 0; k < N; k++)
            {
                k3[k] = h * div(x + 0.5 * h, t2, k);
                t3[k] = y[k] + k3[k];
            }
            for (int k = 0; k < N; k++)
            {
                k4[k] = h * div(x + h, t3, k);
            }
            for (int k = 0; k < N; k ++)
            {
                y[k] += (k1[k] + 2 * k2[k] + 2 * k3[k] + k4[k]) / 6.0;
            }
            for (int k = 0; k < N;  k ++)
            {
                historyBuffer[k][stepIndex] = y[k];
            }
            if (stepIndex == stepCount - 1)
            {
                for (int j = 0; j < historyBuffer.Length;j ++ )
                {
                    for (int k = 0; k < historyBuffer[j].Length - 1; k ++ )
                    {
                        historyBuffer[j][k] = historyBuffer[j][k + 1];
                    }
                }
            }
            else
                stepIndex++;
            //stepIndex %= stepCount;
          //  thetaBuffer[i] = thetaBuffer[i - 1] + (k1[0] + 2 * k2[0] + 2 * k3[0] + k4[0]) / 6.0;
         //   vBuffer[i] = vBuffer[i - 1] + (k1[1] + 2 * k2[1] + 2 * k3[1] + k4[1]) / 6.0;
        }

        /// <summary>
        /// 1 = theta
        /// 2 = theta/ t
        /// 3 = lenght
        /// 4 = length / t
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private double div(double x, double[] y, int i)
        {
            switch (i)
            {
                case 0:
                    {
                        return y[1];

                    }
                case 1:
                    {
                        return -gravityConstant * Math.Sin(y[0]) / y[2];
                    }
                case 2:
                    {
                        return y[3];
                    }
                case 3:
                    {
                        double sh = springHooke;
                        if (y[2] > 1.3 * springLenght)
                        {
                            sh *= Math.Pow(5, y[2] / springLenght);
                        }
                        if (y[2] < 0.7 * springLenght)
                        {
                            sh *= Math.Pow(5, springLenght / y[2] );
                        }
                        return (gravityConstant * Math.Cos(y[0]) - sh * (y[2] - springLenght) / objectMass);
                    }
                default:
                    throw new Exception("Error");
            }
        }
    }
}
