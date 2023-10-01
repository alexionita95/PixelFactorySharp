using Microsoft.Xna.Framework;
using PixelFactory.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory.Items
{
    public class Crafter : Entity
    {
        public bool HasOutputItems { get => Outputs.Count > 0; }
        public bool HasPendingJobs { get => PendingJobs.Count > 0; }
        public bool HasActiveJobs { get=>ActiveJobs.Count > 0; }
        public int ParallelJobs { get; private set; } = 1;
        public List<CraftingJob> ActiveJobs { get; private set; }
        public Queue<ItemSlot> Outputs { get; private set; }
        public Queue<CraftingJob> PendingJobs { get; private set; }
        public Crafter()
        {
            ActiveJobs = new List<CraftingJob>();
            Outputs = new Queue<ItemSlot>();
            PendingJobs = new Queue<CraftingJob>();
        }
        public Crafter(int parallelJobs)
        {
            Outputs = new Queue<ItemSlot>();
            PendingJobs = new Queue<CraftingJob>();
            ActiveJobs = new List<CraftingJob>();
            ParallelJobs = parallelJobs;
        }
        public Crafter(Recipe recipe, int count = 1, int parallelJobs = 1)
        {
            Outputs = new Queue<ItemSlot>();
            PendingJobs = new Queue<CraftingJob>();
            ActiveJobs= new List<CraftingJob>();
            ParallelJobs = parallelJobs;
            Enqueue(recipe, count);
        }
        public void Enqueue(Recipe recipe, int count = 1)
        {
            for(int i = 0; i < count; ++i) 
            {
                PendingJobs.Enqueue(new CraftingJob(recipe));
            }
        }
        
        public ItemSlot DequeueOutput()
        {
            if (Outputs.Count == 0)
                return null;
            return Outputs.Dequeue();
        }
        public void RemoveFirstOutput()
        {
            DequeueOutput();
        }
        public ItemSlot GetFirstOutput()
        {
            return Outputs.Peek();
        }
        public List<ItemSlot> GetOutputs()
        {
            List<ItemSlot > outputs = new List<ItemSlot>();
            while(HasOutputItems)
            {
                outputs.Add(DequeueOutput());
            }
            return outputs;
        }
        private bool CanAcceptActiveJobs { get => ActiveJobs.Count <= ParallelJobs; }
        public override void Update(GameTime gameTime)
        {

            if (!HasPendingJobs && !HasActiveJobs)
                return;

            List<CraftingJob> toRemove = new List<CraftingJob>();
            foreach(var activeJob in ActiveJobs) 
            {
                activeJob.Update(gameTime);
                if(activeJob.Finished)
                {
                    toRemove.Add(activeJob);
                }
            }
            foreach(var job in toRemove)
            {
                while(job.HasOutputItems)
                {
                    Outputs.Enqueue(job.GetOutput());
                }
                ActiveJobs.Remove(job);
            }
            if(HasPendingJobs && CanAcceptActiveJobs)
            {
                for(int i =0; i< ParallelJobs - ActiveJobs.Count; ++i) 
                {
                    ActiveJobs.Add(PendingJobs.Dequeue());
                }
            }
            base.Update(gameTime);
        }
    }
}
