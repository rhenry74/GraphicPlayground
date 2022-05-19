using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicPlayground
{
    public class WorldsCollide : IModelDrawer
    {
       
        public List<IModelDrawer> DotCollection { get; set; }
        public List<IModelDrawer> StarCollection { get; set; }
        public List<Explosion> ExplosionCollection { get; set; }
        public int Size { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float X { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float Y { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private Random Rnd = new Random(
            Guid.NewGuid().ToByteArray()[0] + 
            Guid.NewGuid().ToByteArray()[1] + 
            Guid.NewGuid().ToByteArray()[2] + 
            Guid.NewGuid().ToByteArray()[3]);

        SoundPlayer CenterHitPlayer;
        SoundPlayer LeftHitPlayer; 
        SoundPlayer RightHitPlayer;

        SoundPlayer CenterMissPlayer;
        SoundPlayer LeftMissPlayer;
        SoundPlayer RightMissPlayer;

        public WorldsCollide()
        {
            DotCollection = new List<IModelDrawer>();
            ExplosionCollection = new List<Explosion>();
            StarCollection = new List<IModelDrawer>();

            CenterHitPlayer = new System.Media.SoundPlayer(@"c:\SoundEffects\hit_center.wav");
            LeftHitPlayer = new System.Media.SoundPlayer(@"c:\SoundEffects\hit_left.wav"); 
            RightHitPlayer = new System.Media.SoundPlayer(@"c:\SoundEffects\hit_right.wav");

            CenterMissPlayer = new System.Media.SoundPlayer(@"c:\SoundEffects\miss_center.wav");
            LeftMissPlayer = new System.Media.SoundPlayer(@"c:\SoundEffects\miss_left.wav");
            RightMissPlayer = new System.Media.SoundPlayer(@"c:\SoundEffects\miss_right.wav");
        }

        public void Setup(int w, int h, int count, IHost host = null)
        {
            for (var cnt = 0; cnt < count/2; cnt++)
            {
                var dot = new RandomDot();
                dot.A = Rnd.Next(150, 250);
                dot.R = Rnd.Next(100, 255);
                dot.G = Rnd.Next(100, 255);
                dot.B = Rnd.Next(100, 255);
                dot.X = Rnd.Next(0, w);
                dot.Y = Rnd.Next(0, h);
                dot.Setup(w, h, 0);
                DotCollection.Add(dot);
            }

            for (var cnt = 0; cnt < count / 2; cnt++)
            {
                var dot = new DirectionalDot();
                dot.A = Rnd.Next(150, 250);
                dot.R = Rnd.Next(100, 255);
                dot.G = Rnd.Next(100, 255);
                dot.B = Rnd.Next(100, 255);
                dot.X = Rnd.Next(0, w);
                dot.Y = Rnd.Next(0, h);
                dot.Setup(w, h, 0);
                DotCollection.Add(dot);
            }

            if (StarCollection.Count == 0)
            {
                for (var cnt = 0; cnt < w/8; cnt++)
                {
                    var star = new Star();                    
                    star.Setup(w, h, 1);
                    StarCollection.Add(star);
                }
            }
        }

        private long CollisionCount = 0;
        private long KillCount = 0;
        private DateTime LastRan = new DateTime();
        private int DotGrowElapsed = 0;
        private int Score = 0;        

        public bool DrawOn(Graphics g, IHost host, int elapsed)
        {
            host.SetStatictic("Worlds", DotCollection.Count);
            host.SetStatictic("Worlds Collided", CollisionCount);
            host.SetStatictic("Worlds Destroyed", KillCount);
            host.SetStatictic("Score", Score);            
            
            if (DotCollection.Count == 0)
                return false;

            //if (DotGrowElapsed > 100)//grow 1 dot per second
            //{
            //    var aDot = DotCollection[Rnd.Next(0, DotCollection.Count)];
            //    aDot.Size = aDot.Size + Rnd.Next(1, 6);
            //    DotGrowElapsed = 0;
            //}
            //else
            //{
            //    DotGrowElapsed += elapsed;
            //}

            foreach (var dot in StarCollection)
            {
                dot.DrawOn(g, host, elapsed);
            }

            foreach (var dot in DotCollection)
            {
                dot.DrawOn(g, host, elapsed);
            }

            var explosionsToRemove = new List<Explosion>();
            foreach (var explosion in ExplosionCollection)
            {
                if (!explosion.DrawOn(g, host, elapsed))
                {
                    //returns false if the explosion is over
                    explosionsToRemove.Add(explosion);
                }
            }
            explosionsToRemove.ForEach(exp => ExplosionCollection.Remove(exp));

            var collisions = new List<IModelDrawer>();
            var detector = new DotCollisionDetector();
            foreach (var x in DotCollection)
            {                
                foreach (var y in DotCollection)
                {
                    if (detector.Equals(x,y))
                    {
                        if (x.Size > y.Size)
                        {
                            if (!collisions.Contains(y))
                            {
                                collisions.Add(y);
                                CollisionCount++;
                                x.Size = x.Size + y.Size / 2;
                            }
                        }
                        else if (y.Size > x.Size)
                        {
                            if (!collisions.Contains(x))
                            {
                                collisions.Add(x);
                                CollisionCount++;
                                y.Size = y.Size + x.Size / 2;
                            }
                        }
                        else
                        {
                            collisions.Add(x);
                            CollisionCount++;
                            collisions.Add(y);
                            CollisionCount++;
                        }
                    }
                }
            }
            //replace them with explosions
            foreach(var dot in collisions)
            {                
                ExplosionCollection.Add(new Explosion()
                {
                    X = dot.X,
                    Y = dot.Y,
                    Size = dot.Size
                });
            }
            collisions.ToList().ForEach(c => DotCollection.Remove(c));            

            //add one every now and then
            var add = Rnd.Next(0, 100);
            if (add >= 93)
                Setup(host.Width, host.Height, 2);

            ProcessUserInput(host);

            return true;
            
        }

        private void ProcessUserInput(IHost host)
        {
            if (host.PeakMouse() != null && host.PeakMouse().Clicks == 1)
            {
                var mouseEvent = host.DequeueMouse();
                var clickPoint = new Rectangle(mouseEvent.Location, new Size(2, 2));
                var detector = new DotCollisionDetector();
                var dotsToRemove = new List<IModelDrawer>();
                bool kill = false;
                foreach (var x in DotCollection)
                {
                    if (detector.Equals(x, clickPoint))
                    {
                        dotsToRemove.Add(x);
                        ExplosionCollection.Add(new Explosion()
                        {
                            X = x.X,
                            Y = x.Y,
                            Size = x.Size
                        });

                        Score = Score + ((200 - x.Size > 10) ? 200 - x.Size : 10);
                        if(mouseEvent.Location.X < host.Width / 3)
                        {
                            LeftHitPlayer.Play();
                        }
                        else if(mouseEvent.Location.X > host.Width / 3 * 2)
                        {
                            RightHitPlayer.Play();
                        }
                        else
                        {
                            CenterHitPlayer.Play();
                        }
                        
                        kill = true;
                        KillCount++;
                    }
                    
                }
                foreach(var toRemove in dotsToRemove)
                {
                    DotCollection.Remove(toRemove);
                }

                if (!kill)
                {
                    Score = Score - 100;
                    if (mouseEvent.Location.X < host.Width / 3)
                    {
                        LeftMissPlayer.Play();
                    }
                    else if (mouseEvent.Location.X > host.Width / 3 * 2)
                    {
                        RightMissPlayer.Play();
                    }
                    else
                    {
                        CenterMissPlayer.Play();
                    }
                }
            }
        }
    }
}
