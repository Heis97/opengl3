using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.UI;

namespace opengl3
{
    public class Scanner
    {
        LaserSurface laserSurface;
        public PointCloud pointCloud;
        CameraCV cameraCV;
        StereoCamera stereoCamera;


        public LinearAxis linearAxis;

        public Scanner(CameraCV cam)
        {
            cameraCV = cam;
            laserSurface = new LaserSurface();
            pointCloud = new PointCloud();
            linearAxis = new LinearAxis();
            
        }
        public Scanner(CameraCV[] cameraCVs)
        {
            laserSurface = new LaserSurface();
            pointCloud = new PointCloud();
            linearAxis = new LinearAxis();
            stereoCamera = new StereoCamera(cameraCVs);

        }
        public void initStereo(Mat[] mats, PatternType patternType)
        {
            stereoCamera.calibrate(mats, patternType);
        }

        public void clearPoints()
        {
            pointCloud.clearPoints();
        }
        public bool calibrateLaser(Mat[] mats,PatternType patternType,GraphicGL graphicGL = null)
        {
            return laserSurface.calibrate(mats,null, cameraCV, patternType, graphicGL);
        }

        public bool calibrateLinear(Mat[] mats, double[] positions, PatternType patternType, GraphicGL graphicGL = null)
        {
            return linearAxis.calibrate(mats, positions, cameraCV, patternType, graphicGL);
        }

        public bool calibrateLinearLas(Mat[][] mats, Mat[] origs, double[] positions, PatternType patternType, GraphicGL graphicGL = null)
        {
            return linearAxis.calibrateLas(mats, origs, positions, cameraCV, patternType, graphicGL);
        }


        /// <summary>
        /// one line
        /// </summary>
        /// <param name="mats"></param>
        public void addPointsStereoLas(Mat[] mats, bool undist = true)
        {
            pointCloud.addPointsStereoLas(mats, stereoCamera, undist);
        }

       /* public void addPointsStereoLas(Mat[][] mats)
        {
            for(int i=0;i<mats.Length;i++)
            {
                Console.WriteLine("loading...      "+i+"/"+mats.Length);
                pointCloud.addPointsStereoLas(mats[i], stereoCamera);
            }            
        }*/

        public bool addPoints(Mat mat)
        {
            return pointCloud.addPoints(mat, cameraCV, laserSurface);
        }
        public bool addPointsLin(Mat mat, double linPos)
        {
            return pointCloud.addPointsLin(mat, linPos,  cameraCV, laserSurface,linearAxis);
        }

        public bool addPointsLinLas(Mat mat, double linPos,Mat orig, PatternType patternType)
        {
            return pointCloud.addPointsLinLas(mat, linPos, cameraCV, linearAxis,orig, patternType);
        }
        public int addPointsLinLas(Mat[] mats, double[] linPos,Mat orig, PatternType patternType)
        {
            int ret = 0;
            if (mats.Length != linPos.Length)
            {
                return 0;
            }
            for (int i = 0; i < mats.Length; i++)
            {
                if (addPointsLinLas(mats[i] - orig, linPos[i],orig,patternType))
                {
                    ret++;
                }
            }
            return ret;
        }
        public int addPointsLin(Mat[] mats, double[] linPos)
        {
            int ret = 0;
            if(mats.Length!=linPos.Length)
            {
                return 0;
            }
            for(int i=0; i<mats.Length;i++)
            {
                if(addPointsLin(mats[i],linPos[i]))
                {
                    ret++;
                }
            }
            return ret;
        }
        public bool addPoints(Mat[] mats)
        {
            bool ret = false;
            foreach(var mat in mats)
            {
                ret = addPoints(mat);
            }
            return ret;
        }
        public Point3d_GL[] getPointsScene()
        {
            return pointCloud.points3d;
        }

        public Point3d_GL[][] getPointsLinesScene()
        {
            return pointCloud.points3d_lines.ToArray();
        }

        public Point3d_GL[] getPointsCam()
        {
            if(stereoCamera!=null)
            {
                return Point3d_GL.multMatr(pointCloud.points3d, stereoCamera.cameraCVs[0].matrixSC);
            }
            return Point3d_GL.multMatr(pointCloud.points3d,cameraCV.matrixSC);
        }
        public Point3d_GL[][] getPointsLinesCam()
        {
            return Point3d_GL.multMatr(pointCloud.points3d_lines.ToArray(), cameraCV.matrixSC);
        }
    }
}
