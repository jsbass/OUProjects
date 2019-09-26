using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Portal.Helpers
{
    public interface IRegion
    {
        bool Contains(PointD p);
        bool IsContainedBy(RectangleD r);
        bool Overlaps(RectangleD r);
    }
    
    public class QuadTree<T> where T : IRegion
    {
        public Node<T> Root;

        public QuadTree(RectangleD bounds)
        {
            Root = new Node<T>()
            {
                Bounds = bounds
            };
        }

        public void Insert(T item)
        {
            Root.Insert(item);
        }

        public void Remove(T item)
        {
            var leaf = FindLeaf(item);
            if (leaf == null) return;
            leaf.Contents.Remove(item);
            leaf.UnDivide();
        }

        public Node<T> FindLeaf(T item)
        {
            return Root.FindLeaf(item);
        }

        public Node<T> FindLeaf(PointD p)
        {
            return Root.FindLeaf(p);
        }
    }

    public class Node<T> where T : IRegion
    {
        public RectangleD Bounds;
        public Node<T> Parent;
        private List<Node<T>> Children = new List<Node<T>>();

        public Node<T> NW => Children.Count != 4 ? null : Children[0];
        public Node<T> NE => Children.Count != 4 ? null : Children[1];
        public Node<T> SW => Children.Count != 4 ? null : Children[2];
        public Node<T> SE => Children.Count != 4 ? null : Children[3];
        
        public List<T> Contents = new List<T>();

        public int Count => Contents.Count + Children.Sum(c => c.Count);
        public bool IsLeaf => Children.Count == 0;
        public bool IsEmpty => Children.Count == 0;
        public bool IsRoot => Parent == null;

        public void UnDivide()
        {
            if (Count == 0)
            {
                Children.Clear();
                Parent?.UnDivide();
            }
        }

        public Node<T> FindLeaf(T item)
        {
            if (Contents.Contains(item))return this;

            if (item.IsContainedBy(Bounds))
            {
                foreach (var child in Children)
                {
                    if (item.IsContainedBy(child.Bounds))
                    {
                        var result = child.FindLeaf(item);
                        return result;
                    }
                }
            }

            return null;
        }

        public Node<T> FindLeaf(PointD p)
        {
            if (Bounds.Contains(p))
            {
                //TODO: use center point to select quadrant
                foreach (var child in Children)
                {
                    if (child.Bounds.Contains(p))
                    {
                        return child.FindLeaf(p);
                    }
                }

                //Either there were no children or floating point math screwed up
                return this;
            }

            //The point wasn't in the box
            return null;
        }

        public void Insert(T item)
        {
            if (item.IsContainedBy(Bounds))
            {
                if (IsEmpty) Split();
                
                foreach (var child in Children)
                {
                    child.Insert(item);
                }

                return;
            }

            if (item.Overlaps(Bounds))
            {
                this.Contents.Add(item);
            }
        }
        
        private void Split()
        {
            if (!IsLeaf) throw new Exception("Already a branch");
            
            var center = new PointD(Bounds.X + Bounds.Width / 2, Bounds.Y + Bounds.Height / 2);
            //Top Left
            Children.Add(new Node<T>()
            {
                Bounds = RectangleD.FromLTRB(Bounds.Left, Bounds.Top, center.X, center.Y),
                Parent = this
            });
            //Top Right
            Children.Add(new Node<T>()
            {
                Bounds = RectangleD.FromLTRB(center.X, Bounds.Top, Bounds.Right, center.Y),
                Parent = this
            });
            //Bottom Left
            Children.Add(new Node<T>()
            {
                Bounds = RectangleD.FromLTRB(Bounds.Left, center.Y, center.X, Bounds.Bottom),
                Parent = this
            });
            //Bottom Right
            Children.Add(new Node<T>()
            {
                Bounds = RectangleD.FromLTRB(center.X, center.Y, Bounds.Right, Bounds.Bottom),
                Parent = this
            });
        }
    }
}