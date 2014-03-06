using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;

using NatNetML;

namespace Recognizer.HMM
{
    class Optitrack
    {
        // [NatNet] Our NatNet object
        private NatNetML.NatNetClientML _NatNet;
        // [NatNet] Our NatNet Frame of Data object
        private NatNetML.FrameOfMocapData _FrameOfData = new NatNetML.FrameOfMocapData();

        private float xOffSet = 0, yOffSet = 0;
        private int scale = 1;
        private int frameCnt = 0;
        private const int MinNoPoints = 5;

        //HiResTimer timer;
        //Int64 lastTime = 0;

        //private Queue<NatNetML.FrameOfMocapData> m_FrameQueue = new Queue<NatNetML.FrameOfMocapData>();
        private ArrayList _points;

        public Optitrack(ArrayList points)
        {
            _points = points;
            ConnectMotive();
        }

        private void ConnectMotive()
        {
            if (_NatNet != null)
            {
                _NatNet.Uninitialize();
            }
            //timer = new HiResTimer();
            _NatNet = new NatNetML.NatNetClientML(0);

            // [NatNet] set a "Frame Ready" callback function (event handler) handler that will be
            // called by NatNet when NatNet receives a frame of data from the server application
            _NatNet.OnFrameReady += new NatNetML.FrameReadyEventHandler(m_NatNet_OnFrameReady);

            // [NatNet] connect to a NatNet server
            string strLocalIP = "127.0.0.1";
            string strServerIP = "127.0.0.1";

            int code = _NatNet.Initialize(strLocalIP, strServerIP);


        }

        private void ConnectMotive(string strLocalIP, string strServerIP)
        {
            if (_NatNet != null)
            {
                _NatNet.Uninitialize();
            }
            //timer = new HiResTimer();
            _NatNet = new NatNetML.NatNetClientML(0);

            // [NatNet] set a "Frame Ready" callback function (event handler) handler that will be
            // called by NatNet when NatNet receives a frame of data from the server application
            _NatNet.OnFrameReady += new NatNetML.FrameReadyEventHandler(m_NatNet_OnFrameReady);

            //// [NatNet] connect to a NatNet server
            //string strLocalIP = "127.0.0.1";
            //string strServerIP = "127.0.0.1";

            int code = _NatNet.Initialize(strLocalIP, strServerIP);
        }

        // [NatNet] m_NatNet_OnFrameReady will be called when a frame of Mocap
        // data has is received from the server application.
        //
        // Note: This callback is on the network service thread, so it is
        // important  to return from this function quickly as possible 
        // to prevent incoming frames of data from buffering up on the
        // network socket.
        //
        // Note: "data" is a reference structure to the current frame of data.
        // NatNet re-uses this same instance for each incoming frame, so it should
        // not be kept (the values contained in "data" will become replaced after
        // this callback function has exited).
        private void m_NatNet_OnFrameReady(NatNetML.FrameOfMocapData data, NatNetML.NatNetClientML client)
        {
            //Int64 currTime = timer.Value;
            //if (lastTime != 0)
            //{
            //    // Get time elapsed in tenths of a millisecond.
            //    Int64 timeElapsedInTicks = currTime - lastTime;
            //    Int64 timeElapseInTenthsOfMilliseconds = (timeElapsedInTicks * 10000) / timer.Frequency;

            //}

                //m_FrameQueue.Clear();
                //m_FrameQueue.Enqueue(data);
            lock (_points)
            {


                int n = data.nOtherMarkers;
                //if (_isDown)
                //{
                    while (n > 0)
                    {
                        float x = 0;
                        float y = 0;

                        x = -data.OtherMarkers[n - 1].z;
                        y = -data.OtherMarkers[n - 1].y;
                        if (0 == frameCnt)
                        {
                            scale = Math.Abs((int)(300 / y));
                            x = x * scale;
                            y = y * scale;
                            xOffSet = 1200 / 2 - x;
                            yOffSet = 720 / 2 - y;
                        }
                        else
                        {
                            //lblResult.Text = "x:" + x+"y:"+y;
                            x = x * scale + xOffSet;
                            y = y * scale + yOffSet;
                            //lock (_points)
                            //{

                            //    //pointNow = new TimePointF(x, y, TimeEx.NowMs);
                            //    //if (Math.Sqrt(Math.Pow((pointNow.X - x), 2) + Math.Pow((pointNow.Y - y), 2)) > 5)
                            //    //{
                            //    _points.Add(new TimePointF(x, y, TimeEx.NowMs));
                            //    //}
                            _points.Add(new PointR(x,y));
                            //    Invalidate(new Rectangle((int)x - 2, (int)y - 2, 4, 4));
                            //}
                        }
                        frameCnt++;
                        n--;
                    }
            }

            //    }
          //  }
           // lastTime = currTime;

        }

        private void Disconnect()
        {
            // [NatNet] disconnect

            _NatNet.Uninitialize();
        }
    }
}
