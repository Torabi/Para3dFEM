using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet;
using BriefFiniteElementNet.Mathh;
namespace FiniteElementMethod
{
    /// <summary>
    /// a voxel represent a cube which can contains other voxels . this is used to expedite the search for the nodes in a model
    /// </summary>
    public class Voxel
    {
        /// <summary>
        /// list of nodes which this voxel contains
        /// </summary>
        List<MAX_Node> _nodes;

        /// <summary>
        /// list of sub voxels 
        /// </summary>
        List<Voxel> _voxels;
        /// <summary>
        /// the lower left corner of the voxel
        /// </summary>
        Point _lowerLeft;
        /// <summary>
        /// the upper right corber of the voxel
        /// </summary>
        Point _upperRight;
        /// <summary>
        /// length of the voxel in X direction
        /// </summary>
        public double X { get; private set; }
        /// <summary>
        /// length of the voxel in Y direction
        /// </summary>
        public double Y { get; private set; }
        /// <summary>
        /// length of the voxel in Z direction
        /// </summary>
        public double Z { get; private set; }

        /// <summary>
        /// number of sub voxels in X direction ( always start from 1)
        /// </summary>
        public int I { get; private set; } = 1;
        /// <summary>
        /// number of sub voxels in Y direction ( always start from 1)
        /// </summary>
        public int J { get; private set; } = 1;
        /// <summary>
        /// number of sub voxels in Z direction ( always start from 1)
        /// </summary>
        public int K { get; private set; } = 1;

        /// <summary>
        /// length of the partition in X direction
        /// </summary>
        public double Dx { get; private set; }

        /// <summary>
        /// length of the partition in Y direction
        /// </summary>
        public double Dy { get; private set; }
        /// <summary>
        /// length of the partition in Z direction
        /// </summary>
        public double Dz { get; private set; }

        /// <summary>
        /// create a voxel from two opposite corner 
        /// </summary>
        /// <param name="lowerLeft"></param>
        /// <param name="upperRight"></param>
        public Voxel (Point lowerLeft, Point upperRight)
        {
            _lowerLeft = lowerLeft;
            _upperRight = upperRight;
            X = upperRight.X - lowerLeft.X;
            Y = upperRight.Y - lowerLeft.Y;
            Z = upperRight.Z - lowerLeft.Z;
            
            _voxels = new List<Voxel>();
            _nodes = new List<MAX_Node>();
        }

        /// <summary>
        /// an empty voxel, use add voxel to make it a valid voxel 
        /// </summary>
        public Voxel()
        {
            _lowerLeft = new Point(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            _upperRight = new Point(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);
            X = double.PositiveInfinity;
            Y = double.PositiveInfinity;
            Z = double.PositiveInfinity;

            _voxels = new List<Voxel>();
            _nodes = new List<MAX_Node>();
        }
        /// <summary>
        /// create voxel using the lower left corner (origin) and the lenht of the voxel in x ,y and z directions
        /// </summary>
        /// <param name="lowerLeft"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Voxel (Point lowerLeft , double x,double y, double z)
        {
            _lowerLeft = lowerLeft;
            _upperRight = new Point(_lowerLeft.X+x, _lowerLeft.Y + y, _lowerLeft.Z + z);
            X = x;Y = y;Z = z;
            _voxels = new List<Voxel>();
            _nodes = new List<MAX_Node>();
        }
        /// <summary>
        /// genertae sub voxels by given number of divisions in x , y and z . (3d grid)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Partitionize(int x, int y , int z)
        {
            Point p = _lowerLeft;
            Dx = X / x; Dy = Y / y; Dz = Z / z;
            for (int k=0;k<z;k++)
            {
                p.Y = _lowerLeft.Y;
                for (int j=0;j<y;j++)
                {
                    p.X = _lowerLeft.X;
                    for( int i=0;i<x;i++)
                    {
                         
                        var v = new Voxel(p, Dx, Dy, Dz);
                        v.I = i; v.J = j; v.K = k;
                        _voxels.Add(v);
                        p.X += Dx;
                    }
                    p.Y += Dy;
                }
                p.Z += Dz;
            }
        
        }
        /// <summary>
        /// return the node which was added to the voxles based on its position. 
        /// if a node exist in the proximity of the given node then the exisitng node is return ,
        /// otherwise the node is added to voxel and null is return.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public MAX_Node Find(MAX_Node node, double tolerance)
        {
            // find the i,j k coordinates
            Vector offset = node.Location - _lowerLeft;
            int i = Math.Min(I, (int)(offset.X / Dx));
            int j = Math.Min(J, (int)(offset.Y / Dy));
            int k = Math.Min(K,(int)(offset.Z / Dz));
            // find the index of the voxel in the voxel list 
            int index = k * (I * J) + j * (I) + i;
            // search for the point in the sub-voxel
            var existing_node =  _voxels[index]._nodes.FirstOrDefault(n => (n.Location - node.Location).Length < tolerance);
            if (existing_node == null)
            {
                _voxels[index]._nodes.Add(node);
                return null;
            }
            else
            {
                return existing_node;
            }
        }

        public void Voxelization()
        {
            // divide the biggest length to 10 
            //Voxel voxel = new Voxel(new Point(x1, y1, z1), new Point(x2, y2, z2));
            double d = Math.Max(Math.Max(X, Z), Math.Max(X, Y))/10.0;
            Partitionize(Math.Max(1, (int)(X / d)), Math.Max(1, (int)(Y / d)), Math.Max(1, (int)(Z / d)));
            //return voxel;
        }
        /// <summary>
        /// expands this voxel to contain the oter one
        /// </summary>
        /// <param name="voxel"></param>
        public void Add(double x1,double y1,double z1,double x2,double y2,double z2)
        {
            if (_lowerLeft.X > x1)
                _lowerLeft.X = x1;
            if (_lowerLeft.Y > y1)
                _lowerLeft.Y = y1;
            if (_lowerLeft.Z > z1)
                _lowerLeft.Z = z1;

            if (_upperRight.X < x2)
                _upperRight.X = x2;
            if (_upperRight.Y < y2)
                _upperRight.Y = y2;
            if (_upperRight.Z < z2)
                _upperRight.Z = z2;
            X = _upperRight.X - _lowerLeft.X;
            Y = _upperRight.Y - _lowerLeft.Y;
            Z = _upperRight.Z - _lowerLeft.Z;


        }

    }
}
