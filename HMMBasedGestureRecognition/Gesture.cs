using System;
using System.Collections;

namespace Recognizer.HMM
{
	public class Gesture : IComparable
	{
        public string Name;
        public ArrayList RawPoints;     // raw points (for drawing) -- read in from XML
        public ArrayList Points;        // processed points (rotated, scaled, translated)

		public Gesture()
		{
			this.Name = String.Empty;
            this.RawPoints = null;
            this.Points = null;
		}

		public Gesture(string name, ArrayList points)
		{
			this.Name = name;
            this.RawPoints = new ArrayList(points); // copy (saved for drawing)
            this.Points = points;

            // rotate so that the centroid-to-1st-point is at zero degrees
            double radians = Utils.AngleInRadians(Utils.Centroid(Points), (PointR) Points[0], false);
            Points = Utils.RotateByRadians(Points, -radians); // undo angle

		}

        public int Duration
        {
            get
            {
                if (RawPoints.Count >= 2)
                {
                    PointR p0 = (PointR) RawPoints[0];
                    PointR pn = (PointR) RawPoints[RawPoints.Count - 1];
                    return pn.T - p0.T;
                }
                else
                {
                    return 0;
                }
            }
        }

        // sorts in descending order of Score
        public int CompareTo(object obj)
        {
            if (obj is Gesture)
            {
                Gesture g = (Gesture) obj;
                return Name.CompareTo(g.Name);
            }
            else throw new ArgumentException("object is not a Gesture");
        }

        /// <summary>
        /// Pulls the gesture name from the file name, e.g., "circle03" from "C:\gestures\circles\circle03.xml".
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ParseName(string filename)
        {
            int start = filename.LastIndexOf('\\');
            int end = filename.LastIndexOf('.');
            return filename.Substring(start + 1, end - start - 1);
        }

	}
}
